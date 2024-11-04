document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    const chatMessages = document.getElementById('messages-container');
    
    connection.start().then(() => {
        console.log("Connected to SignalR hub");
    }).catch(err => {
        console.error("SignalR connection error:", err.toString());
    });

    connection.on("ReceiveMessage", (sender, message) => {
        const messageElement = document.createElement('div');
        messageElement.className = 'message-received mb-3';
        messageElement.innerHTML = `
            <div class="d-flex">
                <div class="message-content bg-body rounded-3 p-3 shadow-sm">
                    ${escapeHtml(message)}
                </div>
            </div>
            <small class="text-muted ms-2">
                ${new Date().toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
            </small>
        `;
        chatMessages.appendChild(messageElement);
        scrollToBottom();
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
                document.getElementById('chatId'),
                document.getElementById('senderId'),
                content
            )
            .catch(err => {
                console.error("Send message error:", err);
            });
        
        const messageElement = document.createElement('div');
        messageElement.className = 'message-sent mb-3';
        messageElement.innerHTML = `
            <div class="d-flex justify-content-end">
                <div class="message-content bg-primary text-white rounded-3 p-3 shadow-sm">
                    ${escapeHtml(content)}
                </div>
            </div>
            <div class="text-end">
                <small class="text-muted me-2">${time}</small>
            </div>
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