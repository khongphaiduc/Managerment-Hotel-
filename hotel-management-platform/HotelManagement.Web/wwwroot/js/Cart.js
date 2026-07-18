// Hiệu ứng fade-in khi load trang
document.querySelectorAll('.fade-in').forEach(el => { el.style.opacity = 1; });
updateAllRows();
// Xóa phòng khỏi giỏ hàng
document.querySelectorAll('.remove-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const tr = btn.closest('tr');
        tr.classList.add('fadeOut');
        setTimeout(() => {
            tr.remove();
            updateCartTotal();
        }, 350);
    });
});

// Tính số ngày booking và cập nhật tổng tiền khi đổi ngày nhận/trả phòng
document.querySelectorAll('.checkin-date, .checkout-date').forEach(input => {
    input.addEventListener('change', function () {
        updateRow(input.closest('tr'));
        updateCartTotal();
    });
});

function updateAllRows() {
    document.querySelectorAll('#cart-items tr').forEach(row => {
        updateRow(row);
    });
    updateCartTotal();
}

function updateRow(row) {
    const checkin = row.querySelector('.checkin-date').value;
    const checkout = row.querySelector('.checkout-date').value;
    const price = parseFloat(row.querySelector('.price').dataset.price);
    let days = calculateDays(checkin, checkout);
    if (days < 1) days = 1;
    row.querySelector('.days-input').value = days;
    row.querySelector('.subtotal').textContent = '$' + (days * price);
}
function calculateDays(checkin, checkout) {
    const d1 = new Date(checkin);
    const d2 = new Date(checkout);
    return Math.max(1, Math.floor((d2 - d1) / (1000 * 3600 * 24)));
}
function updateCartTotal() {
    let total = 0;
    document.querySelectorAll('#cart-items tr').forEach(row => {
        const subtotal = row.querySelector('.subtotal');
        if (subtotal) total += parseFloat(subtotal.textContent.replace('$', '')) || 0;
    });
    document.getElementById('cart-total').textContent = '$' + total;
}