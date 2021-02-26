setTimeout(function () {
    window.location.href = document.querySelector('a[class="alert-link"]').getAttribute("href");
}, document.querySelector('a[class="alert-link"]').getAttribute("data-timeout"));
