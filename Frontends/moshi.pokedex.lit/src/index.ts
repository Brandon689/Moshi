import { PokemonApp } from './app.ts';

// This line is usually not needed if you've already defined the custom element in app.js
// customElements.define('pokemon-app', PokemonApp);

document.body.appendChild(new PokemonApp());
