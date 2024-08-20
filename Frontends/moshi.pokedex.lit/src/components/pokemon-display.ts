import { LitElement, html, css } from 'lit';

export class PokemonDisplay extends LitElement {
  static styles = css`
    .pokemon-card {
      border: 1px solid #ddd;
      border-radius: 8px;
      padding: 1em;
      max-width: 300px;
      margin: 1em auto;
    }
    img {
      max-width: 100%;
      height: auto;
    }
  `;

  static properties = {
    pokemon: { type: Object },
  };

  render() {
    if (!this.pokemon) {
      return html`<p>No Pokémon data to display</p>`;
    }

    const { name, id, height, weight, types, spriteUrl } = this.pokemon;

    return html`
      <div class="pokemon-card">
        <h2>${name} (#${id})</h2>
        <img src="${spriteUrl}" alt="${name}">
        <p>Height: ${height}</p>
        <p>Weight: ${weight}</p>
        <p>Types: ${types.join(', ')}</p>
      </div>
    `;
  }
}

customElements.define('pokemon-display', PokemonDisplay);
