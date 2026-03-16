(function () {
    'use strict';

    // ========== ДЕСКТОПНАЯ ГАЛЕРЕЯ (МИНИАТЮРЫ) ==========
    const thumbnails = document.querySelectorAll('.thumbnail');
    const mainImage = document.getElementById('mainProductImage');

    if (thumbnails.length && mainImage) {
        thumbnails.forEach(thumb => {
            thumb.addEventListener('click', (e) => {
                e.preventDefault();

                // Простая проверка, что src не пустой
                if (!thumb.src) return;

                // Меняем изображение
                mainImage.src = thumb.src;
                mainImage.style.opacity = '1'; // Убираем возможный эффект загрузки

                // Обновляем активный класс
                thumbnails.forEach(t => t.classList.remove('active'));
                thumb.classList.add('active');
            });
        });
    }

    // ========== МОБИЛЬНЫЙ СЛАЙДЕР ==========
    class MobileSlider {
        constructor(container) {
            this.container = container;
            this.track = container.querySelector('.slider-track');
            this.slides = container.querySelectorAll('.slider-slide');
            this.prevBtn = container.querySelector('.slider-nav.prev');
            this.nextBtn = container.querySelector('.slider-nav.next');

            if (!this.track || !this.slides.length) return;

            this.currentIndex = 0;
            this.slideCount = this.slides.length;

            this.init();
        }

        init() {
            // Если только один слайд - скрываем кнопки
            if (this.slideCount <= 1) {
                if (this.prevBtn) this.prevBtn.style.display = 'none';
                if (this.nextBtn) this.nextBtn.style.display = 'none';
                return;
            }

            this.updateButtons();
            this.bindEvents();
        }

        bindEvents() {
            // Кнопки навигации
            if (this.prevBtn) {
                this.prevBtn.addEventListener('click', () => this.goToSlide(this.currentIndex - 1));
            }
            if (this.nextBtn) {
                this.nextBtn.addEventListener('click', () => this.goToSlide(this.currentIndex + 1));
            }

            // Простые свайпы для мобильных
            let touchStartX = 0;

            this.track.addEventListener('touchstart', (e) => {
                touchStartX = e.touches[0].clientX;
            }, { passive: true });

            this.track.addEventListener('touchend', (e) => {
                const touchEndX = e.changedTouches[0].clientX;
                const diff = touchStartX - touchEndX;

                if (Math.abs(diff) > 50) {
                    if (diff > 0) {
                        this.goToSlide(this.currentIndex + 1);
                    } else {
                        this.goToSlide(this.currentIndex - 1);
                    }
                }
            }, { passive: true });
        }

        goToSlide(index) {
            if (index < 0 || index >= this.slideCount) return;

            this.currentIndex = index;
            this.track.style.transform = `translateX(-${index * 100}%)`;
            this.updateButtons();
        }

        updateButtons() {
            if (this.prevBtn) {
                this.prevBtn.style.display = this.currentIndex === 0 ? 'none' : 'flex';
            }
            if (this.nextBtn) {
                this.nextBtn.style.display = this.currentIndex === this.slideCount - 1 ? 'none' : 'flex';
            }
        }
    }

    // ========== ИНИЦИАЛИЗАЦИЯ ==========

    // Функция инициализации слайдера
    const initSlider = () => {
        const sliderContainers = document.querySelectorAll('.mobile-slider');
        if (!sliderContainers.length) return;

        sliderContainers.forEach(container => {
            // Проверяем, не инициализирован ли уже слайдер
            if (!container.dataset.sliderInitialized) {
                new MobileSlider(container);
                container.dataset.sliderInitialized = 'true';
            }
        });
    };

    // Инициализация при загрузке
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            // Слайдер только на мобильных
            if (window.innerWidth <= 768) {
                initSlider();
            }
        });
    } else {
        if (window.innerWidth <= 768) {
            initSlider();
        }
    }

    // Переинициализация при изменении размера окна (с debounce)
    let resizeTimer;
    window.addEventListener('resize', () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            if (window.innerWidth <= 768) {
                initSlider();
            }
        }, 150);
    });

})();