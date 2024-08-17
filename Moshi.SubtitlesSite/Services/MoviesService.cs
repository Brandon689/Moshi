using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;

namespace Moshi.SubtitlesSite.Services;

public class MoviesService
{
    private readonly SubtitleRepository _repository;

    public MoviesService(SubtitleRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Movie> GetMoviesByFirstLetter(char letter)
    {
        if (letter == '0')
        {
            return _repository.GetAllMovies().Where(m => char.IsDigit(m.Title[0]));
        }
        return _repository.GetAllMovies().Where(m => m.Title.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _repository.GetAllMovies();
    }

    public Movie GetMovieById(int id)
    {
        return _repository.GetMovieById(id);
    }
    public IEnumerable<Movie> GetPopularMovies(int count)
    {
        return _repository.GetMoviesWithMostSubtitles(count);
    }
    public int CreateMovie(Movie movie)
    {
        return _repository.CreateMovie(movie);
    }

    public bool UpdateMovie(Movie movie)
    {
        return _repository.UpdateMovie(movie);
    }

    public bool DeleteMovie(int id)
    {
        return _repository.DeleteMovie(id);
    }

    public IEnumerable<Movie> SearchMovies(string query)
    {
        return _repository.SearchMovies(query);
    }

    public IEnumerable<Subtitle> GetMovieSubtitles(int movieId)
    {
        return _repository.GetSubtitlesByMovieId(movieId);
    }

    public IEnumerable<AlternativeTitle> GetAlternativeTitles(int movieId)
    {
        return _repository.GetAlternativeTitles(movieId);
    }

    public bool AddAlternativeTitle(AlternativeTitle alternativeTitle)
    {
        return _repository.AddAlternativeTitle(alternativeTitle);
    }

    public IEnumerable<MovieLink> GetMovieLinks(int movieId)
    {
        return _repository.GetMovieLinks(movieId);
    }

    public bool AddMovieLink(MovieLink movieLink)
    {
        return _repository.AddMovieLink(movieLink);
    }
}