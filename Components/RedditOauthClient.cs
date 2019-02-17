using System;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;
using System.IO;
using System.Net;
using System.Collections.Generic;
using DotNetNuke.Instrumentation;
using System.Collections.Specialized;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuclear.DNN.Connectors.Reddit.Components;

namespace DotNetNuclear.DNN.Authentication.Reddit.Components
{
    public class RedditOauthClient : OAuthClientBase
    {
        private RedditConnectorManager _redditConnectorManager;

        #region Constructor
         
        public const string RedditService = "Reddit";
        private string RedditApiTokenCookieName { get; set; }

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(RedditOauthClient));
        public RedditOauthClient(int portalId, AuthMode mode)
            : base(portalId, mode, RedditService)
        {
            _redditConnectorManager = new RedditConnectorManager(portalId);
            if (!_redditConnectorManager.HasConfig)
            {
                return;
            }
            else
            {
                AuthorizationEndpoint = new Uri(_redditConnectorManager.AuthorizationEndpoint);
                Scope = _redditConnectorManager.ApiScope;
                TokenMethod = HttpMethod.POST;
                TokenEndpoint = new Uri(_redditConnectorManager.AccessTokenEndpoint);
                MeGraphEndpoint = new Uri(_redditConnectorManager.MeGraphEndpoint);
                RedditApiTokenCookieName = _redditConnectorManager.AccessTokenCookieName;
                OAuthConfigBase.GetConfig(Service, portalId).APIKey = _redditConnectorManager.ClientId;
                OAuthConfigBase.GetConfig(Service, portalId).APISecret = _redditConnectorManager.ClientSecret;
            }

            AuthTokenName = "RedditUserToken";
            OAuthHeaderCode = "Basic";
            OAuthVersion = "2.0";
        }

        #endregion

        #region Overrides

        protected override string GetToken(string responseText)
        {
            if (string.IsNullOrEmpty(responseText))
            {
                throw new Exception("There was an error processing the credentials. Contact your system administrator.");
            }
            var tokenDictionary = Json.Deserialize<Dictionary<string, object>>(responseText);
            AccessToken = Convert.ToString(tokenDictionary["access_token"]);

            //Sets a cookie future API calls
            HttpCookie sessionAccessToken = new HttpCookie(RedditApiTokenCookieName)
            {
                Value = AccessToken,
                Expires = DateTime.Now.Add(GetExpiry(responseText))
            };
            HttpContext.Current.Response.Cookies.Add(sessionAccessToken);
            var token = responseText;

            return token;
        }

        protected override TimeSpan GetExpiry(string responseText)
        {
            var tokenDictionary = Json.Deserialize<Dictionary<string, object>>(responseText);

            return new TimeSpan(0, 0, Convert.ToInt32(tokenDictionary["expires_in"]));
        }

        public override TUserData GetCurrentUser<TUserData>()
        {
            if (!IsCurrentUserAuthorized())
            {
                return null;
            }

            var redditApiClient = new RedditApiClient(AccessToken);

            var redditAcct = redditApiClient.MyAccount();
            var user = new RedditUserData()
            {
                FirstName = "Reddit",
                LastName = "User",
                DisplayName = redditAcct.Name,
                Email = $"{redditAcct.Name}@reddit.com",
                Id = redditAcct.Id,
                Roles = new NameValueCollection(),
            };
            if (redditAcct.IsModerator)
            {
                user.Roles.Add("Moderator", "Moderator");
            }

            return user as TUserData;
        }


        public override void AuthenticateUser(UserData user, PortalSettings settings, string IPAddress, Action<NameValueCollection> addCustomProperties, Action<UserAuthenticatedEventArgs> onAuthenticated)
        {
            var loginStatus = UserLoginStatus.LOGIN_FAILURE;

            var userData = GetCurrentUser<RedditUserData>();

            string userName = "Reddit-" + userData.Email;

            var userInfo = UserController.ValidateUser(settings.PortalId, userName, "",
                                                                RedditService, "",
                                                                settings.PortalName, IPAddress,
                                                                ref loginStatus);


            var eventArgs = new UserAuthenticatedEventArgs(userInfo, userName, loginStatus, RedditService)
            {
                AutoRegister = true
            };

            eventArgs.Profile = new NameValueCollection();

            if (userInfo == null || (string.IsNullOrEmpty(userInfo.FirstName) && !string.IsNullOrEmpty(userData.FirstName)))
            {
                eventArgs.Profile.Add("FirstName", userData.FirstName);
            }
            if (userInfo == null || (string.IsNullOrEmpty(userInfo.LastName) && !string.IsNullOrEmpty(userData.LastName)))
            {
                eventArgs.Profile.Add("LastName", userData.LastName);
            }
            if (userInfo == null || (string.IsNullOrEmpty(userInfo.Email) && !string.IsNullOrEmpty(userData.Email)))
            {
                eventArgs.Profile.Add("Email", userData.PreferredEmail);
            }
            if (userInfo == null
                    || (string.IsNullOrEmpty(userInfo.DisplayName) && !string.IsNullOrEmpty(userData.DisplayName))
                    || (userInfo.DisplayName == userData.Name && userInfo.DisplayName != userData.DisplayName))
            {
                eventArgs.Profile.Add("DisplayName", userData.DisplayName);
            }

            onAuthenticated(eventArgs);

        }

        #endregion
    }
}

