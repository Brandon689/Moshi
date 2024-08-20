const API_URL = 'https://localhost:7016/api';

export async function getPokemon(name) {
  const response = await fetch(`${API_URL}/Pokemon/${name}`);
  if (!response.ok) {
    throw new Error('Pokemon not found');
  }
  return response.json();
}
