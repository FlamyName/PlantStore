document.addEventListener('DOMContentLoaded', function () {
    const thumbnails = document.querySelectorAll('.thumbnail');
    const mainImage = document.getElementById('mainProductImage');

    if (!thumbnails.length || !mainImage) return;

    // Функция для обновления главного изображения
    function updateMainImage(thumbnail) {
        const newSrc = thumbnail.src;

        // Если изображение уже активное - не меняем
        if (mainImage.src === newSrc) return;

        // Показываем эффект загрузки (опционально)
        mainImage.style.opacity = '0.5';

        // Создаем новое изображение для предзагрузки
        const tempImage = new Image();
        tempImage.src = newSrc;

        tempImage.onload = function () {
            // Когда изображение загружено - обновляем главное
            mainImage.src = newSrc;
            mainImage.style.opacity = '1';

            // Убираем активный класс у всех миниатюр
            thumbnails.forEach(t => t.classList.remove('active'));

            // Добавляем активный класс текущей миниатюре
            thumbnail.classList.add('active');
        };

        // Обработка ошибки загрузки
        tempImage.onerror = function () {
            console.error('Failed to load image:', newSrc);
            mainImage.style.opacity = '1';
        };
    }

    // Добавляем обработчик на каждую миниатюру
    thumbnails.forEach(thumbnail => {
        thumbnail.addEventListener('click', function (e) {
            e.preventDefault();
            updateMainImage(this);
        });
    });
});


class ImageSlider {
    constructor(container) {
        this.container = container;
        this.track = container.querySelector('.slider-track');
        this.slides = Array.from(container.querySelectorAll('.slider-slide'));
        this.prevBtn = container.querySelector('.slider-nav.prev');
        this.nextBtn = container.querySelector('.slider-nav.next');
        this.dotsContainer = container.querySelector('.slider-dots');

        this.currentIndex = 0;
        this.slideCount = this.slides.length;
        this.startX = 0;
        this.currentX = 0;
        this.isDragging = false;

        this.init();
    }

    init() {
        if (this.slideCount <= 1) {
            if (this.prevBtn) this.prevBtn.classList.add('hidden');
            if (this.nextBtn) this.nextBtn.classList.add('hidden');
            return;
        }

        this.createDots();
        this.updateSlider();
        this.bindEvents();
    }

    createDots() {
        if (!this.dotsContainer) return;

        this.dotsContainer.innerHTML = '';

        for (let i = 0; i < this.slideCount; i++) {
            const dot = document.createElement('span');
            dot.classList.add('slider-dot');
            if (i === 0) dot.classList.add('active');

            dot.addEventListener('click', () => {
                this.goToSlide(i);
            });

            this.dotsContainer.appendChild(dot);
        }

        this.dots = Array.from(this.dotsContainer.children);
    }

    bindEvents() {
        // Кнопки навигации
        if (this.prevBtn) {
            this.prevBtn.addEventListener('click', () => this.prevSlide());
        }

        if (this.nextBtn) {
            this.nextBtn.addEventListener('click', () => this.nextSlide());
        }

        // Свайпы для touch-устройств
        this.track.addEventListener('touchstart', (e) => {
            this.startX = e.touches[0].clientX;
            this.isDragging = true;
        });

        this.track.addEventListener('touchmove', (e) => {
            if (!this.isDragging) return;

            this.currentX = e.touches[0].clientX;
            const diff = this.currentX - this.startX;

            // Ограничиваем перетаскивание
            if (Math.abs(diff) > 50) {
                e.preventDefault();
            }
        });

        this.track.addEventListener('touchend', (e) => {
            if (!this.isDragging) return;

            const diff = this.currentX - this.startX;

            if (Math.abs(diff) > 50) {
                if (diff > 0 && this.currentIndex > 0) {
                    this.prevSlide();
                } else if (diff < 0 && this.currentIndex < this.slideCount - 1) {
                    this.nextSlide();
                }
            }

            this.isDragging = false;
            this.startX = 0;
            this.currentX = 0;
        });

        // Клавиатурная навигация
        document.addEventListener('keydown', (e) => {
            if (e.key === 'ArrowLeft') {
                this.prevSlide();
            } else if (e.key === 'ArrowRight') {
                this.nextSlide();
            }
        });
    }

    updateSlider() {
        const offset = -this.currentIndex * 100;
        this.track.style.transform = `translateX(${offset}%)`;

        // Обновляем кнопки навигации
        if (this.prevBtn) {
            this.prevBtn.classList.toggle('hidden', this.currentIndex === 0);
        }

        if (this.nextBtn) {
            this.nextBtn.classList.toggle('hidden', this.currentIndex === this.slideCount - 1);
        }

        // Обновляем точки
        if (this.dots) {
            this.dots.forEach((dot, index) => {
                dot.classList.toggle('active', index === this.currentIndex);
            });
        }
    }

    goToSlide(index) {
        if (index < 0 || index >= this.slideCount) return;

        this.currentIndex = index;
        this.updateSlider();
    }

    prevSlide() {
        if (this.currentIndex > 0) {
            this.goToSlide(this.currentIndex - 1);
        }
    }

    nextSlide() {
        if (this.currentIndex < this.slideCount - 1) {
            this.goToSlide(this.currentIndex + 1);
        }
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    // Инициализация слайдера на мобильных устройствах
    if (window.innerWidth <= 768) {
        const sliderContainers = document.querySelectorAll('.mobile-slider');

        sliderContainers.forEach(container => {
            new ImageSlider(container);
        });
    }

    // Десктопная логика для миниатюр
    const thumbnails = document.querySelectorAll('.thumbnail');
    const mainImage = document.getElementById('mainProductImage');

    if (thumbnails.length > 0 && mainImage) {
        thumbnails.forEach(thumb => {
            thumb.addEventListener('click', function () {
                // Обновляем главное изображение
                mainImage.src = this.src;

                // Обновляем активный класс
                thumbnails.forEach(t => t.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }
});

// Обновляем слайдер при изменении размера окна
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);

    resizeTimeout = setTimeout(() => {
        if (window.innerWidth <= 768) {
            const sliderContainers = document.querySelectorAll('.mobile-slider');

            sliderContainers.forEach(container => {
                if (!container.slider) {
                    container.slider = new ImageSlider(container);
                }
            });
        }
    }, 250);
});