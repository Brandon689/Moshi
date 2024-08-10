using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.MyAnimeList.Models;
using System.Text.Json;

namespace Moshi.MyAnimeList;

public class AnimeDatabase
{
    private readonly string _connectionString;
    private readonly DatabaseInitializer _initializer;
    private const int BatchSize = 200; // Adjust this value based on your system's capabilities

    public AnimeDatabase(string connectionString)
    {
        _connectionString = connectionString;
        _initializer = new DatabaseInitializer();
    }

    private void BulkInsertAnime(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var sql = @"
            INSERT INTO Anime (Title, Type, Episodes, Status, Picture, Thumbnail)
            VALUES (@Title, @Type, @Episodes, @Status, @Picture, @Thumbnail)";

        connection.Execute(sql, animes);
    }

    private void BulkInsertSources(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var sources = animes.SelectMany(a => a.Sources.Select(s => new { AnimeTitle = a.Title, URL = s })).ToList();
        var sql = @"
            INSERT INTO Sources (AnimeID, URL)
            VALUES ((SELECT AnimeID FROM Anime WHERE Title = @AnimeTitle), @URL)";

        connection.Execute(sql, sources);
    }

    private void BulkInsertSynonyms(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var synonyms = animes.SelectMany(a => a.Synonyms.Select(s => new { AnimeTitle = a.Title, Synonym = s })).ToList();
        var sql = @"
            INSERT INTO Synonyms (AnimeID, Synonym)
            VALUES ((SELECT AnimeID FROM Anime WHERE Title = @AnimeTitle), @Synonym)";

        connection.Execute(sql, synonyms);
    }

    private void BulkInsertRelatedAnime(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var relatedAnime = animes.SelectMany(a => a.RelatedAnime.Select(r => new { AnimeTitle = a.Title, RelatedAnimeURL = r })).ToList();
        var sql = @"
            INSERT INTO RelatedAnime (AnimeID, RelatedAnimeURL)
            VALUES ((SELECT AnimeID FROM Anime WHERE Title = @AnimeTitle), @RelatedAnimeURL)";

        connection.Execute(sql, relatedAnime);
    }

    private void BulkInsertTags(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var tags = animes.SelectMany(a => a.Tags.Select(t => new { AnimeTitle = a.Title, Tag = t })).ToList();
        var sql = @"
            INSERT INTO Tags (AnimeID, Tag)
            VALUES ((SELECT AnimeID FROM Anime WHERE Title = @AnimeTitle), @Tag)";

        connection.Execute(sql, tags);
    }

    private void BulkInsertSeasons(SqliteConnection connection, List<MoshiAnime> animes)
    {
        var seasons = animes.Where(a => a.AnimeSeason != null && !string.IsNullOrEmpty(a.AnimeSeason.Season) && a.AnimeSeason.Season != "UNDEFINED")
                            .Select(a => new
                            {
                                AnimeTitle = a.Title,
                                Season = a.AnimeSeason.Season,
                                Year = a.AnimeSeason.Year.HasValue ? (object)a.AnimeSeason.Year.Value : DBNull.Value
                            })
                            .Distinct() // Add this to remove duplicates
                            .ToList();

        var insertSeasonSql = @"
        INSERT OR IGNORE INTO Season (Season, Year)
        VALUES (@Season, @Year)";

        connection.Execute(insertSeasonSql, seasons);

        var linkAnimeSeasonSql = @"
        INSERT OR IGNORE INTO AnimeSeason (AnimeID, SeasonID)
        VALUES (
            (SELECT AnimeID FROM Anime WHERE Title = @AnimeTitle),
            (SELECT SeasonID FROM Season WHERE Season = @Season AND ((@Year IS NULL AND Year IS NULL) OR Year = @Year))
        )";

        connection.Execute(linkAnimeSeasonSql, seasons);
    }

    public void CreateIndexes()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            Console.WriteLine("Creating indexes...");

            // Index on Anime.Title (for text searches)
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_anime_title ON Anime(Title);");

            // Index on Season (Season, Year) - although it's already UNIQUE, an index might help queries
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_season_season_year ON Season(Season, Year);");

            // Index on AnimeSeason.SeasonID (AnimeID is already part of the primary key)
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_animeseason_seasonid ON AnimeSeason(SeasonID);");

            // Index on Sources.AnimeID
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_sources_animeid ON Sources(AnimeID);");

            // Index on Synonyms.AnimeID and Synonyms.Synonym
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_synonyms_animeid ON Synonyms(AnimeID);");
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_synonyms_synonym ON Synonyms(Synonym);");

            // Index on RelatedAnime.AnimeID
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_relatedanime_animeid ON RelatedAnime(AnimeID);");

            // Index on Tags.AnimeID and Tags.Tag
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_tags_animeid ON Tags(AnimeID);");
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_tags_tag ON Tags(Tag);");

            Console.WriteLine("Indexes created successfully.");
        }
    }

    public void ImportFromJson(string jsonFilePath)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            _initializer.CreateTables(connection);

            // Disable indexes and foreign keys
            connection.Execute("PRAGMA foreign_keys = OFF;");

            try
            {
                // Create temporary tables
                CreateTemporaryTables(connection);

                using (var jsonStream = File.OpenRead(jsonFilePath))
                using (var jsonDocument = JsonDocument.Parse(jsonStream))
                {
                    var root = jsonDocument.RootElement;
                    var dataArray = root.GetProperty("data").EnumerateArray();

                    var animeBatch = new List<MoshiAnime>();
                    int count = 0;

                    foreach (var animeElement in dataArray)
                    {
                        var anime = JsonSerializer.Deserialize<MoshiAnime>(animeElement.GetRawText());
                        animeBatch.Add(anime);
                        count++;

                        if (count % BatchSize == 0)
                        {
                            ProcessBatchWithTransaction(connection, animeBatch);
                            animeBatch.Clear();
                            Console.WriteLine($"Processed {count} anime entries");
                        }
                    }

                    // Process any remaining anime
                    if (animeBatch.Any())
                    {
                        ProcessBatchWithTransaction(connection, animeBatch);
                    }
                }

                // Insert from temporary tables to main tables
                InsertFromTemporaryTablesWithTransaction(connection);

                Console.WriteLine("Import completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
            finally
            {
                // Re-enable indexes and foreign keys
                connection.Execute("PRAGMA foreign_keys = ON;");
                // Rebuild indexes if necessary
                // connection.Execute("REINDEX;");
            }
        }
    }

    private void ProcessBatchWithTransaction(SqliteConnection connection, List<MoshiAnime> animeBatch)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                ProcessBatch(connection, animeBatch);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    private void InsertFromTemporaryTablesWithTransaction(SqliteConnection connection)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                InsertFromTemporaryTables(connection);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    private void CreateTemporaryTables(SqliteConnection connection)
    {
        connection.Execute(@"
        CREATE TEMPORARY TABLE TempAnime (
            Title TEXT PRIMARY KEY,
            Type TEXT,
            Episodes INTEGER,
            Status TEXT,
            Picture TEXT,
            Thumbnail TEXT
        );

        CREATE TEMPORARY TABLE TempSources (
            AnimeTitle TEXT,
            URL TEXT
        );

        CREATE TEMPORARY TABLE TempSynonyms (
            AnimeTitle TEXT,
            Synonym TEXT
        );

        CREATE TEMPORARY TABLE TempRelatedAnime (
            AnimeTitle TEXT,
            RelatedAnimeURL TEXT
        );

        CREATE TEMPORARY TABLE TempTags (
            AnimeTitle TEXT,
            Tag TEXT
        );

        CREATE TEMPORARY TABLE TempSeasons (
            AnimeTitle TEXT,
            Season TEXT,
            Year INTEGER
        );
    ");
    }

    private void ProcessBatch(SqliteConnection connection, List<MoshiAnime> animeBatch)
    {
        var animeData = animeBatch.Select(a => new
        {
            a.Title,
            a.Type,
            a.Episodes,
            a.Status,
            a.Picture,
            a.Thumbnail
        });

        connection.Execute(@"
        INSERT OR REPLACE INTO TempAnime (Title, Type, Episodes, Status, Picture, Thumbnail)
        VALUES (@Title, @Type, @Episodes, @Status, @Picture, @Thumbnail)", animeData);

        var sources = animeBatch.SelectMany(a => a.Sources.Select(s => new { AnimeTitle = a.Title, URL = s }));
        connection.Execute("INSERT INTO TempSources (AnimeTitle, URL) VALUES (@AnimeTitle, @URL)", sources);

        var synonyms = animeBatch.SelectMany(a => a.Synonyms.Select(s => new { AnimeTitle = a.Title, Synonym = s }));
        connection.Execute("INSERT INTO TempSynonyms (AnimeTitle, Synonym) VALUES (@AnimeTitle, @Synonym)", synonyms);

        var relatedAnime = animeBatch.SelectMany(a => a.RelatedAnime.Select(r => new { AnimeTitle = a.Title, RelatedAnimeURL = r }));
        connection.Execute("INSERT INTO TempRelatedAnime (AnimeTitle, RelatedAnimeURL) VALUES (@AnimeTitle, @RelatedAnimeURL)", relatedAnime);

        var tags = animeBatch.SelectMany(a => a.Tags.Select(t => new { AnimeTitle = a.Title, Tag = t }));
        connection.Execute("INSERT INTO TempTags (AnimeTitle, Tag) VALUES (@AnimeTitle, @Tag)", tags);

        var seasons = animeBatch
            .Where(a => a.AnimeSeason != null && !string.IsNullOrEmpty(a.AnimeSeason.Season) && a.AnimeSeason.Season != "UNDEFINED")
            .Select(a => new
            {
                AnimeTitle = a.Title,
                Season = a.AnimeSeason.Season,
                Year = a.AnimeSeason.Year.HasValue ? (object)a.AnimeSeason.Year.Value : DBNull.Value
            });
        connection.Execute("INSERT INTO TempSeasons (AnimeTitle, Season, Year) VALUES (@AnimeTitle, @Season, @Year)", seasons);
    }

    private void InsertFromTemporaryTables(SqliteConnection connection)
    {
        connection.Execute(@"
        INSERT OR REPLACE INTO Anime (Title, Type, Episodes, Status, Picture, Thumbnail)
        SELECT Title, Type, Episodes, Status, Picture, Thumbnail FROM TempAnime;

        INSERT OR IGNORE INTO Sources (AnimeID, URL)
        SELECT a.AnimeID, ts.URL
        FROM TempSources ts
        JOIN Anime a ON ts.AnimeTitle = a.Title;

        INSERT OR IGNORE INTO Synonyms (AnimeID, Synonym)
        SELECT a.AnimeID, ts.Synonym
        FROM TempSynonyms ts
        JOIN Anime a ON ts.AnimeTitle = a.Title;

        INSERT OR IGNORE INTO RelatedAnime (AnimeID, RelatedAnimeURL)
        SELECT a.AnimeID, tr.RelatedAnimeURL
        FROM TempRelatedAnime tr
        JOIN Anime a ON tr.AnimeTitle = a.Title;

        INSERT OR IGNORE INTO Tags (AnimeID, Tag)
        SELECT a.AnimeID, tt.Tag
        FROM TempTags tt
        JOIN Anime a ON tt.AnimeTitle = a.Title;

        INSERT OR IGNORE INTO Season (Season, Year)
        SELECT DISTINCT Season, Year FROM TempSeasons;

        INSERT OR IGNORE INTO AnimeSeason (AnimeID, SeasonID)
        SELECT a.AnimeID, s.SeasonID
        FROM TempSeasons ts
        JOIN Anime a ON ts.AnimeTitle = a.Title
        JOIN Season s ON ts.Season = s.Season AND (ts.Year IS NULL AND s.Year IS NULL OR ts.Year = s.Year);
    ");
    }

    public void VacuumDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            Console.WriteLine("Starting VACUUM process...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Execute VACUUM
            connection.Execute("VACUUM;");

            stopwatch.Stop();
            Console.WriteLine($"VACUUM completed in {stopwatch.ElapsedMilliseconds} ms");

            // Run ANALYZE and gather some information
            Console.WriteLine("Running ANALYZE...");
            stopwatch.Restart();

            connection.Execute("ANALYZE;");

            stopwatch.Stop();
            Console.WriteLine($"ANALYZE completed in {stopwatch.ElapsedMilliseconds} ms");

            // Query sqlite_stat1 for some basic statistics
            var stats = connection.Query("SELECT * FROM sqlite_stat1 LIMIT 5").AsList();
            Console.WriteLine("Sample statistics from sqlite_stat1:");
            foreach (var stat in stats)
            {
                Console.WriteLine($"Table: {stat.tbl}, Index: {stat.idx}, Stats: {stat.stat}");
            }

            // Show an example of how EXPLAIN QUERY PLAN might use these statistics
            var plan = connection.Query("EXPLAIN QUERY PLAN SELECT * FROM Anime WHERE Title LIKE '%Naruto%'").AsList();
            Console.WriteLine("Query plan for a sample query:");
            foreach (var step in plan)
            {
                Console.WriteLine($"{step.id}|{step.parent}|{step.notused}|{step.detail}");
            }
        }
    }
}