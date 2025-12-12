document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('fileInput');
    const leftPreview = document.getElementById('leftPreview');
    const nameInput = document.getElementById('nameInput');
    const descInput = document.getElementById('descInput');
    const updateBtn = document.getElementById('updateBtn');
    const backBtn = document.getElementById('backBtn');
    const idInput = document.getElementById('amenityId');

    let currentFile = null;

    // Xem trước ảnh khi chọn file
    if (fileInput) {
        fileInput.addEventListener('change', e => {
            const file = e.target.files[0];
            if (!file) return;
            currentFile = file;

            const reader = new FileReader();
            reader.onload = ev => {
                leftPreview.innerHTML = `<img src="${ev.target.result}" alt="Preview" />`;
            };
            reader.readAsDataURL(file);
        });
    }

    // Nút quay lại
    if (backBtn) {
        backBtn.addEventListener('click', function () {
            history.back();
        });
    }

    // Hàm hiển thị Toast
    const showToast = (message, type = 'success') => {
        const toast = document.getElementById('toast');
        if (!toast) return;

        toast.textContent = message;
        toast.className = `show ${type}`;

        setTimeout(() => {
            toast.className = '';
        }, 3000);
    };

    // Xử lý cập nhật tiện ích
    if (updateBtn) {
        updateBtn.addEventListener('click', () => {
            const id = idInput.value;
            const name = nameInput.value.trim();
            const description = descInput.value.trim();

            if (!name) {
                showToast('Tên tiện ích không được để trống!', 'error');
                return;
            }

            const formData = new FormData();
            formData.append('AmenityId', id);
            formData.append('Name', name);
            formData.append('Description', description);

            if (currentFile) {
                formData.append('UpdateImage', currentFile);
            }

            // Disable nút để tránh spam
            updateBtn.disabled = true;
            showToast('Đang lưu...', 'success');

            fetch(`/admin/amenity`, {
                method: 'PUT',
                body: formData
            })
                .then(async response => {
                    const data = await response.json();
                    if (response.ok) {
                        showToast(data.message || 'Đã cập nhật thành công!', 'success');
                    } else {
                        showToast(data.message || 'Cập nhật thất bại!', 'error');
                    }
                })
                .catch(() => {
                    showToast('Có lỗi xảy ra khi kết nối server!', 'error');
                })
                .finally(() => {
                    updateBtn.disabled = false;
                });
        });
    }
});