using Moshi.Forums.Data;
using Moshi.Forums.Models;

namespace Moshi.Forums.Services;

public class NotificationService
{
    private readonly NotificationRepository _notificationRepository;

    public NotificationService(NotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task CreateNotification(int userId, string message, NotificationType type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.CreateAsync(notification);
    }

    public async Task<IEnumerable<Notification>> GetUserNotifications(int userId)
    {
        return await _notificationRepository.GetUserNotificationsAsync(userId);
    }

    public async Task MarkNotificationAsRead(int notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }
}
