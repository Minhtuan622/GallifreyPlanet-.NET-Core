$(document).ready(function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    document.querySelector('.toggle-sidebar').addEventListener('click', () => {
        document.querySelector('.chat-sidebar').classList.toggle('show');
    });

    connection.start().then(function () {
        console.log("Connected to SignalR hub");
    }).catch(function (err) {
        console.error("SignalR connection error:", err.toString());
    });

    // Nhận tin nhắn
    connection.on("ReceiveMessage", function (sender, message) {
        $("#chat-messages").append(`
            <div class="message-received mb-3">
                <div class="d-flex">
                    <div class="message-content bg-body rounded-3 p-3 shadow-sm">
                        ${escapeHtml(message)}
                    </div>
                </div>
                <small class="text-muted ms-2">
                    ${new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </small>
            </div>
        `);
        scrollToBottom();
    });

    // Gửi tin nhắn
    function sendMessage() {
        const message = $("#message-input").val().trim();
        if (message) {
            const time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
            connection.invoke("SendMessage", "sender-id", message).catch(function (err) {
                console.error("Send message error:", err.toString());
            });

            const messageHtml = `
                        <div class="message-sent mb-3">
                            <div class="d-flex justify-content-end">
                                <div class="message-content bg-primary text-white rounded-3 p-3 shadow-sm">
                                    ${escapeHtml(message)}
                                </div>
                            </div>
                            <div class="text-end">
                                <small class="text-muted me-2">${time}</small>
                            </div>
                        </div>
                    `;
            $("#chat-messages").append(messageHtml);
            $("#message-input").val('').focus();
            scrollToBottom();
        }
    }

    // Gọi hàm sendMessage khi người dùng nhấn nút gửi hoặc nhấn Enter
    $("#send-message").click(sendMessage);
    $("#message-input").keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
            e.preventDefault();
        }
    });

    function scrollToBottom() {
        $("#chat-messages").scrollTop($("#chat-messages")[0].scrollHeight);
    }

    function escapeHtml(text) {
        return text.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
});