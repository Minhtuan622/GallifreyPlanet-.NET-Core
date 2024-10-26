const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

connection.start().catch(err => console.error(err));

connection.on("ReceiveNotification", (user, message) => {
    const notificationList = document.getElementById("notificationList");
    const notificationCount = document.getElementById("notificationCount");

    const notification = document.createElement("div");
    notification.className = "notification-item";
    notification.innerHTML = `
        <div class="notification-message">${message}</div>
        <div class="notification-time">Just now</div>
    `;

    notificationList.insertBefore(notification, notificationList.firstChild);

    const currentCount = parseInt(notificationCount.textContent);
    notificationCount.textContent = currentCount + 1;

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