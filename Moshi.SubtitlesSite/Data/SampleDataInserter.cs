using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System;
using System.Collections.Generic;

namespace Moshi.SubtitlesSite.Data;

public class SampleDataInserter
{
    private readonly string _connectionString;

    public SampleDataInserter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InsertSampleData()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Check if data already exists
        var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Movies");
        if (count > 0)
        {
            Console.WriteLine("Sample data already exists. Skipping insertion.");
            return;
        }

        var users = InsertUsers(connection);
        var movies = InsertMovies(connection);
        InsertSubtitles(connection, users, movies);
        InsertAlternativeTitles(connection, movies);
        InsertMovieLinks(connection, movies);
        InsertSubtitleRatings(connection, users);
        InsertSubtitleComments(connection, users);
        InsertUserBadges(connection, users);

        Console.WriteLine("Sample data inserted successfully.");
    }

    private List<User> InsertUsers(SqliteConnection connection)
    {
        var users = new List<User>
        {
            new User { Username = "admin", Email = "admin@example.com", PasswordHash = "hashed_password_admin", RegistrationDate = DateTime.UtcNow, IsAdmin = true },
            new User { Username = "john_doe", Email = "john@example.com", PasswordHash = "hashed_password_john", RegistrationDate = DateTime.UtcNow.AddDays(-30), IsAdmin = false },
            new User { Username = "jane_smith", Email = "jane@example.com", PasswordHash = "hashed_password_jane", RegistrationDate = DateTime.UtcNow.AddDays(-15), IsAdmin = false },
        };

        foreach (var user in users)
        {
            user.UserId = connection.ExecuteScalar<int>(@"
                INSERT INTO Users (Username, Email, PasswordHash, RegistrationDate, IsAdmin)
                VALUES (@Username, @Email, @PasswordHash, @RegistrationDate, @IsAdmin);
                SELECT last_insert_rowid();", user);
        }

        return users;
    }

    private List<Movie> InsertMovies(SqliteConnection connection)
    {
        var movies = new List<Movie>
{
    new Movie { ImdbId = "tt4520988", Title = "Frozen II", Year = 2019, Synopsis = "Anna, Elsa, Kristoff, Olaf and Sven leave Arendelle to travel to an ancient, autumn-bound forest of an enchanted land.", Director = "Jennifer Lee, Chris Buck", Cast = "Kristen Bell, Idina Menzel, Josh Gad", Genre = "Animation, Adventure, Comedy", ImdbRating = 6.8, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt1375666", Title = "Inception", Year = 2010, Synopsis = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.", Director = "Christopher Nolan", Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt, Elliot Page", Genre = "Action, Adventure, Sci-Fi", ImdbRating = 8.8, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0110912", Title = "Pulp Fiction", Year = 1994, Synopsis = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.", Director = "Quentin Tarantino", Cast = "John Travolta, Uma Thurman, Samuel L. Jackson", Genre = "Crime, Drama", ImdbRating = 8.9, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0111161", Title = "The Shawshank Redemption", Year = 1994, Synopsis = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.", Director = "Frank Darabont", Cast = "Tim Robbins, Morgan Freeman, Bob Gunton", Genre = "Drama", ImdbRating = 9.3, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0068646", Title = "The Godfather", Year = 1972, Synopsis = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", Director = "Francis Ford Coppola", Cast = "Marlon Brando, Al Pacino, James Caan", Genre = "Crime, Drama", ImdbRating = 9.2, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0468569", Title = "The Dark Knight", Year = 2008, Synopsis = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.", Director = "Christopher Nolan", Cast = "Christian Bale, Heath Ledger, Aaron Eckhart", Genre = "Action, Crime, Drama", ImdbRating = 9.0, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0050083", Title = "12 Angry Men", Year = 1957, Synopsis = "A jury holdout attempts to prevent a miscarriage of justice by forcing his colleagues to reconsider the evidence.", Director = "Sidney Lumet", Cast = "Henry Fonda, Lee J. Cobb, Martin Balsam", Genre = "Crime, Drama", ImdbRating = 9.0, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0108052", Title = "Schindler's List", Year = 1993, Synopsis = "In German-occupied Poland during World War II, industrialist Oskar Schindler gradually becomes concerned for his Jewish workforce after witnessing their persecution by the Nazis.", Director = "Steven Spielberg", Cast = "Liam Neeson, Ralph Fiennes, Ben Kingsley", Genre = "Biography, Drama, History", ImdbRating = 9.0, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0167260", Title = "The Lord of the Rings: The Return of the King", Year = 2003, Synopsis = "Gandalf and Aragorn lead the World of Men against Sauron's army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.", Director = "Peter Jackson", Cast = "Elijah Wood, Viggo Mortensen, Ian McKellen", Genre = "Action, Adventure, Drama", ImdbRating = 9.0, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0120737", Title = "The Lord of the Rings: The Fellowship of the Ring", Year = 2001, Synopsis = "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth from the Dark Lord Sauron.", Director = "Peter Jackson", Cast = "Elijah Wood, Ian McKellen, Orlando Bloom", Genre = "Action, Adventure, Drama", ImdbRating = 8.8, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0109830", Title = "Forrest Gump", Year = 1994, Synopsis = "The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man with an IQ of 75, whose only desire is to be reunited with his childhood sweetheart.", Director = "Robert Zemeckis", Cast = "Tom Hanks, Robin Wright, Gary Sinise", Genre = "Drama, Romance", ImdbRating = 8.8, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0133093", Title = "The Matrix", Year = 1999, Synopsis = "A computer programmer discovers that reality as he knows it is a simulation created by machines to subjugate humanity, and joins a rebellion to overthrow them.", Director = "Lana Wachowski, Lilly Wachowski", Cast = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss", Genre = "Action, Sci-Fi", ImdbRating = 8.7, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0047478", Title = "Seven Samurai", Year = 1954, Synopsis = "A poor village under attack by bandits recruits seven unemployed samurai to help them defend themselves.", Director = "Akira Kurosawa", Cast = "Toshirô Mifune, Takashi Shimura, Keiko Tsushima", Genre = "Action, Drama", ImdbRating = 8.6, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0076759", Title = "Star Wars: Episode IV - A New Hope", Year = 1977, Synopsis = "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a Wookiee and two droids to save the galaxy from the Empire's world-destroying battle station, while also attempting to rescue Princess Leia from the mysterious Darth Vader.", Director = "George Lucas", Cast = "Mark Hamill, Harrison Ford, Carrie Fisher", Genre = "Action, Adventure, Fantasy", ImdbRating = 8.6, DateAdded = DateTime.UtcNow },
    new Movie { ImdbId = "tt0114369", Title = "Se7en", Year = 1995, Synopsis = "Two detectives, a rookie and a veteran, hunt a serial killer who uses the seven deadly sins as his motives.", Director = "David Fincher", Cast = "Morgan Freeman, Brad Pitt, Kevin Spacey", Genre = "Crime, Drama, Mystery", ImdbRating = 8.6, DateAdded = DateTime.UtcNow },
};

        foreach (var movie in movies)
        {
            movie.MovieId = connection.ExecuteScalar<int>(@"
                INSERT INTO Movies (ImdbId, Title, Year, Synopsis, Director, Cast, ImdbRating, DateAdded)
                VALUES (@ImdbId, @Title, @Year, @Synopsis, @Director, @Cast, @ImdbRating, @DateAdded);
                SELECT last_insert_rowid();", movie);
        }

        return movies;
    }

    private void InsertSubtitles(SqliteConnection connection, List<User> users, List<Movie> movies)
    {
        var subtitles = new List<Subtitle>
        {
            new Subtitle { MovieId = movies[0].MovieId, UserId = users[0].UserId, Language = "English", Format = "SRT", ReleaseInfo = "Frozen.II.2019.720p.BluRay.x264-YOL0W", StorageFileName = "frozen_ii_en.srt", OriginalFileName = "Frozen.II.2019.720p.BluRay.x264-YOL0W.srt", UploadDate = DateTime.UtcNow, Downloads = 10, FPS = 23.976 },
            new Subtitle { MovieId = movies[1].MovieId, UserId = users[1].UserId, Language = "Spanish", Format = "SRT", ReleaseInfo = "Inception.2010.1080p.BluRay.x264-SPARKS", StorageFileName = "inception_es.srt", OriginalFileName = "Inception.2010.1080p.BluRay.x264-SPARKS.srt", UploadDate = DateTime.UtcNow.AddDays(-5), Downloads = 25, FPS = 24 },
            new Subtitle { MovieId = movies[2].MovieId, UserId = users[2].UserId, Language = "French", Format = "SUB", ReleaseInfo = "Pulp.Fiction.1994.Remastered.1080p.BluRay.x264-LEVERAGE", StorageFileName = "pulp_fiction_fr.sub", OriginalFileName = "Pulp.Fiction.1994.Remastered.1080p.BluRay.x264-LEVERAGE.sub", UploadDate = DateTime.UtcNow.AddDays(-10), Downloads = 15, FPS = 23.976 },
        };

        foreach (var subtitle in subtitles)
        {
            subtitle.SubtitleId = connection.ExecuteScalar<int>(@"
                INSERT INTO Subtitles (MovieId, UserId, Language, Format, ReleaseInfo, StorageFileName, OriginalFileName, UploadDate, Downloads, FPS)
                VALUES (@MovieId, @UserId, @Language, @Format, @ReleaseInfo, @StorageFileName, @OriginalFileName, @UploadDate, @Downloads, @FPS);
                SELECT last_insert_rowid();", subtitle);
        }
    }

    private void InsertAlternativeTitles(SqliteConnection connection, List<Movie> movies)
    {
        var alternativeTitles = new List<(int MovieId, string Title)>
        {
            (movies[0].MovieId, "Frozen 2"),
            (movies[1].MovieId, "Origin"),
            (movies[2].MovieId, "Black Mask"),
        };

        foreach (var (MovieId, Title) in alternativeTitles)
        {
            connection.Execute(@"
                INSERT INTO AlternativeTitles (MovieId, Title)
                VALUES (@MovieId, @Title)", new { MovieId, Title });
        }
    }

    private void InsertMovieLinks(SqliteConnection connection, List<Movie> movies)
    {
        var movieLinks = new List<(int MovieId, string LinkType, string Url)>
        {
            (movies[0].MovieId, "Watch Online", "https://example.com/watch/frozen-ii"),
            (movies[1].MovieId, "Trailer", "https://example.com/trailer/inception"),
            (movies[2].MovieId, "IMDb", "https://www.imdb.com/title/tt0110912/"),
        };

        foreach (var (MovieId, LinkType, Url) in movieLinks)
        {
            connection.Execute(@"
                INSERT INTO MovieLinks (MovieId, LinkType, Url)
                VALUES (@MovieId, @LinkType, @Url)", new { MovieId, LinkType, Url });
        }
    }

    private void InsertSubtitleRatings(SqliteConnection connection, List<User> users)
    {
        var subtitleRatings = new List<SubtitleRating>
        {
            new SubtitleRating { SubtitleId = 1, UserId = users[1].UserId, Rating = 8, RatingDate = DateTime.UtcNow.AddDays(-2) },
            new SubtitleRating { SubtitleId = 2, UserId = users[2].UserId, Rating = 9, RatingDate = DateTime.UtcNow.AddDays(-1) },
            new SubtitleRating { SubtitleId = 3, UserId = users[0].UserId, Rating = 7, RatingDate = DateTime.UtcNow },
        };

        foreach (var rating in subtitleRatings)
        {
            connection.Execute(@"
                INSERT INTO SubtitleRatings (SubtitleId, UserId, Rating, RatingDate)
                VALUES (@SubtitleId, @UserId, @Rating, @RatingDate)", rating);
        }
    }

    private void InsertSubtitleComments(SqliteConnection connection, List<User> users)
    {
        var subtitleComments = new List<SubtitleComment>
        {
            new SubtitleComment { SubtitleId = 1, UserId = users[1].UserId, Comment = "Great subtitle, perfect sync!", CommentDate = DateTime.UtcNow.AddDays(-1) },
            new SubtitleComment { SubtitleId = 2, UserId = users[2].UserId, Comment = "Good translation, but a few typos.", CommentDate = DateTime.UtcNow.AddHours(-12) },
            new SubtitleComment { SubtitleId = 3, UserId = users[0].UserId, Comment = "Excellent work, thank you!", CommentDate = DateTime.UtcNow.AddHours(-2) },
        };

        foreach (var comment in subtitleComments)
        {
            connection.Execute(@"
                INSERT INTO SubtitleComments (SubtitleId, UserId, Comment, CommentDate)
                VALUES (@SubtitleId, @UserId, @Comment, @CommentDate)", comment);
        }
    }

    private void InsertUserBadges(SqliteConnection connection, List<User> users)
    {
        var userBadges = new List<UserBadge>
        {
            new UserBadge { UserId = users[0].UserId, BadgeName = "Admin", AwardDate = DateTime.UtcNow.AddDays(-30) },
            new UserBadge { UserId = users[1].UserId, BadgeName = "Subtitle Master", AwardDate = DateTime.UtcNow.AddDays(-15) },
            new UserBadge { UserId = users[2].UserId, BadgeName = "Rising Star", AwardDate = DateTime.UtcNow.AddDays(-5) },
        };

        foreach (var badge in userBadges)
        {
            connection.Execute(@"
                INSERT INTO UserBadges (UserId, BadgeName, AwardDate)
                VALUES (@UserId, @BadgeName, @AwardDate)", badge);
        }
    }
}