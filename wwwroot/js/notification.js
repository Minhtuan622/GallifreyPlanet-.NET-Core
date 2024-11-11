$(document).ready(function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    connection.start()
        .then(() => {
            connection.invoke("GetNotifications");
        })
        .catch(err => console.error("Connection error:", err));

    connection.on("ReceiveNotifications", (notifications) => {
        renderNotificationList(notifications);
    });

    connection.on("ReceiveNotification", (userId, message) => {
        const notificationCount = $("#notification-count");

        $("#notification-list").prepend($("<li>", {
            class: "dropdown-item unread-notification"
        }).html(`
            <p class="notification-message">${message}</p>
            <small class="notification-time">Just now</small>
        `));

        const currentCount = parseInt(notificationCount.text());
        notificationCount.text(currentCount + 1);

        showToast(message);
    });

    $("#btnMarkAllAsRead").on("click", function (event) {
        event.preventDefault();
        markAllAsRead();
    });

    function renderNotificationList(notifications) {
        const notificationList = $("#notification-list");
        const notReads = notifications
            .filter(notification => !notification.isRead)
            .map(notification => notification.id);

        notificationList.data("not-read-ids", notReads);

        let unreadCount = 0;

        if (notifications.length > 0) {
            notifications.forEach(notification => {
                if (!notification.isRead) {
                    unreadCount++;
                }

                const listItem = $("<li>", {
                    class: "dropdown-item"
                });

                if (!notification.isRead) {
                    listItem.addClass("unread-notification");
                }

                listItem.html(`
                    <p class="notification-message">${notification.content}</p>
                    <small class="notification-time">${new Date(notification.createdAt).toLocaleString()}</small>
                `);
                notificationList.append(listItem);
            });
        } else {
            notificationList.append($("<li>", {
                class: "dropdown-item"
            }).html(`
                <p class="notification-message">Chưa có thông báo</p>
            `));
        }

        $("#notification-count").text(unreadCount);
    }

    function markAllAsRead() {
        connection
            .invoke(
                "MarkAllAsRead",
                $("#notification-list").data("not-read-ids")
            )
            .then(() => {
                $(".unread-notification").removeClass("unread-notification");
                $("#notification-count").text("0");
            })
            .catch(console.error);
    }

    function showToast(message) {
        const toast = $("<div>", {
            class: "toast align-items-center text-bg-primary border-0",
            role: "alert",
            "aria-live": "assertive",
            "aria-atomic": "true"
        }).html(`
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button 
                    type="button" 
                    class="btn-close btn-close-white me-2 m-auto" 
                    data-bs-dismiss="toast" 
                    aria-label="Close"
                ></button>
            </div>
        `);

        $(".toast-container").append(toast);

        new bootstrap.Toast(toast[0]).show();

        setTimeout(() => {
            toast.remove();
        }, 3000);
    }
});
