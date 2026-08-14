$(document).ready(function () {
    var token = $('input[name="__RequestVerificationToken"]').val();
    let currentModal = null;
    let isUploading = false;
    let isFormDirty = false; // ✅ Отслеживаем изменения

    // ============================================================
    // ФУНКЦИИ ПРОВЕРКИ СИГНАТУРЫ ФАЙЛА
    // ============================================================

    function readFileSignature(file) {
        return new Promise(function (resolve, reject) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var array = new Uint8Array(e.target.result);
                var signature = '';
                for (var i = 0; i < Math.min(array.length, 12); i++) {
                    signature += array[i].toString(16).padStart(2, '0') + ' ';
                }
                resolve(signature.trim());
            };
            reader.onerror = function () {
                reject(new Error('Не удалось прочитать файл'));
            };
            reader.readAsArrayBuffer(file.slice(0, 12));
        });
    }

    function getTypeFromSignature(signature) {
        var signatures = {
            'ff d8 ff': 'image/jpeg',
            'ff d8 ff e0': 'image/jpeg',
            'ff d8 ff e1': 'image/jpeg',
            'ff d8 ff e2': 'image/jpeg',
            'ff d8 ff e8': 'image/jpeg',
            'ff d8 ff db': 'image/jpeg',
            '89 50 4e 47 0d 0a 1a 0a': 'image/png',
            '47 49 46 38 37 61': 'image/gif',
            '47 49 46 38 39 61': 'image/gif',
            '52 49 46 46': 'image/webp',
            '42 4d': 'image/bmp',
            '00 00 01 00': 'image/x-icon',
            '49 49 2a 00': 'image/tiff',
            '4d 4d 00 2a': 'image/tiff',
        };

        for (var key in signatures) {
            if (signature.indexOf(key) === 0) {
                return signatures[key];
            }
        }
        return null;
    }

    function validateImageFile(file) {
        return new Promise(function (resolve, reject) {
            if (file.size > 5 * 1024 * 1024) {
                resolve({ valid: false, error: 'Файл слишком большой (макс 5 МБ)' });
                return;
            }

            var allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.bmp'];
            var fileName = file.name.toLowerCase();
            var isValidExtension = allowedExtensions.some(function (ext) {
                return fileName.endsWith(ext);
            });
            if (!isValidExtension) {
                resolve({ valid: false, error: 'Недопустимое расширение файла' });
                return;
            }

            var allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/bmp'];
            if (!allowedTypes.includes(file.type)) {
                resolve({ valid: false, error: 'Недопустимый тип файла' });
                return;
            }

            return readFileSignature(file)
                .then(function (signature) {
                    var detectedType = getTypeFromSignature(signature);

                    if (!detectedType) {
                        resolve({
                            valid: false,
                            error: 'Не удалось определить тип файла'
                        });
                        return;
                    }

                    var isJpeg = detectedType === 'image/jpeg' && file.type === 'image/jpeg';
                    var isMatch = detectedType === file.type || isJpeg;

                    if (!isMatch) {
                        resolve({
                            valid: false,
                            error: 'Файл не является изображением'
                        });
                        return;
                    }

                    resolve({ valid: true, error: null });
                })
                .catch(function (err) {
                    resolve({ valid: false, error: err.message });
                });
        });
    }

    // ============================================================
    // УПРАВЛЕНИЕ КНОПКОЙ СОХРАНИТЬ
    // ============================================================

    function setSaveButtonState(disabled) {
        var $saveBtn = currentModal.find('.btn-save');
        if (disabled) {
            $saveBtn.prop('disabled', true);
            $saveBtn.text('Сохранить');
        } else {
            $saveBtn.prop('disabled', false);
            $saveBtn.text('Сохранить');
        }
    }

    function enableSaveButton() {
        isFormDirty = true;
        setSaveButtonState(false);
    }

    function disableSaveButton() {
        isFormDirty = false;
        setSaveButtonState(true);
    }

    function resetFormState() {
        isFormDirty = false;
        isUploading = false;
        setSaveButtonState(true);
    }

    // ============================================================
    // ОТКРЫТИЕ МОДАЛКИ
    // ============================================================

    function openEditModal(productId) {
        $.ajax({
            url: '/Catalog',
            method: 'GET',
            data: { handler: 'EditModal', id: productId },
            headers: { 'RequestVerificationToken': token },
            success: function (modalHtml) {
                if (currentModal) {
                    currentModal.remove();
                    currentModal = null;
                }

                $('#modalContainer').html(modalHtml);
                currentModal = $('#editProductModal');
                currentModal.show();

                // ✅ Кнопка сохранить заблокирована по умолчанию
                resetFormState();

                attachModalHandlers();
            },
            error: function () {
                window.showMessage && window.showMessage('Ошибка при открытии формы', 'error');
            }
        });
    }

    // ============================================================
    // ОБРАБОТЧИКИ
    // ============================================================

    function attachModalHandlers() {
        if (!currentModal) return;

        // --- Закрытие ---
        currentModal.find('.close-btn').off('click').on('click', function (e) {
            e.stopPropagation();
            closeModal();
        });
        currentModal.off('click', '.btn-cancel').on('click', '.btn-cancel', closeModal);

        currentModal.off('click').on('click', function (e) {
            if (e.target === currentModal[0]) closeModal();
        });

        // --- Отслеживание изменений в полях формы ---
        currentModal.off('input', '#editProductForm input, #editProductForm textarea').on('input', '#editProductForm input, #editProductForm textarea', function () {
            // ✅ Активируем кнопку при изменении полей
            enableSaveButton();
        });

        // --- Клик по слоту ---
        currentModal.off('click', '.image-slot').on('click', '.image-slot', function (e) {
            if ($(e.target).closest('.delete-image-btn').length) return;
            if ($(e.target).is('.image-file-input') || $(e.target).closest('.image-file-input').length) return;

            var $slot = $(this);
            var fileInput = $slot.find('.image-file-input');
            if (fileInput.length) {
                fileInput[0].click();
            }
        });

        // --- ЗАГРУЗКА ФАЙЛА ---
        currentModal.off('change', '.image-file-input').on('change', '.image-file-input', function () {
            var fileInput = $(this);
            var file = fileInput[0].files[0];
            if (!file) return;

            if (isUploading) {
                window.showMessage && window.showMessage('Подождите, идет загрузка...', 'warning');
                fileInput.val('');
                return;
            }

            var $slot = fileInput.closest('.image-slot');
            var index = $slot.data('index');

            validateImageFile(file)
                .then(function (result) {
                    if (!result.valid) {
                        window.showMessage && window.showMessage(result.error, 'warning');
                        fileInput.val('');
                        return;
                    }

                    var formData = new FormData();
                    formData.append('file', file);
                    formData.append('index', index);
                    formData.append('__RequestVerificationToken', token);

                    // ✅ Блокируем кнопку и показываем загрузку
                    setSaveButtonState(true);
                    $slot.addClass('loading');
                    isUploading = true;

                    $.ajax({
                        url: '/Catalog?handler=UploadTempImage',
                        method: 'POST',
                        headers: { 'RequestVerificationToken': token },
                        data: formData,
                        processData: false,
                        contentType: false,
                        success: function (html) {
                            $slot.removeClass('loading');
                            $slot.replaceWith(html);
                            isUploading = false;

                            // ✅ Активируем кнопку после успешной загрузки
                            enableSaveButton();

                            window.showMessage && window.showMessage('Изображение загружено', 'success');
                        },
                        error: function (xhr) {
                            $slot.removeClass('loading');
                            isUploading = false;
                            removeImageSlot($slot, index);

                            // ✅ Если ошибка - кнопка остается заблокированной
                            setSaveButtonState(true);

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
        });

        // --- УДАЛЕНИЕ ИЗОБРАЖЕНИЯ ---
        currentModal.off('click', '.delete-image-btn').on('click', '.delete-image-btn', function (e) {
            e.stopPropagation();
            var $slot = $(this).closest('.image-slot');
            var index = $slot.data('index');

            // ✅ Активируем кнопку при удалении
            enableSaveButton();
            removeImageSlot($slot, index);
        });

        // --- ОТПРАВКА ФОРМЫ ---
        currentModal.off('submit', '#editProductForm').on('submit', '#editProductForm', function (e) {
            e.preventDefault();
            var $form = $(this);
            var $saveBtn = $form.find('.btn-save');
            var formData = new FormData($form[0]);
            formData.append('__RequestVerificationToken', token);

            // ✅ Блокируем кнопку на время сохранения
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
                    } else {
                        // ✅ Если ошибка - разблокируем кнопку
                        $saveBtn.prop('disabled', false).text('Сохранить');
                    }
                },
                error: function (xhr) {
                    // ✅ Разблокируем кнопку при ошибке
                    $saveBtn.prop('disabled', false).text('Сохранить');

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
                    // ✅ Гарантированно разблокируем
                    $saveBtn.prop('disabled', false).text('Сохранить');
                }
            });
        });
    }

    // ============================================================
    // ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
    // ============================================================

    function removeImageSlot($slot, index) {
        $.ajax({
            url: '/Catalog?handler=RemoveImage',
            method: 'POST',
            data: { index: index },
            headers: { 'RequestVerificationToken': token },
            success: function (html) {
                $slot.replaceWith(html);
                window.showMessage && window.showMessage('Изображение удалено', 'success');
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
        resetFormState();
    }

    // ============================================================
    // КНОПКА РЕДАКТИРОВАНИЯ
    // ============================================================

    $(document).on('click', '.btn-editing', function () {
        openEditModal($(this).data('id'));
    });
});