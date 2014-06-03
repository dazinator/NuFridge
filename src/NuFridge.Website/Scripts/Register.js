$(function () {
    AdjustPage();
    LoadInvite();

    $('#firstNameInput,#lastNameInput').keyup(function () {
        EnableDisableCreateButton();
    });

    $("#createAccountButton").click(function () {
        CreateAccount();
    });
});

function CreateAccount() {
    $("#firstNameInput").attr("readonly", "readonly");
    $("#lastNameInput").attr("readonly", "readonly");
    $("#passwordInput").attr("readonly", "readonly");

    var request = new FeedManagerWebsite.Services.CreateAccountRequest();
    request.InviteToken = getParameterByName("token");
    request.FirstName = $("#firstNameInput").val();
    request.LastName = $("#lastNameInput").val();
    request.Password = $("#passwordInput").val();

    FeedManagerWebsite.Services.UserService.CreateAccount(request, function (response) {
        if (response && response.Success) {
            redirectToHomeWithPopup(true);
        }
        else {
            $("#submitMessage").text(response.Message);
            $("#submitMessage").show();
        }
        $("#firstNameInput").attr("readonly", "");
        $("#lastNameInput").attr("readonly", "");
        $("#passwordInput").attr("readonly", "");
    },
     function () {
         $("#submitMessage").text("An unexpected error has occurred. Your invite may no longer be valid.");
         $("#submitMessage").show();
         $("#firstNameInput").attr("readonly", "");
         $("#lastNameInput").attr("readonly", "");
         $("#passwordInput").attr("readonly", "");
     });
}

function EnableDisableCreateButton() {
    if ($("#firstNameInput").val() === '' || $("#lastNameInput").val() === '') {
        $('#createAccountButton').prop('disabled', 'disabled');
    }
    else {
        $('#createAccountButton').prop('disabled', '');
    }
}

function AdjustPage() {
    $("#leftContent").hide();
    $("#rightContent").removeClass("col-md-9");
    $("#rightContent").addClass("col-md-12");
    $(".navbar-nav").hide();
}

function LoadInvite() {

    var request = new FeedManagerWebsite.Services.GetInviteRequest();
    request.InviteToken = getParameterByName("token");

    FeedManagerWebsite.Services.UserService.GetInvite(request, function (response) {
        if (response && response.Success) {
            if (response.IsValidInvite) {
                if (response.EmailAddress) {
                    $("#emailAddressInput").val(response.EmailAddress);
                    $("#firstNameInput").focus();
                }
                else {
                    redirectToHomeWithPopup(false);
                }
            }
            else {
                redirectToHomeWithPopup(false);
            }
        }
        else {
            redirectToHomeWithPopup(false);
        }
    },
     function () {
         redirectToHomeWithPopup(false);
     });
}

function redirectToHomeWithPopup(success) {

    var title = "Loading invite...";
    var message = "<p>" + "There was a problem opening your invite. You will be redirected back to the homepage." + "</p>";

    if (success === true) {
        title = "Account Created";
        message = "<p>You have successfully create an account.</p><p>You will be redirected to the homepage shortly.</p>";
    }

    setTimeout(function () { document.location.href = "/"; }, 7500)

    BootstrapDialog.show({
        title: title,
        message: message
    });
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}