document.addEventListener('DOMContentLoaded', function () {
    // Example JS for voucher and form validation

    document.getElementById('applyVoucher').addEventListener('click', function () {
        let code = document.getElementById('voucherCode').value.trim();
        if (code === "") {
            alert("Vui lòng nhập mã voucher hoặc khuyến mãi.");
        } else {
            // Fake: always apply 0% discount for demo
            alert("Mã " + code + " đã được áp dụng!");
        }
    });

    document.getElementById('bookingForm').addEventListener('submit', function (e) {
        e.preventDefault();
        // Basic frontend validation
        const form = e.target;
        if (form.email.value !== form.emailConfirm.value) {
            alert("Email nhập lại không khớp!");
            form.emailConfirm.focus();
            return false;
        }
        if (!form.acceptTerms.checked) {
            alert("Bạn cần đồng ý với điều khoản đặt phòng!");
            form.acceptTerms.focus();
            return false;
        }
        // Simulate payment redirect
        window.location.href = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    });
});

// Thêm vào hoặc nhập vào file BookingInfo.js

// Danh sách ngày đã được booking (yyyy-mm-dd)
const bookedDates = [
    "2025-09-24", "2025-09-25", "2025-09-26", // ví dụ: đã được booking
];

function pad(n) { return n < 10 ? '0' + n : n; }
function formatDate(date) {
    return date.getFullYear() + '-' + pad(date.getMonth() + 1) + '-' + pad(date.getDate());
}

function renderCalendar(year, month, checkin, checkout) {
    const container = document.getElementById('calendar-container');
    container.innerHTML = '';

    const today = new Date();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    // Calendar header
    const nav = document.createElement('div');
    nav.className = 'calendar-nav mb-2';
    nav.innerHTML = `
        <button id="prevMonth">&lt;</button>
        <span class="fw-bold">${firstDay.toLocaleString('vi', { month: 'long', year: 'numeric' })}</span>
        <button id="nextMonth">&gt;</button>
    `;
    container.appendChild(nav);

    // Calendar table
    const table = document.createElement('table');
    table.className = 'calendar-table table table-bordered';
    let thead = '<thead><tr>';
    ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'].forEach(d => thead += `<th>${d}</th>`);
    thead += '</tr></thead>';

    let tbody = '<tbody><tr>';
    let startDay = firstDay.getDay();
    let printed = 0;
    for (let i = 0; i < startDay; i++) { tbody += '<td class="disabled"></td>'; printed++; }

    for (let d = 1; d <= lastDay.getDate(); d++) {
        let date = new Date(year, month, d);
        let dateStr = formatDate(date);
        let cls = 'selectable';
        if (bookedDates.includes(dateStr)) cls += ' booked';
        if (dateStr === formatDate(today)) cls += ' today';
        if (date < today) cls += ' disabled';
        if (checkin && dateStr === formatDate(checkin)) cls += ' selected';
        if (checkin && checkout && date > checkin && date < checkout) cls += ' in-range';
        if (checkout && dateStr === formatDate(checkout)) cls += ' selected';

        tbody += `<td class="${cls}" data-date="${dateStr}">${d}</td>`;
        printed++;
        if (printed % 7 === 0 && d !== lastDay.getDate()) tbody += '</tr><tr>';
    }
    while (printed % 7 !== 0) { tbody += '<td class="disabled"></td>'; printed++; }
    tbody += '</tr></tbody>';

    table.innerHTML = thead + tbody;
    container.appendChild(table);

    document.getElementById('prevMonth').onclick = () => renderCalendar(year, month - 1, checkin, checkout);
    document.getElementById('nextMonth').onclick = () => renderCalendar(year, month + 1, checkin, checkout);

    // Chọn ngày
    table.querySelectorAll('td.selectable:not(.booked):not(.disabled)').forEach(td => {
        td.onclick = function () {
            let selectedDate = new Date(td.dataset.date);
            if (!checkin || (checkin && checkout)) {
                renderCalendar(year, month, selectedDate, null);
                document.getElementById('selected-checkin').textContent = td.dataset.date;
                document.getElementById('selected-checkout').textContent = '';
            } else if (checkin && !checkout) {
                if (selectedDate > checkin) {
                    renderCalendar(year, month, checkin, selectedDate);
                    document.getElementById('selected-checkout').textContent = td.dataset.date;
                }
            }
        };
    });
}

// Khởi tạo lịch khi vào trang
document.addEventListener('DOMContentLoaded', function () {
    // Nếu dùng file BookingInfo.js đã có, chỉ thêm đoạn này
    let today = new Date();
    renderCalendar(today.getFullYear(), today.getMonth(), null, null);
});