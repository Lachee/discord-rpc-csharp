using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscordRPC.Registry
{
    class UnixUriSchemeCreator : IUriSchemeCreator
    {
        public void RegisterUriScheme(ILogger logger, string appid, string steamid = null)
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(home))
            {
                logger.Error("Failed to register because the HOME variable was not set.");
                return;
            }

            string exe = UriScheme.GetApplicationLocation();
            if (string.IsNullOrEmpty(exe))
            {
                logger.Error("Failed to register because the application was not located.");
                return;
            }

            //Prepare the command
            string command = null;
            if (!string.IsNullOrEmpty(steamid))
            {
                //A steam command isntead
                command = "xdg-open steam://rungameid/" + steamid;
            }
            else
            {
                //Just a regular discord command
                command = exe;
            }


            //Prepare the file
            string desktopFileFormat = 
@"[Desktop Entry]
Name=Game {0}
Exec={1} %u
Type=Application
NoDisplay=true
Categories=Discord;Games;
MimeType=x-scheme-handler/discord-{2}";
            
            string file = string.Format(desktopFileFormat, appid, command, appid);

            //Prepare the path
            string filename = "/discord-" + appid + ".desktop";
            string filepath = home + "/.local/share/applications";
            var directory = Directory.CreateDirectory(filepath);
            if (!directory.Exists)
            {
                logger.Error("Failed to register because {0} does not exist", filepath);
                return;
            }

            //Write the file
            File.WriteAllText(filepath + filename, file);

            //Register the Mime type
            if (!RegisterMime(appid))
            {
                logger.Error("Failed to register because the Mime failed.");
                return;
            }
        }

        private bool RegisterMime(string appid)
        {
            //Format the arguments
            string format = "default discord-{0}.desktop x-scheme-handler/discord-{0}";
            string arguments = string.Format(format, appid);

            //Run the process and wait for response
            Process process = Process.Start("xdg-mime", arguments);
            process.WaitForExit();
            
            //Return if succesful
            return process.ExitCode >= 0;
        }
    }
}
