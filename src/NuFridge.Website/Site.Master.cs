using NuFridge.Common.Managers;
using System;
using System.Web.UI;
using WebMatrix.WebData;

namespace NuFridge.Website
{
    public partial class SiteMaster : MasterPage
    {
        public ScriptManager ScriptManager { get { return scriptManager; } }

        protected void Page_Init(object sender, EventArgs e)
        {
           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddServiceToScriptManager("Services/UserService.asmx");

            if (WebSecurity.IsAuthenticated)
            {
                inviteUsersMenuItem.Visible = true;
                //  retentionPolicyMenuItem.Visible = true;
            }
            else
            {
                inviteUsersMenuItem.Visible = false;
                //   retentionPolicyMenuItem.Visible = false;
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
           // new AccountManager().SignIn(loginEmailAddress.Text, loginPassword.Text, loginRememberMe.Checked);

            Response.Redirect(Request.Url.ToString());
        }

        public void AddServiceToScriptManager(string servicePath)
        {
            var ioPath = System.IO.Path.Combine(Request.Url.GetLeftPart(UriPartial.Authority), servicePath);

            ScriptManager.Services.Add(new ServiceReference(ioPath));
        }
    }
}