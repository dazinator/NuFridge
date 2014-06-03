using ExtendedMongoMembership.Services;
using NuFridge.DataAccess.Connection;
using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using WebMatrix.WebData;

namespace NuFridge.Common.Managers
{
    public class AccountManager : BaseUsersService<AccountEntity>
    {
        public AccountManager() : base(MongoRead.Instance.FullConnectionString)
        {

        }

        public bool CreateAccount(Guid inviteId, string firstName, string lastName, string password, out string message)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                message = "No first name was provided";
                return false;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                message = "No first name was provided";
                return false;
            }

            MongoDbRepository<AccountInviteEntity> accountInviteRepository = new MongoDbRepository<AccountInviteEntity>();

            var invite = accountInviteRepository.GetById(inviteId);
            if (invite == null || !invite.IsValid || string.IsNullOrWhiteSpace(invite.EmailAddress))
            {
                //Don't acknowledge there is an invite if found - IsValid
                message = "No invite was found";
                return false;
            }

            var existingAccount = base.GetAll().FirstOrDefault(acc => acc.UserName == invite.EmailAddress);
            if (existingAccount != null)
            {
                message = "You have already registered an account using this email address.";
                return false;
            }

            try
            {
                AccountEntity profile = new AccountEntity();
                profile.UserName = invite.EmailAddress;
                profile.FirstName = firstName;
                profile.LastName = lastName;
               
                base.Save(profile);

                WebSecurity.CreateAccount(invite.EmailAddress, password);

                //WebSecurity.ResetPassword(WebSecurity.GeneratePasswordResetToken(invite.EmailAddress), password);
            }
            catch (Exception ex)
            {
                message = "There was an error creating the account: " + ex.Message;
                return false;
            }
            finally
            {
                //Stop the invite from being used again...
                invite.IsValid = false;
                accountInviteRepository.Update(invite);
            }
   

            message = "Successfully created account an for " + firstName + " " + lastName;
            return true;
        }

        public bool SignIn(string emailAddress, string password, bool persistant)
        {
            bool credentialsAreCorrect = WebSecurity.Login(emailAddress, password, persistant);
            FormsAuthentication.SetAuthCookie(emailAddress, persistant);
            return credentialsAreCorrect;
        }
    }
}