// Hàm cập nhật số lượng trên icon giỏ hàng
function updateCartCount() {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const count = cart.reduce((total, item) => total + item.quantity, 0);
    const cartCount = document.getElementById('cart-count');
    if (cartCount) cartCount.textContent = count;
}

// Cập nhật khi load trang
document.addEventListener("DOMContentLoaded", updateCartCount);
  // Lấy tất cả các link có data-category trong menu
    const categoryLinks = document.querySelectorAll('a[data-category]');
    const products = document.querySelectorAll('.product');

  categoryLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault(); // chặn load lại trang
            const category = link.dataset.category;

            products.forEach(product => {
                if (category === 'all' || product.dataset.category === category) {
                    product.style.display = 'block';
                } else {
                    product.style.display = 'none';
                }
            });
        });
  });