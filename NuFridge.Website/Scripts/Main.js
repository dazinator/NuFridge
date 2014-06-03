$(function () {
    SetUpLoginButton();



  //  $("#loginSignIn").click(function () {
      //  SignIn();
 //   });

   // $('#loginEmailAddress,#loginPassword').keyup(function () {
   //     EnableDisableSignInButton();
    //});
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

//function EnableDisableSignInButton() {
//    if ($("#loginEmailAddress").val() === '' || $("#loginPassword").val() === '') {
//        $('#loginSignIn').prop('disabled', 'disabled');
//    }
//    else {
//        $('#loginSignIn').prop('disabled', '');
//    }
//}

//function SignIn() {
//    var request = new FeedManagerWebsite.Services.SignInRequest();
//    request.EmailAddress = $("#loginEmailAddress").val();
//    request.Password = $("#loginPassword").val();

//  //  $("#loginEmailAddress").attr("readonly", "readonly");
//  //  $("#loginPassword").attr("readonly", "readonly");

//    FeedManagerWebsite.Services.UserService.SignIn(request, function (response) {
//        if (response && response.Success) {
//            alert("Signed in");
//        }
//        else {
//            alert("Failed to sign in");
//        }
//      //  $("#loginEmailAddress").attr("readonly", "");
//      //  $("#loginPassword").attr("readonly", "");
//    },
//     function () {
//         alert("Failed to sign in");
//       //  $("#loginEmailAddress").attr("readonly", "");
//       //  $("#loginPassword").attr("readonly", "");
//     });
//}