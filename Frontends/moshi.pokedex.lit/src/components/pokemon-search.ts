import { LitElement, html, css } from 'lit';

export class PokemonSearch extends LitElement {
  static styles = css`
    input, button {
      font-size: 1em;
      padding: 0.5em;
      margin: 0.5em;
    }
  `;

  static properties = {
    value: { type: String },
  };

  constructor() {
    super();
    this.value = '';
  }

  render() {
    return html`
      <div>
        <input
          type="text"
          .value="${this.value}"
          @input="${this._handleInput}"
          placeholder="Enter Pokémon name"
        />
        <button @click="${this._handleSearch}">Search</button>
      </div>
    `;
  }

  _handleInput(e) {
    this.value = e.target.value;
  }

  _handleSearch() {
    this.dispatchEvent(new CustomEvent('search', {
      detail: { pokemonName: this.value },
      bubbles: true,
      composed: true
    }));
  }
}

customElements.define('pokemon-search', PokemonSearch);
