(function () {
    'use strict';

    // Элементы
    const loadMoreBtn = document.getElementById('loadMoreBtn');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const loadMoreContainer = document.getElementById('loadMoreContainer');

    // Определяем страницу и селектор для подсчета
    const isCatalog = document.getElementById('catalogGrid') !== null;
    const isNews = document.getElementById('newsGrid') !== null;

    // Выбираем правильный селектор
    const itemSelector = isCatalog ? '.product-card-link' : '.news-container';
    const gridId = isCatalog ? 'catalogGrid' : 'newsGrid';
    const loadUrl = isCatalog ? '/Catalog' : '/Index';
    const searchAttr = isCatalog ? 'search' : null;

    // Если нет кнопки или грида - выходим
    const grid = document.getElementById(gridId);
    if (!loadMoreBtn || !grid) return;

    // Состояние
    let isLoading = false;
    const totalItems = parseInt(document.getElementById('totalItems')?.value || '0');
    const pageSize = parseInt(document.getElementById('pageSize')?.value || '12');

    // Подсчет загруженных элементов
    const countLoadedItems = () => document.querySelectorAll(itemSelector).length;

    // Проверка, есть ли еще элементы
    const hasMoreItems = () => countLoadedItems() < totalItems;

    // Скрытие кнопки если все загружено
    const checkAndHideButton = () => {
        if (!hasMoreItems() && loadMoreContainer) {
            loadMoreContainer.style.display = 'none';
            return true;
        }
        return false;
    };

    const setLoading = (loading) => {
        isLoading = loading;
        if (loadMoreBtn) {
            loadMoreBtn.disabled = loading;
            loadMoreBtn.style.opacity = loading ? '0.5' : '1';
        }
        if (loadingSpinner) {
            loadingSpinner.style.display = loading ? 'flex' : 'none';
        }
    };

    // Загрузка
    const loadMoreItems = async () => {
        if (isLoading) return;
        if (checkAndHideButton()) return;

        const page = loadMoreBtn.dataset.page;
        const search = searchAttr ? (loadMoreBtn.dataset.search || '') : '';

        setLoading(true);

        try {
            const url = new URL(loadUrl, window.location.origin);
            url.searchParams.append('handler', 'LoadMore');
            url.searchParams.append('page', page);
            if (search) url.searchParams.append('searchTerm', search);
            url.searchParams.append('_', Date.now());

            const response = await fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) throw new Error();

            const html = await response.text();

            if (html?.trim()) {
                grid.insertAdjacentHTML('beforeend', html);
                loadMoreBtn.dataset.page = parseInt(page) + 1;
                checkAndHideButton();
            } else {
                if (loadMoreContainer) loadMoreContainer.style.display = 'none';
            }
        } catch (error) {
            alert('Ошибка загрузки. Попробуйте позже.');
        } finally {
            setLoading(false);
        }
    };

    // Обработчики
    loadMoreBtn.addEventListener('click', (e) => {
        e.preventDefault();
        loadMoreItems();
    });

    // Проверка при старте
    checkAndHideButton();

    // Валидация поиска (для каталога)
    const searchForm = document.getElementById('searchForm');
    const searchInput = document.getElementById('searchInput');

    if (searchForm && searchInput) {
        searchForm.addEventListener('submit', (e) => {
            const value = searchInput.value.trim();
            if (value.length > 50) {
                e.preventDefault();
                alert('Максимум 50 символов');
                return;
            }
            if (/[<>{}]/.test(value)) {
                e.preventDefault();
                alert('Недопустимые символы');
                return;
            }
        });
    }

    // ClickJacking защита
    if (window.self !== window.top) {
        window.top.location = window.self.location;
    }
})();