// Confetti effect when page loads
window.onload = function () {
    setTimeout(() => {
        confetti({
            particleCount: 150,
            spread: 70,
            origin: { y: 0.6 },
            colors: ['#12c048', '#f5d442', '#e85f5c', '#3ec6ff', '#e3ffe4'],
        });
    }, 700);
};

// Hiệu ứng pháo hoa bắn từ hai bên vào trung tâm

window.onload = function () {
    // Bắn pháo hoa từ trái sang phải
    confetti({
        particleCount: 80,
        angle: 60,
        spread: 55,
        origin: { x: 0, y: 0.6 },
        colors: ['#12c048', '#f5d442', '#e85f5c', '#3ec6ff', '#e3ffe4'],
    });
    // Bắn pháo hoa từ phải sang trái
    confetti({
        particleCount: 80,
        angle: 120,
        spread: 55,
        origin: { x: 1, y: 0.6 },
        colors: ['#12c048', '#f5d442', '#e85f5c', '#3ec6ff', '#e3ffe4'],
    });
    // Bắn pháo hoa nhỏ giữa trung tâm (tùy chọn, có thể bỏ)
    setTimeout(() => {
        confetti({
            particleCount: 50,
            spread: 70,
            origin: { x: 0.5, y: 0.6 },
            colors: ['#12c048', '#f5d442', '#e85f5c', '#3ec6ff', '#e3ffe4'],
        });
    }, 900);
};