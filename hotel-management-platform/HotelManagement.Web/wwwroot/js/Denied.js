document.addEventListener("DOMContentLoaded", function () {
    var btn = document.getElementById('goHomeBtn');
    if (btn) {
        btn.addEventListener('click', function () {
            window.location.href = '/';
        });
    }
});