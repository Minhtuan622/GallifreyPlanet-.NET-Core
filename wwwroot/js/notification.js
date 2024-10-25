// notification.js
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

connection.start().catch(err => console.error(err));

connection.on("ReceiveNotification", (user, message) => {
    // Thêm thông báo mới vào UI
    const notificationList = document.getElementById("notificationList");
    const notificationCount = document.getElementById("notificationCount");

    const notification = document.createElement("div");
    notification.className = "notification-item";
    notification.innerHTML = `
        <div class="notification-message">${message}</div>
        <div class="notification-time">Just now</div>
    `;

    notificationList.insertBefore(notification, notificationList.firstChild);

    // Cập nhật số lượng thông báo
    const currentCount = parseInt(notificationCount.textContent);
    notificationCount.textContent = currentCount + 1;

    // Hiển thị toast notification
    showToast(message);
});

function showToast(message) {
    const toast = document.createElement("div");
    toast.className = "toast";
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3000);
}