using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Repositories;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace NuFridge.Common.Managers
{
    public static class InviteManager
    {
        public static bool CreateInvite(string emailAddress, out string message)
        {
            message = "";

            var smtpServer = ConfigurationManager.AppSettings["Email_SMTPServer"];
            if (string.IsNullOrEmpty(smtpServer))
            {
                message = "No SMTP server has been configured.";
                return false;
            }

            var fromAddress = ConfigurationManager.AppSettings["Email_FromAddress"];
            if (string.IsNullOrEmpty(fromAddress))
            {
                message = "No email from address has been configured.";
                return false;
            }

            var fromAddressDisplayName = ConfigurationManager.AppSettings["Email_FromAddressDisplayName"];
            if (string.IsNullOrEmpty(fromAddressDisplayName))
            {
                message = "No email from display name has been configured.";
                return false;
            }

            SmtpClient client = new SmtpClient(smtpServer);
            MailMessage mailMessage;

            try
            {
                mailMessage = new MailMessage(new MailAddress(fromAddress, fromAddressDisplayName), new MailAddress(emailAddress));
            }
            catch (FormatException ex)
            {
                message = "The email address format is incorrect: " + ex.Message;
                return false;
            }
            catch (ArgumentNullException ex)
            {
                message = "No email address was provided: " + ex.Message;
                return false;
            }


            var nuFridgeWebsiteName = ConfigurationManager.AppSettings["NuFridge.Website.Name"];

            ServerManager mgr = new ServerManager();

            var site = mgr.Sites.FirstOrDefault(st => st.Name == nuFridgeWebsiteName);
            if (site == null)
            {
                message = "IIS Website not found for " + nuFridgeWebsiteName;
                return false;
            }

            var binding = site.Bindings.FirstOrDefault();
            if (binding == null)
            {
                throw new Exception("No IIS bindings found for " + nuFridgeWebsiteName);
            }

            MongoDbRepository<AccountInviteEntity> repository = new MongoDbRepository<AccountInviteEntity>();

            AccountInviteEntity accountInvite = new AccountInviteEntity();
            accountInvite.IsValid = true;
            accountInvite.EmailAddress = emailAddress;

            repository.Insert(accountInvite);


            var websiteUrl = string.Format("{0}://{1}:{2}", binding.Protocol, Environment.MachineName, binding.EndPoint.Port);

            string inviteWebsiteUrl = string.Format("{0}/Register.aspx?token={1}", websiteUrl, accountInvite.Id);

            mailMessage.Subject = "[Feed Manager] Please activate your new account";
            mailMessage.Body = @"You have been sent an invite to join the feed manager at " + "<a href=\"" + websiteUrl + "\" target=\"_blank\">" + websiteUrl + " </a>" + ".<br>We just need you to confirm your account by visiting the URL below.<br> " + "<a href=\"" + inviteWebsiteUrl + "\" target=\"_blank\">Activate my account</a>" + ".<br>Thanks!";
            mailMessage.IsBodyHtml = true;

            bool deleteInvite = false;
            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                deleteInvite = true;
                message = "There was an exception from the SMTP server: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                deleteInvite = true;
                message = "An unexpected error occurred sending the invite: " + ex.Message;
                return false;
            }
            finally
            {
                if (deleteInvite)
                {
                    repository.Delete(accountInvite);
                }
            }

            message = "Successfully sent invite to " + emailAddress;
            return true;
        }

        public static AccountInviteEntity GetInvite(Guid Id, out string message)
        {
            MongoDbRepository<AccountInviteEntity> repository = new MongoDbRepository<AccountInviteEntity>();

            var invite = repository.Get(inv => inv.Id == Id).FirstOrDefault();

            if (invite != null)
            {
                message = "Found invite";
            }
            else
            {
                message = "Could not find invite";
            }

            return invite;
        }
    }
}