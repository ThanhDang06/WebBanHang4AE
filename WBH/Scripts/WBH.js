function login() {
    const user = document.getElementById('username').value;
    const pass = document.getElementById('password').value;

    //Thử nghiệm đăng nhập
    // Giả lập tài khoản
    if (user === 'admin' && pass === '123') {
        alert('Đăng nhập thành công!');
        localStorage.setItem('loggedIn', 'true'); // Lưu trạng thái đăng nhập
        window.location.href = 'sanpham.html';
    } else {
        alert('Sai tài khoản hoặc mật khẩu!');
    }
}   