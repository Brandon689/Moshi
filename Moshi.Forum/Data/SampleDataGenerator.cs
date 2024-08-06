using Bogus;
using Moshi.Forums.Models;
using Moshi.Forums.Services;
namespace Moshi.Forums.Data;
public class SampleDataGenerator
{
    private readonly IServiceProvider _serviceProvider;

    public SampleDataGenerator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task GenerateSampleDataAsync()
    {
        var forumService = _serviceProvider.GetRequiredService<ForumService>();
        var threadService = _serviceProvider.GetRequiredService<ThreadService>();
        var postService = _serviceProvider.GetRequiredService<PostService>();
        var userService = _serviceProvider.GetRequiredService<UserService>();

        var faker = new Faker();

        // Generate Users
        for (int i = 0; i < 50; i++)
        {
            var user = new User
            {
                Username = faker.Internet.UserName(),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Internet.Password(),
                CreatedAt = faker.Date.Past(2),
                LastLoginAt = faker.Date.Recent()
            };
            await userService.CreateUserAsync(user);
        }

        // Create top-level forums
        for (int i = 0; i < 3; i++)
        {
            var forum = new Forum
            {
                Name = faker.Lorem.Word(),
                Description = faker.Lorem.Sentence(),
                ParentForumId = null, // Top-level forum
                CreatedAt = faker.Date.Past(1),
                UpdatedAt = DateTime.UtcNow,
                DisplayOrder = i
            };
            await forumService.CreateForumAsync(forum);
        }

        // Get all created forums
        var allForums = await forumService.GetAllForumsAsync();

        // Create sub-forums
        foreach (var parentForum in allForums)
        {
            for (int i = 0; i < faker.Random.Number(0, 3); i++)
            {
                var subForum = new Forum
                {
                    Name = faker.Lorem.Word(),
                    Description = faker.Lorem.Sentence(),
                    ParentForumId = parentForum.Id, // Set parent forum ID
                    CreatedAt = faker.Date.Past(1),
                    UpdatedAt = DateTime.UtcNow,
                    DisplayOrder = i
                };
                await forumService.CreateForumAsync(subForum);
            }
        }

        // Generate Threads and Posts
        var users = await userService.GetAllUsersAsync();
        var forums = await forumService.GetAllForumsAsync();

        foreach (var forum in forums)
        {
            for (int i = 0; i < faker.Random.Number(3, 10); i++)
            {
                var thread = new ForumThread
                {
                    Title = faker.Lorem.Sentence(),
                    ForumId = forum.Id,
                    UserId = faker.PickRandom(users).Id,
                    CreatedAt = faker.Date.Recent(),
                    UpdatedAt = faker.Date.Recent(),
                    ViewCount = faker.Random.Number(0, 1000),
                    ReplyCount = faker.Random.Number(0, 50)
                };
                var threadId = await threadService.CreateThreadAsync(thread);

                for (int j = 0; j < faker.Random.Number(1, 15); j++)
                {
                    var post = new Post
                    {
                        ThreadId = threadId,
                        UserId = faker.PickRandom(users).Id,
                        Content = faker.Lorem.Paragraph(),
                        CreatedAt = faker.Date.Recent(),
                        UpdatedAt = faker.Date.Recent()
                    };
                    await postService.CreatePostAsync(post);
                }
            }
        }
    }
}
