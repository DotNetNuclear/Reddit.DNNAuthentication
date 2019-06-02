using System;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Security.Membership;
using System.Collections.Generic;
using DotNetNuke.Security.Roles;
using DotNetNuke.Common;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuclear.DNN.Authentication.Reddit.Components;

namespace DotNetNuclear.DNN.Authentication.Reddit
{
    public partial class Login : OAuthLoginBase
    {
        private int _portalRegistrationValue;

        protected override string AuthSystemApplicationName
        {
            get { return "Reddit"; }
        }

        public override bool Enabled
        {
            get { return Config.GetConfig(PortalId).Enabled; }
        }

        public override bool SupportsRegistration
        {
            get { return true; }
        }

        protected override UserData GetCurrentUser()
        {
            return OAuthClient.GetCurrentUser<RedditUserData>();
        }

        protected override void AddCustomProperties(System.Collections.Specialized.NameValueCollection properties)
        {
            base.AddCustomProperties(properties);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            loginButton.Click += LoginButton_Click;
            OAuthClient = new RedditOauthClient(PortalId, Mode);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ClientResourceManager.RegisterStyleSheet(this.Page, base.ControlPath + "resources/css/login.css");
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            AuthorisationResult result = OAuthClient.Authorize();
            if (result == AuthorisationResult.Denied)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);

            }
        }

        protected override void OnUserAuthenticated(UserAuthenticatedEventArgs ea)
        {
            _portalRegistrationValue = -1;

            // Adding roles to list and removing in profile list
            var roles = new List<String>();
            string userName = null;
            foreach (var key in ea.Profile.AllKeys)
            {
                if (key.StartsWith("Role_"))
                {
                    roles.Add(ea.Profile[key]);
                    ea.Profile.Remove(key);
                }
                if (key.StartsWith("epicDisplayName"))
                    userName = ea.Profile[key];
            }

            switch (ea.LoginStatus)
            {
                case UserLoginStatus.LOGIN_SUCCESS:

                    // Adding roles
                    var user = ea.User;
                    var userRoles = RoleController.Instance.GetUserRoles(ea.User, true);

                    if (user != null && userRoles != null)
                    {
                        foreach (var userRol in userRoles)
                        {
                            RoleInfo oldRole = RoleController.Instance.GetRoleByName(user.PortalID, userRol.RoleName);
                            if (oldRole != null && oldRole.RoleName != "Registered Users" && oldRole.RoleName != "Subscribers")
                            {
                                // I used the release date of the first game of Epic Games ZZT October 1, 1991 
                                // as the effective date to mark the roles synchronized by the Epic Games provider.
                                if (userRol.EffectiveDate == DateTime.Parse("1991-10-01"))
                                {
                                    RoleController.DeleteUserRole(user, oldRole, PortalSettings, false);
                                }
                            }
                        }
                        foreach (var rol in roles)
                        {
                            RoleInfo newRole = RoleController.Instance.GetRoleByName(user.PortalID, rol);
                            if (newRole != null)
                            {
                                // I used the release date of the first game of Epic Games ZZT October 1, 1991 
                                // as the effective date to mark the roles synchronized by the Epic Games provider.
                                RoleController.Instance.AddUserRole(user.PortalID, user.UserID, newRole.RoleID, RoleStatus.Approved, false, DateTime.Parse("1991-10-01"), DateTime.MaxValue);
                            }
                        }
                    }

                    base.OnUserAuthenticated(ea);

                    break;

                case UserLoginStatus.LOGIN_FAILURE:
                    // TODO: confirm if we have to create authorized users
                    SavePortalRegistrationType();
                    //prevent send new user registration mail
                    var emailSetting = PortalSettings.Email;
                    PortalSettings.Email = string.Empty;

                    base.OnUserAuthenticated(ea);

                    //restore the email setting
                    PortalSettings.Email = emailSetting;

                    // Adding roles

                    var newUser = DotNetNuke.Entities.Users.UserController.GetUserByName(userName);
                    if (newUser != null)
                    {
                        foreach (var rol in roles)
                        {
                            RoleInfo newRole = RoleController.Instance.GetRoleByName(newUser.PortalID, rol);
                            if (newRole != null)
                            {
                                RoleController.Instance.AddUserRole(newUser.PortalID, newUser.UserID, newRole.RoleID, RoleStatus.Approved, false, DateTime.MinValue, DateTime.MaxValue);
                            }
                        }
                    }

                    RestorePortalRegistrationType();
                    break;
                case UserLoginStatus.LOGIN_USERLOCKEDOUT:
                    AddModuleMessage("UserLockout", ModuleMessage.ModuleMessageType.RedError, true);
                    break;
                case UserLoginStatus.LOGIN_USERNOTAPPROVED:
                    AddModuleMessage("UserNotApproved", ModuleMessage.ModuleMessageType.RedError, true);
                    break;
            }
        }

        private void SavePortalRegistrationType()
        {
            if (PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.PublicRegistration)
            {
                _portalRegistrationValue = PortalSettings.UserRegistration;
                PortalSettings.UserRegistration = (int)Globals.PortalRegistrationType.PublicRegistration;
            }
        }

        private void RestorePortalRegistrationType()
        {
            if (_portalRegistrationValue >= 0)
            {
                PortalSettings.UserRegistration = _portalRegistrationValue;
            }
        }

    }
}