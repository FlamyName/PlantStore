using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Notification.Extensions
{
    public static class NotificationExtensions
    {
        public static string ToNotificationType(this NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "success",
                NotificationType.Error => "error",
                NotificationType.Warning => "warning",
                NotificationType.Info => "info",
                _ => "info",
            };
        }
    }
}
