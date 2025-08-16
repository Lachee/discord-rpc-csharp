using DiscordRPC.Logging;
using System;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// URI Scheme information to register to the system.
    /// </summary>
    public struct SchemeInfo
    {
        /// <summary>
        /// The Discord application ID for the game or application.
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// The Steam application ID if the game is registered through Steam. When set, the application will be launched through Steam instead of directly.
        /// <para>Can be null if the game is not registered through Steam.</para>
        /// </summary>
        public string SteamAppID { get; set; }

        /// <summary>
        /// The path to the executable that will be launched when the URI scheme is invoked.
        /// </summary>
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Indicates whether the scheme is using a Steam application ID.
        /// </summary>
        public bool UsingSteamApp => !string.IsNullOrEmpty(SteamAppID);
    }

    /// <summary>
    /// Registers a URI Scheme for the current platform.
    /// </summary>
    public static class UriScheme
    {
        /// <summary>
        /// Registers the URI Scheme for the current platform.
        /// </summary>
        public static bool Register(SchemeInfo info, ILogger logger = null)
        {
#if NET471_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            // .NET >4.7.1 adds support for RuntimeInformation
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return new WindowsUriScheme(logger).Register(info);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                return new UnixUriScheme(logger).Register(info);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                return new MacUriScheme(logger).Register(info);
            }
            else
            {
                logger?.Error("Unknown Platform: {0}", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
                throw new PlatformNotSupportedException("Platform does not support registration.");
            }
#else
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return new WindowsUriScheme(logger).Register(info);

                case PlatformID.Unix:
                    return new UnixUriScheme(logger).Register(info);

                case PlatformID.MacOSX:
                    return new MacUriScheme(logger).Register(info);

                default:
                    logger?.Error("Unknown Platform: {0}", Environment.OSVersion.Platform);
                    throw new PlatformNotSupportedException("Platform does not support registration.");
            }
#endif
        }
    }
}
