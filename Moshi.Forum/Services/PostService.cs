using Moshi.Forums.Data;
using Moshi.Forums.Models;
namespace Moshi.Forums.Services;
public class PostService
{
    private readonly PostRepository _repository;
    private readonly ThreadRepository _threadRepository;

    public PostService(PostRepository repository, ThreadRepository threadRepository)
    {
        _repository = repository;
        _threadRepository = threadRepository;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        var postId = await _repository.CreateAsync(post);
        post.Id = postId;

        // Update the thread's reply count and last post information
        await _threadRepository.IncrementReplyCountAsync(post.ThreadId);
        await _threadRepository.UpdateLastPostAsync(post.ThreadId, postId, post.CreatedAt);

        return post;
    }

    public async Task<Post> UpdatePostAsync(int id, Post post)
    {
        var existingPost = await _repository.GetByIdAsync(id);
        if (existingPost == null)
        {
            return null;
        }

        existingPost.Content = post.Content;
        existingPost.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existingPost);
        return existingPost;
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<Post>> GetPostsByThreadIdAsync(int threadId)
    {
        return await _repository.GetPostsByThreadIdAsync(threadId);
    }
}
