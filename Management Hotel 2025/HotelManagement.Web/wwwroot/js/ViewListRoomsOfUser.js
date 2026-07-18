document.addEventListener("DOMContentLoaded", function () {
    var searchInput = document.getElementById('roomSearch');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            var search = this.value.toLowerCase();
            var cards = document.querySelectorAll('.room-card-horizontal');
            cards.forEach(function (card) {
                var roomNumber = card.getAttribute('data-roomnumber');
                var roomType = card.getAttribute('data-roomtype');
                var services = card.getAttribute('data-services');
                if (
                    roomNumber.includes(search) ||
                    roomType.includes(search) ||
                    services.includes(search)
                ) {
                    card.classList.remove('hide');
                } else {
                    card.classList.add('hide');
                }
            });
        });
    }
});

function showRoomDetail(roomNumber, roomType, imageUrl, price, guestCount, checkIn, checkOut, status, services, note) {
    var html = `
        <div class="mb-3 text-center">
            <img src="${imageUrl}" alt="Ảnh phòng ${roomType}" class="img-fluid rounded" style="max-height:200px;object-fit:cover;">
        </div>
        <ul class="list-group list-group-flush mb-2">
            <li class="list-group-item"><strong>Số phòng:</strong> ${roomNumber}</li>
            <li class="list-group-item"><strong>Loại phòng:</strong> ${roomType}</li>
            <li class="list-group-item"><strong>Giá phòng:</strong> <span class="text-danger">${parseInt(price).toLocaleString()} VNĐ/đêm</span></li>
            <li class="list-group-item"><strong>Số lượng khách:</strong> ${guestCount}</li>
            <li class="list-group-item"><strong>Ngày nhận phòng:</strong> ${checkIn}</li>
            <li class="list-group-item"><strong>Ngày trả phòng:</strong> ${checkOut}</li>
            <li class="list-group-item"><strong>Dịch vụ:</strong> ${services}</li>
            <li class="list-group-item"><strong>Trạng thái:</strong> <span class="badge bg-success">${status}</span></li>
            ${note ? `<li class="list-group-item"><strong>Ghi chú:</strong> ${note}</li>` : ""}
        </ul>
    `;
    document.getElementById('roomDetailBody').innerHTML = html;
    var myModal = new bootstrap.Modal(document.getElementById('roomDetailModal'));
    myModal.show();
}