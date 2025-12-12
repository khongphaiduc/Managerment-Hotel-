// --- 1. SIGNALR CONFIG & CHAT LOGIC ---

// Lấy thông tin từ window object (được gán trong View)
const currentUserName = window.serverUserName ? window.serverUserName : "Khách_" + Math.floor(Math.random() * 9999);
const userId = window.userId;

// Khởi tạo connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taixiugame") // Đảm bảo khớp với map ở Program.cs
    .withAutomaticReconnect()
    .build();

// Nhận tin nhắn từ Server
connection.on("ReceiveMessage", (user, message) => {
    const isMe = user === currentUserName;
    appendMessage(user, message, isMe);
});

// Nhận số lượng người online
connection.on("UserCountUpdated", (count) => {
    const el = document.getElementById('total-online');
    if (el) el.innerText = count;
});

// Admin thông báo tham gia/thoát
connection.on("AdminInformJoin", (nameuser) => {
    appendMessage("admin", `Người chơi "${nameuser}" vừa tham gia!`, true);
});

connection.on("AdminInformOut", (nameuser) => {
    appendMessage("admin", `Người chơi "${nameuser}" đã thoát!`, true);
});

// Bắt đầu kết nối
connection.start()
    .then(() => console.log("SignalR Connected!"))
    .catch(err => console.error(err));

// Xử lý gửi tin nhắn
const chatBox = document.getElementById('chatBox');
const chatInput = document.getElementById('chatInput');

if (chatInput) {
    chatInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter") sendChatMessage();
    });
}

function sendChatMessage() {
    const message = chatInput.value.trim();
    if (message === "") return;

    // Gửi lên server
    connection.invoke("SendMessage", currentUserName, message)
        .catch(err => console.error(err));

    chatInput.value = "";
}

// Hàm render tin nhắn ra giao diện
function appendMessage(user, msg, isMe = false) {
    const div = document.createElement('div');
    div.className = 'chat-message';

    const userSpan = document.createElement('span');
    userSpan.className = 'chat-user';
    if (user.toLowerCase().includes("admin")) userSpan.classList.add("admin");
    userSpan.innerText = user + ":";
    if (isMe) userSpan.style.color = "#fbbf24";

    const contentSpan = document.createElement('span');
    contentSpan.className = 'chat-content';
    contentSpan.innerText = " " + msg;

    div.appendChild(userSpan);
    div.appendChild(contentSpan);

    chatBox.appendChild(div);
    chatBox.scrollTop = chatBox.scrollHeight;
}


// --- 2. GAME LOGIC (CLIENT SIDE MOCK) ---

const timerElement = document.getElementById('countdown');
let currentBalance = 0; // Giá trị ban đầu, sẽ load từ API
let selectedChipValue = 0;
let isAllInMode = false;
let betTai = 0;
let betXiu = 0;
let timeLeft = 30;
let isBettingOpen = true;
let countdownInterval;
let usersTai = 0;
let usersXiu = 0;

function formatMoney(amount) {
    return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

// Load balance
async function loadUserBalance() {
    if (!userId) return;

    try {
        const response = await fetch(`/games/totalcoinuser/${userId}`);
        if (!response.ok) throw new Error('Failed to fetch balance');

        const data = await response.json();
        currentBalance = data.coin ?? 0;

        document.getElementById('userBalance').innerText = formatMoney(currentBalance);
    } catch (error) {
        console.error('Error loading balance:', error);
        document.getElementById('userBalance').innerText = '0';
    }
}

function selectChip(value, element) {
    isAllInMode = false;
    selectedChipValue = value;
    updateChipUI(element);
}

function selectAllIn(element) {
    if (currentBalance <= 0) {
        alert("Bạn đã cháy túi, không thể All In!");
        return;
    }
    isAllInMode = true;
    selectedChipValue = currentBalance;
    updateChipUI(element);
}

function updateChipUI(element) {
    document.querySelectorAll('.chip-btn').forEach(el => el.classList.remove('selected'));
    element.classList.add('selected');
}

function placeBet(side) {
    if (!isBettingOpen) return;
    if (isAllInMode) selectedChipValue = currentBalance;
    if (selectedChipValue === 0) { alert("Vui lòng chọn chip!"); return; }
    if (currentBalance < selectedChipValue) { alert("Số dư không đủ!"); return; }

    let betAmount = selectedChipValue;
    currentBalance -= betAmount;
    document.getElementById('userBalance').innerText = formatMoney(currentBalance);

    if (side === 'TAI') {
        betTai += betAmount;
        document.getElementById('bet-tai').innerText = formatMoney(betTai);
        triggerClickEffect('.area-tai');
        usersTai++;
        document.getElementById('users-tai').innerText = usersTai;
    } else {
        betXiu += betAmount;
        document.getElementById('bet-xiu').innerText = formatMoney(betXiu);
        triggerClickEffect('.area-xiu');
        usersXiu++;
        document.getElementById('users-xiu').innerText = usersXiu;
    }

    if (isAllInMode) {
        isAllInMode = false;
        selectedChipValue = 0;
        document.querySelectorAll('.chip-btn').forEach(el => el.classList.remove('selected'));
    }
}

function triggerClickEffect(selector) {
    const el = document.querySelector(selector);
    el.classList.remove('active-click');
    void el.offsetWidth; // Trigger reflow
    el.classList.add('active-click');
}

function startTimer() {
    if (countdownInterval) clearInterval(countdownInterval);
    countdownInterval = setInterval(() => {
        timeLeft--;
        if (timerElement) {
            timerElement.innerText = timeLeft;

            if (timeLeft <= 5) timerElement.style.color = "#ef4444";
            else timerElement.style.color = "#fff";
        }

        if (timeLeft <= 0) {
            clearInterval(countdownInterval);
            isBettingOpen = false;
            if (timerElement) timerElement.innerText = "0";
            handleResultPhase();
        }
    }, 1000);
}

function handleResultPhase() {
    const resultOverlay = document.getElementById('result-overlay');
    resultOverlay.style.display = 'block';
    resultOverlay.innerText = "ĐANG LẮC...";
    resultOverlay.style.color = "#fff";

    setTimeout(() => {
        const totalPoint = Math.floor(Math.random() * 16) + 3;
        let resultSide = totalPoint >= 11 ? "TÀI" : "XỈU";
        let textElement = resultSide === "TÀI" ? document.querySelector('.text-tai') : document.querySelector('.text-xiu');

        resultOverlay.innerText = `${totalPoint} ĐIỂM - ${resultSide}`;
        resultOverlay.style.color = resultSide === "TÀI" ? "#ef4444" : "#3b82f6";
        if (textElement) textElement.classList.add('win-animation');
        calculateWinLoss(resultSide);

        setTimeout(() => {
            if (textElement) textElement.classList.remove('win-animation');
            resetGame();
        }, 5000);
    }, 2000);
}

function calculateWinLoss(winner) {
    let winAmount = 0;
    if (winner === "TÀI" && betTai > 0) {
        winAmount = betTai * 2;
        currentBalance += winAmount;
    } else if (winner === "XỈU" && betXiu > 0) {
        winAmount = betXiu * 2;
        currentBalance += winAmount;
    }
    document.getElementById('userBalance').innerText = formatMoney(currentBalance);
}

function resetBets() {
    if (!isBettingOpen) return;
    currentBalance += betTai + betXiu;
    betTai = 0; betXiu = 0;
    document.getElementById('bet-tai').innerText = "0";
    document.getElementById('bet-xiu').innerText = "0";
    document.getElementById('userBalance').innerText = formatMoney(currentBalance);
}

function resetGame() {
    timeLeft = 30;
    isBettingOpen = true;
    if (timerElement) {
        timerElement.innerText = timeLeft;
        timerElement.style.color = "#fff";
    }
    document.getElementById('result-overlay').style.display = 'none';
    betTai = 0; betXiu = 0;
    document.getElementById('bet-tai').innerText = "0";
    document.getElementById('bet-xiu').innerText = "0";

    // Reset số lượng người chơi (ảo)
    usersTai = 0; usersXiu = 0;
    document.getElementById('users-tai').innerText = "0";
    document.getElementById('users-xiu').innerText = "0";

    isAllInMode = false;
    selectedChipValue = 0;
    document.querySelectorAll('.chip-btn').forEach(el => el.classList.remove('selected'));

    startTimer();
}

// Khởi chạy khi trang load
document.addEventListener('DOMContentLoaded', () => {
    loadUserBalance();
    startTimer();
});