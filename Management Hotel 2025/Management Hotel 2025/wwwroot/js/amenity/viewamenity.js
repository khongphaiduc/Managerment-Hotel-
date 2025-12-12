document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("customModal");
    const modalMessage = document.getElementById("modalMessage");
    const modalConfirm = document.getElementById("modalConfirm");
    const modalCancel = document.getElementById("modalCancel");
    const loadingOverlay = document.getElementById("loadingOverlay");
    const toastContainer = document.getElementById("toastContainer");

    let currentAction = null;
    let currentId = null;

    function showLoading() {
        if (loadingOverlay) loadingOverlay.style.display = "flex";
    }

    function hideLoading() {
        if (loadingOverlay) loadingOverlay.style.display = "none";
    }

    function showToast(message, type = "success") {
        const toast = document.createElement("div");
        toast.className = `toast toast-${type}`;
        toast.textContent = message;
        if (toastContainer) {
            toastContainer.appendChild(toast);
            setTimeout(() => { toast.remove(); }, 3000);
        }
    }

    // Capture clicks for Delete / Toggle actions
    document.addEventListener("click", function (e) {
        const deleteBtn = e.target.closest(".delete-amenity");
        const toggleBtn = e.target.closest(".toggle-visibility");

        if (deleteBtn) {
            currentAction = "delete";
            currentId = deleteBtn.getAttribute("data-id");
            modalMessage.textContent = "Bạn có chắc muốn xóa tiện ích này không?";
            modal.classList.add("active");
        }

        if (toggleBtn) {
            currentAction = "toggle";
            const tr = toggleBtn.closest("tr");
            currentId = tr.getAttribute("data-id");
            const visible = tr.getAttribute("data-visible") === "true";
            modalMessage.textContent = visible ? "Bạn có muốn ẩn tiện ích này không?" : "Bạn có muốn hiển thị tiện ích này không?";
            modal.classList.add("active");
        }
    });

    if (modalCancel) {
        modalCancel.addEventListener("click", function () {
            modal.classList.remove("active");
            currentAction = null;
            currentId = null;
        });
    }

    if (modalConfirm) {
        modalConfirm.addEventListener("click", function () {
            if (!currentAction || !currentId) return;
            showLoading();

            let url = `/admin/amenity/${currentId}`;
            let method = currentAction === "delete" ? "DELETE" : "PATCH";

            fetch(url, { method: method })
                .then(response => {
                    hideLoading();
                    if (response.ok) {
                        showToast(
                            currentAction === "delete" ? "Xóa tiện ích thành công!" : "Cập nhật trạng thái thành công!",
                            "success"
                        );
                        // Reload page after success
                        setTimeout(() => location.reload(), 800);
                    } else {
                        showToast(
                            currentAction === "delete" ? "Xóa thất bại!" : "Cập nhật thất bại!",
                            "error"
                        );
                    }
                })
                .catch(() => {
                    hideLoading();
                    showToast("Có lỗi xảy ra!", "error");
                });

            modal.classList.remove("active");
            currentAction = null;
            currentId = null;
        });
    }

    // Clear search input logic
    const clearSearchBtn = document.getElementById("clearSearch");
    if (clearSearchBtn) {
        clearSearchBtn.addEventListener("click", function () {
            const searchInput = document.getElementById("searchInput");
            if (searchInput) searchInput.value = "";
        });
    }
});