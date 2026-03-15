class ProductGallery {
    constructor() {
        this.mainImage = document.getElementById('mainProductImage');
        this.thumbnails = document.querySelectorAll('.thumbnail');
        this.fallbackImage = '/images/placeholder.jpg'; // одна заглушка на случай ошибок
        this.init();
    }

    init() {
        if (!this.mainImage || this.thumbnails.length === 0) return;

        this.bindEvents();
        this.setupErrorHandling();
    }

    bindEvents() {
        this.thumbnails.forEach(thumb => {
            thumb.addEventListener('click', () => {
                this.changeMainImage(thumb.src);
                this.highlightThumbnail(thumb);
            });
        });
    }

    changeMainImage(src) {
        if (!this.mainImage || !src) return;
        this.mainImage.src = src;
    }

    highlightThumbnail(activeThumb) {
        this.thumbnails.forEach(thumb => thumb.classList.remove('active'));
        activeThumb.classList.add('active');
    }

    setupErrorHandling() {
        this.mainImage.addEventListener('error', () => {
            this.mainImage.src = this.fallbackImage;
        });
    }
}

// Инициализация
document.addEventListener('DOMContentLoaded', () => {
    new ProductGallery();
});