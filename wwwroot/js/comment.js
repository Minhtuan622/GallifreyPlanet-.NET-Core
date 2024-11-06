﻿$(document).ready(function () {
    fetchComments($('input[name="blogId"]').val());

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/commentHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveComment", (commentableId, content, userName, createdAt) => {
        updateCommentsUI(commentableId, content, userName, createdAt);
    });

    connection.on("ReceiveReply", (parentCommentId, content, userName, createdAt) => {
        updateRepliesUI(parentCommentId, content, userName, createdAt);
    });

    connection.start().catch(err => console.error(err));
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
    let commentEl = $('<div>').addClass('comment-item');

    let avatarEl = $('<div>').addClass('avatar-placeholder me-3')
        .append($('<img>').attr('src', comment.user.avatar || defaultAvatarPath)
            .attr('alt', comment.user.name)
            .addClass('h-100 w-100 object-fit-cover rounded-circle'));

    let contentEl = $('<div>').addClass('flex-grow-1')
        .append($('<div>').addClass('rounded-4 p-3')
            .append($('<div>').addClass('d-flex justify-content-between align-items-start mb-2')
                .append($('<h6>').addClass('mb-0').text(comment.user.name))
                .append(createCommentActions(comment)))
            .append($('<p>').addClass('mb-1').text(comment.content))
            .append($('<small>').addClass('text-muted')
                .append($('<i>').addClass('far fa-clock me-1'))
                .append(comment.createdAt)));

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
            .addClass('h-100 w-100 object-fit-cover rounded-circle'));

    let contentEl = $('<div>').addClass('flex-grow-1')
        .append($('<div>').addClass('rounded-4 p-3')
            .append($('<div>').addClass('d-flex justify-content-between align-items-start')
                .append($('<h6>').addClass('mb-1 fs-6').text(reply.user.name))
                .append(createReplyActions(reply)))
            .append($('<p>').addClass('mb-1').text(reply.content))
            .append($('<small>').addClass('text-muted')
                .append($('<i>').addClass('far fa-clock me-1'))
                .append(reply.createdAt)));

    return replyEl.append(avatarEl, contentEl);
}

function createCommentActions(comment) {
    let actions = $('<div>').addClass('dropdown');
    let toggleBtn = $('<button>').addClass('btn btn-link text-muted p-0')
        .attr('type', 'button')
        .attr('data-bs-toggle', 'dropdown')
        .append($('<i>').addClass('fas fa-ellipsis-v'));
    let dropdownMenu = $('<ul>').addClass('dropdown-menu dropdown-menu-end');

    if (comment.user.id === '@User.Identity.Name') {
        let deleteItem = $('<li>')
            .append($('<a>').addClass('dropdown-item text-danger')
                .attr('href', '#')
                .attr('onclick', `deleteComment(${comment.id})`)
                .append($('<i>').addClass('fas fa-trash-alt me-2'))
                .append('Xóa'));
        dropdownMenu.append(deleteItem);
    }

    return actions.append(toggleBtn, dropdownMenu);
}

function createReplyActions(reply) {
    let actions = $('<div>').addClass('dropdown');
    let toggleBtn = $('<button>').addClass('btn btn-link text-muted p-0')
        .attr('type', 'button')
        .attr('data-bs-toggle', 'dropdown')
        .append($('<i>').addClass('fas fa-ellipsis-v'));
    let dropdownMenu = $('<ul>').addClass('dropdown-menu dropdown-menu-end');

    if (reply.user.id === '@User.Identity.Name') {
        let deleteItem = $('<li>')
            .append($('<a>').addClass('dropdown-item text-danger')
                .attr('href', '#')
                .attr('onclick', `deleteReply(${reply.id})`)
                .append($('<i>').addClass('fas fa-trash-alt me-2'))
                .append('Xóa'));
        dropdownMenu.append(deleteItem);
    }

    return actions.append(toggleBtn, dropdownMenu);
}

function updateCommentsUI(commentableId, content, userName, createdAt) {
    const comment = {
        id: -1, // New comment will have an ID assigned by the server
        user: {
            id: '@User.Identity.Name',
            name: userName,
            avatar: null
        },
        content: content,
        createdAt: createdAt,
        replies: []
    };
    const commentEl = createCommentElement(comment);
    $('.comments-list').prepend(commentEl);
}

function updateRepliesUI(parentCommentId, content, userName, createdAt) {
    const reply = {
        id: -1, // New reply will have an ID assigned by the server
        user: {
            id: '@User.Identity.Name',
            name: userName,
            avatar: null
        },
        content: content,
        createdAt: createdAt
    };
    const replyEl = createReplyElement(reply);
    $(`[data-comment-id="${parentCommentId}"]`).find('.ms-4.mt-3').prepend(replyEl);
}

function submitComment(form) {
    const formData = new FormData(form);
    $.ajax({
        url: '/Comment/Add',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
    }).done(response => {
        if (response.success) {
            connection.invoke("SendComment", $('input[name="blogId"]').val(), formData.get('content'), '@User.Identity.Name', response.data.createdAt);
            form.reset();
        } else {
            alert(response.message);
        }
    }).fail(handleAjaxError);
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
            connection.invoke("ReplyToComment", commentId, formData.get('content'), '@User.Identity.Name', response.data.createdAt);
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