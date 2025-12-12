// --- HELPER FUNCTIONS ---
function formatCurrency(number) {
    return new Intl.NumberFormat('vi-VN').format(number);
}

function closeModal() {
    document.getElementById('successModal').classList.remove('active');
}

// --- LOGIC TẠO QR CODE ---
document.getElementById('napBtn').addEventListener('click', async function () {
    const amountInput = document.getElementById('amount');
    const amount = amountInput.value;
    const napBtn = this;
    const originalText = napBtn.textContent;

    if (!amount || amount <= 0) {
        alert("Vui lòng nhập số tiền hợp lệ!");
        amountInput.focus();
        return;
    }

    napBtn.disabled = true;
    napBtn.textContent = "Đang tạo QR code...";
    napBtn.style.background = 'var(--primary-dark)';
    napBtn.style.boxShadow = 'none';
    napBtn.style.color = 'var(--white)';

    try {
        // Lưu ý: URL này đang hardcode ngrok, cần đảm bảo ngrok đang chạy hoặc thay bằng domain thật
        const response = await fetch('https://wastingly-preroyal-leonardo.ngrok-free.dev/payos/createpayos', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Amount: amount, Description: "" })
        });

        if (!response.ok) throw new Error("Lỗi tạo QR code");

        const data = await response.json();
        const qrText = data.qrCode;

        const qrContainer = document.getElementById('qrCodeContainer');
        qrContainer.innerHTML = "";
        qrContainer.classList.add('p-3');

        new QRCode(qrContainer, {
            text: qrText,
            width: 250,
            height: 250,
            colorDark: "#1a293a",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });

        qrContainer.insertAdjacentHTML('beforeend', '<p class="text-success mt-4">✅ Vui lòng Quét mã để hoàn tất thanh toán</p>');
        qrContainer.style.boxShadow = '0 0 20px rgba(184, 134, 11, 0.4)';

    } catch (err) {
        console.error(err);
        alert("Tạo QR code thất bại! Vui lòng thử lại.");
        const qrContainer = document.getElementById('qrCodeContainer');
        qrContainer.innerHTML = '<p class="text-danger mt-3" style="font-weight: 600;">❌ Lỗi: Không thể tạo mã QR.</p>';
    } finally {
        napBtn.disabled = false;
        napBtn.textContent = originalText;
        napBtn.style.background = '';
        napBtn.style.boxShadow = '';
        napBtn.style.color = '';
    }
});

// --- SIGNALR ---
document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/coinhub")
        .build();

    connection.on("ReceiveCoinUpdate", (amount, message) => {
        document.getElementById('modalMessage').textContent = message || "Tiền đã vào tài khoản!";
        document.getElementById('modalAmount').textContent = formatCurrency(amount);
        document.getElementById('successModal').classList.add('active');
    });

    connection.start()
        .then(() => console.log("Kết nối SignalR thành công!"))
        .catch(err => console.error(err));
});