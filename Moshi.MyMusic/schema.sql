--Enable foreign key support
PRAGMA foreign_keys = ON;

--Users table
CREATE TABLE users (
    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL UNIQUE,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    date_of_birth DATE NOT NULL,
    country TEXT NOT NULL,
    premium_status BOOLEAN NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP
);

--Create index on username and email for faster lookups
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);

--Artists table
CREATE TABLE artists (
    artist_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    bio TEXT,
    country TEXT,
    formed_year INTEGER,
    website TEXT
);

--Create index on artist name for faster searches
CREATE INDEX idx_artists_name ON artists(name);

--Albums table
CREATE TABLE albums (
    album_id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    artist_id INTEGER NOT NULL,
    release_date DATE NOT NULL,
    genre TEXT,
    label TEXT,
    FOREIGN KEY (artist_id) REFERENCES artists(artist_id)
);

--Create index on album title and release date for faster searches
CREATE INDEX idx_albums_title ON albums(title);
CREATE INDEX idx_albums_release_date ON albums(release_date);

--Songs table
CREATE TABLE songs (
    song_id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    album_id INTEGER,
    duration INTEGER NOT NULL, -- in seconds
    track_number INTEGER,
    explicit BOOLEAN NOT NULL DEFAULT 0,
    lyrics TEXT,
    audio_file_path TEXT NOT NULL,
    FOREIGN KEY (album_id) REFERENCES albums(album_id)
);

--Create index on song title for faster searches
CREATE INDEX idx_songs_title ON songs(title);

--Junction table for songs and artists (allows for multiple artists per song)
CREATE TABLE song_artists (
    song_id INTEGER,
    artist_id INTEGER,
    role TEXT NOT NULL DEFAULT 'primary', -- e.g., 'primary', 'featured', 'producer'
    PRIMARY KEY (song_id, artist_id, role),
    FOREIGN KEY (song_id) REFERENCES songs(song_id),
    FOREIGN KEY (artist_id) REFERENCES artists(artist_id)
);

--Genres table
CREATE TABLE genres (
    genre_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

--Junction table for songs and genres (allows for multiple genres per song)
CREATE TABLE song_genres (
    song_id INTEGER,
    genre_id INTEGER,
    PRIMARY KEY (song_id, genre_id),
    FOREIGN KEY (song_id) REFERENCES songs(song_id),
    FOREIGN KEY (genre_id) REFERENCES genres(genre_id)
);

--Playlists table
CREATE TABLE playlists (
    playlist_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    user_id INTEGER NOT NULL,
    description TEXT,
    is_public BOOLEAN NOT NULL DEFAULT 1,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

--Create index on playlist name for faster searches
CREATE INDEX idx_playlists_name ON playlists(name);

--Junction table for playlists and songs
CREATE TABLE playlist_songs (
    playlist_id INTEGER,
    song_id INTEGER,
    position INTEGER NOT NULL,
    added_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (playlist_id, song_id),
    FOREIGN KEY (playlist_id) REFERENCES playlists(playlist_id),
    FOREIGN KEY (song_id) REFERENCES songs(song_id)
);

--User library table(for saved songs, albums, and artists)
CREATE TABLE user_library (
    user_id INTEGER,
    item_type TEXT NOT NULL, -- 'song', 'album', or 'artist'
    item_id INTEGER NOT NULL,
    added_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, item_type, item_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

--Create index on item_type and item_id for faster lookups
CREATE INDEX idx_user_library_item ON user_library(item_type, item_id);

--User followers table
CREATE TABLE user_followers (
    follower_id INTEGER,
    followed_id INTEGER,
    followed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (follower_id, followed_id),
    FOREIGN KEY (follower_id) REFERENCES users(user_id),
    FOREIGN KEY (followed_id) REFERENCES users(user_id)
);

--Artist followers table
CREATE TABLE artist_followers (
    user_id INTEGER,
    artist_id INTEGER,
    followed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, artist_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (artist_id) REFERENCES artists(artist_id)
);

--User listening history
CREATE TABLE listening_history (
    history_id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    song_id INTEGER NOT NULL,
    listened_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    duration_listened INTEGER NOT NULL, -- in seconds
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (song_id) REFERENCES songs(song_id)
);

--Create index on user_id and listened_at for faster queries
CREATE INDEX idx_listening_history_user_time ON listening_history(user_id, listened_at);

--User recommendations table
CREATE TABLE user_recommendations (
    recommendation_id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    item_type TEXT NOT NULL, -- 'song', 'album', 'artist', or 'playlist'
    item_id INTEGER NOT NULL,
    score REAL NOT NULL, -- recommendation score
    generated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

--Create index on user_id and score for faster retrieval of top recommendations
CREATE INDEX idx_user_recommendations_score ON user_recommendations(user_id, score DESC);

--Podcasts table
CREATE TABLE podcasts (
    podcast_id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    description TEXT,
    publisher TEXT NOT NULL,
    language TEXT NOT NULL,
    rss_feed_url TEXT NOT NULL UNIQUE
);

--Create index on podcast title for faster searches
CREATE INDEX idx_podcasts_title ON podcasts(title);

--Podcast episodes table
CREATE TABLE podcast_episodes (
    episode_id INTEGER PRIMARY KEY AUTOINCREMENT,
    podcast_id INTEGER NOT NULL,
    title TEXT NOT NULL,
    description TEXT,
    duration INTEGER NOT NULL, -- in seconds
    release_date TIMESTAMP NOT NULL,
    audio_file_path TEXT NOT NULL,
    FOREIGN KEY (podcast_id) REFERENCES podcasts(podcast_id)
);

--Create index on episode title and release date for faster searches
CREATE INDEX idx_podcast_episodes_title ON podcast_episodes(title);
CREATE INDEX idx_podcast_episodes_release_date ON podcast_episodes(release_date);

--User podcast subscriptions
CREATE TABLE podcast_subscriptions (
    user_id INTEGER,
    podcast_id INTEGER,
    subscribed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, podcast_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (podcast_id) REFERENCES podcasts(podcast_id)
);

--Trigger to update the 'updated_at' timestamp on playlists
CREATE TRIGGER update_playlist_timestamp
AFTER UPDATE ON playlists
FOR EACH ROW
BEGIN
    UPDATE playlists SET updated_at = CURRENT_TIMESTAMP WHERE playlist_id = OLD.playlist_id;
END;

--View for top songs by listen count
CREATE VIEW top_songs AS
SELECT s.song_id, s.title, a.name AS artist_name, COUNT(*) AS listen_count
FROM listening_history lh
JOIN songs s ON lh.song_id = s.song_id
JOIN song_artists sa ON s.song_id = sa.song_id
JOIN artists a ON sa.artist_id = a.artist_id
GROUP BY s.song_id
ORDER BY listen_count DESC;

--Full - text search virtual table for songs
CREATE VIRTUAL TABLE song_fts USING fts5(title, artist_name, album_title);

--Trigger to keep the FTS table up to date
CREATE TRIGGER song_fts_insert AFTER INSERT ON songs
BEGIN
    INSERT INTO song_fts(rowid, title, artist_name, album_title)
    SELECT NEW.song_id, NEW.title, a.name, al.title
    FROM songs s
    JOIN song_artists sa ON s.song_id = sa.song_id
    JOIN artists a ON sa.artist_id = a.artist_id
    LEFT JOIN albums al ON s.album_id = al.album_id
    WHERE s.song_id = NEW.song_id;
END;

CREATE TRIGGER song_fts_delete AFTER DELETE ON songs
BEGIN
    DELETE FROM song_fts WHERE rowid = OLD.song_id;
END;

CREATE TRIGGER song_fts_update AFTER UPDATE ON songs
BEGIN
    DELETE FROM song_fts WHERE rowid = OLD.song_id;
INSERT INTO song_fts(rowid, title, artist_name, album_title)
    SELECT NEW.song_id, NEW.title, a.name, al.title
    FROM songs s
    JOIN song_artists sa ON s.song_id = sa.song_id
    JOIN artists a ON sa.artist_id = a.artist_id
    LEFT JOIN albums al ON s.album_id = al.album_id
    WHERE s.song_id = NEW.song_id;
END;