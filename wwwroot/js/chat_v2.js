document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    const chatMessages = document.getElementById('messages-container');

    scrollToBottom();

    document.getElementById("send-message").disabled = true;

    connection.start().then(() => {
        document.getElementById("send-message").disabled = false;
        console.log("Connected to SignalR hub");
    }).catch(err => {
        console.error("SignalR connection error:", err.toString());
    });

    connection.on("ReceiveMessage", (senderId, message) => {
        if (document.getElementById('senderId').value !== senderId) {
            const messageElement = document.createElement('div');
            messageElement.className = 'message message-received';
            messageElement.innerHTML = `  
            <div class="message-content">
                ${escapeHtml(message)}
            </div>
            <span class="message-time">
                ${new Date().toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
            </span>
        `;
            chatMessages.appendChild(messageElement);
            scrollToBottom();
        }
    });

    function sendMessage() {
        const contentInput = document.getElementById('content');
        const content = contentInput.value.trim();

        if (!content) {
            return;
        }

        const time = new Date().toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'});

        connection
            .invoke(
                "SendMessage",
                document.getElementById('chatId').value,
                document.getElementById('senderId').value,
                content
            )
            .catch(err => {
                console.error("Send message error:", err);
            });

        const messageElement = document.createElement('div');
        messageElement.className = 'message message-sent';
        messageElement.innerHTML = `
            <div class="message-content">
                ${escapeHtml(content)}
            </div>
            <span class="message-time">${time}</span>
        `;
        chatMessages.appendChild(messageElement);
        contentInput.value = '';
        contentInput.focus();
        scrollToBottom();
    }

    document.getElementById('send-message').addEventListener('click', sendMessage);
    document.getElementById('content').addEventListener('keypress', e => {
        if (e.key === 'Enter') {
            sendMessage();
            e.preventDefault();
        }
    });

    function scrollToBottom() {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function escapeHtml(text) {
        return text.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
});