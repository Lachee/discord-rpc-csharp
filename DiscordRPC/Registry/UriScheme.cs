using DiscordRPC.Logging;
using System;
using System.Diagnostics;

namespace DiscordRPC.Registry
{
    internal static class UriScheme
	{
        public static void RegisterUriScheme(ILogger logger, string appid, string steamid = null)
        {
            //Get the creator
            IUriSchemeCreator creator = null;
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    creator = new WindowsUriSchemeCreator();
                    break;

                case PlatformID.Unix:
                    creator = new UnixUriSchemeCreator();
                    break;
                
                //case PlatformID.MacOSX:
                //    creator = new MacUriSchemeCreator();
                //    break;
            }

            //Make sure its valid
            if (creator == null)
                throw new PlatformNotSupportedException("Platform does not support registration.");

            //Register the endpoitns
            creator.RegisterUriScheme(logger, appid, steamid);
        }

        /// <summary>
        /// Gets the current location of the app
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationLocation()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }
    }
}
