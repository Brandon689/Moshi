using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public SearchController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Search?query=example&type=all
        [HttpGet]
        public async Task<ActionResult<SearchResult>> Search(string query, string type = "all")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var result = new SearchResult();
            using var connection = CreateConnection();

            switch (type.ToLower())
            {
                case "all":
                    result.Songs = await SearchSongs(connection, query);
                    result.Albums = await SearchAlbums(connection, query);
                    result.Artists = await SearchArtists(connection, query);
                    result.Playlists = await SearchPlaylists(connection, query);
                    result.Podcasts = await SearchPodcasts(connection, query);
                    break;
                case "songs":
                    result.Songs = await SearchSongs(connection, query);
                    break;
                case "albums":
                    result.Albums = await SearchAlbums(connection, query);
                    break;
                case "artists":
                    result.Artists = await SearchArtists(connection, query);
                    break;
                case "playlists":
                    result.Playlists = await SearchPlaylists(connection, query);
                    break;
                case "podcasts":
                    result.Podcasts = await SearchPodcasts(connection, query);
                    break;
                default:
                    return BadRequest("Invalid search type.");
            }

            return Ok(result);
        }

        private async Task<IEnumerable<dynamic>> SearchSongs(IDbConnection connection, string query)
        {
            var sql = @"
                SELECT s.*, a.name AS artist_name
                FROM songs s
                JOIN song_artists sa ON s.song_id = sa.song_id
                JOIN artists a ON sa.artist_id = a.artist_id
                WHERE s.title LIKE @Query OR a.name LIKE @Query
                ORDER BY s.title
                LIMIT 20";

            return await connection.QueryAsync(sql, new { Query = $"%{query}%" });
        }

        private async Task<IEnumerable<dynamic>> SearchAlbums(IDbConnection connection, string query)
        {
            var sql = @"
                SELECT al.*, a.name AS artist_name
                FROM albums al
                JOIN artists a ON al.artist_id = a.artist_id
                WHERE al.title LIKE @Query OR a.name LIKE @Query
                ORDER BY al.title
                LIMIT 20";

            return await connection.QueryAsync(sql, new { Query = $"%{query}%" });
        }

        private async Task<IEnumerable<dynamic>> SearchArtists(IDbConnection connection, string query)
        {
            var sql = @"
                SELECT *
                FROM artists
                WHERE name LIKE @Query
                ORDER BY name
                LIMIT 20";

            return await connection.QueryAsync(sql, new { Query = $"%{query}%" });
        }

        private async Task<IEnumerable<dynamic>> SearchPlaylists(IDbConnection connection, string query)
        {
            var sql = @"
                SELECT p.*, u.username AS creator_name
                FROM playlists p
                JOIN users u ON p.user_id = u.user_id
                WHERE p.name LIKE @Query AND p.is_public = 1
                ORDER BY p.name
                LIMIT 20";

            return await connection.QueryAsync(sql, new { Query = $"%{query}%" });
        }

        private async Task<IEnumerable<dynamic>> SearchPodcasts(IDbConnection connection, string query)
        {
            var sql = @"
                SELECT *
                FROM podcasts
                WHERE title LIKE @Query OR publisher LIKE @Query
                ORDER BY title
                LIMIT 20";

            return await connection.QueryAsync(sql, new { Query = $"%{query}%" });
        }

        // GET: api/Search/Songs?query=example
        [HttpGet("Songs")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchSongsOnly(string query)
        {
            using var connection = CreateConnection();
            var songs = await SearchSongs(connection, query);
            return Ok(songs);
        }

        // GET: api/Search/Albums?query=example
        [HttpGet("Albums")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchAlbumsOnly(string query)
        {
            using var connection = CreateConnection();
            var albums = await SearchAlbums(connection, query);
            return Ok(albums);
        }

        // GET: api/Search/Artists?query=example
        [HttpGet("Artists")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchArtistsOnly(string query)
        {
            using var connection = CreateConnection();
            var artists = await SearchArtists(connection, query);
            return Ok(artists);
        }

        // GET: api/Search/Playlists?query=example
        [HttpGet("Playlists")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchPlaylistsOnly(string query)
        {
            using var connection = CreateConnection();
            var playlists = await SearchPlaylists(connection, query);
            return Ok(playlists);
        }

        // GET: api/Search/Podcasts?query=example
        [HttpGet("Podcasts")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchPodcastsOnly(string query)
        {
            using var connection = CreateConnection();
            var podcasts = await SearchPodcasts(connection, query);
            return Ok(podcasts);
        }
    }

    public class SearchResult
    {
        public IEnumerable<dynamic> Songs { get; set; }
        public IEnumerable<dynamic> Albums { get; set; }
        public IEnumerable<dynamic> Artists { get; set; }
        public IEnumerable<dynamic> Playlists { get; set; }
        public IEnumerable<dynamic> Podcasts { get; set; }
    }
}