namespace childspace_backend.Services
{
    public interface IFirebaseNotificationService
    {
        Task SendNotificationAsync(string fcmToken, string title, string body);
    }
}
