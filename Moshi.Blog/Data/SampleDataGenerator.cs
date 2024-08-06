using Bogus;
using Moshi.Blog.Models;

namespace Moshi.Blog.Data
{
    public class SampleDataGenerator
    {
        private readonly UserRepository _userRepository;
        private readonly PostRepository _postRepository;
        private readonly CommentRepository _commentRepository;
        private readonly CategoryRepository _categoryRepository;

        public SampleDataGenerator(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository, CategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task GenerateSampleData()
        {
            var categories = await GenerateCategories();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past())
                .RuleFor(u => u.IsAdmin, f => f.Random.Bool(0.1f));

            var postFaker = new Faker<Post>()
                .RuleFor(p => p.Title, f => f.Lorem.Sentence())
                .RuleFor(p => p.Content, f => f.Lorem.Paragraphs())
                .RuleFor(p => p.CreatedAt, f => f.Date.Past())
                .RuleFor(p => p.UpdatedAt, f => f.Date.Recent())
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id);

            var commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Content, f => f.Lorem.Paragraph())
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent());

            for (int i = 0; i < 10; i++)
            {
                var user = userFaker.Generate();
                await _userRepository.CreateUser(user);

                for (int j = 0; j < 5; j++)
                {
                    var post = postFaker.Generate();
                    post.AuthorId = user.Id;
                    await _postRepository.CreatePost(post);

                    for (int k = 0; k < 3; k++)
                    {
                        var comment = commentFaker.Generate();
                        comment.PostId = post.Id;
                        comment.UserId = user.Id;
                        await _commentRepository.CreateComment(comment);
                    }
                }
            }
        }

        private async Task<List<Category>> GenerateCategories()
        {
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Lorem.Word())
                .RuleFor(c => c.Description, f => f.Lorem.Sentence());

            var categories = new List<Category>();
            for (int i = 0; i < 5; i++)
            {
                var category = categoryFaker.Generate();
                await _categoryRepository.CreateCategory(category);
                categories.Add(category);
            }

            return categories;
        }
    }
}
