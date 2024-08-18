using PokeApiNet;
using Moshi.Pokédex.Models;

namespace Moshi.Pokédex.Services
{
    public class PokemonService
    {
        private PokeApiClient _client;

        public PokemonService(PokeApiClient client)
        {
            _client = client;
        }

        public async Task<PokemonDto> GetPokemon(string pokemonName)
        {
            var pokemon = await _client.GetResourceAsync<Pokemon>(pokemonName);
            return MapToPokemonDto(pokemon);
        }

        private PokemonDto MapToPokemonDto(Pokemon pokemon)
        {
            return new PokemonDto
            {
                Id = pokemon.Id,
                Name = pokemon.Name,
                Height = pokemon.Height,
                Weight = pokemon.Weight,
                Types = pokemon.Types.Select(t => t.Type.Name).ToList(),
                SpriteUrl = pokemon.Sprites.FrontDefault
            };
        }
    }
}
