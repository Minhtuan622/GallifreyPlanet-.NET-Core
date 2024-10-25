$(document).ready(function () {
    fetchComments($('input[name="blogId"]').val());
});

function fetchComments(commentableId) {
    $.ajax({
        url: '/Comment/Get',
        type: 'GET',
        data: { commentableId },
        contentType: 'application/json',
    }).done(response => {
        console.table(response);
    }).fail(handleAjaxError);
}

function showReplyForm(commentId) {
    const replyFormId = `#replyForm-${commentId}`;
    const existingForm = $(replyFormId);

    if (existingForm.length) {
        existingForm.remove(); 
        return;
    }

    $(`[data-comment-id="${commentId}"]`).append(getReplyFormHtml(commentId));

    $(replyFormId).find('form').on('submit', function (e) {
        e.preventDefault();
        const formData = new FormData(this);
        submitReply(formData, commentId);
    });
}

function getReplyFormHtml(commentId) {
    return `
        <div id="replyForm-${commentId}" class="mt-3">
            <form class="d-flex gap-2" method="post" action="/Comment/AddReply" autocomplete="off">
                <input type="hidden" name="commentId" value="${commentId}">
                <input type="text" name="content" class="form-control form-control-sm" 
                       placeholder="Nhập phản hồi của bạn..." required>
                <button type="submit" class="btn btn-primary btn-sm">Gửi</button>
            </form>
        </div>
    `;
}

function submitReply(formData, commentId) {
    $.ajax({
        url: '/Comment/AddReply',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
    }).done(response => {
        response.success
            ? reloadPageSection(commentId)
            : alert(response.message);
    }).fail(handleAjaxError);
}

function deleteComment(commentId) {
    if (confirm('Bạn có chắc chắn muốn xóa bình luận này?')) {
        $.ajax({
            url: `/Comment/Delete/${commentId}`,
            type: 'DELETE',
            contentType: 'application/json',
        }).done(response => {
            response.success
                ? reloadPageSection(commentId)
                : alert(response.message);
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
            response.success
                ? reloadPageSection(replyId)
                : alert('Không thể xóa phản hồi');
        }).fail(error => handleAjaxError(error, 'Bạn không có quyền xóa phản hồi này'));
    }
}

function reloadPageSection() {
    location.reload();
}

function handleAjaxError(error, unauthorizedMessage = 'Đã có lỗi xảy ra') {
    console.error('Error:', error);
    alert(error.status === 401 ? unauthorizedMessage : 'Đã có lỗi xảy ra');
}
