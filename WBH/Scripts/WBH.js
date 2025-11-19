
// ======================== WBH.js =======================
document.addEventListener("DOMContentLoaded", () => {
    // -------------------- DROPDOWN HOVER --------------------
    

    function initDropdown(dropdownsSelector, toggleSelector = null) {
        const dropdowns = document.querySelectorAll(dropdownsSelector);

        dropdowns.forEach(dropdown => {
            const menu = dropdown.querySelector('.dropdown-menu');
            if (!menu) return;
            const toggle = toggleSelector ? dropdown.querySelector(toggleSelector) : null;
            const link = dropdown.querySelector('.nav-link');

            // Desktop hover
            dropdown.addEventListener('mouseenter', () => {
                if (window.innerWidth > 768) menu.classList.add('show');
            });
            dropdown.addEventListener('mouseleave', () => {
                if (window.innerWidth > 768) menu.classList.remove('show');
            });

            // Mobile click
            toggle?.addEventListener('click', e => {
                e.preventDefault();
                e.stopPropagation();
                dropdowns.forEach(d => {
                    const m = d.querySelector('.dropdown-menu');
                    if (m !== menu) m.classList.remove('show');
                });
                menu.classList.toggle('show');
            });

            // Ngăn link nhầm trên mobile
            if (link && toggleSelector) {
                link.addEventListener('click', e => {
                    if (window.innerWidth <= 768 && !e.target.closest(toggleSelector)) e.preventDefault();
                });
            }
        });

        // Click ngoài đóng dropdown
        document.addEventListener('click', e => {
            dropdowns.forEach(d => {
                const m = d.querySelector('.dropdown-menu');
                if (m && !e.target.closest(dropdownsSelector)) {
                    m.classList.remove('show');
                }
            });
        });

        // Resize reset
        window.addEventListener('resize', () => {
            dropdowns.forEach(d => {
                const m = d.querySelector('.dropdown-menu');
                if (m) m.classList.remove('show');
            });
        });

    }

    // Menu chính
    initDropdown('.nav-item.dropdown', '.dropdown-toggle-mobile');
    // Login dropdown
    initDropdown('.header-login.dropdown', '.dropdown-toggle');
    // -------------------- GIỎ HÀNG -------------------------
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

    // Chuyển trang giỏ hàng
    cartIcon?.addEventListener("click", () => {
        window.location.href = "/Carts/Index";
    });

    async function fetchCart() {
        try {
            const res = await fetch("/Carts/GetCartItems", { credentials: "include" });
            if (!res.ok) return [];
            return await res.json();
        } catch {
            return [];
        }
    }

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
            <div>Màu: ${item.ColorName} | Size: ${item.SizeName}</div>
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
        <p>Màu: ${item.ColorName} | Size: ${item.SizeName}</p>
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

        // Lấy voucher từ dataset
        const discount = parseFloat(voucherEl?.dataset.value) || 0;
        if (totalEl) totalEl.textContent = (subtotal - discount).toLocaleString() + "đ";
    }


    async function loadCart() {
        cart = await fetchCart();
        renderCartPopup();
        renderCartPage();
    }

    // Thêm sản phẩm
    document.body.addEventListener("click", async e => {
        const btn = e.target.closest(".add-to-cart");
        if (!btn) return;

        const id = btn.dataset.id;
        const colorId = btn.dataset.color || ""; 
        const sizeId = btn.dataset.size || "";
        const quantity = btn.dataset.quantity || 1;

        if (!id) {
            alert("Sản phẩm không hợp lệ");
            return;
        }

        try {
            const res = await fetch("/Carts/AddToCart", {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: `id=${id}&colorId=${colorId || 0}&sizeId=${sizeId || 0}&quantity=${quantity}`
            });

            const data = await res.json();

            if (data.success) {
                await loadCart();
                cartIcon?.classList.add("added");
                setTimeout(() => cartIcon?.classList.remove("added"), 800);
            } else {
                alert(data.message);
            }
        } catch (err) {
            console.error(err);
            alert("Vui lòng đăng nhập, trước khi thêm vào giỏ hàng");
        }
    });


    // Xóa / tăng giảm trên trang cart
    cartPageItemsDiv?.addEventListener("click", e => {
        const idx = e.target.closest(".cart-item")?.dataset.index;
        if (idx === undefined) return;
        if (e.target.classList.contains("increase")) cart[idx].Quantity++;
        if (e.target.classList.contains("decrease")) cart[idx].Quantity = Math.max(1, cart[idx].Quantity - 1);
        if (e.target.classList.contains("remove") && confirm("Xóa sản phẩm?")) cart.splice(idx, 1);
        renderCartPage();
        renderCartPopup();
    });

    // Xóa trên popup
    cartItemsEl?.addEventListener("click", async e => {
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

    // Hover popup
    if (cartIcon && cartPopup) {
        let hideCartTimer;
        cartIcon.addEventListener("mouseenter", () => { clearTimeout(hideCartTimer); cartPopup.classList.add("show"); });
        cartIcon.addEventListener("mouseleave", () => { hideCartTimer = setTimeout(() => cartPopup.classList.remove("show"), 300); });
        cartPopup.addEventListener("mouseenter", () => clearTimeout(hideCartTimer));
        cartPopup.addEventListener("mouseleave", () => { hideCartTimer = setTimeout(() => cartPopup.classList.remove("show"), 300); });
    }
    // -------------------- VOUCHER -------------------------
    const voucherRadios = document.querySelectorAll('input[name="voucher"]');
    const applyVoucherBtn = document.getElementById('applyVoucher');
    const customVoucherInput = document.getElementById('customVoucher');
    const removeVoucherBtn = document.getElementById('removeVoucherBtn');

    function applyVoucher(code) {
        fetch('/Carts/ApplyVoucher', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ code })
        })
            .then(res => res.json())
            .then(res => {
                if (res.success) {
                    const discount = parseFloat(res.discountValue) || 0;
                    if (voucherEl) {
                        voucherEl.dataset.value = discount;
                        voucherEl.textContent = discount.toLocaleString('vi-VN') + "₫";
                    }

                    // Cập nhật total ngay, không gọi renderCartPage() để tránh ghi đè
                    const subtotal = cart.reduce((sum, i) => sum + i.Price * i.Quantity, 0);
                    if (totalEl) totalEl.textContent = (subtotal - discount).toLocaleString('vi-VN') + "₫";
                    removeVoucherBtn.style.display = "inline-block";
                } else {
                    alert(res.message);
                }
            })

            .catch(() => alert("Lỗi khi áp dụng voucher"));
    }
    function removeVoucher() {
        // Xóa session trên server nếu muốn
        fetch('/Carts/RemoveVoucher', { method: 'POST', credentials: 'include' });

        // Bỏ chọn radio và clear input custom
        voucherRadios.forEach(r => r.checked = false);
        customVoucherInput.value = '';

        // Reset giá
        const subtotal = cart.reduce((sum, i) => sum + i.Price * i.Quantity, 0);
        if (voucherEl) {
            voucherEl.dataset.value = 0;
            voucherEl.textContent = '0₫';
        }
        if (totalEl) totalEl.textContent = subtotal.toLocaleString('vi-VN') + "₫";
        removeVoucherBtn.style.display = "none";
    }

    removeVoucherBtn?.addEventListener('click', removeVoucher);
    // Áp dụng khi chọn radio
    voucherRadios.forEach(radio => {
        radio.addEventListener('change', () => {
            if (radio.checked) applyVoucher(radio.value);
        });
    });

    // Áp dụng khi nhập voucher custom
    applyVoucherBtn?.addEventListener('click', () => {
        // Bỏ chọn radio nếu có
        voucherRadios.forEach(r => r.checked = false);
        const code = customVoucherInput.value.trim();
        if (code) applyVoucher(code);
    });


    // -------------------- CHECKOUT -------------------------
    document.querySelectorAll(".checkout").forEach(btn => {
        btn.addEventListener("click", () => {
            window.location.href = "/Carts/Index";
        });
    });

    const checkoutBtn = document.getElementById("checkoutBtn");
    checkoutBtn?.addEventListener("click", async (e) => {
        e.preventDefault();

        if (!cart.length) { alert("Giỏ hàng trống!"); return; }
        const fullname = document.getElementById("fullname")?.value.trim();
        const phone = document.getElementById("phone")?.value.trim();
        const address = document.getElementById("address")?.value.trim();
        const cityText = citySelect.options[citySelect.selectedIndex].textContent;
        const districtText = districtSelect.options[districtSelect.selectedIndex].textContent;
        const wardText = wardSelect.options[wardSelect.selectedIndex].textContent;
        const note = document.getElementById("note")?.value.trim();
        const paymentMethod = document.querySelector('input[name="pay"]:checked')?.value;

        if (!fullname || !phone || !address || !city || !district || !ward) {
            alert("Điền đầy đủ thông tin!"); return;
        }

        const parts = [address, wardText, districtText, cityText].filter(p => p && p.trim() !== "");
        const fullAddress = parts.join(", ");


        try {
            const res = await fetch("/Carts/Checkout", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({
                    fullName: fullname,
                    phone,
                    address: fullAddress,
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
    loadCart();

    // -------------------- ĐỊA CHỈ -------------------------
    const citySelect = document.getElementById("city");
    const districtSelect = document.getElementById("district");
    const wardSelect = document.getElementById("ward");

    if (citySelect && districtSelect && wardSelect) {
        fetch("/Location/Cities")
            .then(res => res.json())
            .then(data => {
                const cities = data.results || data;
                if (!Array.isArray(cities) || !cities.length) return;
                cities.forEach(c => {
                    const option = document.createElement("option");
                    option.value = c.province_id;
                    option.textContent = c.province_name;
                    citySelect.appendChild(option);
                });
            }).catch(err => console.error("Lỗi khi tải Tỉnh/Thành phố:", err));

        citySelect.addEventListener("change", () => {
            const cityId = citySelect.value;
            districtSelect.innerHTML = '<option value="">Quận/huyện</option>';
            wardSelect.innerHTML = '<option value="">Phường/xã</option>';
            wardSelect.disabled = true;
            if (!cityId) { districtSelect.disabled = true; return; }

            fetch(`/Location/Districts/${cityId}`)
                .then(res => res.json())
                .then(data => {
                    const districts = data.results || data;
                    districts.forEach(d => {
                        const option = document.createElement("option");
                        option.value = d.district_id;
                        option.textContent = d.district_name;
                        districtSelect.appendChild(option);
                    });
                    districtSelect.disabled = false;
                }).catch(err => console.error("Lỗi khi tải Quận/Huyện:", err));
        });

        districtSelect.addEventListener("change", () => {
            const districtId = districtSelect.value;
            wardSelect.innerHTML = '<option value="">Phường/xã</option>';
            if (!districtId) { wardSelect.disabled = true; return; }

            fetch(`/Location/Wards/${districtId}`)
                .then(res => res.json())
                .then(data => {
                    const wards = data.results || data;
                    wards.forEach(w => {
                        const option = document.createElement("option");
                        option.value = w.ward_id;
                        option.textContent = w.ward_name;
                        wardSelect.appendChild(option);
                    });
                    wardSelect.disabled = false;
                }).catch(err => console.error("Lỗi khi tải Phường/Xã:", err));
        });
    }

    // -------------------- SEARCH AJAX -----------------------
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

        // Nhấn Enter đi trang Search
        if (e.key === "Enter") {
            e.preventDefault();
            if (keyword.length) window.location.href = searchPage + "?keyword=" + encodeURIComponent(keyword);
            return;
        }

        if (keyword.length < 2) {
            $results.empty().hide();
            return;
        }

        $.getJSON(ajaxUrl, { keyword }, data => {
            if (!data.length) {
                $results.html('<div class="p-2 text-muted">Không có sản phẩm phù hợp</div>').show();
                return;
            }

            let html = '';
            data.forEach(item => {
                html += `
            <a href="/Products/Details/${item.IDProduct}" class="list-group-item d-flex align-items-center ">
                <img src="${item.Image}" class="me-2" width="50" />
                <div>
                    <div>${item.ProductName}</div>
                    <div>
                        ${item.SalePrice != null
                        ? `<span class="text-muted text-decoration-line-through">${item.OriginalPrice.toLocaleString('vi-VN')}₫</span>
                               <span class="text-danger fw-bold ms-1">${item.SalePrice.toLocaleString('vi-VN')}₫</span>`
                        : `<span>${item.OriginalPrice.toLocaleString('vi-VN')}₫</span>`
                    }
                    </div>
                    ${item.SaleLabel != null ? `<span class="badge bg-danger mt-1">${item.SaleLabel}</span>` : ''}
                </div>
            </a>
        `;
            });

            $results.html(html).show();
        });
    }, 300));

    // click ngoài ẩn dropdown
    $(document).click(e => {
        if (!$(e.target).closest(".position-relative").length) $results.hide();
    });

});
