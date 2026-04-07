$(document).ready(function () {

    // === Функция для получения токена ===
    var token = $('input[name="__RequestVerificationToken"]').val();

    // === Закрытие уведомления ===
    function closeNotification(notification) {
        notification.css('animation', 'slideOut 0.3s ease-out');
        setTimeout(function () {
            notification.remove();
        }, 300);
    }

    // === Закрытие по клику на кнопку × ===
    $(document).on('click', '.notification-close', function () {
        closeNotification($(this).closest('.notification'));
    });

    // === Показать уведомление (полученное от сервера) ===
    window.showNotification = function (html) {
        var $notification = $(html);
        $('#notification-container').append($notification);

        // Авто-закрытие через 3 секунды
        setTimeout(function () {
            closeNotification($notification);
        }, 3000);

        return $notification;
    };

    // === Показать уведомление из текста (через Partial View) ===
    window.showMessage = function (message, type) {
        $.ajax({
            url: '/Privacy?handler=GetNotification',
            type: 'POST',
            data: {
                message: message,
                type: type
            },
            headers: {
                'RequestVerificationToken': token
            },
            success: function (html) {
                window.showNotification(html);
            },
            error: function () {
                console.error('Ошибка при получении уведомления');
            }
        });
    };

});