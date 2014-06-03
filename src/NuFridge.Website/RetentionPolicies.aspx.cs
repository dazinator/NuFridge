using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using WebMatrix.WebData;

namespace NuFridge.Website
{
    public partial class RetentionPolicies : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSecurity.IsAuthenticated)
            {
                editPanel.Visible = false;
            }

            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                siteMaster.AddServiceToScriptManager("Services/FeedService.asmx");
            }

        }
    }
}