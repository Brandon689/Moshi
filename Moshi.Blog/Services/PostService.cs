using Moshi.Blog.Data;
using Moshi.Blog.Models;

namespace Moshi.Blog.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;

        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            return await _postRepository.GetAllPosts();
        }

        public async Task<Post> GetPostById(int id)
        {
            return await _postRepository.GetPostById(id);
        }

        public async Task<Post> CreatePost(Post post)
        {
            return await _postRepository.CreatePost(post);
        }

        public async Task<Post> UpdatePost(int id, Post post)
        {
            post.Id = id;
            return await _postRepository.UpdatePost(post);
        }

        public async Task<bool> DeletePost(int id)
        {
            return await _postRepository.DeletePost(id);
        }
    }
}