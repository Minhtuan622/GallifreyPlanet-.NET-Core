function showReplyForm(commentId) {
    // Check if form already exists
    const existingForm = $(`#replyForm-${commentId}`);
    if (existingForm.length) {
        existingForm.remove();
        return;
    }

    // Find the comment item and append the form
    $(`[data-comment-id="${commentId}"]`).append(`
        <div id="replyForm-${commentId}" class="mt-3 ms-4">
            <form class="d-flex gap-2">
                <input type="hidden" name="commentId" value="${commentId}">
                <input type="text" name="content" class="form-control form-control-sm" 
                       placeholder="Nhập phản hồi của bạn..." required>
                <button type="submit" class="btn btn-primary btn-sm">Gửi</button>
            </form>
        </div>
    `);

    // Attach event handler for form submission
    $(`#replyForm-${commentId} form`).on('submit', function (event) {
        submitReply(event, commentId);
    });
}

async function submitReply(event, commentId) {
    event.preventDefault();
    const content = $(event.target).find('input[name="content"]').val();

    try {
        const response = await $.ajax({
            url: '/Comment/AddReply',
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            data: JSON.stringify({ commentId, content })
        });

        // Handle success response
        $(`#replies-${commentId}`).append(response);
        $(`#replyForm-${commentId}`).remove();
    } catch (error) {
        console.error('Error:', error);
        alert('Đã có lỗi xảy ra khi gửi phản hồi');
    }
}

async function deleteComment(commentId) {
    if (confirm('Bạn có chắc chắn muốn xóa bình luận này?')) {
        try {
            const response = await $.ajax({
                url: `/Comment/DeleteComment/${commentId}`,
                method: 'DELETE',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response) {
                $(`[data-comment-id="${commentId}"]`).remove();
            } else {
                alert('Đã có lỗi xảy ra khi xóa bình luận');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Đã có lỗi xảy ra khi xóa bình luận');
        }
    }
}

async function deleteReply(replyId, commentId) {
    if (confirm('Bạn có chắc chắn muốn xóa phản hồi này?')) {
        try {
            const response = await $.ajax({
                url: `/Comment/DeleteReply/${replyId}`,
                method: 'DELETE',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response) {
                $(`[data-reply-id="${replyId}"]`).remove();
            } else {
                alert('Đã có lỗi xảy ra khi xóa phản hồi');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Đã có lỗi xảy ra khi xóa phản hồi');
        }
    }
}
