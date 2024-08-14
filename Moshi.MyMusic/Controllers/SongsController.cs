using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System.Data;

namespace Moshi.MyMusic.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SongsController : ControllerBase
{
    private readonly DatabaseConfig _databaseConfig;

    public SongsController(IOptions<DatabaseConfig> databaseConfig)
    {
        _databaseConfig = databaseConfig.Value;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_databaseConfig.ConnectionString);
    }

    // GET: api/Songs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
    {
        using var connection = CreateConnection();
        var songs = await connection.QueryAsync<Song>("SELECT * FROM songs");
        return Ok(songs);
    }

    // GET: api/Songs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Song>> GetSong(int id)
    {
        using var connection = CreateConnection();
        var song = await connection.QuerySingleOrDefaultAsync<Song>(
            "SELECT * FROM songs WHERE song_id = @Id", new { Id = id });

        if (song == null)
        {
            return NotFound();
        }

        return Ok(song);
    }

    // POST: api/Songs
    [HttpPost]
    public async Task<ActionResult<Song>> PostSong(Song song)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO songs (title, album_id, duration, track_number, explicit, lyrics, audio_file_path)
                        VALUES (@Title, @AlbumId, @Duration, @TrackNumber, @Explicit, @Lyrics, @AudioFilePath);
                        SELECT last_insert_rowid();";

        var id = await connection.ExecuteScalarAsync<int>(sql, song);
        song.SongId = id;

        return CreatedAtAction(nameof(GetSong), new { id = song.SongId }, song);
    }

    // PUT: api/Songs/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSong(int id, Song song)
    {
        if (id != song.SongId)
        {
            return BadRequest();
        }

        using var connection = CreateConnection();
        var sql = @"UPDATE songs 
                        SET title = @Title, album_id = @AlbumId, duration = @Duration, 
                            track_number = @TrackNumber, explicit = @Explicit, 
                            lyrics = @Lyrics, audio_file_path = @AudioFilePath
                        WHERE song_id = @SongId";

        var affected = await connection.ExecuteAsync(sql, song);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Songs/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSong(int id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM songs WHERE song_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET: api/Songs/ByAlbum/5
    [HttpGet("ByAlbum/{albumId}")]
    public async Task<ActionResult<IEnumerable<Song>>> GetSongsByAlbum(int albumId)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM songs WHERE album_id = @AlbumId ORDER BY track_number";
        var songs = await connection.QueryAsync<Song>(sql, new { AlbumId = albumId });

        return Ok(songs);
    }

    // GET: api/Songs/Search?title={title}
    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<Song>>> SearchSongs(string title)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM songs WHERE title LIKE @Title";
        var songs = await connection.QueryAsync<Song>(sql, new { Title = $"%{title}%" });

        return Ok(songs);
    }

    // GET: api/Songs/TopSongs
    [HttpGet("TopSongs")]
    public async Task<ActionResult<IEnumerable<dynamic>>> GetTopSongs(int limit = 10)
    {
        using var connection = CreateConnection();
        var sql = @"
                SELECT s.*, a.name AS artist_name, COUNT(*) AS listen_count
                FROM listening_history lh
                JOIN songs s ON lh.song_id = s.song_id
                JOIN song_artists sa ON s.song_id = sa.song_id
                JOIN artists a ON sa.artist_id = a.artist_id
                GROUP BY s.song_id
                ORDER BY listen_count DESC
                LIMIT @Limit";

        var topSongs = await connection.QueryAsync(sql, new { Limit = limit });

        return Ok(topSongs);
    }
}