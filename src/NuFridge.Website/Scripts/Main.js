$(function () {
    SetUpLoginButton();
});

function SetUpLoginButton() {
    $('.loginStatus').click(function (e) {
        //Conditional states allow the dropdown box appear and disappear 
        if ($('#signin-dropdown').is(":visible")) {
            $('#signin-dropdown').hide()
            $('.loginStatus').removeClass('active'); // When the dropdown is not visible removes the class "active"
        } else {
            $('#signin-dropdown').show()
            $('.loginStatus').addClass('active'); // When the dropdown is visible add class "active"
            $("#loginEmailAddress").focus();
        }
        if ($(".loginStatus").text() != "Sign Out") {
            return false;
        }
    });

    $('#signin-dropdown').click(function (e) {
        e.stopPropagation();
    });
    $(document).click(function () {
        $('#signin-dropdown').hide();
        $('.loginStatus').removeClass('active');
    });
}