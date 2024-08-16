using Moshi.SubtitlesSite.Models;
using SubtitlesSite.Data;

namespace Moshi.SubtitlesSite.Services;

public class ShowsService
{
    private readonly SubtitleRepository _repository;

    public ShowsService(SubtitleRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Show> GetAllShows()
    {
        return _repository.GetAllShows();
    }

    public Show GetShowById(int id)
    {
        return _repository.GetShowById(id);
    }

    public int CreateShow(Show show)
    {
        return _repository.CreateShow(show);
    }

    public bool UpdateShow(Show show)
    {
        return _repository.UpdateShow(show);
    }

    public bool DeleteShow(int id)
    {
        return _repository.DeleteShow(id);
    }

    public IEnumerable<Show> SearchShows(string query)
    {
        return _repository.SearchShows(query);
    }

    public IEnumerable<Subtitle> GetShowSubtitles(int showId)
    {
        return _repository.GetSubtitlesByShowId(showId);
    }
}