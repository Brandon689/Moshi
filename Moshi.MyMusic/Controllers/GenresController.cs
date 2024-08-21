using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public GenresController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            using var connection = CreateConnection();
            var genres = await connection.QueryAsync<Genre>(@"
                SELECT 
                    genre_id AS GenreId, 
                    name AS Name 
                FROM genres 
                ORDER BY name");
            return Ok(genres);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            using var connection = CreateConnection();
            var genre = await connection.QuerySingleOrDefaultAsync<Genre>(@"
                SELECT 
                    genre_id AS GenreId, 
                    name AS Name 
                FROM genres 
                WHERE genre_id = @Id", new { Id = id });

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<Genre>> CreateGenre(Genre genre)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO genres (name) 
                VALUES (@Name);
                SELECT last_insert_rowid();";

            var id = await connection.ExecuteScalarAsync<int>(sql, genre);
            genre.GenreId = id;

            return CreatedAtAction(nameof(GetGenre), new { id = genre.GenreId }, genre);
        }

        // PUT: api/Genres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, Genre genre)
        {
            if (id != genre.GenreId)
            {
                return BadRequest();
            }

            using var connection = CreateConnection();
            var sql = "UPDATE genres SET name = @Name WHERE genre_id = @GenreId";

            var affected = await connection.ExecuteAsync(sql, genre);

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM genres WHERE genre_id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Genres/5/Songs
        [HttpGet("{id}/Songs")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetSongsByGenre(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    s.song_id AS SongId, 
                    s.title AS Title, 
                    s.duration AS Duration, 
                    s.track_number AS TrackNumber, 
                    s.explicit AS Explicit, 
                    s.audio_file_path AS AudioFilePath,
                    a.name AS ArtistName
                FROM songs s
                JOIN song_genres sg ON s.song_id = sg.song_id
                JOIN song_artists sa ON s.song_id = sa.song_id
                JOIN artists a ON sa.artist_id = a.artist_id
                WHERE sg.genre_id = @GenreId
                ORDER BY s.title";

            var songs = await connection.QueryAsync(sql, new { GenreId = id });

            return Ok(songs);
        }

        // POST: api/Genres/5/AddSong/10
        [HttpPost("{genreId}/AddSong/{songId}")]
        public async Task<IActionResult> AddSongToGenre(int genreId, int songId)
        {
            using var connection = CreateConnection();
            var sql = "INSERT INTO song_genres (song_id, genre_id) VALUES (@SongId, @GenreId)";

            try
            {
                await connection.ExecuteAsync(sql, new { SongId = songId, GenreId = genreId });
            }
            catch (SqliteException)
            {
                // If the insert fails, it's likely because the relationship already exists
                return BadRequest("Song is already associated with this genre");
            }

            return NoContent();
        }

        // DELETE: api/Genres/5/RemoveSong/10
        [HttpDelete("{genreId}/RemoveSong/{songId}")]
        public async Task<IActionResult> RemoveSongFromGenre(int genreId, int songId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM song_genres WHERE song_id = @SongId AND genre_id = @GenreId";
            var affected = await connection.ExecuteAsync(sql, new { SongId = songId, GenreId = genreId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Genres/Popular
        [HttpGet("Popular")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPopularGenres(int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    g.genre_id AS GenreId, 
                    g.name AS Name, 
                    COUNT(DISTINCT s.song_id) AS SongCount
                FROM genres g
                JOIN song_genres sg ON g.genre_id = sg.genre_id
                JOIN songs s ON sg.song_id = s.song_id
                GROUP BY g.genre_id
                ORDER BY SongCount DESC
                LIMIT @Limit";

            var popularGenres = await connection.QueryAsync(sql, new { Limit = limit });

            return Ok(popularGenres);
        }
    }
}