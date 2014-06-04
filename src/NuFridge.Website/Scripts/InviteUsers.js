$(function () {

    $("#manageFeedMenuCount").hide();
    $("#sendInviteButton").click(function () {
        var email = $("#emailAddressInput").val();


        var request = new NuFridge.Website.Services.Messages.CreateInviteRequest();
        request.EmailAddress = email;

        $("#submitMessage").text("");
        $("#submitMessage").hide();
        $("#sendInviteButton").attr("disabled", "true");

        NuFridge.Website.Services.UserService.CreateInvite(request, function (response) {
            if (response && response.Success) {
                $("#submitMessage").text(response.Message);
                $("#submitMessage").removeClass("alert-danger");
                $("#submitMessage").addClass("alert-success");
                $("#submitMessage").show();
                $("#emailAddressInput").val("");
                $("#emailAddressInput").focus();
            } else {
                $("#submitMessage").text(response.Message);
                $("#submitMessage").addClass("alert-danger");
                $("#submitMessage").removeClass("alert-success");
                $("#submitMessage").show();
            }
            $("#sendInviteButton").removeAttr("disabled");
        }, function () {
            $("#submitMessage").text("An unknown error has occurred while sending the invite");
            $("#sendInviteButton").attr("disabled", "false");
            $("#submitMessage").addClass("alert-danger");
            $("#submitMessage").removeClass("alert-success");
            $("#submitMessage").show();
        });
    });
});