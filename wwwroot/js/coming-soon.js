// Đặt ngày ra mắt (1 tháng kể từ hiện tại)
const launchDate = new Date();
launchDate.setMonth(launchDate.getMonth() + 1);

function updateTimer() {
    const now = new Date();
    const distance = launchDate - now;

    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

    $('#days').text(String(days).padStart(2, '0'));
    $('#hours').text(String(hours).padStart(2, '0'));
    $('#minutes').text(String(minutes).padStart(2, '0'));
    $('#seconds').text(String(seconds).padStart(2, '0'));
}

// Cập nhật đồng hồ mỗi giây
setInterval(updateTimer, 1000);
updateTimer();

$('.subscribe button').on('click', function () {
    const input = $('.subscribe input');
    if (input.val()) {
        alert('Cảm ơn bạn đã đăng ký! Chúng tôi sẽ thông báo ngay khi ra mắt.');
        input.val('');
    }
});