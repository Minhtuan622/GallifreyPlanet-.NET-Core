document.getElementById('avatarInput').addEventListener('change', function (e) {
    const reader = new FileReader();

    reader.onload = function (event) {
        const img = document.createElement('img');
        img.src = event.target.result;
        img.classList.add("w-100", "h-100");

        document.getElementById('avatarCropper').appendChild(img);
    }

    reader.readAsDataURL(e.target.files[0]);
});