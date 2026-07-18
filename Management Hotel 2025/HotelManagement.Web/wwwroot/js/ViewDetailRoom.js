// ========== SLIDER LOGIC ==========
let gallery = [];
let currentSlide = 0;
let sliderInterval = null;

// Hàm khởi tạo dữ liệu gallery từ attribute (hoặc backend truyền vào)
function initSlider(galleryData) {
    gallery = galleryData;
    currentSlide = 0;
    showSlide(0);
    autoSlide();
}

function showSlide(idx) {
    if (!gallery.length) return;
    const mainImg = document.getElementById('mainSliderImg');
    currentSlide = (idx + gallery.length) % gallery.length;
    if (mainImg) mainImg.src = gallery[currentSlide];
    // update dots
    const dots = document.querySelectorAll('.slider-dots .dot');
    dots.forEach((dot, i) => {
        dot.classList.toggle('active', i === currentSlide);
    });
}

function changeSlide(delta) {
    showSlide(currentSlide + delta);
    resetAutoSlide();
}

function goToSlide(idx) {
    showSlide(idx);
    resetAutoSlide();
}

function autoSlide() {
    sliderInterval = setInterval(() => {
        showSlide(currentSlide + 1);
    }, 3500);
}

function resetAutoSlide() {
    clearInterval(sliderInterval);
    autoSlide();
}

// ========== MODAL LOGIC ==========
function luxBookNow(roomName) {
    document.getElementById("lux-modal-room").textContent = roomName;
    document.getElementById("lux-modal").classList.add("show");
}
function luxCloseModal() {
    document.getElementById("lux-modal").classList.remove("show");
}
document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") luxCloseModal();
});

// ========== DOMContentLoaded ==========
document.addEventListener('DOMContentLoaded', function () {
    // Lấy gallery từ attribute data-gallery trên mainSliderImg (chuỗi JSON)
    const mainImg = document.getElementById('mainSliderImg');
    if (mainImg && mainImg.dataset.gallery) {
        try {
            const galleryData = JSON.parse(mainImg.dataset.gallery);
            initSlider(galleryData);
        } catch (e) {
            // fallback nếu có lỗi hoặc không có data-gallery
        }
    }
});