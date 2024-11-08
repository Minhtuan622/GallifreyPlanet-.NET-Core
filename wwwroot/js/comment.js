$(document).ready(function () {
    fetchComments($('input[name="blogId"]').val());

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/commentHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveComment", (commentableId, content, result) => {
        updateCommentsUI(commentableId, content, result);
    });

    connection.on("ReceiveReply", (parentCommentId, content, result) => {
        updateRepliesUI(parentCommentId, content, result);
    });

    connection.start().catch(err => console.error(err));

    $('form[name="commentForm"]').on('submit', function (e) {
        e.preventDefault();
        $(this).find('button[type="submit"]').disabled = true;

        const contentEl = $(this).find('textarea[name="content"]');

        connection
            .invoke(
                "SendComment",
                parseInt($('input[name="blogId"]').val()),
                contentEl.val(),
                $('input[name="activeUserName"]').val(),
            )
            .then(() => {
                contentEl.val("");
                contentEl.focus();
                $(this).find('button[type="submit"]').disabled = false;
            })
            .catch(err => {
                console.error('Error invoking SendComment:', err)
            });
    });
});

function fetchComments(commentableId) {
    $.ajax({
        url: '/Comment/Get',
        type: 'GET',
        data: {commentableId},
        contentType: 'application/json',
    }).done(response => {
        if (response.success) {
            populateComments(response.data);
        } else {
            console.error(response.message);
        }
    }).fail(handleAjaxError);
}

function populateComments(comments) {
    const commentsListEl = $('.comments-list');
    commentsListEl.empty();

    comments.forEach(comment => {
        const commentEl = createCommentElement(comment);
        commentsListEl.append(commentEl);
    });
}

function createCommentElement(comment) {
    const defaultAvatarPath = "/uploads/accounts/default-avatar.jpg";
    let commentEl = $('<div>').addClass('comment-item d-flex');

    let avatarEl = $('<div>').addClass('avatar-placeholder me-3')
        .append($('<img>').attr('src', comment.user.avatar || defaultAvatarPath)
            .attr('alt', comment.user.name)
            .addClass('h-100 w-100 object-fit-cover rounded-circle')
        );

    let contentEl = $('<div>').addClass('flex-grow-1')
        .append($('<div>').addClass('rounded-4 p-3')
            .append($('<div>').addClass('d-flex justify-content-between align-items-start mb-2')
                .append($('<h6>').addClass('mb-0').text(comment.user.name))
                .append(createCommentActions(comment)
                )
            )
            .append($('<p>').addClass('mb-1').text(comment.content))
            .append($('<small>').addClass('text-muted')
                .append($('<i>').addClass('far fa-clock me-1'))
                .append(formatDate(comment.createdAt))
            )
        );

    if (comment.replies && comment.replies.length > 0) {
        let repliesEl = $('<div>').addClass('ms-4 mt-3');
        comment.replies.forEach(reply => {
            repliesEl.append(createReplyElement(reply));
        });
        contentEl.append(repliesEl);
    }

    return commentEl.append(avatarEl, contentEl);
}

function createReplyElement(reply) {
    const defaultAvatarPath = "/uploads/accounts/default-avatar.jpg";
    let replyEl = $('<div>').addClass('d-flex mb-3');

    let avatarEl = $('<div>').addClass('avatar-placeholder me-2')
        .css({'width': '32px', 'height': '32px'})
        .append($('<img>').attr('src', reply.user.avatar || defaultAvatarPath)
            .attr('alt', reply.user.name)
            .addClass('h-100 w-100 object-fit-cover rounded-circle')
        );

    let contentEl = $('<div>').addClass('flex-grow-1')
        .append($('<div>').addClass('rounded-4 p-3')
            .append($('<div>').addClass('d-flex justify-content-between align-items-start')
                .append($('<h6>').addClass('mb-1 fs-6').text(reply.user.name))
                .append(createReplyActions(reply)
                )
            )
            .append($('<p>').addClass('mb-1').text(reply.content))
            .append($('<small>').addClass('text-muted')
                .append($('<i>').addClass('far fa-clock me-1'))
                .append(reply.createdAt)
            )
        );

    return replyEl.append(avatarEl, contentEl);
}

function createCommentActions(comment) {
    if (comment.user.userName === $('#user-name').val()) {
        let actions = $('<div>').addClass('dropdown');
        let toggleBtn = $('<button>').addClass('btn btn-link text-muted p-0')
            .attr('type', 'button')
            .attr('data-bs-toggle', 'dropdown')
            .append($('<i>').addClass('fas fa-ellipsis-v'));
        let dropdownMenu = $('<ul>').addClass('dropdown-menu dropdown-menu-end');
        let deleteItem = $('<li>')
            .append($('<a>').addClass('dropdown-item text-danger')
                .attr('href', '#')
                .attr('onclick', `deleteComment(${comment.id})`)
                .append($('<i>').addClass('fas fa-trash-alt me-2'))
                .append('Xóa')
            );
        dropdownMenu.append(deleteItem);

        return actions.append(toggleBtn, dropdownMenu);
    }
}

function createReplyActions(reply) {
    if (reply.user.userName === $('#user-name').val()) {
        let actions = $('<div>').addClass('dropdown');
        let toggleBtn = $('<button>').addClass('btn btn-link text-muted p-0')
            .attr('type', 'button')
            .attr('data-bs-toggle', 'dropdown')
            .append($('<i>').addClass('fas fa-ellipsis-v'));
        let dropdownMenu = $('<ul>').addClass('dropdown-menu dropdown-menu-end');
        let deleteItem = $('<li>')
            .append($('<a>').addClass('dropdown-item text-danger')
                .attr('href', '#')
                .attr('onclick', `deleteReply(${reply.id})`)
                .append($('<i>').addClass('fas fa-trash-alt me-2'))
                .append('Xóa')
            );
        dropdownMenu.append(deleteItem);

        return actions.append(toggleBtn, dropdownMenu);
    }
}

function updateCommentsUI(commentableId, content, result) {
    $('.comments-list').prepend(createCommentElement({
        id: -1,
        user: {
            id: $('#user-name').val(),
            name: result.user.name,
            avatar: result.user.avatar
        },
        content: content,
        createdAt: formatDate(result.createdAt),
        replies: []
    }));
}

function updateRepliesUI(parentCommentId, content, result) {
    $(`[data-comment-id="${parentCommentId}"]`)
        .find('.ms-4.mt-3')
        .prepend(createReplyElement({
            id: -1,
            user: {
                id: $('#user-name').val(),
                name: result.user.name,
                avatar: result.user.avatar
            },
            content: content,
            createdAt: formatDate(result.createdAt),    
        }));
}

function submitReply(form, commentId) {
    const formData = new FormData(form);
    $.ajax({
        url: '/Comment/AddReply',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
    }).done(response => {
        if (response.success) {
            connection.invoke(
                "ReplyToComment",
                commentId,
                formData.get('content'),
                $('#user-name').val(),
                response.data.value.createdAt
            );
            form.reset();
        } else {
            alert(response.message);
        }
    }).fail(handleAjaxError);
}

function deleteComment(commentId) {
    if (confirm('Bạn có chắc chắn muốn xóa bình luận này?')) {
        $.ajax({
            url: `/Comment/Delete/${commentId}`,
            type: 'DELETE',
            contentType: 'application/json',
        }).done(response => {
            if (response.success) {
                $(`[data-comment-id="${commentId}"]`).remove();
            } else {
                alert(response.message);
            }
        }).fail(error => handleAjaxError(error, 'Bạn không có quyền xóa bình luận này'));
    }
}

function deleteReply(replyId) {
    if (confirm('Bạn có chắc chắn muốn xóa phản hồi này?')) {
        $.ajax({
            url: `/Comment/DeleteReply/${replyId}`,
            type: 'DELETE',
            contentType: 'application/json',
        }).done(response => {
            if (response.success) {
                $(`[data-reply-id="${replyId}"]`).remove();
            } else {
                alert(response.message);
            }
        }).fail(error => handleAjaxError(error, 'Bạn không có quyền xóa phản hồi này'));
    }
}

function handleAjaxError(error, unauthorizedMessage = 'Đã có lỗi xảy ra') {
    console.error('Error:', error);
    alert(error.status === 401 ? unauthorizedMessage : 'Đã có lỗi xảy ra');
}

function formatDate(date){
    return new Intl.DateTimeFormat('en-US').format(new Date(date));
}