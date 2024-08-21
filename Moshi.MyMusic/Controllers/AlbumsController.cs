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
public class AlbumsController : ControllerBase
{
    private readonly DatabaseConfig _databaseConfig;

    public AlbumsController(IOptions<DatabaseConfig> databaseConfig)
    {
        _databaseConfig = databaseConfig.Value;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_databaseConfig.ConnectionString);
    }

    // GET: api/Albums
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Album>>> GetAlbums()
    {
        using var connection = CreateConnection();
        var albums = await connection.QueryAsync<Album>(@"
        SELECT 
            album_id AS AlbumId, 
            title AS Title, 
            artist_id AS ArtistId, 
            release_date AS ReleaseDate, 
            genre AS Genre, 
            label AS Label 
        FROM albums");
        return Ok(albums);
    }

    // GET: api/Albums/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Album>> GetAlbum(int id)
    {
        using var connection = CreateConnection();
        var album = await connection.QuerySingleOrDefaultAsync<Album>(@"
        SELECT 
            album_id AS AlbumId, 
            title AS Title, 
            artist_id AS ArtistId, 
            release_date AS ReleaseDate, 
            genre AS Genre, 
            label AS Label 
        FROM albums 
        WHERE album_id = @Id",
            new { Id = id });

        if (album == null)
        {
            return NotFound();
        }

        return Ok(album);
    }

    // POST: api/Albums
    [HttpPost]
    public async Task<ActionResult<Album>> PostAlbum(Album album)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO albums (title, artist_id, release_date, genre, label)
                        VALUES (@Title, @ArtistId, @ReleaseDate, @Genre, @Label);
                        SELECT last_insert_rowid();";

        var id = await connection.ExecuteScalarAsync<int>(sql, album);
        album.AlbumId = id;

        return CreatedAtAction(nameof(GetAlbum), new { id = album.AlbumId }, album);
    }

    // PUT: api/Albums/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAlbum(int id, Album album)
    {
        if (id != album.AlbumId)
        {
            return BadRequest();
        }

        using var connection = CreateConnection();
        var sql = @"UPDATE albums 
                        SET title = @Title, artist_id = @ArtistId, release_date = @ReleaseDate, 
                            genre = @Genre, label = @Label
                        WHERE album_id = @AlbumId";

        var affected = await connection.ExecuteAsync(sql, album);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Albums/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlbum(int id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM albums WHERE album_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET: api/Albums/ByArtist/5
    [HttpGet("ByArtist/{artistId}")]
    public async Task<ActionResult<IEnumerable<Album>>> GetAlbumsByArtist(int artistId)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM albums WHERE artist_id = @ArtistId ORDER BY release_date DESC";
        var albums = await connection.QueryAsync<Album>(sql, new { ArtistId = artistId });

        return Ok(albums);
    }

    // GET: api/Albums/Search?title={title}
    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<Album>>> SearchAlbums(string title)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM albums WHERE title LIKE @Title";
        var albums = await connection.QueryAsync<Album>(sql, new { Title = $"%{title}%" });

        return Ok(albums);
    }
}