using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;

namespace Moshi.SubtitlesSite.Services;

public class SubtitleRequestService
{
    private readonly SubtitleRequestRepository _subtitleRequestRepository;

    public SubtitleRequestService(SubtitleRequestRepository subtitleRequestRepository)
    {
        _subtitleRequestRepository = subtitleRequestRepository;
    }

    public IEnumerable<SubtitleRequest> GetSubtitleRequests(int count)
    {
        return _subtitleRequestRepository.GetSubtitleRequests(count);
    }

    public IEnumerable<SubtitleRequest> GetAllRequests()
    {
        return _subtitleRequestRepository.GetAllSubtitleRequests();
    }

    public SubtitleRequest GetRequestById(int id)
    {
        return _subtitleRequestRepository.GetSubtitleRequestById(id);
    }

    public int CreateRequest(SubtitleRequest request)
    {
        return _subtitleRequestRepository.CreateSubtitleRequest(request);
    }

    public bool UpdateRequest(SubtitleRequest request)
    {
        return _subtitleRequestRepository.UpdateSubtitleRequest(request);
    }

    public bool DeleteRequest(int id)
    {
        return _subtitleRequestRepository.DeleteSubtitleRequest(id);
    }

    public IEnumerable<SubtitleRequest> SearchRequests(string query)
    {
        return _subtitleRequestRepository.SearchSubtitleRequests(query);
    }

    public IEnumerable<SubtitleRequest> GetRecentRequests(int count)
    {
        return _subtitleRequestRepository.GetMostRecentSubtitleRequests(count);
    }

    public IEnumerable<SubtitleRequest> GetRequestsByLanguage(string language)
    {
        return _subtitleRequestRepository.GetSubtitleRequestsByLanguage(language);
    }

    public int GetTotalRequestCount()
    {
        return _subtitleRequestRepository.GetTotalSubtitleRequestCount();
    }

    public IEnumerable<SubtitleRequest> GetRequestsByFirstLetter(char letter)
    {
        if (letter == '0')
        {
            return GetAllRequests().Where(r => char.IsDigit(r.MovieName[0]));
        }
        return GetAllRequests().Where(r => r.MovieName.StartsWith(letter.ToString(), StringComparison.OrdinalIgnoreCase));
    }
}
