using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiscordRPC.Registry
{
    class WindowsUriSchemeCreator : IUriSchemeCreator
    {
        /// <summary>
        /// Registers the URI scheme. If Steam ID is passed, the application will be launched through steam instead of directly.
        /// <para>Additional arguments can be supplied if required.</para>
        /// </summary>
        /// <param name="logger">The logger to report messages too.</param>
        /// <param name="appid">The ID of the discord application</param>
        /// <param name="steamid">Optional field to indicate if this game should be launched through steam instead</param>
        public void RegisterUriScheme(ILogger logger, string appid, string steamid = null)
        {
            //Prepare our location
            string location = UriScheme.GetApplicationLocation();
            if (location == null)
            {
                logger.Error("Failed to register application because the location was null.");
                return;
            }

            //Prepare the Scheme, Friendly name, default icon and default command
            string scheme = "discord-" + appid;
            string friendlyName = "Run game " + appid + " protocol";
            string defaultIcon = location;
            string command = location;

            //We have a steam ID, so attempt to replce the command with a steam command
            if (!string.IsNullOrEmpty(steamid))
            {
                //Try to get the steam location. If found, set the command to a run steam instead.
                string steam = GetSteamLocation();
                if (steam != null)
                    command = string.Format("\"{0}\" steam://rungameid/{1}", steam, steamid);

            }

            //Okay, now actually register it
            CreateUriScheme(scheme, friendlyName, defaultIcon, command);
        }

        /// <summary>
        /// Creates the actual scheme
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="friendlyName"></param>
        /// <param name="defaultIcon"></param>
        /// <param name="command"></param>
        private void CreateUriScheme(string scheme, string friendlyName, string defaultIcon, string command)
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + scheme))
            {
                key.SetValue("", "URL:" + friendlyName);
                key.SetValue("URL Protocol", "");

                using (var iconKey = key.CreateSubKey("DefaultIcon"))
                    iconKey.SetValue("", defaultIcon);

                using (var commandKey = key.CreateSubKey("shell\\open\\command"))
                    commandKey.SetValue("", command);
            }
        }

        /// <summary>
        /// Gets the current location of the steam client
        /// </summary>
        /// <returns></returns>
        public string GetSteamLocation()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam"))
            {
                if (key == null) return null;
                return key.GetValue("SteamExe") as string;
            }
        }

   

    }
}
