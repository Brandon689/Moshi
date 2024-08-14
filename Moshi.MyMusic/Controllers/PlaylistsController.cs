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
    public class PlaylistsController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public PlaylistsController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            using var connection = CreateConnection();
            var playlists = await connection.QueryAsync<Playlist>("SELECT * FROM playlists");
            return Ok(playlists);
        }

        // GET: api/Playlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(int id)
        {
            using var connection = CreateConnection();
            var playlist = await connection.QuerySingleOrDefaultAsync<Playlist>(
                "SELECT * FROM playlists WHERE playlist_id = @Id", new { Id = id });

            if (playlist == null)
            {
                return NotFound();
            }

            return Ok(playlist);
        }

        // POST: api/Playlists
        [HttpPost]
        public async Task<ActionResult<Playlist>> CreatePlaylist(Playlist playlist)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO playlists (name, user_id, description, is_public, created_at, updated_at)
                        VALUES (@Name, @UserId, @Description, @IsPublic, @CreatedAt, @UpdatedAt);
                        SELECT last_insert_rowid();";

            playlist.CreatedAt = DateTime.UtcNow;
            playlist.UpdatedAt = playlist.CreatedAt;

            var id = await connection.ExecuteScalarAsync<int>(sql, playlist);
            playlist.PlaylistId = id;

            return CreatedAtAction(nameof(GetPlaylist), new { id = playlist.PlaylistId }, playlist);
        }

        // PUT: api/Playlists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(int id, Playlist playlist)
        {
            if (id != playlist.PlaylistId)
            {
                return BadRequest();
            }

            using var connection = CreateConnection();
            var sql = @"UPDATE playlists 
                        SET name = @Name, description = @Description, is_public = @IsPublic, updated_at = @UpdatedAt
                        WHERE playlist_id = @PlaylistId";

            playlist.UpdatedAt = DateTime.UtcNow;

            var affected = await connection.ExecuteAsync(sql, playlist);

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM playlists WHERE playlist_id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Playlists/User/5
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetUserPlaylists(int userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM playlists WHERE user_id = @UserId ORDER BY created_at DESC";
            var playlists = await connection.QueryAsync<Playlist>(sql, new { UserId = userId });

            return Ok(playlists);
        }

        // POST: api/Playlists/5/AddSong
        [HttpPost("{playlistId}/AddSong")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, [FromBody] int songId)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO playlist_songs (playlist_id, song_id, position, added_at)
                        VALUES (@PlaylistId, @SongId, (SELECT COALESCE(MAX(position), 0) + 1 FROM playlist_songs WHERE playlist_id = @PlaylistId), @AddedAt)";

            var affected = await connection.ExecuteAsync(sql, new { PlaylistId = playlistId, SongId = songId, AddedAt = DateTime.UtcNow });

            if (affected == 0)
            {
                return BadRequest("Failed to add song to playlist");
            }

            return NoContent();
        }

        // DELETE: api/Playlists/5/RemoveSong/10
        [HttpDelete("{playlistId}/RemoveSong/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM playlist_songs WHERE playlist_id = @PlaylistId AND song_id = @SongId";
            var affected = await connection.ExecuteAsync(sql, new { PlaylistId = playlistId, SongId = songId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Playlists/5/Songs
        [HttpGet("{playlistId}/Songs")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPlaylistSongs(int playlistId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT s.*, ps.position, ps.added_at
                        FROM playlist_songs ps
                        JOIN songs s ON ps.song_id = s.song_id
                        WHERE ps.playlist_id = @PlaylistId
                        ORDER BY ps.position";

            var songs = await connection.QueryAsync(sql, new { PlaylistId = playlistId });

            return Ok(songs);
        }
    }
}