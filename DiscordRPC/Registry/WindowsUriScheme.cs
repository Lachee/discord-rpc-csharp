using DiscordRPC.Logging;
using System;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// Registers a URI scheme on Windows.
    /// </summary>
    public sealed class WindowsUriScheme : IRegisterUriScheme
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsUriScheme"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public WindowsUriScheme(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public bool Register(SchemeInfo info)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                throw new PlatformNotSupportedException("URI schemes can only be registered on Windows");
            }

            //Prepare our location
            string location = info.ExecutablePath;
            if (location == null)
            {
                logger.Error("Failed to register application because the location was null.");
                return false;
            }

            //Prepare the Scheme, Friendly name, default icon and default command
            string schemePath = $"discord-{info.ApplicationID}";
            string friendlyName = $"Run game {info.ApplicationID} protocol";
            string defaultIcon = location;
            string command = location;

            //We have a steam ID, so attempt to replce the command with a steam command
            if (info.UsingSteamApp)
            {
                //Try to get the steam location. If found, set the command to a run steam instead.
                string steam = GetSteamLocation();
                if (steam != null)
                    command = string.Format("\"{0}\" steam://rungameid/{1}", steam, info.SteamAppID);

            }

            //Okay, now actually register it
            CreateUriScheme(schemePath, friendlyName, defaultIcon, command);
            return true;
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
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey($"SOFTWARE\\Classes\\{scheme}"))
            {
                key.SetValue("", $"URL:{friendlyName}");
                key.SetValue("URL Protocol", "");

                using (var iconKey = key.CreateSubKey("DefaultIcon"))
                    iconKey.SetValue("", defaultIcon);

                using (var commandKey = key.CreateSubKey("shell\\open\\command"))
                    commandKey.SetValue("", command);
            }

            logger.Trace("Registered {0}, {1}, {2}", scheme, friendlyName, command);
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
