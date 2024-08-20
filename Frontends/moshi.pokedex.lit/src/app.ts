import { LitElement, html } from 'lit';
import './components/pokemon-search.ts';
import './components/pokemon-display.ts';
import { getPokemon } from './services/pokemon-service.ts';

export class PokemonApp extends LitElement {
  static properties = {
    pokemonData: { type: Object },
    error: { type: String },
  };

  constructor() {
    super();
    this.pokemonData = null;
    this.error = '';
  }

  render() {
    return html`
      <h1>Pokédex</h1>
      <pokemon-search @search="${this._handleSearch}"></pokemon-search>
      ${this.error ? html`<p style="color: red;">${this.error}</p>` : ''}
      <pokemon-display .pokemon="${this.pokemonData}"></pokemon-display>
    `;
  }

  async _handleSearch(e) {
    const { pokemonName } = e.detail;
    try {
      this.error = '';
      this.pokemonData = null;
      this.pokemonData = await getPokemon(pokemonName.toLowerCase());
    } catch (error) {
      this.error = 'Pokémon not found. Please try another name.';
    }
  }
}

customElements.define('pokemon-app', PokemonApp);
