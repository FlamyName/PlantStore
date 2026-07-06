$(document).ready(function () {
    // Получаем токен (можно вынести в глобальную переменную)
    var token = $('input[name="__RequestVerificationToken"]').val();
    let currentModal = null;

    // === Функция открытия модального окна ===
    function openEditModal(productId) {
        $.ajax({
            url: '/Catalog',
            method: 'GET',
            data: {
                handler: 'EditModal',
                id: productId
            },
            headers: {
                'RequestVerificationToken': token
            },
            success: function (modalHtml) {
                // Вставляем модалку в контейнер
                $('#modalContainer').html(modalHtml);
                currentModal = $('#editProductModal');
                currentModal.show();
                attachModalHandlers();
            },
            error: function (xhr, status, error) {
                console.error('Ошибка загрузки модалки:', error);
                if (window.showMessage) {
                    window.showMessage('Ошибка при открытии формы редактирования', 'error');
                } else {
                    alert('Ошибка при открытии формы редактирования');
                }
            }
        });
    }

    // === Обработчики для модального окна ===
    function attachModalHandlers() {
        if (!currentModal) return;

        // Закрытие по крестику
        currentModal.find('.close-btn').off('click').on('click', closeModal);
        currentModal.find('.btn-cancel').off('click').on('click', closeModal);

        // Закрытие по клику вне окна
        currentModal.off('click').on('click', function (e) {
            if (e.target === currentModal[0]) {
                closeModal();
            }
        });

        // Отправка формы
        currentModal.find('#editProductForm').off('submit').on('submit', function (e) {
            e.preventDefault();

            const $form = $(this);
            const $saveBtn = $form.find('.btn-save');
            const formData = {
                Id: parseInt($form.find('#productId').val()),
                Name: $form.find('#productName').val(),
                Description: $form.find('#productDescription').val()
            };

            // Блокируем кнопку
            $saveBtn.prop('disabled', true).text('Сохранение...');

            $.ajax({
                url: '/Catalog',
                method: 'POST',
                data: JSON.stringify(formData),
                contentType: 'application/json',
                headers: {
                    'RequestVerificationToken': token
                },
                dataType: 'json',
                success: function (result) {
                    if (result.success) {
                        if (window.showMessage) {
                            window.showMessage(result.message, 'success');
                        } else {
                            alert(result.message);
                        }
                        closeModal();
                        setTimeout(function () {
                            location.reload();
                        }, 500);
                    } else {
                        // Если success = false, но это не исключение
                        throw new Error(result.message || 'Ошибка сохранения');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Ошибка сохранения:', error);
                    if (window.showMessage) {
                        window.showMessage('Ошибка при сохранении: ' + error.message, 'error');
                    } else {
                        alert('Ошибка при сохранении: ' + error.message);
                    }
                },
                complete: function () {
                    $saveBtn.prop('disabled', false).text('Сохранить');
                }
            });
        });
    }

    // === Функция закрытия модалки ===
    function closeModal() {
        if (currentModal) {
            currentModal.hide();
            $('body').css('overflow', 'auto');
            currentModal.remove();
            currentModal = null;
        }
    }

    // === Обработчики на кнопки редактирования ===
    $(document).on('click', '.btn-editing', function () {
        var productId = $(this).data('id');
        openEditModal(productId);
    });
});