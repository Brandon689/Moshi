import React, { useState, useEffect } from 'react';


async function fetchAlbums(): Promise<Album[]> {
  try {
    const response = await fetch('https://localhost:7114/api/Albums', {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const albums: Album[] = await response.json();
    console.log(albums);
    return albums;
  } catch (error) {
    console.error('There was a problem fetching the albums:', error);
    throw error;
  }
}

// Usage
// fetchAlbums()
//   .then((albums) => {
//     console.log('Fetched albums:', albums);
//     // Process the albums here
//   })
//   .catch((error) => {
//     console.error('Error:', error);
//   });


const AlbumList: React.FC = () => {
  const [albums, setAlbums] = useState<Album[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchAlbums()
      .then(setAlbums)
      .catch((err) => setError(err.message));
  }, []);

  if (error) return <div>Error: {error}</div>;
  if (albums.length === 0) return <div>Loading...</div>;

  return (
    <ul>
      {albums.map((album) => (
        <li key={album.albumId}>{album.title}</li>
      ))}
    </ul>
  );
};

export default AlbumList;