$(document).ready(() => {
    // Initialize SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    // Select DOM elements
    const chatMessages = $('#messages-container');
    const contentInput = $('#content');
    const sendMessageButton = $('#send-message');
    const senderId = $('#senderId').val();
    const chatId = $('#chatId').val();

    // Scroll to the bottom of the chat messages
    function scrollToBottom() {
        chatMessages.scrollTop(chatMessages[0].scrollHeight);
    }

    // Escape HTML to prevent XSS attacks
    function escapeHtml(text) {
        return text.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    // Add a message to the chat container
    function addMessage(senderId, message) {
        const isReceived = $('#senderId').val() !== senderId;
        const messageType = isReceived ? 'message-received' : 'message-sent';
        const messageElement = $('<div>', {
            class: `message ${messageType}`,
            'data-message-id': message.id
        });

        messageElement.html(`
            <div class="message-content">
                ${escapeHtml(message.content)}
            </div>
            <span class="message-time">
                ${new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </span>
            ${!isReceived ? `
                <div class="message-actions">
                    <button class="btn btn-sm btn-action" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="fas fa-ellipsis-v"></i>
                    </button>
                    <ul class="dropdown-menu">
                        <li><button class="dropdown-item revoke-message" data-message-id="${message.id}">Revoke</button></li>
                    </ul>
                </div>` : ''}
        `);

        chatMessages.append(messageElement);
        scrollToBottom();
    }

    // Revoke a message visually
    function revokeMessage(messageId) {
        const messageElement = $(`[data-message-id="${messageId}"]`);
        if (messageElement.length) {
            messageElement.find('.message-content').text('[Tin nhắn đã bị thu hồi]');
            messageElement.find('.message-actions').remove();
            messageElement.addClass('revoked');
        }
    }

    // Send a message through SignalR
    function sendMessage() {
        const content = contentInput.val().trim();

        if (!content) {
            return;
        }

        connection.invoke("SendMessage", chatId, senderId, content)
            .then(() => {
                contentInput.val('');
                contentInput.focus();
                scrollToBottom();
            })
            .catch(err => console.error("Error sending message:", err));
    }

    // Setup SignalR connection
    function setupConnection() {
        connection.start()
            .then(() => {
                sendMessageButton.prop('disabled', false);
                console.log("Connected to SignalR hub");
            })
            .catch(err => {
                sendMessageButton.prop('disabled', true);
                console.error("SignalR connection error:", err.toString());
            });

        connection.on("ReceiveMessage", (senderId, message) => addMessage(senderId, message));
        connection.on("MessageRevoked", (messageId) => revokeMessage(messageId));
    }

    // Event Handlers
    sendMessageButton.click(sendMessage);

    contentInput.keypress(e => {
        if (e.key === 'Enter') {
            sendMessage();
            e.preventDefault();
        }
    });

    $(document).on('click', '.revoke-message', function () {
        const messageId = $(this).data('message-id');
        connection.invoke('RevokeMessage', messageId)
            .catch(err => console.error("Error revoking message:", err));
    });

    // Initial setup
    scrollToBottom();
    sendMessageButton.prop('disabled', true);
    setupConnection();
});
