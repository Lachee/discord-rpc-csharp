using System;
using DiscordRPC.Core.Logging;

namespace DiscordRPC.Core.Registry.SchemeCreators
{
    internal class WindowsUriSchemeCreator : IUriSchemeCreator
    {
        private readonly ILogger _logger;
        
        public WindowsUriSchemeCreator(ILogger logger)
        {
            _logger = logger;
        }

        public bool RegisterUriScheme(UriSchemeRegister register)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                throw new PlatformNotSupportedException("URI schemes can only be registered on Windows");
            }

            // Prepare our location
            var location = register.ExecutablePath;
            if (location == null)
            {
                _logger.Error("Failed to register application because the location was null.");
                return false;
            }

            // Prepare the Scheme, Friendly name, default icon and default command
            var scheme = $"discord-{register.ApplicationId}";
            var friendlyName = $"Run game {register.ApplicationId} protocol";
            var defaultIcon = location;
            var command = location;

            // We have a steam ID, so attempt to replace the command with a steam command
            if (register.UsingSteamApp)
            {
                // Try to get the steam location. If found, set the command to a run steam instead.
                var steam = GetSteamLocation();
                if (steam != null) command = $"\"{steam}\" steam://rungameid/{register.SteamAppId}";

            }

            // Okay, now actually register it
            CreateUriScheme(scheme, friendlyName, defaultIcon, command);
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
                key?.SetValue("", "URL:" + friendlyName);
                key?.SetValue("URL Protocol", "");

                using (var iconKey = key?.CreateSubKey("DefaultIcon")) iconKey?.SetValue("", defaultIcon);

                using (var commandKey = key?.CreateSubKey("shell\\open\\command")) commandKey?.SetValue("", command);
            }

            _logger.Trace("Registered {0}, {1}, {2}", scheme, friendlyName, command);
        }

        /// <summary>
        /// Gets the current location of the steam client
        /// </summary>
        /// <returns></returns>
        public string GetSteamLocation()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam"))
            {
                return key?.GetValue("SteamExe") as string;
            }
        }
    }
}