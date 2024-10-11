let cropper;

document.getElementById('avatarInput').addEventListener('change', function (e) {
    const file = e.target.files[0];
    const reader = new FileReader();

    reader.onload = function (event) {
        const img = document.createElement('img');
        img.src = event.target.result;

        const cropperContainer = document.getElementById('avatarCropper');
        cropperContainer.innerHTML = '';
        cropperContainer.appendChild(img);

        cropper = new Cropper(img, {
            aspectRatio: 1,
            viewMode: 1,
        });
    }

    reader.readAsDataURL(file);
});

document.getElementById('saveAvatar').addEventListener('click', function () {
    if (cropper) {
        const croppedCanvas = cropper.getCroppedCanvas();
        croppedCanvas.toBlob(function (blob) {
            const formData = new FormData();
            formData.append('avatar', blob, 'avatar.png');

            fetch('/AccountManager/UploadAvatar', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.getElementById('avatar').src = data.avatarUrl;
                        $('#avatarModal').modal('hide');
                    } else {
                        alert('Có lỗi xảy ra khi tải lên ảnh đại diện.');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('Có lỗi xảy ra khi tải lên ảnh đại diện.');
                });
        });
    }
});