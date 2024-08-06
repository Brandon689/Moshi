using Moshi.Blog.Data;
using Moshi.Blog.Models;

namespace Moshi.Blog.Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepository;

        public CommentService(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
        {
            return await _commentRepository.GetCommentsByPostId(postId);
        }

        public async Task<Comment> CreateComment(Comment comment)
        {
            return await _commentRepository.CreateComment(comment);
        }

        public async Task<Comment> UpdateComment(int id, Comment comment)
        {
            comment.Id = id;
            return await _commentRepository.UpdateComment(comment);
        }

        public async Task<bool> DeleteComment(int id)
        {
            return await _commentRepository.DeleteComment(id);
        }
    }
}