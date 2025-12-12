$(document).ready(function () {
    // Select2 Customization
    $('#nationalitySelect').select2({
        minimumResultsForSearch: 5,
        placeholder: "Chọn quốc tịch"
    });
});

// Xử lý chọn phương thức thanh toán UI
function selectPayment(value) {
    document.querySelectorAll('.payment-option').forEach(el => el.classList.remove('active'));
    const radio = document.querySelector(`input[value="${value}"]`);
    if (radio) {
        radio.checked = true;
        radio.closest('.payment-option').classList.add('active');
    }
}

// --- XỬ LÝ SUBMIT FORM ---
document.getElementById('bookingForm').addEventListener('submit', function (e) {
    const paymentMethod = document.querySelector('input[name="paymentMethod"]:checked').value;
    const spinner = document.getElementById('submitSpinner');
    const btn = document.getElementById('bookingBtn');

    // Bật Loading
    spinner.classList.remove('d-none');
    btn.disabled = true;

    function resetButton() {
        spinner.classList.add('d-none');
        btn.disabled = false;
    }

    if (paymentMethod === 'momo') {
        // --- XỬ LÝ THANH TOÁN QR (PayOS / Chuyển khoản) ---
        e.preventDefault();

        const formData = new FormData(this);

        // Gọi API lấy chuỗi QR String
        fetch('/bookingbypayos/booking', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // 1. Xóa mã QR cũ (nếu có)
                    document.getElementById("qrcode-container").innerHTML = "";

                    // 2. Tạo mã QR mới
                    new QRCode(document.getElementById("qrcode-container"), {
                        text: data.qrCode,
                        width: 250,
                        height: 250,
                        colorDark: "#000000",
                        colorLight: "#ffffff",
                        correctLevel: QRCode.CorrectLevel.H
                    });

                    // 3. Hiển thị Modal
                    const qrModal = new bootstrap.Modal(document.getElementById('qrModal'));
                    qrModal.show();

                    resetButton();
                } else {
                    Swal.fire('Lỗi', data.message || "Không thể tạo giao dịch", 'error');
                    resetButton();
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire('Lỗi', 'Có lỗi xảy ra khi xử lý thanh toán.', 'error');
                resetButton();
            });

    } else {
        // --- TRƯỜNG HỢP: VNPAY ---
        // Để form submit bình thường
    }
});

// ============================================
// CODE SIGNALR XỬ LÝ REAL-TIME PAYMENT
// ============================================

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationsystem")
    .build();

// Lắng nghe sự kiện từ Server khi thanh toán thành công
connection.on("NotificationBookingByPayOS", (bookingcode, depositsAmount) => {
    console.log("Nhận thông báo thanh toán thành công:", bookingcode);

    // 1. Ẩn modal QR Code
    const qrModalEl = document.getElementById('qrModal');
    const modalInstance = bootstrap.Modal.getInstance(qrModalEl);
    if (modalInstance) {
        modalInstance.hide();
    }

    // 2. Format số tiền
    const formattedAmount = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(depositsAmount);

    // 3. Hiển thị thông báo thành công (Phong cách Luxury)
    Swal.fire({
        title: 'Thanh Toán Thành Công!',
        html: `Hệ thống đã nhận được: <b style="color:#D4AF37">${formattedAmount}</b>.<br>Mã đặt phòng của bạn là: <b>${bookingcode}</b>`,
        icon: 'success',
        background: '#fff',
        confirmButtonColor: '#0f172a',
        confirmButtonText: 'Quay lại trang chủ',
        allowOutsideClick: false
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/home/trungducluxuryhotel`;
        }
    });
});

// Bắt đầu kết nối
connection.start()
    .then(() => {
        console.log("Kết nối SignalR thành công!");
    })
    .catch(err => console.error("Lỗi kết nối SignalR:", err));