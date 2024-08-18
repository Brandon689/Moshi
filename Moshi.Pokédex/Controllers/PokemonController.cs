using Microsoft.AspNetCore.Mvc;
using Moshi.Pokédex.Models;
using Moshi.Pokédex.Services;
using PokeApiNet;

namespace Moshi.Pokédex.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PokemonController : ControllerBase
{
    private readonly PokemonService _pokemonService;

    public PokemonController(PokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }


    // GET: api/<PokemonController>
    [HttpGet("{pokemonName}")]
    public async Task<ActionResult<PokemonDto>> GetPokemon(string pokemonName)
    {
        try
        {
            var pokemon = await _pokemonService.GetPokemon(pokemonName);
            if (pokemon == null)
            {
                return NotFound($"Pokemon '{pokemonName}' not found.");
            }
            return Ok(pokemon);
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // POST api/<PokemonController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<PokemonController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<PokemonController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}