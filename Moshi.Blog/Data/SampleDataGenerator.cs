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

        private const int NumberOfUsers = 10;
        private const int NumberOfCategories = 5;
        private const int PostsPerUser = 5;
        private const int CommentsPerPost = 3;

        public SampleDataGenerator(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository, CategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task GenerateSampleDataIfNeeded()
        {
            if (await IsSampleDataPresent())
            {
                Console.WriteLine("Sample data already exists. Skipping generation.");
                return;
            }

            Console.WriteLine("Generating sample data...");

            var categories = await GenerateCategories();
            var users = await GenerateUsers();
            await GeneratePosts(users, categories);

            Console.WriteLine("Sample data generation completed.");
        }

        private async Task<bool> IsSampleDataPresent()
        {
            var userCount = await _userRepository.GetUserCount();
            //var categoryCount = await _categoryRepository.GetCategoryCount();
            //var postCount = await _postRepository.GetPostCount();

            return userCount > 0; //&& categoryCount > 0 && postCount > 0;
        }

        private async Task<List<Category>> GenerateCategories()
        {
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.Department())
                .RuleFor(c => c.Description, f => f.Lorem.Sentence());

            var categories = new List<Category>();
            for (int i = 0; i < NumberOfCategories; i++)
            {
                var category = categoryFaker.Generate();
                await _categoryRepository.CreateCategory(category);
                categories.Add(category);
            }

            return categories;
        }

        private async Task<List<User>> GenerateUsers()
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                .RuleFor(u => u.IsAdmin, f => f.Random.Bool(0.1f));

            var users = new List<User>();
            for (int i = 0; i < NumberOfUsers; i++)
            {
                var user = userFaker.Generate();
                await _userRepository.CreateUser(user);
                users.Add(user);
            }

            return users;
        }

        private async Task GeneratePosts(List<User> users, List<Category> categories)
        {
            var postFaker = new Faker<Post>()
                .RuleFor(p => p.Title, f => f.Lorem.Sentence())
                .RuleFor(p => p.Content, f => f.Lorem.Paragraphs(3))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
                .RuleFor(p => p.UpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.Now))
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id);

            var commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Content, f => f.Lorem.Paragraph())
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent())
                .RuleFor(c => c.UserId, f => f.PickRandom(users).Id);

            foreach (var user in users)
            {
                for (int j = 0; j < PostsPerUser; j++)
                {
                    var post = postFaker.Generate();
                    post.AuthorId = user.Id;
                    await _postRepository.CreatePost(post);

                    for (int k = 0; k < CommentsPerPost; k++)
                    {
                        var comment = commentFaker.Generate();
                        comment.PostId = post.Id;
                        await _commentRepository.CreateComment(comment);
                    }
                }
            }
        }
    }
}
