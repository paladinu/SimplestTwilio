// Bootstrap 5 Form Validation
// Enable native HTML5 validation with Bootstrap styling
(function () {
    'use strict';

    // Fetch all forms that need validation
    const forms = document.querySelectorAll('form[novalidate]');

    // Loop over them and prevent submission if invalid
    Array.from(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        }, false);
    });
})();
