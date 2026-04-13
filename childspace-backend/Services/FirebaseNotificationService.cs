using FirebaseAdmin.Messaging;

namespace childspace_backend.Services
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        public async Task SendNotificationAsync(string fcmToken, string title, string body)
        {
            if (string.IsNullOrEmpty(fcmToken)) return;

            var message = new Message()
            {
                Token = fcmToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Успішно відправлено повідомлення: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка відправки FCM: {ex.Message}");
            }
        }
    }
}
