(function () {
    const fileInput = document.getElementById('fileInput');
    const leftPreview = document.getElementById('leftPreview');
    const leftPlaceholder = document.getElementById('leftPlaceholder');
    const fileNameEl = document.getElementById('fileName');
    const clearImageBtn = document.getElementById('clearImageBtn');
    const nameInput = document.getElementById('nameInput');
    const descInput = document.getElementById('descInput');
    const statusInput = document.getElementById('statusInput');
    const createBtn = document.getElementById('createBtn');
    const feedback = document.getElementById('feedback');

    let currentFile = null;

    function updateCreateButtonState() {
        const hasFile = !!currentFile;
        const hasName = nameInput.value.trim().length > 0;
        createBtn.disabled = !(hasFile && hasName);
    }

    function clearPreview() {
        currentFile = null;
        const img = leftPreview.querySelector('img');
        if (img) img.remove();
        leftPlaceholder.style.display = 'block';
        fileNameEl.textContent = 'Không có tệp nào được chọn';
        clearImageBtn.style.display = 'none';
        updateCreateButtonState();
    }

    fileInput.addEventListener('change', function (e) {
        const file = e.target.files && e.target.files[0];
        handleFileSelected(file);
    });

    // Drag and Drop events
    leftPreview.addEventListener('dragover', function (e) {
        e.preventDefault();
        leftPreview.style.background = '#f1f3f5';
    });
    leftPreview.addEventListener('dragleave', function (e) {
        leftPreview.style.background = '';
    });
    leftPreview.addEventListener('drop', function (e) {
        e.preventDefault();
        leftPreview.style.background = '';
        const file = e.dataTransfer.files && e.dataTransfer.files[0];
        if (file) {
            try {
                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                fileInput.files = dataTransfer.files;
            } catch { }
            handleFileSelected(file);
        }
    });

    function handleFileSelected(file) {
        if (!file) {
            clearPreview();
            return;
        }

        if (!file.type.startsWith('image/')) {
            alert('Vui lòng chọn tệp hình ảnh (jpg, png, ...).');
            fileInput.value = '';
            clearPreview();
            return;
        }

        const maxBytes = 5 * 1024 * 1024; // 5MB
        if (file.size > maxBytes) {
            alert('Kích thước ảnh quá lớn. Vui lòng chọn ảnh < 5MB.');
            fileInput.value = '';
            clearPreview();
            return;
        }

        fileNameEl.textContent = file.name;
        clearImageBtn.style.display = 'inline-block';

        const reader = new FileReader();
        reader.onload = function (ev) {
            leftPlaceholder.style.display = 'none';
            let img = leftPreview.querySelector('img');
            if (!img) {
                img = document.createElement('img');
                img.alt = 'Ảnh xem trước';
                leftPreview.appendChild(img);
            }
            img.src = ev.target.result;
        };
        reader.readAsDataURL(file);

        currentFile = file;
        updateCreateButtonState();
    }

    clearImageBtn.addEventListener('click', function () {
        fileInput.value = '';
        clearPreview();
    });

    nameInput.addEventListener('input', updateCreateButtonState);

    createBtn.addEventListener('click', function () {
        if (!currentFile) return alert('Vui lòng chọn ảnh.');
        const name = nameInput.value.trim();
        if (!name) return alert('Vui lòng nhập tên.');
        const desc = descInput.value.trim();
        const status = statusInput.value;

        const formData = new FormData();
        formData.append('Name', name);
        formData.append('Status', status);
        formData.append('Description', desc);
        formData.append('UpdateImage', currentFile);

        feedback.textContent = 'Đang tạo...';
        createBtn.disabled = true;

        fetch('/admin/amenity', {
            method: 'POST',
            body: formData
        })
            .then(res => res.json())
            .then(data => {
                if (data.status) {
                    feedback.innerHTML = 'Đã tạo: <strong>' + escapeHtml(name) + '</strong>';
                    clearPreview();
                    nameInput.value = '';
                    descInput.value = '';
                } else {
                    feedback.textContent = data.message || 'Có lỗi xảy ra.';
                }
                createBtn.disabled = false;
                updateCreateButtonState();
            })
            .catch(err => {
                feedback.textContent = 'Có lỗi xảy ra.';
                createBtn.disabled = false;
                console.error(err);
            });
    });

    function escapeHtml(str) {
        return str.replace(/[&<>"']/g, function (m) {
            return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]);
        });
    }

    // Init
    clearPreview();
    updateCreateButtonState();
})();