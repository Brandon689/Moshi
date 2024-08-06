using Moshi.Forums.Data;
using Moshi.Forums.Models;
namespace Moshi.Forums.Services;
public class ThreadService
{
    private readonly ThreadRepository _repository;

    public ThreadService(ThreadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ForumThread>> GetAllThreadsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ForumThread> GetThreadByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> CreateThreadAsync(ForumThread thread)
    {
        thread.CreatedAt = DateTime.UtcNow;
        thread.UpdatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(thread);
    }

    public async Task<ForumThread> UpdateThreadAsync(int id, ForumThread thread)
    {
        var existingThread = await _repository.GetByIdAsync(id);
        if (existingThread == null)
        {
            return null;
        }

        existingThread.Title = thread.Title;
        existingThread.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existingThread);
        return existingThread;
    }

    public async Task<bool> DeleteThreadAsync(int id)
    {
        var thread = await _repository.GetByIdAsync(id);
        if (thread == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }
}
