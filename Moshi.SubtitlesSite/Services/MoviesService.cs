using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;

namespace Moshi.SubtitlesSite.Services;

public class MoviesService
{
    private readonly SubtitleRepository _subtitleRepository;
    private readonly MovieRepository _movieRepository;

    public MoviesService(SubtitleRepository subtitleRepository, MovieRepository movieRepository)
    {
        _subtitleRepository = subtitleRepository;
        _movieRepository = movieRepository;
    }

    public IEnumerable<Movie> GetMoviesByFirstLetter(char letter)
    {
        if (letter == '0')
        {
            return _movieRepository.GetAllMovies().Where(m => char.IsDigit(m.Title[0]));
        }
        return _movieRepository.GetAllMovies().Where(m => m.Title.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _movieRepository.GetAllMovies();
    }

    public Movie GetMovieById(int id)
    {
        return _movieRepository.GetMovieById(id);
    }
    public IEnumerable<Movie> GetPopularMovies(int count)
    {
        return _movieRepository.GetMoviesWithMostSubtitles(count);
    }
    public int CreateMovie(Movie movie)
    {
        return _movieRepository.CreateMovie(movie);
    }

    public bool UpdateMovie(Movie movie)
    {
        return _movieRepository.UpdateMovie(movie);
    }

    public bool DeleteMovie(int id)
    {
        return _movieRepository.DeleteMovie(id);
    }

    public IEnumerable<Movie> SearchMovies(string query)
    {
        return _movieRepository.SearchMovies(query);
    }

    public IEnumerable<Subtitle> GetMovieSubtitles(int movieId)
    {
        return _subtitleRepository.GetSubtitlesByMovieId(movieId);
    }

    public IEnumerable<AlternativeTitle> GetAlternativeTitles(int movieId)
    {
        return _movieRepository.GetAlternativeTitles(movieId);
    }

    public bool AddAlternativeTitle(AlternativeTitle alternativeTitle)
    {
        return _movieRepository.AddAlternativeTitle(alternativeTitle);
    }

    public IEnumerable<MovieLink> GetMovieLinks(int movieId)
    {
        return _movieRepository.GetMovieLinks(movieId);
    }

    public bool AddMovieLink(MovieLink movieLink)
    {
        return _movieRepository.AddMovieLink(movieLink);
    }
}