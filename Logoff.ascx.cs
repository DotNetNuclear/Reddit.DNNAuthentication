using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using DotNetNuke.Services.Authentication;

namespace DotNetNuclear.DNN.Authentication.Reddit
{
    public partial class Logoff : AuthenticationLogoffBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnInit(e);
            try
            {
                // Clear session
                Session.Abandon();

                /*
                 * Perform any custom logout tasks
                */
                
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }

            base.OnLogOff(EventArgs.Empty);
            base.OnRedirect(EventArgs.Empty);
        }

    }
}