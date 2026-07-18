function generateToken(data) {
    // Demo: sinh token giả lập. Thực tế nên tạo ở backend!
    return btoa(JSON.stringify(data)) + '-' + Math.random().toString(36).slice(2, 18); // Đã sửa .substr thành .slice tránh deprecated!
}

document.getElementById('tokenForm').onsubmit = function (e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const audience = document.getElementById('audience').value;
    const desc = document.getElementById('desc').value;
    const expiry = document.getElementById('expiry').value;
    const scopes = [];
    ['read_rooms', 'book_rooms', 'manage_customers', 'view_reports', 'manage_services', 'admin'].forEach(scope => {
        if (document.getElementById('scope-' + scope).checked) scopes.push(scope);
    });
    // Hiệu ứng loading
    const btn = document.getElementById('btnCreate');
    btn.disabled = true; btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Đang tạo...';
    setTimeout(function () {
        const token = generateToken({ username, audience, desc, expiry, scopes });
        document.getElementById('tokenResult').style.display = 'block';
        document.getElementById('tokenResult').innerHTML = `
      <strong><i class="bi bi-check-circle-fill text-success"></i> Token của bạn:</strong>
      <br>
      <code>${token}</code>
      <br>
      <button class="btn btn-success mt-3" onclick="navigator.clipboard.writeText('${token}')">
        <i class="bi bi-clipboard"></i> Copy token
      </button>
    `;
        btn.disabled = false; btn.innerHTML = '<i class="bi bi-magic"></i> Tạo token';
    }, 1000);
};