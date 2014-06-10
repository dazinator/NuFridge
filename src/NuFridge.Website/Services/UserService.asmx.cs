using NuFridge.Common.Managers;
using NuFridge.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using WebMatrix.WebData;

namespace NuFridge.Website.Services
{
    [WebService(Namespace = "http://tempuri.org/userservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public SignInResponse SignIn(SignInRequest request)
        {
            try
            {
                string message;
                bool result = false;
               // bool result = new AccountManager().SignIn(request.EmailAddress, request.Password, request.Persistant);

                if (result)
                {
                    var authTicket = new FormsAuthenticationTicket(2, request.EmailAddress, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, null);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket))
                    {
                        HttpOnly = true
                    };
                    HttpContext.Current.Response.Cookies.Clear();
                    HttpContext.Current.Response.Cookies.Add(authCookie);
                }

                return new SignInResponse(result);
            }
            catch (Exception ex)
            {
                return new SignInResponse(false);
            }
        }
                   
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CreateAccountResponse CreateAccount(CreateAccountRequest request)
        {
            try
            {
                string message = "";
                //bool result = new AccountManager().CreateAccount(request.InviteToken, request.FirstName, request.LastName, request.Password, out message);
                return new CreateAccountResponse(false, message);
            }
            catch (Exception ex)
            {
                return new CreateAccountResponse(false, "An unhandled exception has been thrown: " + ex.Message);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CreateInviteResponse CreateInvite(CreateInviteRequest request)
        {
            try
            {
                string message;
                bool result = InviteManager.CreateInvite(request.EmailAddress, out message);
                return new CreateInviteResponse(result, message);
            }
            catch (Exception ex)
            {
                return new CreateInviteResponse(false, "An unhandled exception has been thrown: " + ex.Message);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GetInviteResponse GetInvite(GetInviteRequest request)
        {
            try
            {
                string message;
                var invite = InviteManager.GetInvite(request.InviteToken, out message);
                return new GetInviteResponse(invite, message);
            }
            catch (Exception ex)
            {
                return new GetInviteResponse(null, "An unhandled exception has been thrown: " + ex.Message);
            }
        }
    }
    public class GetInviteRequest
    {
        public Guid InviteToken { get; set; }
    }
    public class GetInviteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool IsValidInvite { get; set; }
        public string EmailAddress { get; set; }

        public GetInviteResponse(AccountInviteEntity invite, string message)
        {
            Success = invite != null;
            Message = message;
            if (invite != null)
            {
                IsValidInvite = invite.IsValid;

                //Security... don't show the email if its already been used
                if (invite.IsValid)
                {
                    EmailAddress = invite.EmailAddress;
                }
            }
        }
    }
    public class SignInRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool Persistant { get; set; }
    }
    public class SignInResponse
    {
        public bool Success { get; set; }

        public SignInResponse()
        {

        }

        public SignInResponse(bool success)
        {
            Success = success;
        }
    }
    public class CreateAccountRequest
    {
        public Guid InviteToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
    public class CreateAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public CreateAccountResponse()
        {

        }

        public CreateAccountResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
    public class CreateInviteRequest
    {
        public string EmailAddress { get; set; }
    }
    public class CreateInviteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public CreateInviteResponse()
        {

        }

        public CreateInviteResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}