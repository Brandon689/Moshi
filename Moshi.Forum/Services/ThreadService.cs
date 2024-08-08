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
        var thread = await _repository.GetByIdAsync(id);
        if (thread != null)
        {
            await _repository.IncrementViewCountAsync(id);
        }
        return thread;
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

    public async Task<IEnumerable<ForumThread>> GetThreadsByForumIdAsync(int forumId)
    {
        return await _repository.GetThreadsByForumIdAsync(forumId);
    }

    public async Task LockThreadAsync(int threadId)
    {
        await _repository.LockThreadAsync(threadId);
    }

    public async Task UnlockThreadAsync(int threadId)
    {
        await _repository.UnlockThreadAsync(threadId);
    }

    public async Task PinThreadAsync(int threadId)
    {
        await _repository.PinThreadAsync(threadId);
    }

    public async Task UnpinThreadAsync(int threadId)
    {
        await _repository.UnpinThreadAsync(threadId);
    }
}
