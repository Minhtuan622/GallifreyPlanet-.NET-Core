$(document).ready(() => {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    const chatMessages = $('#messages-container');
    const contentInput = $('#content');

    scrollToBottom();

    $("#send-message").prop('disabled', true);

    connection.start().then(() => {
        $("#send-message").prop('disabled', false);
        console.log("Connected to SignalR hub");
    }).catch(err => {
        console.error("SignalR connection error:", err.toString());
    });

    connection.on("ReceiveMessage", (senderId, message) => {
        const typeMessage = $('#senderId').val() !== senderId ? 'message-received' : 'message-sent';
        const messageElement = $('<div>', {class: `message ${typeMessage}`, 'data-message-id': message.id});

        messageElement.html(`
            <div class="message-content">
                ${escapeHtml(message.content)}
            </div>
            <span class="message-time">
                ${new Date().toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
            </span>
            <div class="message-actions">
                <button class="btn btn-sm btn-action" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="fas fa-ellipsis-v"></i>
                </button>
                <ul class="dropdown-menu">
                    <li><button class="dropdown-item revoke-message" data-message-id="${message.id}">Revoke</button></li>
                </ul>
            </div>
        `);
        chatMessages.append(messageElement);
        scrollToBottom();
    });

    connection.on("MessageRevoked", (messageId) => {
        const messageElement = $(`[data-message-id="${messageId}"]`);
        if (messageElement.length) {
            messageElement.find('.message-content').text('[Tin nhắn đã bị thu hồi]');
            messageElement.find('.message-actions').remove();
            messageElement.addClass('revoked');
        }
    });

    function sendMessage() {
        const content = contentInput.val().trim();

        if (!content) {
            return;
        }

        connection
            .invoke(
                "SendMessage",
                $('#chatId').val(),
                $('#senderId').val(),
                content
            )
            .catch(err => {
                console.error("Send message error:", err);
            });

        contentInput.val('');
        contentInput.focus();
        scrollToBottom();
    }

    $('#send-message').click(sendMessage);
    contentInput.keypress(e => {
        if (e.key === 'Enter') {
            sendMessage();
            e.preventDefault();
        }
    });

    $(document).on('click', '.revoke-message', function () {
        connection
            .invoke('RevokeMessage', $(this).data('message-id'))
            .catch(console.error);
    });

    function scrollToBottom() {
        chatMessages.scrollTop(chatMessages[0].scrollHeight);
    }

    function escapeHtml(text) {
        return text.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
});