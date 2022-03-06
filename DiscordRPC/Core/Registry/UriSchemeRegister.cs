using System;
using System.Diagnostics;
using DiscordRPC.Core.Logging;
using DiscordRPC.Core.Registry.SchemeCreators;

namespace DiscordRPC.Core.Registry
{
    internal class UriSchemeRegister
	{
        /// <summary>
        /// The ID of the Discord App to register
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Optional Steam App ID to register. If given a value, then the game will launch through steam instead of Discord.
        /// </summary>
        public string SteamAppId { get; set; }

        /// <summary>
        /// Is this register using steam?
        /// </summary>
        public bool UsingSteamApp => !string.IsNullOrEmpty(SteamAppId) && SteamAppId != "";

        /// <summary>
        /// The full executable path of the application.
        /// </summary>
        public string ExecutablePath { get; set; }

        private readonly ILogger _logger;
        
        public UriSchemeRegister(ILogger logger, string applicationId, string steamAppId = null, string executable = null)
        {
            _logger = logger;
            ApplicationId = applicationId.Trim();
            SteamAppId = steamAppId?.Trim();
            ExecutablePath = executable ?? GetApplicationLocation();
        }

        /// <summary>
        /// Registers the URI scheme, using the correct creator for the correct platform
        /// </summary>
        public bool RegisterUriScheme()
        {
            // Get the creator
            IUriSchemeCreator creator;
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    _logger.Trace("Creating Windows Scheme Creator");
                    creator = new WindowsUriSchemeCreator(_logger);
                    break;

                case PlatformID.Unix:
                    _logger.Trace("Creating Unix Scheme Creator");
                    creator = new UnixUriSchemeCreator(_logger);
                    break;
                
                case PlatformID.MacOSX:
                    _logger.Trace("Creating MacOSX Scheme Creator");
                    creator = new MacUriSchemeCreator(_logger);
                    break;

                default:
                    _logger.Error($"Unknown Platform: {Environment.OSVersion.Platform}");
                    throw new PlatformNotSupportedException("Platform does not support registration.");
            }

            // Register the app
            if (creator.RegisterUriScheme(this))
            {
                _logger.Info("URI scheme registered.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the FileName for the currently executing application
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationLocation() => Process.GetCurrentProcess().MainModule?.FileName;
    }
}