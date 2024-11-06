﻿const connection = new signalR.HubConnectionBuilder()
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
    toast.className = "toast";
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3000);
}