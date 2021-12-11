//
function waitTime(refresh) {
    setTimeout(function () {
        refresh();
    }, 1500);
}

// Hub URL
function systemHubUrl() {
    return '/systemHub';
}


//
function showWindowPopUp(url) {
    $.get(url, function (data) {
        $('#ModalWindow').html(data);
        $('#ModalWindow').modal({ backdrop: 'static', keyboard: false });
    });
}
