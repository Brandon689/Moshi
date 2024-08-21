// User-related types
type User = {
    userId: number;
    username: string;
    email: string;
    passwordHash: string;
    dateOfBirth: Date;
    country: string;
    premiumStatus: boolean;
    createdAt: Date;
    lastLogin: Date | null;
  };
  
  type Artist = {
    artistId: number;
    name: string;
    bio: string | null;
    country: string | null;
    formedYear: number | null;
    website: string | null;
  };
  
  type Album = {
    albumId: number;
    title: string;
    artistId: number;
    releaseDate: Date;
    genre: string | null;
    label: string | null;
  };
  
  type Song = {
    songId: number;
    title: string;
    albumId: number | null;
    duration: number;
    trackNumber: number | null;
    explicit: boolean;
    lyrics: string | null;
    audioFilePath: string;
  };
  
  type SongArtist = {
    songId: number;
    artistId: number;
    role: string;
  };
  
  type Genre = {
    genreId: number;
    name: string;
  };
  
  type SongGenre = {
    songId: number;
    genreId: number;
  };
  
  type Playlist = {
    playlistId: number;
    name: string;
    userId: number;
    description: string | null;
    isPublic: boolean;
    createdAt: Date;
    updatedAt: Date;
  };
  
  type PlaylistSong = {
    playlistId: number;
    songId: number;
    position: number;
    addedAt: Date;
  };
  
  type UserLibraryItem = {
    userId: number;
    itemType: 'song' | 'album' | 'artist';
    itemId: number;
    addedAt: Date;
  };
  
  type UserFollower = {
    followerId: number;
    followedId: number;
    followedAt: Date;
  };
  
  type ArtistFollower = {
    userId: number;
    artistId: number;
    followedAt: Date;
  };
  
  type ListeningHistoryEntry = {
    historyId: number;
    userId: number;
    songId: number;
    listenedAt: Date;
    durationListened: number;
  };
  
  type UserRecommendation = {
    recommendationId: number;
    userId: number;
    itemType: 'song' | 'album' | 'artist' | 'playlist';
    itemId: number;
    score: number;
    generatedAt: Date;
  };
  
  type Podcast = {
    podcastId: number;
    title: string;
    description: string | null;
    publisher: string;
    language: string;
    rssFeedUrl: string;
  };
  
  type PodcastEpisode = {
    episodeId: number;
    podcastId: number;
    title: string;
    description: string | null;
    duration: number;
    releaseDate: Date;
    audioFilePath: string;
  };
  
  type PodcastSubscription = {
    userId: number;
    podcastId: number;
    subscribedAt: Date;
  };
  
  // View type
  type TopSong = {
    songId: number;
    title: string;
    artistName: string;
    listenCount: number;
  };
  
  // Full-text search type
  type SongSearchResult = {
    songId: number;
    title: string;
    artistName: string;
    albumTitle: string | null;
  };