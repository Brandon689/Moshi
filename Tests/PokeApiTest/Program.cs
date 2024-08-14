using PokeApiNet;

PokeApiClient pokeClient = new PokeApiClient(local: true);

// get a resource by name
Pokemon hoOh = await pokeClient.GetResourceAsync<Pokemon>("ho-oh");

// ... or by id
Item clawFossil = await pokeClient.GetResourceAsync<Item>(100);