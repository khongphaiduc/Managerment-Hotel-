document.addEventListener("DOMContentLoaded", function () {

    let selectedRoomId = null;

    // Loader khi nhấn edit
    const editButtons = document.querySelectorAll('.btn-edit');
    editButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const loader = document.getElementById('page-loader');
            if (loader) loader.style.display = 'block';
        });
    });

    // Auto-submit form khi chọn tầng hoặc trạng thái
    const autoSubmitSelects = document.querySelectorAll('.auto-submit');
    autoSubmitSelects.forEach(select => {
        select.addEventListener('change', function () {
            document.getElementById('filterForm').submit();
        });
    });

    // Reset filter
    const resetBtn = document.getElementById('resetFilter');
    if (resetBtn) {
        resetBtn.addEventListener('click', function () {
            const form = document.getElementById('filterForm');
            form.querySelectorAll('select, input').forEach(el => {
                if (el.tagName.toLowerCase() === 'select') el.selectedIndex = 0;
                else if (el.tagName.toLowerCase() === 'input') el.value = '';
            });
            form.submit();
        });
    }

    // Xử lý nút ẩn phòng
    const hideButtons = document.querySelectorAll('.btn-hide-room');
    hideButtons.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            selectedRoomId = this.getAttribute('data-id');

            // Mở modal
            const modalElement = document.getElementById('hideRoomModal');
            if (modalElement) {
                var hideModal = new bootstrap.Modal(modalElement);
                hideModal.show();
            }
        });
    });

    // Xử lý nút "Có" trong modal
    const confirmBtn = document.getElementById('confirmHideBtn');
    if (confirmBtn) {
        confirmBtn.addEventListener('click', function () {
            if (!selectedRoomId) return;

            const loader = document.getElementById('page-loader');
            if (loader) loader.style.display = 'block';

            fetch(`/admin/hide/${selectedRoomId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
                .then(response => {
                    if (response.ok) {
                        location.reload();
                    } else {
                        alert("Ẩn phòng thất bại!");
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Có lỗi xảy ra!");
                })
                .finally(() => {
                    if (loader) loader.style.display = 'none';
                    selectedRoomId = null;
                    var hideModalEl = document.getElementById('hideRoomModal');
                    var hideModal = bootstrap.Modal.getInstance(hideModalEl);
                    if (hideModal) hideModal.hide();
                });
        });
    }
});