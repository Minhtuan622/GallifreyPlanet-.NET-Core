$(document).ready(function () {
    var formData = new FormData();
    formData.append("commentableId", $('input[name="blogId"]').val());

    $.ajax({
        url: '/Comment/Get',
        type: 'GET',
        data: formData,
        processData: false,
        contentType: false,
    }).done(response => {
        console.table(response);
    }).fail(error => {
        console.error(error);
    });
})

function showReplyForm(commentId) {
    const existingForm = $(`#replyForm-${commentId}`);
    if (existingForm.length) {
        existingForm.remove();
        return;
    }

    $(`[data-comment-id="${commentId}"]`).append(`
        <div id="replyForm-${commentId}" class="mt-3">
            <form class="d-flex gap-2" method="post" action="/Comment/AddReply">
                <input type="hidden" name="commentId" value="${commentId}">
                <input type="text" name="content" class="form-control form-control-sm" 
                       placeholder="Nhập phản hồi của bạn..." required>
                <button type="submit" class="btn btn-primary btn-sm">Gửi</button>
            </form>
        </div>
    `);

    $(`#replyForm-${commentId} form`).on('submit', function (e) {
        e.preventDefault();
        const formData = new FormData(this);
        submitReply(formData, commentId);
    });
}

function submitReply(formData, commentId) {
    $.ajax({
        url: '/Comment/AddReply',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
    }).done(response => {
        console.log(response)
        if (response.success) {
            location.reload();
        }
    }).catch(error => {
        console.error('Error:', error);
        alert('Đã có lỗi xảy ra khi gửi phản hồi');
    });
}

function deleteComment(commentId) {
    if (!confirm('Bạn có chắc chắn muốn xóa bình luận này?')) return;

    $.ajax({
        url: `/Comment/Delete/${commentId}`,
        type: 'DELETE',
        processData: false,
        contentType: false,
    }).done(response => {
        console.log(response)
        if (response.success) {
            $(`.comment-item`).has(`[data-comment-id="${commentId}"]`).remove();
        } else {
            alert('Không thể xóa bình luận');
        }
    }).fail(error => {
        console.error('Error:', error);
        if (error.status === 401) {
            alert('Bạn không có quyền xóa bình luận này');
        } else {
            alert('Đã có lỗi xảy ra khi xóa bình luận');
        }
    });
}

function deleteReply(replyId) {
    if (!confirm('Bạn có chắc chắn muốn xóa phản hồi này?')) return;

    $.ajax({
        url: `/Comment/DeleteReply/${replyId}`,
        type: 'DELETE',
        processData: false,
        contentType: false,
    }).done(response => {
        console.log(response)
        if (response.success) {
            $(`[data-reply-id="${replyId}"]`).closest('.d-flex.mb-3').remove();
        } else {
            alert('Không thể xóa phản hồi');
        }
    }).fail(error => {
        console.error('Error:', error);
        if (error.status === 401) {
            alert('Bạn không có quyền xóa phản hồi này');
        } else {
            alert('Đã có lỗi xảy ra khi xóa phản hồi');
        }
    });
}