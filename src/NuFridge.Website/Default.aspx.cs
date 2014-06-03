using System;
using System.Collections.Generic;
using System.Web.UI;
using WebMatrix.WebData;


namespace NuFridge.Website
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                siteMaster.AddServiceToScriptManager("Services/FeedService.asmx");
            }

            if (WebSecurity.IsAuthenticated)
            {
                adminButtons.Visible = true;
                pnlImportPackages.Visible = true;
            }
        }
    }
}