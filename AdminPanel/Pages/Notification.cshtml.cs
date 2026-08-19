using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages
{
    public class NotificationModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound(); 
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
