using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuclear.DNN.Authentication.Reddit.Components;


namespace DotNetNuclear.DNN.Authentication.Reddit
{
    public partial class Settings : AuthenticationSettingsBase
    {

        #region Protected Methods

        /// <summary>
        /// The OnLoad method runs when the Control is loaded
        /// </summary>
        /// <param name="e">An EventArgs object</param>
        protected override void OnLoad(EventArgs e)
        {
            //Call the base classes method
            base.OnLoad(e);

            try
            {
                Config config = Config.GetConfig(this.PortalId);

                SettingsEditor.DataSource = config;
                SettingsEditor.DataBind();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Public Methods

        public override void UpdateSettings()
        {
            if (SettingsEditor.IsValid)
            {
                if (SettingsEditor.IsValid)
                {
                    Config.UpdateConfig(SettingsEditor.DataSource as Config);
                }
            }
        }

        #endregion

    }
}