// Kiểm tra trạng thái đơn hàng định kỳ
$(document).ready(function () {

    function checkOrderStatus() {
        $('.order-status-badge').each(function () {
            var badge = $(this);
            var orderId = badge.data('order-id');
            var getStatusUrl = badge.data('get-status-url');

            if (!orderId || !getStatusUrl) return;

            $.ajax({
                url: getStatusUrl,
                type: 'GET',
                data: { id: orderId },
                success: function (response) {
                    if (response.status && response.status !== badge.text()) {
                        badge.text(response.status);

                        // Đổi màu theo trạng thái
                        var color = 'secondary';
                        if (response.status === "Đã giao" || response.status === "Hoàn thành") color = 'success';
                        else if (response.status === "Đang xử lý") color = 'warning';
                        else if (response.status === "Đã hủy") color = 'danger';

                        badge.removeClass().addClass('badge bg-' + color);
                    }
                },
                error: function () {
                    console.warn("Lỗi khi lấy trạng thái đơn hàng #" + orderId);
                }
            });
        });
    }

    // Kiểm tra trạng thái mỗi 5 giây
    setInterval(checkOrderStatus, 5000);
});
