$(document).ready(function () {

    // === Функция для получения токена ===
    var token = $('input[name="__RequestVerificationToken"]').val();

    // === Удаление элемента ===
    $(document).on('click', '.delete-item', function () {
        var button = $(this);
        var itemId = button.data('id');
        var card = button.closest('.card');

        $.ajax({
            url: '/Privacy?handler=DeleteItem',
            type: 'POST',
            data: { id: itemId },
            headers: {
                'RequestVerificationToken': token
            },

            beforeSend: function () {
                button.prop('disabled', true).text('Удаление...');
            },
            success: function (html) {
                showNotification(html);
                card.fadeOut(300, function () {
                    card.remove();
                });
            },
            error: function (xhr, status, error) {
                showMessage('Ошибка при удалении: ' + error, 'error');
            },
            complete: function () {
                button.prop('disabled', false).text('Удалить');
            }
        });
    });

    // === Добавление элемента ===
    $('#addItemBtn').click(function () {
        var itemName = $('#newItemName').val().trim();

        if (!itemName) {
            showMessage('Введите название элемента', 'warning');
            return;
        }

        $.ajax({
            url: '/Privacy?handler=AddItem',
            type: 'POST',
            data: { name: itemName },
            headers: {
                'RequestVerificationToken': token
            },

            beforeSend: function () {
                $('#addItemBtn').prop('disabled', true).text('Добавление...');
            },
            success: function (html) {
                showNotification(html);
                $('#newItemName').val('');
            },
            error: function (xhr, status, error) {
                showMessage('Ошибка при добавлении: ' + error, 'error');
            },
            complete: function () {
                $('#addItemBtn').prop('disabled', false).text('Добавить элемент');
            }
        });
    });

});