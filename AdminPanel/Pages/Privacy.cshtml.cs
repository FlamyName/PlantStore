using BusinessLogic.Core.Notification;
using BusinessLogic.Core.Notification.Extensions;
using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostDeleteItemAsync(int id)
        {
            try
            {
                await Task.Delay(100);

                var notification = new NotificationViewModel
                {
                    Message = "Элемент успешно удален!",
                    Type = NotificationType.Success.ToNotificationType(),
                };
                return Partial("_Notification", notification);
            }
            catch (Exception ex)
            {
                var notification = new NotificationViewModel
                {
                    Message = $"Ошибка: {ex.Message}",
                    Type = NotificationType.Error.ToNotificationType(),
                };
                return Partial("_Notification", notification);
            }
        }
        public async Task<IActionResult> OnPostAddItemAsync(string name)
        {
            try
            {
                await Task.Delay(100);

                var notification = new NotificationViewModel
                {
                    Message = "Элемент успешно добавлен",
                    Type = NotificationType.Success.ToNotificationType(),
                };
                return Partial("_Notification", notification);
            }
            catch (Exception ex)
            {
                var notification = new NotificationViewModel
                {
                    Message = $"Ошибка: {ex.Message}",
                    Type = NotificationType.Error.ToNotificationType(),
                };
                return Partial("_Notification", notification);
            }
        }
        public IActionResult OnPostGetNotification(string message, string type)
        {
            var notification = new NotificationViewModel
            {
                Message = message,
                Type = type
            };
            return Partial("_Notification", notification);
        }
    }

}
