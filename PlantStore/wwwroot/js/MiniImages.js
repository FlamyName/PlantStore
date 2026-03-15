/**
 * Product Gallery - управление галереей товара
 * Позволяет менять главное изображение при клике на миниатюры
 */

class ProductGallery {
    constructor() {
        this.mainImage = document.getElementById('mainProductImage');
        this.thumbnails = document.querySelectorAll('.thumbnail');
        this.init();
    }

    init() {
        if (!this.mainImage || this.thumbnails.length === 0) {
            console.warn('ProductGallery: Не найдены элементы галереи');
            return;
        }

        this.bindEvents();
        this.setupAccessibility();
    }

    bindEvents() {
        // Добавляем обработчики на миниатюры
        this.thumbnails.forEach(thumb => {
            thumb.addEventListener('click', (e) => {
                const imageUrl = thumb.src;
                this.changeMainImage(imageUrl);
                this.updateActiveThumbnail(thumb);
            });
        });
    }

    /**
     * Изменяет главное изображение
     * @param {string} imageUrl - URL нового изображения
     */
    changeMainImage(imageUrl) {
        if (!this.mainImage || !imageUrl) return;

        // Добавляем эффект затухания
        this.mainImage.style.opacity = '0';

        setTimeout(() => {
            this.mainImage.src = imageUrl;
            this.mainImage.style.opacity = '1';
        }, 200);
    }

    /**
     * Обновляет активный класс на миниатюрах
     * @param {HTMLElement} activeThumb - активная миниатюра
     */
    updateActiveThumbnail(activeThumb) {
        this.thumbnails.forEach(thumb => {
            thumb.classList.remove('active');
        });
        activeThumb.classList.add('active');
    }

    /**
     * Настраивает доступность для клавиатуры
     */
    setupAccessibility() {
        this.thumbnails.forEach(thumb => {
            // Добавляем атрибуты для доступности
            thumb.setAttribute('tabindex', '0');
            thumb.setAttribute('role', 'button');
            thumb.setAttribute('aria-label', 'Показать увеличенное изображение');

            // Поддержка клавиатуры
            thumb.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    thumb.click();
                }
            });
        });
    }

    /**
     * Переключает на следующее изображение
     */
    nextImage() {
        const activeThumb = document.querySelector('.thumbnail.active');
        if (!activeThumb) return;

        const nextThumb = activeThumb.nextElementSibling || this.thumbnails[0];
        nextThumb.click();
    }

    /**
     * Переключает на предыдущее изображение
     */
    prevImage() {
        const activeThumb = document.querySelector('.thumbnail.active');
        if (!activeThumb) return;

        const prevThumb = activeThumb.previousElementSibling || this.thumbnails[this.thumbnails.length - 1];
        prevThumb.click();
    }
}

// Инициализация после загрузки DOM
document.addEventListener('DOMContentLoaded', () => {
    window.productGallery = new ProductGallery();
});