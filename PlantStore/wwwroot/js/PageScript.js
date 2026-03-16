(function () {
    'use strict';

    // Элементы
    const loadMoreBtn = document.getElementById('loadMoreBtn');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const productsGrid = document.getElementById('productsGrid');
    const loadMoreContainer = document.getElementById('loadMoreContainer');
    const searchInput = document.getElementById('searchInput');
    const searchForm = document.getElementById('searchForm');

    // Если нет кнопки загрузки - выходим
    if (!loadMoreBtn) return;

    // Состояние
    let isLoading = false;
    const totalItems = parseInt(document.getElementById('totalItems')?.value || '0');

    // Вспомогательные функции
    const countLoadedItems = () => document.querySelectorAll('.product-card-link').length;
    const hasMoreItems = () => countLoadedItems() < totalItems;

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

    // Загрузка товаров
    const loadMoreProducts = async () => {
        if (isLoading) return;

        const page = loadMoreBtn.dataset.page;
        const search = loadMoreBtn.dataset.search || '';

        setLoading(true);

        try {
            const url = new URL('/Catalog', window.location.origin);
            url.searchParams.append('handler', 'LoadMore');
            url.searchParams.append('page', page);
            url.searchParams.append('searchTerm', search);
            url.searchParams.append('_', Date.now());

            const response = await fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) throw new Error();

            const html = await response.text();

            if (html?.trim()) {
                productsGrid.insertAdjacentHTML('beforeend', html);
                loadMoreBtn.dataset.page = parseInt(page) + 1;

                if (!hasMoreItems()) {
                    loadMoreContainer.style.display = 'none';
                }
            } else {
                loadMoreContainer.style.display = 'none';
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
        loadMoreProducts();
    });

    // Валидация поиска (простая)
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