using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiscordRPC.Registry
{
	internal static class UriScheme
	{
        public static void RegisterUriScheme(string appid, string steamid = null, string arguments = null)
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
            creator.RegisterUriScheme(appid, steamid, arguments);
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
