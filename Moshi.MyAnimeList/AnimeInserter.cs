using Dapper;
using Microsoft.Data.Sqlite;

namespace Moshi.MyAnimeList;

public class AnimeInserter
{
    public void InsertAnime(SqliteConnection connection, MoshiAnime anime)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var animeId = InsertMainAnimeData(connection, anime, transaction);
                InsertAnimeSeason(connection, anime, animeId, transaction);
                InsertSources(connection, anime, animeId, transaction);
                InsertSynonyms(connection, anime, animeId, transaction);
                InsertRelatedAnime(connection, anime, animeId, transaction);
                InsertTags(connection, anime, animeId, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                transaction.Rollback();
                throw;
            }
        }
    }

    private long InsertMainAnimeData(SqliteConnection connection, MoshiAnime anime, SqliteTransaction transaction)
    {
        var sql = @"
        INSERT INTO Anime (Title, Type, Episodes, Status, Picture, Thumbnail)
        VALUES (@Title, @Type, @Episodes, @Status, @Picture, @Thumbnail);
        SELECT last_insert_rowid();";

        return connection.ExecuteScalar<long>(sql, anime, transaction);
    }

    private void InsertAnimeSeason(SqliteConnection connection, MoshiAnime anime, long animeId, SqliteTransaction transaction)
    {
        if (anime.AnimeSeason != null &&
            !string.IsNullOrEmpty(anime.AnimeSeason.Season) &&
            anime.AnimeSeason.Season != "UNDEFINED")
        {
            var sql = @"
            INSERT OR IGNORE INTO Season (Season, Year)
            VALUES (@Season, @Year);
            SELECT SeasonID FROM Season WHERE Season = @Season AND ((@Year IS NULL AND Year IS NULL) OR Year = @Year);";

            var seasonParams = new
            {
                Season = anime.AnimeSeason.Season,
                Year = anime.AnimeSeason.Year.HasValue ? (object)anime.AnimeSeason.Year.Value : DBNull.Value
            };

            var seasonId = connection.ExecuteScalar<long>(sql, seasonParams, transaction);

            sql = @"
            INSERT INTO AnimeSeason (AnimeID, SeasonID)
            VALUES (@AnimeID, @SeasonID);";

            connection.Execute(sql, new { AnimeID = animeId, SeasonID = seasonId }, transaction);
        }
    }

    private void InsertSources(SqliteConnection connection, MoshiAnime anime, long animeId, SqliteTransaction transaction)
    {
        if (anime.Sources != null && anime.Sources.Count > 0)
        {
            var sql = @"
            INSERT INTO Sources (AnimeID, URL)
            VALUES (@AnimeID, @URL);";

            connection.Execute(sql, anime.Sources.Select(s => new { AnimeID = animeId, URL = s }), transaction);
        }
    }

    private void InsertSynonyms(SqliteConnection connection, MoshiAnime anime, long animeId, SqliteTransaction transaction)
    {
        if (anime.Synonyms != null && anime.Synonyms.Count > 0)
        {
            var sql = @"
            INSERT INTO Synonyms (AnimeID, Synonym)
            VALUES (@AnimeID, @Synonym);";

            connection.Execute(sql, anime.Synonyms.Select(s => new { AnimeID = animeId, Synonym = s }), transaction);
        }
    }

    private void InsertRelatedAnime(SqliteConnection connection, MoshiAnime anime, long animeId, SqliteTransaction transaction)
    {
        if (anime.RelatedAnime != null && anime.RelatedAnime.Count > 0)
        {
            var sql = @"
            INSERT INTO RelatedAnime (AnimeID, RelatedAnimeURL)
            VALUES (@AnimeID, @RelatedAnimeURL);";

            connection.Execute(sql, anime.RelatedAnime.Select(r => new { AnimeID = animeId, RelatedAnimeURL = r }), transaction);
        }
    }

    private void InsertTags(SqliteConnection connection, MoshiAnime anime, long animeId, SqliteTransaction transaction)
    {
        if (anime.Tags != null && anime.Tags.Count > 0)
        {
            var sql = @"
            INSERT INTO Tags (AnimeID, Tag)
            VALUES (@AnimeID, @Tag);";

            connection.Execute(sql, anime.Tags.Select(t => new { AnimeID = animeId, Tag = t }), transaction);
        }
    }
}