// Dữ liệu giả lập phòng
const rooms = [
    {
        soPhong: "101",
        loai: "Standard",
        trangThai: "trong",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 300000,
        tang: "1"
    },
    {
        soPhong: "102",
        loai: "Deluxe",
        trangThai: "o",
        khach: "Nguyễn Văn A",
        checkIn: "2025-09-18",
        checkOut: "2025-09-22",
        gia: 500000,
        tang: "1"
    },
    {
        soPhong: "103",
        loai: "Suite",
        trangThai: "dattruoc",
        khach: "Trần Thị B",
        checkIn: "2025-09-20",
        checkOut: "2025-09-25",
        gia: 800000,
        tang: "2"
    },
    {
        soPhong: "104",
        loai: "Standard",
        trangThai: "dangdon",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 320000,
        tang: "2"
    },
    {
        soPhong: "105",
        loai: "Deluxe",
        trangThai: "baotri",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 500000,
        tang: "3"
    },
    {
        soPhong: "106",
        loai: "Suite",
        trangThai: "o",
        khach: "Phạm Văn C",
        checkIn: "2025-09-17",
        checkOut: "2025-09-20",
        gia: 900000,
        tang: "4"
    },
    {
        soPhong: "107",
        loai: "Standard",
        trangThai: "dattruoc",
        khach: "Lê Thị D",
        checkIn: "2025-09-21",
        checkOut: "2025-09-23",
        gia: 350000,
        tang: "3"
    },
    {
        soPhong: "108",
        loai: "Deluxe",
        trangThai: "dangdon",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 520000,
        tang: "4"
    },
    {
        soPhong: "109",
        loai: "Suite",
        trangThai: "trong",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 850000,
        tang: "1"
    },
    {
        soPhong: "110",
        loai: "Deluxe",
        trangThai: "trong",
        khach: "",
        checkIn: "",
        checkOut: "",
        gia: 480000,
        tang: "2"
    }
];

// Map trạng thái phòng
const statusMap = {
    trong: { text: "Trống", class: "room-status-trong", icon: "fa-circle text-success" },
    o: { text: "Đang ở", class: "room-status-o", icon: "fa-circle text-danger" },
    dattruoc: { text: "Đặt trước", class: "room-status-dattruoc", icon: "fa-circle text-warning" },
    dangdon: { text: "Đang dọn", class: "room-status-dangdon", icon: "fa-circle text-secondary" },
    baotri: { text: "Bảo trì", class: "room-status-baotri", icon: "fa-circle text-info" }
};

// Render cards phòng
function renderRooms(filtered) {
    const grid = document.getElementById("room-grid");
    grid.innerHTML = "";
    if (filtered.length === 0) {
        grid.innerHTML = "<div class='col-12 text-center text-muted py-5'>Không tìm thấy phòng phù hợp.</div>";
        return;
    }
    filtered.forEach((room, idx) => {
        const st = statusMap[room.trangThai];
        grid.innerHTML += `
            <div class="col-lg-3 col-md-6 mb-4">
                <div class="card card-room ${st.class}">
                    <div class="card-body">
                        <div class="d-flex align-items-center mb-2">
                            <span class="fs-5 fw-bold me-2"><i class="fa ${st.icon}"></i> ${room.soPhong}</span>
                            <span class="badge bg-light text-dark ms-auto">${room.loai}</span>
                        </div>
                        <div class="mb-1"><strong>Giá:</strong> ${room.gia.toLocaleString()}đ</div>
                        <div class="mb-1"><strong>Tầng:</strong> ${room.tang}</div>
                        <div class="mb-1"><strong>Trạng thái:</strong> ${st.text}</div>
                        <div class="mb-1"><strong>Khách:</strong> ${room.khach || "<span class='text-muted'>-</span>"}</div>
                        <div class="mb-1"><strong>Check-in:</strong> ${room.checkIn || "<span class='text-muted'>-</span>"}</div>
                        <div class="mb-1"><strong>Check-out:</strong> ${room.checkOut || "<span class='text-muted'>-</span>"}</div>
                        <div class="room-action mt-3">
                            <button class="btn btn-primary btn-sm"><i class="fa fa-eye"></i> Xem chi tiết</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    });
}

// Xử lý lọc & tìm kiếm
function filterRooms() {
    const minPrice = parseInt(document.getElementById("filter-min-price").value) || 0;
    const maxPrice = parseInt(document.getElementById("filter-max-price").value) || Infinity;
    const floor = document.getElementById("filter-floor").value;
    const type = document.getElementById("filter-type").value;
    const status = document.getElementById("filter-status").value;
    const searchCustomer = document.getElementById("search-customer").value.trim().toLowerCase();
    const searchRoom = document.getElementById("search-room").value.trim().toLowerCase();

    let filtered = rooms.filter(r => {
        // Giá
        if (r.gia < minPrice || r.gia > maxPrice) return false;
        // Tầng
        if (floor && r.tang !== floor) return false;
        // Loại phòng
        if (type && r.loai !== type) return false;
        // Trạng thái
        if (status && r.trangThai !== status) return false;
        // Tên khách
        if (searchCustomer && !(r.khach.toLowerCase().includes(searchCustomer))) return false;
        // Số phòng
        if (searchRoom && !(r.soPhong.toLowerCase().includes(searchRoom))) return false;
        return true;
    });

    renderRooms(filtered);
}

// Sự kiện lọc, tìm kiếm
["filter-min-price", "filter-max-price", "filter-floor", "filter-type", "filter-status", "search-customer", "search-room"].forEach(id => {
    document.getElementById(id).addEventListener("input", filterRooms);
    document.getElementById(id).addEventListener("change", filterRooms);
});

// Khởi tạo
window.onload = filterRooms;