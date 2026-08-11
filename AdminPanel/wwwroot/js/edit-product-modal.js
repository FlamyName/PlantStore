$(document).ready(function () {
    var token = $('input[name="__RequestVerificationToken"]').val();
    let currentModal = null;

    // === Открытие модалки ===
    function openEditModal(productId) {
        $.ajax({
            url: '/Catalog',
            method: 'GET',
            data: { handler: 'EditModal', id: productId },
            headers: { 'RequestVerificationToken': token },
            success: function (modalHtml) {
                $('#modalContainer').html(modalHtml);
                currentModal = $('#editProductModal');
                currentModal.show();
                // ✅ Привязываем обработчики ТОЛЬКО ОДИН РАЗ
                attachModalHandlers();
            },
            error: function () {
                window.showMessage && window.showMessage('Ошибка при открытии формы', 'error');
            }
        });
    }

    // === Обработчики (делегирование) ===
    function attachModalHandlers() {
        if (!currentModal) return;

        // ✅ Используем .off().on() чтобы не дублировать
        // Закрытие
        currentModal.off('click', '.close-btn').on('click', '.close-btn', closeModal);
        currentModal.off('click', '.btn-cancel').on('click', '.btn-cancel', closeModal);

        // Клик по фону модалки
        currentModal.off('click').on('click', function (e) {
            if (e.target === currentModal[0]) closeModal();
        });

        // Клик по слоту - открыть выбор файла
        currentModal.off('click', '.image-slot').on('click', '.image-slot', function (e) {
            if ($(e.target).closest('.delete-image-btn').length) return;
            if ($(e.target).is('.image-file-input') || $(e.target).closest('.image-file-input').length) return;

            var $slot = $(this);
            var fileInput = $slot.find('.image-file-input');
            if (fileInput.length) {
                fileInput[0].click();
            }
        });

        // === ЗАГРУЗКА ФАЙЛА ===
        currentModal.off('change', '.image-file-input').on('change', '.image-file-input', function () {
            var fileInput = $(this);
            var file = fileInput[0].files[0];
            if (!file) return;

            var $slot = fileInput.closest('.image-slot');
            var index = $slot.data('index');

            // ✅ Валидация
            var allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
            if (!allowedTypes.includes(file.type)) {
                window.showMessage && window.showMessage('Недопустимый формат изображения', 'warning');
                fileInput.val('');
                return;
            }

            var allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp'];
            var fileName = file.name.toLowerCase();
            var isValidExtension = allowedExtensions.some(function (ext) {
                return fileName.endsWith(ext);
            });
            if (!isValidExtension) {
                window.showMessage && window.showMessage('Недопустимое расширение файла', 'warning');
                fileInput.val('');
                return;
            }

            if (file.size > 5 * 1024 * 1024) {
                window.showMessage && window.showMessage('Файл слишком большой (макс 5 МБ)', 'warning');
                fileInput.val('');
                return;
            }

            var formData = new FormData();
            formData.append('file', file);
            formData.append('index', index);
            formData.append('__RequestVerificationToken', token);

            // Индикатор загрузки
            var $slotContent = $slot.find('.slot-content');
            $slotContent.html('<div class="loading-spinner">Загрузка...</div>');

            $.ajax({
                url: '/Catalog?handler=UploadTempImage',
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: formData,
                processData: false,
                contentType: false,
                success: function (html) {
                    // ✅ Заменяем слот
                    $slot.replaceWith(html);

                    // ✅ НЕ вызываем attachModalHandlers() - обработчики уже есть!
                    // Просто показываем уведомление
                    window.showNotification && window.showMessage('Изображение загружено', 'success');
                },
                error: function (xhr) {
                    // ✅ Восстанавливаем слот через сервер
                    removeImageSlot($slot, index);

                    if (xhr.responseText) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            if (response.message) {
                                window.showMessage && window.showMessage(response.message, 'error');
                                return;
                            }
                        } catch (e) { }
                    }
                    window.showMessage && window.showMessage('Ошибка при загрузке файла', 'error');
                }
            });
        });

        // === УДАЛЕНИЕ ИЗОБРАЖЕНИЯ ===
        currentModal.off('click', '.delete-image-btn').on('click', '.delete-image-btn', function (e) {
            e.stopPropagation();
            var $slot = $(this).closest('.image-slot');
            var index = $slot.data('index');


            removeImageSlot($slot, index);
        });

        // === ОТПРАВКА ФОРМЫ ===
        currentModal.off('submit', '#editProductForm').on('submit', '#editProductForm', function (e) {
            e.preventDefault();
            var $form = $(this);
            var $saveBtn = $form.find('.btn-save');
            var formData = new FormData($form[0]);
            formData.append('__RequestVerificationToken', token);

            $saveBtn.prop('disabled', true).text('Сохранение...');

            $.ajax({
                url: '/Catalog?handler=UpdateProductWithFiles',
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: formData,
                processData: false,
                contentType: false,
                success: function (html) {
                    window.showNotification(html);

                    if (!html.includes('error') && !html.includes('danger')) {
                        closeModal();
                        setTimeout(function () {
                            location.reload();
                        }, 500);
                    }
                },
                error: function (xhr) {
                    if (xhr.responseText) {
                        try {
                            var response = JSON.parse(xhr.responseText);
                            if (response.message) {
                                window.showMessage && window.showMessage(response.message, 'error');
                                return;
                            }
                        } catch (e) { }
                    }
                    window.showMessage && window.showMessage('Ошибка при сохранении', 'error');
                },
                complete: function () {
                    $saveBtn.prop('disabled', false).text('Сохранить');
                }
            });
        });
    }

    // === Удаление слота через сервер ===
    function removeImageSlot($slot, index) {
        $.ajax({
            url: '/Catalog?handler=RemoveImage',
            method: 'POST',
            data: { index: index },
            headers: { 'RequestVerificationToken': token },
            success: function (html) {
                $slot.replaceWith(html);
                // ✅ НЕ вызываем attachModalHandlers() - обработчики уже есть!

                window.showNotification && window.showMessage('Изображение удалено', 'success');
            },
            error: function () {
                window.showMessage && window.showMessage('Ошибка при удалении изображения', 'error');
            }
        });
    }

    function closeModal() {
        if (currentModal) {
            currentModal.hide();
            currentModal.remove();
            currentModal = null;
        }
    }

    // === Обработчик кнопки редактирования ===
    $(document).on('click', '.btn-editing', function () {
        openEditModal($(this).data('id'));
    });
});