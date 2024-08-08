using Moshi.Forums.Data;
using Moshi.Forums.Models;

namespace Moshi.Forums.Services;

public class SearchService
{
    private readonly ThreadRepository _threadRepository;
    private readonly PostRepository _postRepository;

    public SearchService(ThreadRepository threadRepository, PostRepository postRepository)
    {
        _threadRepository = threadRepository;
        _postRepository = postRepository;
    }

    public async Task<IEnumerable<SearchResult>> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<SearchResult>();
        }

        var threadResults = await _threadRepository.SearchAsync(query);
        var postResults = await _postRepository.SearchAsync(query);

        var results = new List<SearchResult>();

        foreach (var thread in threadResults)
        {
            results.Add(new SearchResult
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = ExtractRelevantContent(thread.Title, query),
                Type = "Thread",
                CreatedAt = thread.CreatedAt,
                AuthorUsername = thread.Username,
                ForumId = thread.ForumId,
                ForumName = thread.ForumName,
                ThreadId = null
            });
        }

        foreach (var post in postResults)
        {
            results.Add(new SearchResult
            {
                Id = post.Id,
                Title = $"Re: {post.ThreadTitle}",
                Content = ExtractRelevantContent(post.Content, query),
                Type = "Post",
                CreatedAt = post.CreatedAt,
                AuthorUsername = post.Username,
                ForumId = post.ForumId,
                ForumName = post.ForumName,
                ThreadId = post.ThreadId
            });
        }

        return results.OrderByDescending(r => CalculateRelevance(r, query))
                      .ThenByDescending(r => r.CreatedAt);
    }

    private string ExtractRelevantContent(string content, string query)
    {
        const int contextLength = 50;
        var index = content.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (index == -1)
        {
            return content.Length <= contextLength * 2
                ? content
                : content.Substring(0, contextLength * 2) + "...";
        }

        var start = Math.Max(0, index - contextLength);
        var length = Math.Min(content.Length - start, query.Length + contextLength * 2);
        var result = content.Substring(start, length);
        if (start > 0) result = "..." + result;
        if (start + length < content.Length) result += "...";
        return result;
    }

    private int CalculateRelevance(SearchResult result, string query)
    {
        var titleMatches = result.Title.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(word => word.Equals(query, StringComparison.OrdinalIgnoreCase));
        var contentMatches = result.Content.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(word => word.Equals(query, StringComparison.OrdinalIgnoreCase));
        return titleMatches * 2 + contentMatches;
    }
}
