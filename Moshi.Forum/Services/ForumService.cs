using Moshi.Forums.Data;
using Moshi.Forums.Models;
namespace Moshi.Forums.Services;
public class ForumService
{
    private readonly ForumRepository _repository;

    public ForumService(ForumRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Forum>> GetAllForumsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Forum> GetForumByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> CreateForumAsync(Forum forum)
    {
        forum.CreatedAt = DateTime.UtcNow;
        forum.UpdatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(forum);
    }

    public async Task<Forum> UpdateForumAsync(int id, Forum forum)
    {
        var existingForum = await _repository.GetByIdAsync(id);
        if (existingForum == null)
        {
            return null;
        }

        existingForum.Name = forum.Name;
        existingForum.Description = forum.Description;
        existingForum.ParentForumId = forum.ParentForumId;
        existingForum.DisplayOrder = forum.DisplayOrder;
        existingForum.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existingForum);
        return existingForum;
    }

    public async Task<bool> DeleteForumAsync(int id)
    {
        var forum = await _repository.GetByIdAsync(id);
        if (forum == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }
}
