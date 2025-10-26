// ======================== CHẠY SAU KHI LOAD TOÀN BỘ TRANG ========================
document.addEventListener("DOMContentLoaded", function () {
    // ======================== GIỎ HÀNG ========================

    // Thêm sản phẩm vào giỏ hàng
    document.querySelectorAll(".add-to-cart").forEach(button => {
        button.addEventListener("click", (event) => {
            const productCard = event.target.closest(".product-card");
            if (!productCard) return;

            const name = productCard.querySelector(".product-name")?.textContent.trim() || "Sản phẩm";
            const priceText = productCard.querySelector(".product-price")?.textContent || "0";
            const price = parseInt(priceText.replace(/[^\d]/g, ""));
            let img = productCard.querySelector(".product-image img")?.getAttribute("src") || "";

            // Xử lý đường dẫn ảnh
            if (img.startsWith("~")) img = img.replace("~", "");
            const baseUrl = window.location.origin;
            if (img.startsWith("/")) img = baseUrl + img;

            // Tạo object sản phẩm
            const product = { name, price, img, quantity: 1 };

            // Lấy giỏ hàng hiện tại từ localStorage
            let cart = JSON.parse(localStorage.getItem("cart")) || [];

            // Kiểm tra trùng sản phẩm
            const existing = cart.find(item => item.name === product.name);
            if (existing) {
                existing.quantity += 1;
            } else {
                cart.push(product);
            }

            // Lưu lại và cập nhật giao diện
            localStorage.setItem("cart", JSON.stringify(cart));
            updateCartCount();
            renderMiniCart();

            alert(`✅ Đã thêm "${name}" vào giỏ hàng!`);
        });
    });

    // Cập nhật ngay khi load trang
    updateCartCount();
    renderMiniCart();
});


// ======================== CẬP NHẬT SỐ TRÊN ICON GIỎ HÀNG ========================
function updateCartCount() {
    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    const count = cart.reduce((total, item) => total + item.quantity, 0);
    const cartCount = document.getElementById("cart-count");
    if (cartCount) cartCount.textContent = count;
}


// ======================== HIỂN THỊ POPUP GIỎ HÀNG ========================
function renderMiniCart() {
    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    const cartItems = document.querySelector(".cart-items");
    const totalPrice = document.getElementById("total-price");

    if (!cartItems || !totalPrice) return;

    cartItems.innerHTML = "";
    let total = 0;

    if (cart.length === 0) {
        cartItems.innerHTML = "<li>Giỏ hàng trống</li>";
    } else {
        cart.forEach(item => {
            const li = document.createElement("li");
            li.classList.add("cart-item");
            li.innerHTML = `
                <img src="${item.img}" alt="${item.name}" class="cart-item-img" width="50">
                <div class="cart-item-info">
                    <span class="cart-item-name">${item.name}</span><br>
                    <span class="cart-item-price">${item.quantity} x ${item.price.toLocaleString()}đ</span>
                </div>
                <button class="remove-btn" onclick="removeFromCart('${item.name}')">Xóa</button>
            `;
            cartItems.appendChild(li);
            total += item.quantity * item.price;
        });
    }

    totalPrice.textContent = total.toLocaleString() + "đ";
}

document.querySelector('.checkout').addEventListener('click', function () {
    window.location.href = '/WBH/Giohang';
});
// ======================== XÓA MỘT SẢN PHẨM TRONG GIỎ ========================
function removeFromCart(name) {
    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    cart = cart.filter(item => item.name !== name);
    localStorage.setItem("cart", JSON.stringify(cart));
    renderMiniCart();
    updateCartCount();
}


// ======================== XÓA TOÀN BỘ GIỎ HÀNG ========================
function clearCart() {
    localStorage.removeItem("cart");
    renderMiniCart();
    updateCartCount();
}