let connection;

$(document).ready(function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/commentHub")
        .withAutomaticReconnect()
        .build();
    
    connection.start().catch(err => console.error(err));

    connection.on("ReceiveComment", (commentableId, content, result) => {
        updateCommentsUI(commentableId, content, result);
    });

    connection.on("ReceiveReply", (parentCommentId, content, result) => {
        updateRepliesUI(parentCommentId, content, result);
    });

    fetchComments($('input[name="blogId"]').val());
    
    $('form[name="commentForm"]').on('submit', function (e) {
        e.preventDefault();
        $(this).find('button[type="submit"]').disabled = true;

        const contentEl = $(this).find('textarea[name="content"]');

        connection
            .invoke(
                "SendComment",
                parseInt($('input[name="blogId"]').val()),
                contentEl.val(),
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
            showComments(response.data);
        } else {
            console.error(response.message);
        }
    }).fail(handleAjaxError);
}

function showComments(comments) {
    const commentsListEl = document.querySelector('.comments-list');
    commentsListEl.innerHTML = comments.map(comment => createCommentElement(comment)).join('');
}

function createCommentElement(comment) {
    const hasReplies = comment.replies && comment.replies.length > 0;
    return `
        <div class="comment-item d-flex">
            <div class="avatar-placeholder me-3">
                <img src="${comment.user.avatar || "/uploads/accounts/default-avatar.jpg"}" 
                     alt="${comment.user.name}" 
                     class="h-100 w-100 object-fit-cover rounded-circle">
            </div>
            <div class="flex-grow-1">
                <div class="rounded-4 p-3" data-comment-id="${comment.id}">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <h6 class="mb-0">${comment.user.name}</h6>
                        ${createCommentActions(comment)}
                    </div>
                    <p class="mb-1">${comment.content}</p>
                    <small class="text-muted">
                        <i class="far fa-clock me-1"></i>
                        ${formatDate(comment.createdAt)}
                    </small>
                    <br>
                    <div class="d-flex btn-action-comment" style="gap: 1rem">
                        <button class="btn btn-link text-decoration-none p-0" 
                                onclick="showReplyForm(${comment.id})">
                            Trả lời
                        </button>
                        
                        ${hasReplies ? `
                            <button class="btn btn-link text-decoration-none p-0"
                                    data-bs-toggle="collapse" 
                                    href="#replies-${comment.id}" 
                                    role="button" 
                                >
                                Hiển thị phản hồi (${comment.replies.length})
                            </button>
                        ` : ''}
                    </div>
                </div>
                ${hasReplies ? `
                    <div id="replies-${comment.id}">
                        <div class="reply-item ms-4 mt-3">
                            ${comment.replies.map(reply => createReplyElement(reply)).join('')}
                        </div>
                    </div>
                ` : ''}
            </div>
        </div>
    `;
}

function createReplyElement(reply) {
    return `
        <div class="d-flex mb-3">
            <div class="avatar-placeholder me-2" style="width: 32px; height: 32px">
                <img src="${reply.user.avatar || "/uploads/accounts/default-avatar.jpg"}" 
                     alt="${reply.user.name}" 
                     class="h-100 w-100 object-fit-cover rounded-circle">
            </div>
            <div class="flex-grow-1">
                <div class="rounded-4 p-3">
                    <div class="d-flex justify-content-between align-items-start">
                        <h6 class="mb-1 fs-6">${reply.user.name}</h6>
                        ${createReplyActions(reply)}
                    </div>
                    <p class="mb-1">${reply.content}</p>
                    <small class="text-muted">
                        <i class="far fa-clock me-1"></i>
                        ${formatDate(reply.createdAt)}
                    </small>
                </div>
            </div>
        </div>
    `;
}

function createCommentActions(comment) {
    if (comment.user.userName !== $('#user-name').val()) {
        return '';
    }

    return `
        <div class="dropdown">
            <button class="btn btn-link text-muted p-0" 
                    type="button" 
                    data-bs-toggle="dropdown">
                <i class="fas fa-ellipsis-v"></i>
            </button>
            <ul class="dropdown-menu dropdown-menu-end">
                <li>
                    <a class="dropdown-item text-danger" 
                       href="#" 
                       onclick="deleteComment(${comment.id})">
                        <i class="fas fa-trash-alt me-2"></i>
                        Xóa
                    </a>
                </li>
            </ul>
        </div>
    `;
}

function createReplyActions(reply) {
    if (reply.user.userName !== $('#user-name').val()) {
        return '';
    }

    return `
        <div class="dropdown">
            <button class="btn btn-link text-muted p-0" 
                    type="button" 
                    data-bs-toggle="dropdown">
                <i class="fas fa-ellipsis-v"></i>
            </button>
            <ul class="dropdown-menu dropdown-menu-end">
                <li>
                    <a class="dropdown-item text-danger" 
                       href="#" 
                       onclick="deleteReply(${reply.id})">
                        <i class="fas fa-trash-alt me-2"></i>
                        Xóa
                    </a>
                </li>
            </ul>
        </div>
    `;
}

function showReplyForm(commentId) {
    const replyFormId = `#replyForm-${commentId}`;
    const existingForm = $(replyFormId);

    if (existingForm.length) {
        existingForm.remove();
        return;
    }

    $(`[data-comment-id="${commentId}"]`).append(generateReplyFormHtml(commentId));

    $(replyFormId).find('form').on('submit', function (e) {
        e.preventDefault();
        submitReply(commentId);
    });
}

function submitReply(commentId) {
    const contentEl = $('input[name="replyContent"]');
    $(this).find('button[type="submit"]').disabled = true;

    connection
        .invoke(
            "ReplyComment",
            commentId,
            contentEl.val(),
        )
        .then(() => {
            contentEl.val("");
            contentEl.focus();
            $(this).find('button[type="submit"]').disabled = false;
        })
        .catch(err => {
            console.error('Error invoking ReplyComment:', err);
        });
}

function generateReplyFormHtml(commentId) {
    return `
        <div id="replyForm-${commentId}" class="mt-3">
            <form class="d-flex gap-2" method="post" asp-controller="Comment" asp-action="AddReply" autocomplete="off">
                <input name="replyContent" class="form-control form-control-sm" 
                       placeholder="Nhập phản hồi của bạn..." required>
                <button type="submit" class="btn btn-primary btn-sm">Gửi</button>
            </form>
        </div>
    `;
}

function updateCommentsUI(commentableId, content, result) {
    const newComment = createCommentElement({
        id: -1,
        user: {
            id: document.querySelector('#user-name').value,
            name: result.user.name,
            avatar: result.user.avatar
        },
        content: content,
        createdAt: formatDate(result.createdAt),
        replies: []
    });

    document.querySelector('.comments-list').insertAdjacentHTML('afterbegin', newComment);
}

function updateRepliesUI(parentCommentId, content, result) {
    const parentComment = document.querySelector(`[data-comment-id="${parentCommentId}"]`);
    const repliesContainer = parentComment.querySelector('.reply-item');

    const newReply = createReplyElement({
        id: -1,
        user: {
            id: document.querySelector('#user-name').value,
            name: result.user.name,
            avatar: result.user.avatar
        },
        content: content,
        createdAt: formatDate(result.createdAt),
    });

    if (repliesContainer) {
        repliesContainer.insertAdjacentHTML('afterbegin', newReply);
    } else {
        parentComment.insertAdjacentHTML('beforeend', `
            <div class="reply-item ms-4 mt-3">
                ${newReply}
            </div>
        `);
    }
}

function deleteComment(commentId) {
    if (confirm('Bạn có chắc chắn muốn xóa bình luận này?')) {
        $.ajax({
            url: `/Comment/Delete/${commentId}`,
            type: 'DELETE',
            contentType: 'application/json',
        }).done(response => {
            if (response.success) {
                $(`[data-comment-id="${commentId}"]`).closest('.comment-item').remove();
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
                $(`[data-reply-id="${replyId}"]`).closest('.reply-item').remove();
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

function formatDate(date) {
    return new Intl.DateTimeFormat('en-UK').format(new Date(date));
}