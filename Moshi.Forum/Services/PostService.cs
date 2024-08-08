using Moshi.Forums.Data;
using Moshi.Forums.Models;
namespace Moshi.Forums.Services;
public class PostService
{
    private readonly PostRepository _repository;

    public PostService(PostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> CreatePostAsync(Post post)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(post);
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
