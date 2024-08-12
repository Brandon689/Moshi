using Dapper;
using Microsoft.Data.Sqlite;

namespace Moshi.MyAnimeList.Data;

public class DatabaseInitializer
{
    public void CreateTables(SqliteConnection connection)
    {
        connection.Execute("PRAGMA foreign_keys = ON;");

        var sql = @"
        CREATE TABLE IF NOT EXISTS Anime (
            AnimeID INTEGER PRIMARY KEY,
            Title TEXT NOT NULL,
            Type TEXT,
            Episodes INTEGER,
            Status TEXT,
            Picture TEXT,
            Thumbnail TEXT
        );

        CREATE TABLE IF NOT EXISTS Season (
            SeasonID INTEGER PRIMARY KEY,
            Season TEXT,
            Year INTEGER,
            UNIQUE(Season, Year)
        );

        CREATE TABLE IF NOT EXISTS AnimeSeason (
            AnimeID INTEGER,
            SeasonID INTEGER,
            FOREIGN KEY (AnimeID) REFERENCES Anime(AnimeID),
            FOREIGN KEY (SeasonID) REFERENCES Season(SeasonID),
            PRIMARY KEY (AnimeID, SeasonID)
        );

        CREATE TABLE IF NOT EXISTS Sources (
            SourceID INTEGER PRIMARY KEY,
            AnimeID INTEGER,
            URL TEXT NOT NULL,
            FOREIGN KEY (AnimeID) REFERENCES Anime(AnimeID)
        );

        CREATE TABLE IF NOT EXISTS Synonyms (
            SynonymID INTEGER PRIMARY KEY,
            AnimeID INTEGER,
            Synonym TEXT NOT NULL,
            FOREIGN KEY (AnimeID) REFERENCES Anime(AnimeID)
        );

        CREATE TABLE IF NOT EXISTS RelatedAnime (
            RelationID INTEGER PRIMARY KEY,
            AnimeID INTEGER,
            RelatedAnimeURL TEXT NOT NULL,
            FOREIGN KEY (AnimeID) REFERENCES Anime(AnimeID)
        );

        CREATE TABLE IF NOT EXISTS Tags (
            TagID INTEGER PRIMARY KEY,
            AnimeID INTEGER,
            Tag TEXT NOT NULL,
            FOREIGN KEY (AnimeID) REFERENCES Anime(AnimeID)
        );";

        connection.Execute(sql);
    }
}