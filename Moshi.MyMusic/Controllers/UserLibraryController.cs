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
    public class UserLibraryController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public UserLibraryController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/UserLibrary/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<UserLibraryItem>>> GetUserLibrary(int userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM user_library WHERE user_id = @UserId ORDER BY added_at DESC";
            var items = await connection.QueryAsync<UserLibraryItem>(sql, new { UserId = userId });
            return Ok(items);
        }

        // POST: api/UserLibrary
        [HttpPost]
        public async Task<IActionResult> AddToLibrary(UserLibraryItem item)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO user_library (user_id, item_type, item_id, added_at)
                        VALUES (@UserId, @ItemType, @ItemId, @AddedAt)";

            item.AddedAt = DateTime.UtcNow;

            try
            {
                await connection.ExecuteAsync(sql, item);
            }
            catch (SqliteException)
            {
                // If the insert fails, it's likely because the item already exists in the library
                return BadRequest("Item already exists in the user's library");
            }

            return CreatedAtAction(nameof(GetUserLibrary), new { userId = item.UserId }, item);
        }

        // DELETE: api/UserLibrary/5/song/10
        [HttpDelete("{userId}/{itemType}/{itemId}")]
        public async Task<IActionResult> RemoveFromLibrary(int userId, string itemType, int itemId)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM user_library 
                        WHERE user_id = @UserId AND item_type = @ItemType AND item_id = @ItemId";

            var affected = await connection.ExecuteAsync(sql, new { UserId = userId, ItemType = itemType, ItemId = itemId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/UserLibrary/5/songs
        [HttpGet("{userId}/songs")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetUserSongs(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT s.*, a.name AS artist_name
                        FROM user_library ul
                        JOIN songs s ON ul.item_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE ul.user_id = @UserId AND ul.item_type = 'song'
                        ORDER BY ul.added_at DESC";

            var songs = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(songs);
        }

        // GET: api/UserLibrary/5/albums
        [HttpGet("{userId}/albums")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetUserAlbums(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT a.*, ar.name AS artist_name
                        FROM user_library ul
                        JOIN albums a ON ul.item_id = a.album_id
                        JOIN artists ar ON a.artist_id = ar.artist_id
                        WHERE ul.user_id = @UserId AND ul.item_type = 'album'
                        ORDER BY ul.added_at DESC";

            var albums = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(albums);
        }

        // GET: api/UserLibrary/5/artists
        [HttpGet("{userId}/artists")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetUserArtists(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT a.*
                        FROM user_library ul
                        JOIN artists a ON ul.item_id = a.artist_id
                        WHERE ul.user_id = @UserId AND ul.item_type = 'artist'
                        ORDER BY ul.added_at DESC";

            var artists = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(artists);
        }

        // GET: api/UserLibrary/5/RecentlyAdded
        [HttpGet("{userId}/RecentlyAdded")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetRecentlyAdded(int userId, int limit = 20)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT ul.*, 
                        CASE 
                            WHEN ul.item_type = 'song' THEN s.title
                            WHEN ul.item_type = 'album' THEN al.title
                            WHEN ul.item_type = 'artist' THEN ar.name
                        END AS item_name
                        FROM user_library ul
                        LEFT JOIN songs s ON ul.item_type = 'song' AND ul.item_id = s.song_id
                        LEFT JOIN albums al ON ul.item_type = 'album' AND ul.item_id = al.album_id
                        LEFT JOIN artists ar ON ul.item_type = 'artist' AND ul.item_id = ar.artist_id
                        WHERE ul.user_id = @UserId
                        ORDER BY ul.added_at DESC
                        LIMIT @Limit";

            var recentlyAdded = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(recentlyAdded);
        }

        // GET: api/UserLibrary/5/Contains/song/10
        [HttpGet("{userId}/Contains/{itemType}/{itemId}")]
        public async Task<ActionResult<bool>> CheckLibraryContains(int userId, string itemType, int itemId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT COUNT(*) FROM user_library 
                        WHERE user_id = @UserId AND item_type = @ItemType AND item_id = @ItemId";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ItemType = itemType, ItemId = itemId });

            return Ok(count > 0);
        }
    }
}