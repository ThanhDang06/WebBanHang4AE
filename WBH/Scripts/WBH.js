// ======================== DROPDOWN HOVER =======================
document.querySelectorAll('.nav-item.dropdown').forEach(dropdown => {
    dropdown.addEventListener('mouseenter', () => {
        dropdown.querySelector('.dropdown-menu')?.classList.add('show');
    });
    dropdown.addEventListener('mouseleave', () => {
        dropdown.querySelector('.dropdown-menu')?.classList.remove('show');
    });
});

// ======================== GIỎ HÀNG =============================
document.addEventListener("DOMContentLoaded", () => {
    const cartIcon = document.getElementById("cart-icon");
    const cartPopup = document.getElementById("cart-popup");
    const cartItemsEl = document.querySelector(".cart-items");
    const totalPriceEl = document.getElementById("total-price");
    const cartCountEl = document.getElementById("cart-count");
    const cartPageItemsDiv = document.getElementById("cart-items");
    const subtotalEl = document.getElementById("subtotal");
    const voucherEl = document.getElementById("voucherDiscount");
    const totalEl = document.getElementById("total");
    let cart = [];
    // ========== CHUYỂN TRANG GIỎ HÀNG ==========

    if (cartIcon) {
        cartIcon.addEventListener("click", () => {
            window.location.href = "/Carts/Index"; // chuyển tới trang giỏ hàng
        });
    }

    // ========== LẤY GIỎ HÀNG TỪ SERVER ==========
    async function fetchCart() {
        try {
            const res = await fetch("/Carts/GetCartItems", { credentials: "include" });
            if (!res.ok) return [];
            return await res.json();
        } catch {
            return [];
        }
    }

    // ========== RENDER POPUP ==========
    function renderCartPopup() {
        if (!cartItemsEl) return;
        cartItemsEl.innerHTML = "";
        let total = 0;
        if (!cart.length) {
            cartItemsEl.innerHTML = "<li>Giỏ hàng trống</li>";
        } else {
            cart.forEach(item => {
                const li = document.createElement("li");
                li.classList.add("cart-item");
                li.innerHTML = `
                    <img src="${item.Image}" alt="${item.ProductName}" width="50">
                    <div class="cart-item-info">
                        <div>${item.ProductName}</div>
                        <div>${item.Quantity} x ${item.Price.toLocaleString()}₫</div>
                    </div>
                    <button class="remove-btn" data-id="${item.IDCart}">&times;</button>
                `;
                cartItemsEl.appendChild(li);
                total += item.Quantity * item.Price;
            });
        }
        if (totalPriceEl) totalPriceEl.textContent = total.toLocaleString() + "₫";
        if (cartCountEl) cartCountEl.textContent = cart.reduce((sum, i) => sum + i.Quantity, 0);
    }

    // ========== RENDER TRÊN TRANG CART ==========
    function renderCartPage() {
        if (!cartPageItemsDiv) return;
        cartPageItemsDiv.innerHTML = "";
        let subtotal = 0;
        if (!cart.length) {
            cartPageItemsDiv.innerHTML = "<p>Giỏ hàng trống</p>";
            if (subtotalEl) subtotalEl.textContent = "0đ";
            if (totalEl) totalEl.textContent = "0đ";
            return;
        }

        cart.forEach((item, idx) => {
            const div = document.createElement("div");
            div.className = "cart-item";
            div.dataset.index = idx;
            div.innerHTML = `
            <img src="${item.Image}" width="70" alt="${item.ProductName}">
            <div class="info">
                <h4>${item.ProductName}</h4>
                <p>Giá: ${item.Price.toLocaleString()}đ</p>
                <div class="quantity">
                    <button class="decrease">-</button>
                    <span>${item.Quantity}</span>
                    <button class="increase">+</button>
                </div>
                <p>Thành tiền: ${(item.Price * item.Quantity).toLocaleString()}đ</p>
            </div>
            <button class="remove">Xóa</button>
        `;
            cartPageItemsDiv.appendChild(div);
            subtotal += item.Price * item.Quantity;
        });

        if (subtotalEl) subtotalEl.textContent = subtotal.toLocaleString() + "đ";

        const voucher = parseInt(voucherEl?.dataset.value) || 0;
        if (totalEl) totalEl.textContent = (subtotal - voucher).toLocaleString() + "đ";
    }

    // ========== LOAD CART ==========
    async function loadCart() {
        cart = await fetchCart();
        renderCartPopup();
        renderCartPage();
    }

    // ========== THÊM SẢN PHẨM ==========
    document.body.addEventListener("click", async e => {
        const btn = e.target.closest(".add-to-cart");
        if (!btn) return;
        const id = btn.dataset.id;
        if (!id) return;
        try {
            const res = await fetch("/Carts/AddToCart?id=" + id, {
                method: "POST",
                credentials: "include"
            });
            const data = await res.json();
            if (data.success) {
                await loadCart();
                cartIcon?.classList.add("added");
                setTimeout(() => cartIcon?.classList.remove("added"), 800);
            } else {
                alert(data.message);
            }
        } catch {
            alert("Lỗi server khi thêm giỏ hàng");
        }
    });

    // ========== XÓA / TĂNG GIẢM TRÊN TRANG CART ==========
    if (cartPageItemsDiv) {
        cartPageItemsDiv.addEventListener("click", e => {
            const idx = e.target.closest(".cart-item")?.dataset.index;
            if (idx === undefined) return;
            if (e.target.classList.contains("increase")) cart[idx].Quantity++;
            if (e.target.classList.contains("decrease")) cart[idx].Quantity = Math.max(1, cart[idx].Quantity - 1);
            if (e.target.classList.contains("remove") && confirm("Xóa sản phẩm?")) cart.splice(idx, 1);
            renderCartPage();
            renderCartPopup();
        });
    }

    // ========== XÓA TRÊN POPUP ==========
    if (cartItemsEl) {
        cartItemsEl.addEventListener("click", async e => {
            if (!e.target.classList.contains("remove-btn")) return;
            const id = e.target.dataset.id;
            if (!id) return;
            try {
                await fetch("/Carts/Remove?id=" + id, { method: "POST", credentials: "include" });
                await loadCart();
            } catch {
                alert("Lỗi server khi xóa sản phẩm");
            }
        });
    }

    // ========== HOVER POPUP ==========
    let hideCartTimer;
    if (cartIcon && cartPopup) {
        cartIcon.addEventListener("mouseenter", () => {
            clearTimeout(hideCartTimer);
            cartPopup.classList.add("show");
        });
        cartIcon.addEventListener("mouseleave", () => {
            hideCartTimer = setTimeout(() => cartPopup.classList.remove("show"), 300);
        });
        cartPopup.addEventListener("mouseenter", () => clearTimeout(hideCartTimer));
        cartPopup.addEventListener("mouseleave", () => {
            hideCartTimer = setTimeout(() => cartPopup.classList.remove("show"), 300);
        });
    }

    // ========== THANH TOÁN ==========
    const checkoutBtn = document.querySelector(".checkout");
    if (checkoutBtn) {
        checkoutBtn.addEventListener("click", async () => {
            if (!cart.length) { alert("Giỏ hàng trống!"); return; }
            const fullname = document.getElementById("fullname")?.value.trim();
            const phone = document.getElementById("phone")?.value.trim();
            const address = document.getElementById("address")?.value.trim();
            const city = document.getElementById("city")?.value;
            const district = document.getElementById("district")?.value;
            const ward = document.getElementById("ward")?.value;
            const note = document.getElementById("note")?.value.trim();
            const paymentMethod = document.querySelector('input[name="pay"]:checked')?.value;

            if (!fullname || !phone || !address || !city || !district || !ward) {
                alert("Điền đầy đủ thông tin!"); return;
            }

            try {
                const res = await fetch("/Carts/Checkout", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({
                        fullName: fullname,
                        phone,
                        address: `${address}, ${ward}, ${district}, ${city}`,
                        payment: paymentMethod,
                        note
                    })
                });
                const data = await res.json();
                if (data.success) window.location.href = data.redirect;
                else alert(data.message);
            } catch (e) {
                alert("Lỗi khi thanh toán!");
            }
        });
    }

    loadCart();
});
// ======================== ĐỊA CHỈ =============================
document.addEventListener("DOMContentLoaded", () => {
    const citySelect = document.getElementById("city");
    const districtSelect = document.getElementById("district");
    const wardSelect = document.getElementById("ward");

    if (!citySelect) {
        console.error("Không tìm thấy select #city");
        return;
    }

    // Lấy danh sách Tỉnh/Thành phố
    fetch("/Location/Cities")
        .then(res => res.json())
        .then(data => {
            // VAPI trả về { "results": [...] } hoặc [...]
            const cities = data.results || data;

            if (!Array.isArray(cities) || cities.length === 0) {
                console.error("Không có dữ liệu tỉnh");
                return;
            }

            cities.forEach(city => {
                const option = document.createElement("option");
                option.value = city.province_id;
                option.textContent = city.province_name;
                citySelect.appendChild(option);
            });
        })
        .catch(err => console.error("Lỗi khi tải Tỉnh/Thành phố:", err));

    // Khi chọn tỉnh → load quận
    citySelect.addEventListener("change", () => {
        const cityId = citySelect.value;
        districtSelect.innerHTML = '<option value="">Quận/huyện</option>';
        wardSelect.innerHTML = '<option value="">Phường/xã</option>';
        wardSelect.disabled = true;

        if (!cityId) {
            districtSelect.disabled = true;
            return;
        }

        fetch(`/Location/Districts/${cityId}`)
            .then(res => res.json())
            .then(data => {
                const districts = data.results || data;
                districts.forEach(district => {
                    const option = document.createElement("option");
                    option.value = district.district_id;
                    option.textContent = district.district_name;
                    districtSelect.appendChild(option);
                });
                districtSelect.disabled = false;
            })
            .catch(err => console.error("Lỗi khi tải Quận/Huyện:", err));
    });

    // Khi chọn quận → load phường
    districtSelect.addEventListener("change", () => {
        const districtId = districtSelect.value;
        wardSelect.innerHTML = '<option value="">Phường/xã</option>';

        if (!districtId) {
            wardSelect.disabled = true;
            return;
        }

        fetch(`/Location/Wards/${districtId}`)
            .then(res => res.json())
            .then(data => {
                const wards = data.results || data;
                wards.forEach(ward => {
                    const option = document.createElement("option");
                    option.value = ward.ward_id;
                    option.textContent = ward.ward_name;
                    wardSelect.appendChild(option);
                });
                wardSelect.disabled = false;
            })
            .catch(err => console.error("Lỗi khi tải Phường/Xã:", err));
    });
});



    // -------------------- SEARCH AJAX --------------------
    const $input = $("#searchInput");
    const $results = $("#searchResults");
    const ajaxUrl = $input.data("ajax-url");
    const searchPage = $input.data("search-page");

    function debounce(func, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), delay);
        }
    }

    $input.on("keyup", debounce(function (e) {
        const keyword = $(this).val().trim();

        if (e.key === "Enter") {
            e.preventDefault();
            if (keyword.length > 0) window.location.href = searchPage + "?keyword=" + encodeURIComponent(keyword);
            return;
        }

        if (keyword.length < 2) {
            $results.empty().hide();
            return;
        }

        $.getJSON(ajaxUrl, { keyword }, data => {
            if (data.length === 0) {
                $results.html('<div class="p-2 text-muted">Không có sản phẩm phù hợp</div>').show();
                return;
            }

            let html = '';
            data.forEach(item => {
                html += `
                    <a href="/Products/Details/${item.IDProduct}" class="list-group-item">
                        <img src="${item.Image}" />
                        <div>
                            <div>${item.ProductName}</div>
                            <div class="text-danger">${item.SalePrice != null ? item.SalePrice.toLocaleString('vi-VN') + '₫' : item.OriginalPrice.toLocaleString('vi-VN') + '₫'}</div>
                        </div>
                    </a>
                `;
            });
            $results.html(html).show();
        });
    }, 300));

    // Ẩn dropdown khi click ra ngoài
    $(document).click(e => {
        if (!$(e.target).closest(".position-relative").length) $results.hide();
    });