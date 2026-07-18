document.addEventListener('DOMContentLoaded', function () {
    const amenityList = document.getElementById('amenityList');
    const newAmenityDropdown = document.getElementById('newAmenityDropdown');
    const imageListContainer = document.getElementById('imageListContainer');
    const imageUpload = document.getElementById('imageUpload');

    // --- Phần tử Avatar mới ---
    const avatarUpload = document.getElementById('avatarUpload');
    const avatarPreview = document.getElementById('avatarPreview');
    // ---

    const editRoomForm = document.getElementById('editRoomForm');
    const loadingOverlay = document.getElementById('loadingOverlay');
    const submitBtn = document.getElementById('submitBtn');
    let deletedImageIds = [];

    function showToast(message) {
        const toast = document.getElementById('successToast');
        toast.textContent = message;
        toast.classList.add('show');
        setTimeout(() => toast.classList.remove('show'), 2000);
    }

    function showError(message) {
        const toast = document.getElementById('errorToast');
        toast.textContent = message;
        toast.classList.add('show');
        setTimeout(() => toast.classList.remove('show'), 2000);
    }

    function showLoading() {
        if (loadingOverlay) {
            loadingOverlay.classList.add('active');
            loadingOverlay.setAttribute('aria-hidden', 'false');
        }
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.setAttribute('aria-disabled', 'true');
        }
    }

    function hideLoading() {
        if (loadingOverlay) {
            loadingOverlay.classList.remove('active');
            loadingOverlay.setAttribute('aria-hidden', 'true');
        }
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.removeAttribute('aria-disabled');
        }
    }

    if (newAmenityDropdown) {
        newAmenityDropdown.addEventListener('click', function (e) {
            if (e.target.matches('a.dropdown-item')) {
                e.preventDefault();
                const amenityId = e.target.dataset.id;
                const itemName = e.target.dataset.name;
                const itemIcon = e.target.dataset.icon;

                const newRow = document.createElement('tr');
                newRow.dataset.id = amenityId;
                newRow.innerHTML = `
                    <td><i class="bi ${itemIcon} me-2"></i>${itemName}</td>
                    <td class="text-end">
                        <button type="button" class="btn btn-outline-danger btn-sm btn-delete-amenity" data-id="${amenityId}">
                            <i class="bi bi-trash-fill"></i> Xóa
                        </button>
                    </td>
                `;
                amenityList.appendChild(newRow);
                e.target.closest('li').style.display = 'none';
            }
        });
    }

    if (amenityList) {
        amenityList.addEventListener('click', function (e) {
            const deleteButton = e.target.closest('.btn-delete-amenity');
            if (deleteButton) {
                const row = deleteButton.closest('tr');
                const amenityId = row.dataset.id;
                const dropdownItem = newAmenityDropdown.querySelector(`a[data-id="${amenityId}"]`);
                if (dropdownItem) dropdownItem.closest('li').style.display = 'block';
                row.remove();
            }
        });
    }

    if (imageUpload) {
        imageUpload.addEventListener('change', function (e) {
            if (e.target.files) {
                Array.from(e.target.files).forEach(file => {
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        const newImageCol = document.createElement('div');
                        newImageCol.className = 'col-6 col-md-4 col-lg-3';
                        newImageCol.innerHTML = `
                            <div class="image-thumbnail-container">
                                <img src="${event.target.result}" alt="Ảnh mới">
                                <button type="button" class="btn btn-danger btn-sm rounded-circle btn-delete-image">
                                    <i class="bi bi-x-lg"></i>
                                </button>
                            </div>
                        `;
                        imageListContainer.appendChild(newImageCol);
                    }
                    reader.readAsDataURL(file);
                });
            }
        });
    }

    if (imageListContainer) {
        imageListContainer.addEventListener('click', function (e) {
            const deleteButton = e.target.closest('.btn-delete-image');
            if (deleteButton) {
                const imageContainer = deleteButton.closest('.col-6');
                const imageId = imageContainer ? imageContainer.dataset.id : null;
                if (imageId) deletedImageIds.push(imageId);
                if (imageContainer) imageContainer.remove();
            }
        });
    }

    // --- Logic cho Avatar ---
    if (avatarUpload) {
        avatarUpload.addEventListener('change', function (e) {
            if (e.target.files && e.target.files[0]) {
                const file = e.target.files[0];
                const reader = new FileReader();
                reader.onload = function (event) {
                    avatarPreview.src = event.target.result;
                }
                reader.readAsDataURL(file);
            }
        });
    }

    if (editRoomForm) {
        editRoomForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(this);

            // 1. Thêm ảnh mới
            if (imageUpload.files) {
                Array.from(imageUpload.files).forEach(file => formData.append('NewImages', file));
            }

            // 2. Xử lý Amenity
            const amenityRows = amenityList.querySelectorAll('tr');
            const currentAmenityIds = Array.from(amenityRows).map(r => r.dataset.id);
            // Lấy dữ liệu từ window (đã được assign ở View)
            const allCurrentIds = window.initialAmenityIds || [];

            allCurrentIds.filter(id => !currentAmenityIds.includes(id.toString()))
                .forEach(id => formData.append('DeletedAmenity', id));

            currentAmenityIds.filter(id => !allCurrentIds.includes(parseInt(id)))
                .forEach(id => formData.append('NewAmenities', id));

            // 3. Xử lý Image cũ (nếu có logic này trong Create)
            deletedImageIds.forEach(id => formData.append('DeletedImageIds', id));

            // Show loading
            showLoading();

            fetch(`/admin/rooms/`, {
                method: 'POST',
                body: formData
            })
                .then(res => res.json())
                .then(data => {
                    hideLoading();

                    if (data.success) {
                        showToast('Tạo phòng thành công');
                        if (data.newAvatarUrl) {
                            avatarPreview.src = data.newAvatarUrl;
                        }

                        // Reset trạng thái
                        deletedImageIds = [];
                        imageUpload.value = '';
                        avatarUpload.value = '';

                        // Clear input fields
                        editRoomForm.querySelectorAll('input, textarea').forEach(input => {
                            if (input.type !== 'file' && input.name !== 'RoomTypeId') {
                                input.value = '';
                            }
                        });
                        // Clear image preview list
                        if (imageListContainer) {
                            imageListContainer.innerHTML = '';
                        }

                    } else {
                        showError(data.message || 'Tạo phòng thất bại');
                    }
                })
                .catch(err => {
                    console.error(err);
                    hideLoading();
                    showError('Lỗi khi gửi dữ liệu.');
                });
        });

        editRoomForm.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                editRoomForm.dispatchEvent(new Event('submit'));
            }
        });
    }
});