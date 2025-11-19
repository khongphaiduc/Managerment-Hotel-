
const allRevenuesByYear = {
    2023: [135, 181, 224, 162, 249, 282, 229, 208, 288, 278, 205, 297],
    2024: [125, 179, 210, 166, 237, 278, 201, 256, 312, 298, 267, 352],
};

const roomRevenuesByYear = {
    2023: [97, 131, 144, 98, 152, 174, 129, 119, 188, 140, 137, 186],
    2024: [84, 103, 127, 102, 145, 157, 112, 149, 182, 150, 128, 197]
};

const serviceRevenuesByYear = {
    2023: [38, 50, 80, 64, 97, 108, 100, 89, 100, 138, 68, 111],
    2024: [41, 76, 83, 64, 92, 121, 89, 107, 130, 148, 139, 155]
};

const allBookingsByYear = {
    2023: [54, 63, 59, 44, 61, 74, 67, 81, 95, 73, 67, 104],
    2024: [44, 53, 68, 39, 71, 84, 57, 91, 105, 83, 77, 114]
};

const genderByYear = {
    2023: [58, 42],
    2024: [61, 39]
};

// ======================
const months = ["Th1", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7", "Th8", "Th9", "Th10", "Th11", "Th12"];
const quarters = ["Q1", "Q2", "Q3", "Q4"];
const genderLabels = ['Nam', 'Nữ'];

const softColors = [
    'rgba(109,104,255,0.95)',
    'rgba(33,206,153,0.92)',
    'rgba(241,120,197,0.91)',
    'rgba(255,196,71,0.90)',
];

const years = Object.keys(allRevenuesByYear).map(Number).sort((a, b) => b - a);

// ==================================
let selectedYear = years[0];
let revenueByMonth = allRevenuesByYear[selectedYear];
let bookingByMonth = allBookingsByYear[selectedYear];
let genderData = genderByYear[selectedYear];

// ======================
function updateAllCards() {
    const tong = allRevenuesByYear[selectedYear].reduce((a, b) => a + b, 0);
    const phong = roomRevenuesByYear[selectedYear].reduce((a, b) => a + b, 0);
    const dichvu = serviceRevenuesByYear[selectedYear].reduce((a, b) => a + b, 0);

    document.getElementById('tongDoanhThu').textContent = tong.toLocaleString('vi-VN') + " triệu";
    document.getElementById('doanhThuPhong').textContent = phong.toLocaleString('vi-VN') + " triệu";
    document.getElementById('doanhThuDichVu').textContent = dichvu.toLocaleString('vi-VN') + " triệu";
}

// ======================
// BOOKING BAR CHART
// ======================

let bookingBarChartObj;

function renderBookingBarChart() {
    const ctx = document.getElementById("bookingBarChart").getContext('2d');
    if (bookingBarChartObj) bookingBarChartObj.destroy();

    bookingBarChartObj = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: months,
            datasets: [{
                label: 'Số lượng booking',
                data: bookingByMonth,
                backgroundColor: softColors[3],
                borderRadius: 8
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            responsive: true,
            maintainAspectRatio: false
        }
    });
}

// ======================
// DOANH THU THÁNG + QUÝ
// ======================

let barChartObj, lineChartObj;

function updateAllCharts() {

    renderBookingBarChart();

    // Bar doanh thu tháng
    if (barChartObj) barChartObj.destroy();
    const barCtx = document.getElementById("barChart").getContext('2d');

    barChartObj = new Chart(barCtx, {
        type: 'bar',
        data: {
            labels: months,
            datasets: [{
                label: 'Doanh thu (triệu)',
                data: revenueByMonth,
                backgroundColor: softColors[0],
                borderRadius: 8
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            responsive: true,
            maintainAspectRatio: false
        }
    });

    // Line doanh thu theo quý
    if (lineChartObj) lineChartObj.destroy();
    const lineCtx = document.getElementById("lineChart").getContext('2d');

    const revenueByQuarter = [
        revenueByMonth.slice(0, 3).reduce((a, b) => a + b),
        revenueByMonth.slice(3, 6).reduce((a, b) => a + b),
        revenueByMonth.slice(6, 9).reduce((a, b) => a + b),
        revenueByMonth.slice(9, 12).reduce((a, b) => a + b)
    ];

    lineChartObj = new Chart(lineCtx, {
        type: 'line',
        data: {
            labels: quarters,
            datasets: [{
                label: "Doanh thu mỗi quý (triệu)",
                data: revenueByQuarter,
                borderColor: softColors[1],
                backgroundColor: "rgba(33,206,153,0.15)",
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            responsive: true,
            maintainAspectRatio: false
        }
    });

    updateAllCards();
    updatePieChart();
}

// ======================
// PIE CHART GIỚI TÍNH
// ======================

let pieChartObj;

function updatePieChart() {
    const pieCtx = document.getElementById("pieChart").getContext("2d");

    if (pieChartObj) pieChartObj.destroy();

    pieChartObj = new Chart(pieCtx, {
        type: 'doughnut',
        data: {
            labels: genderLabels,
            datasets: [{
                data: genderData,
                backgroundColor: [softColors[0], softColors[2]],
                borderColor: "#fff",
                borderWidth: 2
            }]
        },
        options: {
            plugins: { legend: { display: true, position: 'bottom' } },
            cutout: "70%",
            responsive: true,
            maintainAspectRatio: false
        }
    });
}

// ======================
// SELECT NĂM
// ======================

function renderYearSelect() {
    const select = document.getElementById('yearSelect');
    select.innerHTML = years.map(y => `<option value="${y}">${y}</option>`).join('');
    select.value = selectedYear;

    select.onchange = () => {
        selectedYear = +select.value;
        revenueByMonth = allRevenuesByYear[selectedYear];
        bookingByMonth = allBookingsByYear[selectedYear];
        genderData = genderByYear[selectedYear];
        updateAllCharts();
    };
}

// ======================
document.addEventListener("DOMContentLoaded", function () {
    renderYearSelect();
    updateAllCharts();
});
