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
public class ArtistsController : ControllerBase
{
    private readonly DatabaseConfig _databaseConfig;

    public ArtistsController(IOptions<DatabaseConfig> databaseConfig)
    {
        _databaseConfig = databaseConfig.Value;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_databaseConfig.ConnectionString);
    }

    // GET: api/Artists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Artist>>> GetArtists()
    {
        using var connection = CreateConnection();
        var artists = await connection.QueryAsync<Artist>(@"
            SELECT 
                artist_id AS ArtistId, 
                name AS Name, 
                bio AS Bio, 
                country AS Country, 
                formed_year AS FormedYear, 
                website AS Website 
            FROM artists");
        return Ok(artists);
    }

    // GET: api/Artists/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Artist>> GetArtist(int id)
    {
        using var connection = CreateConnection();
        var artist = await connection.QuerySingleOrDefaultAsync<Artist>(@"
            SELECT 
                artist_id AS ArtistId, 
                name AS Name, 
                bio AS Bio, 
                country AS Country, 
                formed_year AS FormedYear, 
                website AS Website 
            FROM artists 
            WHERE artist_id = @Id", new { Id = id });

        if (artist == null)
        {
            return NotFound();
        }

        return Ok(artist);
    }

    // POST: api/Artists
    [HttpPost]
    public async Task<ActionResult<Artist>> PostArtist(Artist artist)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO artists (name, bio, country, formed_year, website)
            VALUES (@Name, @Bio, @Country, @FormedYear, @Website);
            SELECT last_insert_rowid();";

        var id = await connection.ExecuteScalarAsync<int>(sql, artist);
        artist.ArtistId = id;

        return CreatedAtAction(nameof(GetArtist), new { id = artist.ArtistId }, artist);
    }

    // PUT: api/Artists/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutArtist(int id, Artist artist)
    {
        if (id != artist.ArtistId)
        {
            return BadRequest();
        }

        using var connection = CreateConnection();
        var sql = @"
            UPDATE artists 
            SET name = @Name, bio = @Bio, country = @Country, 
                formed_year = @FormedYear, website = @Website
            WHERE artist_id = @ArtistId";

        var affected = await connection.ExecuteAsync(sql, artist);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Artists/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtist(int id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM artists WHERE artist_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id });

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET: api/Artists/Search?name={name}
    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<Artist>>> SearchArtists(string name)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT 
                artist_id AS ArtistId, 
                name AS Name, 
                bio AS Bio, 
                country AS Country, 
                formed_year AS FormedYear, 
                website AS Website 
            FROM artists 
            WHERE name LIKE @Name";
        var artists = await connection.QueryAsync<Artist>(sql, new { Name = $"%{name}%" });

        return Ok(artists);
    }
}