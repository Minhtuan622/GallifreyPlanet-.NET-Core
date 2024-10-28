// initialize ckeditor
ClassicEditor
    .create(document.querySelector('#editor'))
    .catch(error => {
        console.error(error);
    });

// Preview image before upload
document.getElementById('thumbnailUpload').addEventListener('change', function (e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('preview-image').src = e.target.result;
            document.getElementById('preview-container').classList.remove('d-none');
            document.getElementById('upload-placeholder').classList.add('d-none');
        }
        reader.readAsDataURL(file);
    }
});

// Drag and drop functionality
const uploadContainer = document.querySelector('.upload-container');

['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    uploadContainer.addEventListener(eventName, preventDefaults, false);
});

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

['dragenter', 'dragover'].forEach(eventName => {
    uploadContainer.addEventListener(eventName, highlight, false);
});

['dragleave', 'drop'].forEach(eventName => {
    uploadContainer.addEventListener(eventName, unhighlight, false);
});

function highlight(e) {
    uploadContainer.classList.add('border-primary');
    uploadContainer.style.backgroundColor = 'rgba(var(--bs-primary-rgb), 0.05)';
}

function unhighlight(e) {
    uploadContainer.classList.remove('border-primary');
    uploadContainer.style.backgroundColor = '#f8f9fa';
}

uploadContainer.addEventListener('drop', handleDrop, false);

function handleDrop(e) {
    const dt = e.dataTransfer;
    const files = dt.files;
    document.getElementById('thumbnailUpload').files = files;

    if (files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('preview-image').src = e.target.result;
            document.getElementById('preview-container').classList.remove('d-none');
            document.getElementById('upload-placeholder').classList.add('d-none');
        }
        reader.readAsDataURL(files[0]);
    }
}