const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

connection.start().catch(err => console.error(err));

connection.on("ReceiveNotification", (userId, message, type) => {
    const notificationList = document.getElementById("notification-list");
    const notificationCount = document.getElementById("notification-count");

    const notification = document.createElement("li");
    notification.innerHTML = `
        <div class="dropdown-item">
            <p class="notification-message">${message}</p>
            <small class="notification-time">Just now</small>
        </div>
    `;

    notificationList.insertBefore(notification, notificationList.firstChild);

    const currentCount = parseInt(notificationCount.textContent);
    notificationCount.textContent = currentCount + 1;

    showToast(message);
});

function showToast(message) {
    const toast = document.createElement("div");
    toast.className = "toast-container position-fixed bottom-0 end-0 p-3";
    toast.innerHTML = `
        <div id="liveToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header">
              <img src="..." class="rounded me-2" alt="...">
              <strong class="me-auto">Thông báo mới</strong>
              <small>Bây giờ</small>
              <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
              ${message}
            </div>
        </div>
    `;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3000);
}