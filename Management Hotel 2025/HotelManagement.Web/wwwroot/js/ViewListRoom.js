function luxuryCardHover(card) {
    card.classList.add("lux-glow-active");
}
function luxuryCardOut(card) {
    card.classList.remove("lux-glow-active");
}
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

window.addEventListener('DOMContentLoaded', () => {
    const cards = document.querySelectorAll('.lux-room-card');
    cards.forEach((card, idx) => {
        setTimeout(() => {
            card.classList.add('animated');
        }, idx * 120); // hiệu ứng so le giữa các thẻ
    });
});

// Bộ lọc phòng
function luxApplyFilter() {
    const floor = document.getElementById("filter-floor").value;
    const minPrice = parseInt(document.getElementById("filter-min-price").value) || 0;
    const maxPrice = parseInt(document.getElementById("filter-max-price").value) || Infinity;
    const maxPerson = parseInt(document.getElementById("filter-max-person").value) || Infinity;

    const roomCards = document.querySelectorAll(".lux-room-card");
    roomCards.forEach(card => {
        // Lấy dữ liệu từ attribute đã gán ở Razor
        const roomFloor = card.getAttribute("data-floor");
        const roomPrice = parseInt(card.getAttribute("data-price"));
        const roomPerson = parseInt(card.getAttribute("data-person"));

        let show = true;
        if (floor && roomFloor !== floor) show = false;
        if (roomPrice < minPrice || roomPrice > maxPrice) show = false;
        if (maxPerson !== Infinity && roomPerson > maxPerson) show = false;

        card.style.display = show ? "" : "none";
    });
}

function luxResetFilter() {
    document.getElementById("lux-filter-form").reset();
    document.querySelectorAll(".lux-room-card").forEach(card => card.style.display = "");
}