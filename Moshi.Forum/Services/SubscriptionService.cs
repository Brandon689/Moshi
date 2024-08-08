using Moshi.Forums.Data;
using Moshi.Forums.Models;

namespace Moshi.Forums.Services;

public class SubscriptionService
{
    private readonly SubscriptionRepository _subscriptionRepository;

    public SubscriptionService(SubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task SubscribeToForum(int userId, int forumId)
    {
        await _subscriptionRepository.CreateAsync(new Subscription { UserId = userId, ForumId = forumId });
    }

    public async Task UnsubscribeFromForum(int userId, int forumId)
    {
        await _subscriptionRepository.DeleteAsync(userId, forumId);
    }

    public async Task<IEnumerable<Subscription>> GetUserSubscriptions(int userId)
    {
        return await _subscriptionRepository.GetUserSubscriptionsAsync(userId);
    }
}
