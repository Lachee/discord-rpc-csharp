using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscordRPC.Registry
{
    class MacUriSchemeCreator : IUriSchemeCreator
    {
        public void RegisterUriScheme(ILogger logger, string appid, string steamid = null)
        {
            //var home = Environment.GetEnvironmentVariable("HOME");
            //if (string.IsNullOrEmpty(home)) return;     //TODO: Log Error

            string exe = UriScheme.GetApplicationLocation();
            if (string.IsNullOrEmpty(exe))
            {
                logger.Error("Failed to register because the application could not be located.");
                return;
            }
            
            logger.Trace("Registering Steam Command");

            //Prepare the command
            string command = exe;
            if (!string.IsNullOrEmpty(steamid)) command = "steam://rungameid/" + steamid;
            else logger.Warning("This library does not fully support MacOS URI Scheme Registration.");

            //get the folder ready
            string filepath = "~/Library/Application Support/discord/games";
            var directory = Directory.CreateDirectory(filepath);
            if (!directory.Exists)
            {
                logger.Error("Failed to register because {0} does not exist", filepath);
                return;
            }

            //Write the contents to file
            File.WriteAllText(filepath + "/" + appid + ".json", "{ \"command\": \"" + command + "\" }");            
        }
        
    }
}
