using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System.ComponentModel;
using System.Collections.Generic;
using DotNetNuke.Services.Authentication;
using DotNetNuke.UI.WebControls;

namespace DotNetNuclear.DNN.Authentication.Reddit.Components
{
    /// <summary>
    /// The Config class provides a central area for management of Module Configuration Settings.
    /// </summary>
    public class Config : AuthenticationConfigBase
    {
        #region Private Members

        private bool _Enabled = Null.NullBoolean;
        private int _Attribute1 = 0;

        private const string CACHEKEY = "Authentication.REDDIT";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the Config class
        /// </summary>
        /// <remarks>The Constructor is private forcing the class to be constructed by calling the
        /// static GetConfig factory method</remarks>
        protected Config(int portalId)
            : base(portalId)
        {
            try
            {
                Dictionary<string, string> portalSettingsDictionary = PortalController.Instance.GetPortalSettings(portalId);
                _Enabled = PortalController.GetPortalSettingAsBoolean("RedditAuthProvider_Enabled", portalId, Null.NullBoolean);
                _Attribute1 = PortalController.GetPortalSettingAsInteger("RedditAuthProvider_Attribute1", portalId, 0);
            }
            catch
            {
            }
        }

        #endregion

        #region Public Properties

        [SortOrder(0)]
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        [SortOrder(1)]
        public int Attribute1
        {
            get { return _Attribute1; }
            set { _Attribute1 = value; }
        }

        #endregion

        public static void ClearConfig(int portalId)
        {
            string key = String.Format("{0}_{1}", CACHEKEY, portalId);
            DataCache.RemoveCache(key);
        }

        public static Config GetConfig(int portalId)
        {
            string key = String.Format("{0}_{1}", CACHEKEY, portalId);
            Config config = DataCache.GetCache(key) as Config;

            if (config == null)
            {
                config = new Config(portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(Config config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, "RedditAuthProvider_Enabled", config.Enabled.ToString());
            PortalController.UpdatePortalSetting(config.PortalID, "RedditAuthProvider_Attribute1", config.Attribute1.ToString());

            ClearConfig(config.PortalID);
        }

    }
}
