(function () {
    'use strict';

    // ========== УСИЛЕННАЯ ЗАЩИТА ОТ XSS ==========

    // 1. Более надежное экранирование HTML
    const escapeHtml = (text) => {
        if (!text) return text;
        // Используем Map для всех опасных символов
        const htmlEscapes = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;',
            '/': '&#47;',
            '`': '&#96;',
            '=': '&#61;'
        };
        return String(text).replace(/[&<>"'`=\/]/g, char => htmlEscapes[char]);
    };

    // 2. Санитизация HTML перед вставкой в DOM (защита от XSS во вставляемых данных)
    const sanitizeHtml = (dirtyHtml) => {
        if (!dirtyHtml) return '';

        // Создаем временный DOM элемент
        const temp = document.createElement('div');
        temp.innerHTML = dirtyHtml;

        // Удаляем все скрипты и опасные элементы
        const scripts = temp.querySelectorAll('script, iframe, object, embed, frame, frameset, applet');
        scripts.forEach(el => el.remove());

        // Удаляем опасные атрибуты (onclick, onload, javascript: и т.д.)
        const allElements = temp.getElementsByTagName('*');
        for (let el of allElements) {
            const attrs = el.attributes;
            for (let i = attrs.length - 1; i >= 0; i--) {
                const attrName = attrs[i].name.toLowerCase();
                const attrValue = attrs[i].value.toLowerCase();

                // Удаляем обработчики событий
                if (attrName.startsWith('on')) {
                    el.removeAttribute(attrs[i].name);
                }
                // Удаляем javascript: ссылки
                else if (attrValue.includes('javascript:') ||
                    attrValue.includes('data:') ||
                    attrValue.includes('vbscript:')) {
                    el.removeAttribute(attrs[i].name);
                }
            }
        }

        return temp.innerHTML;
    };

    // 3. Валидация URL (защита от открытия вредоносных ссылок)
    const isValidUrl = (url) => {
        if (!url) return true;
        try {
            const parsed = new URL(url, window.location.origin);
            // Разрешаем только http и https протоколы
            return parsed.protocol === 'http:' || parsed.protocol === 'https:';
        } catch {
            return false;
        }
    };

    // 4. Усиленная валидация поискового запроса
    const validateSearch = (query) => {
        if (!query) return true;

        // Блокируем потенциально опасные паттерны
        const dangerousPatterns = [
            /<script/i,
            /javascript:/i,
            /onclick/i,
            /onerror/i,
            /onload/i,
            /eval\(/i,
            /document\.cookie/i,
            /window\.location/i,
            /alert\(/i,
            /--/i,  // SQL injection
            /';/i,  // SQL injection
            /union\s+select/i,
            /etc\/passwd/i,
            /\.\.\//i  // Path traversal
        ];

        for (let pattern of dangerousPatterns) {
            if (pattern.test(query)) {
                console.warn('Обнаружен опасный паттерн в поиске:', query);
                return false;
            }
        }

        // Разрешаем только буквы, цифры, пробелы и базовые знаки препинания
        return /^[а-яА-Яa-zA-Z0-9\s\-\.\,!?]+$/.test(query);
    };

    // ========== ЗАЩИТА ОТ CSRF (RAZOR УЖЕ ЗАЩИЩАЕТ, НО УСИЛИВАЕМ) ==========

    // Получаем CSRF токен из мета-тега (Razor его генерирует)
    const getCsrfToken = () => {
        const meta = document.querySelector('meta[name="csrf-token"]');
        return meta ? meta.getAttribute('content') : '';
    };

    // ========== ЗАЩИТА ОТ ПОВТОРНЫХ ЗАПРОСОВ ==========

    // Debounce функция для предотвращения множественных кликов
    const debounce = (func, wait) => {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    };

    // ========== ПОЛУЧАЕМ ЭЛЕМЕНТЫ ==========

    const loadMoreBtn = document.getElementById('loadMoreBtn');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const productsGrid = document.getElementById('productsGrid');
    const loadMoreContainer = document.getElementById('loadMoreContainer');
    const searchForm = document.getElementById('searchForm');
    const searchInput = document.getElementById('searchInput');

    // ========== СОСТОЯНИЕ ==========

    let isLoading = false;
    let requestCount = 0; // Счетчик запросов для защиты от DDoS

    // Получаем значения из скрытых полей
    const pageSize = parseInt(document.getElementById('pageSize')?.value || '6');
    const totalItems = parseInt(document.getElementById('totalItems')?.value || '0');

    // Валидация полученных чисел
    if (isNaN(pageSize) || pageSize < 1 || pageSize > 100) {
        console.error('Некорректный pageSize');
        pageSize = 6;
    }
    if (isNaN(totalItems) || totalItems < 0) {
        console.error('Некорректный totalItems');
        totalItems = 0;
    }

    // ========== ЗАЩИТА ОТ CLICKJACKING ==========

    // Добавляем заголовки через мета-тег (если не установлены на сервере)
    if (!document.querySelector('meta[http-equiv="X-Frame-Options"]')) {
        const meta = document.createElement('meta');
        meta.httpEquiv = 'X-Frame-Options';
        meta.content = 'DENY';
        document.head.appendChild(meta);
    }

    // ========== БЕЗОПАСНАЯ РАБОТА С DOM ==========

    // Функция для безопасного создания элементов
    const createSafeElement = (tag, text, attributes = {}) => {
        const el = document.createElement(tag);
        if (text) {
            el.textContent = text; // Безопасно, не парсит HTML
        }
        for (let [key, value] of Object.entries(attributes)) {
            // Не позволяем устанавливать опасные атрибуты
            if (!key.startsWith('on') && key !== 'href' && key !== 'src') {
                el.setAttribute(key, value);
            } else if (key === 'href' || key === 'src') {
                // Проверяем URL перед установкой
                if (isValidUrl(value)) {
                    el.setAttribute(key, value);
                }
            }
        }
        return el;
    };

    // ========== ОСНОВНАЯ ЛОГИКА ==========

    const setLoading = (loading) => {
        isLoading = loading;
        if (loadMoreBtn) {
            loadMoreBtn.disabled = loading;
            loadMoreBtn.style.opacity = loading ? '0.5' : '1';
            // Добавляем aria-атрибуты для доступности
            loadMoreBtn.setAttribute('aria-busy', loading.toString());
        }
        if (loadingSpinner) {
            loadingSpinner.style.display = loading ? 'flex' : 'none';
            loadingSpinner.setAttribute('aria-hidden', (!loading).toString());
        }
    };

    const countLoadedItems = () => {
        return document.querySelectorAll('.product-card-link').length;
    };

    const hasMoreItems = () => {
        const loadedCount = countLoadedItems();
        return loadedCount < totalItems;
    };

    // Загрузка новых товаров с усиленной защитой
    const loadMoreProducts = async () => {
        // Защита от слишком частых запросов
        if (isLoading || !loadMoreBtn) return;

        // Защита от DDoS (ограничиваем количество запросов)
        requestCount++;
        if (requestCount > 10) {
            console.warn('Слишком много запросов загрузки');
            return;
        }

        const page = loadMoreBtn.dataset.page;
        const search = loadMoreBtn.dataset.search || '';

        // Валидация номера страницы
        const pageNum = parseInt(page);
        if (isNaN(pageNum) || pageNum < 2 || pageNum > 1000) {
            console.error('Некорректный номер страницы');
            return;
        }

        // Валидация поискового запроса
        if (search && !validateSearch(search)) {
            alert('Поисковый запрос содержит недопустимые символы');
            return;
        }

        console.log('Загрузка страницы:', page, 'Поиск:', escapeHtml(search));

        setLoading(true);

        try {
            // Формируем URL с валидацией
            const baseUrl = window.location.origin + '/Catalog';
            if (!isValidUrl(baseUrl)) {
                throw new Error('Некорректный базовый URL');
            }

            const url = new URL(baseUrl);
            url.searchParams.append('handler', 'LoadMore');
            url.searchParams.append('page', page);
            url.searchParams.append('searchTerm', search);

            // Добавляем CSRF токен для защиты
            const csrfToken = getCsrfToken();

            // Добавляем случайное число для защиты от кэширования
            url.searchParams.append('_', Date.now());

            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'X-CSRF-TOKEN': csrfToken, // CSRF защита
                    'X-Content-Type-Options': 'nosniff'
                },
                credentials: 'same-origin' // Важно для защиты кук
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Проверяем Content-Type
            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('text/html')) {
                throw new Error('Некорректный тип ответа');
            }

            const html = await response.text();

            // Проверяем размер ответа (защита от слишком больших ответов)
            if (html.length > 500000) { // 500KB максимум
                throw new Error('Ответ слишком большой');
            }

            if (html && html.trim().length > 0) {

                const beforeCount = countLoadedItems();

                // САНИТИЗАЦИЯ HTML ПЕРЕД ВСТАВКОЙ (ВАЖНО!)
                const cleanHtml = sanitizeHtml(html);

                // Безопасная вставка
                productsGrid.insertAdjacentHTML('beforeend', cleanHtml);

                const afterCount = countLoadedItems();
                const newItemsCount = afterCount - beforeCount;

                console.log('Добавлено новых товаров:', newItemsCount);

                // Обновляем номер следующей страницы
                const nextPage = parseInt(page) + 1;
                loadMoreBtn.dataset.page = nextPage;

                // Проверяем, есть ли еще товары
                if (!hasMoreItems() || newItemsCount < pageSize) {
                    console.log('Товаров больше нет - скрываем кнопку');
                    loadMoreContainer.style.transition = 'opacity 0.3s';
                    loadMoreContainer.style.opacity = '0';
                    setTimeout(() => {
                        loadMoreContainer.style.display = 'none';
                    }, 300);
                }
            } else {
                loadMoreContainer.style.display = 'none';
            }
        } catch (error) {
            console.error('Ошибка загрузки:', error);

            // Создаем сообщение об ошибке безопасно
            const errorMsg = createSafeElement('div', 'Не удалось загрузить товары. Пожалуйста, попробуйте позже.', {
                class: 'error-message',
                role: 'alert'
            });

            // Стилизуем безопасно
            errorMsg.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: #f8d7da;
                color: #721c24;
                padding: 12px 20px;
                border-radius: 5px;
                border: 1px solid #f5c6cb;
                z-index: 1000;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            `;

            document.body.appendChild(errorMsg);
            setTimeout(() => errorMsg.remove(), 5000);
        } finally {
            setLoading(false);
            // Сбрасываем счетчик запросов через некоторое время
            setTimeout(() => {
                requestCount = Math.max(0, requestCount - 1);
            }, 60000); // Сбрасываем 1 запрос в минуту
        }
    };

    // Debounced версия для защиты от множественных кликов
    const debouncedLoadMore = debounce(loadMoreProducts, 300);

    // Обработчик кнопки
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', (e) => {
            e.preventDefault();
            debouncedLoadMore();
        });
    }

    // ========== УСИЛЕННАЯ ЗАЩИТА ФОРМЫ ПОИСКА ==========

    if (searchForm && searchInput) {
        // Защита от вставки опасного содержимого через paste
        searchInput.addEventListener('paste', (e) => {
            e.preventDefault();
            const pastedText = (e.clipboardData || window.clipboardData).getData('text');
            if (validateSearch(pastedText)) {
                searchInput.value = escapeHtml(pastedText);
            } else {
                alert('Вставленный текст содержит недопустимые символы');
            }
        });

        // Защита от ввода опасных символов
        searchInput.addEventListener('keypress', (e) => {
            const char = String.fromCharCode(e.charCode);
            if (!validateSearch(char) && char !== '') {
                e.preventDefault();
            }
        });

        // Основная валидация при отправке
        searchForm.addEventListener('submit', (e) => {
            const searchValue = searchInput.value.trim();

            // Проверка длины
            if (searchValue.length > 50) {
                e.preventDefault();
                alert('Поисковый запрос не может быть длиннее 50 символов');
                return;
            }

            // Проверка на пустой запрос
            if (searchValue.length === 0) {
                // Разрешаем пустой поиск (покажет все товары)
                return;
            }

            // Усиленная валидация содержимого
            if (!validateSearch(searchValue)) {
                e.preventDefault();
                alert('Поисковый запрос содержит недопустимые символы');
                return;
            }

            // Экранируем и устанавливаем безопасное значение
            searchInput.value = escapeHtml(searchValue);
        });
    }

    // ========== ЗАЩИТА ОТ ПЕРЕХВАТА СООБЩЕНИЙ ==========

    // Очищаем консоль от чувствительной информации в production
    if (window.location.hostname !== 'localhost' && !window.location.hostname.includes('127.0.0.1')) {
        console.log = function () { };
        console.info = function () { };
        console.warn = function () { };
        // Оставляем console.error для отладки ошибок
    }

    // ========== ЗАЩИТА ОТ СLICKJACKING ==========

    // Дополнительная защита через JavaScript
    if (window.self !== window.top) {
        // Сайт открыт в iframe - перенаправляем
        window.top.location = window.self.location;
    }

})();