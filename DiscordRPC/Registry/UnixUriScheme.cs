using DiscordRPC.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// Registers a URI scheme on Unix-like systems using the xdg-open command. 
    /// The scheme is saved as a .desktop file in the user's local applications directory.
    /// </summary>
    public sealed class UnixUriScheme : IRegisterUriScheme
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixUriScheme"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public UnixUriScheme(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public bool Register(SchemeInfo info)
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(home))
            {
                logger.Error("Failed to register because the HOME variable was not set.");
                return false;
            }

            string exe = info.ExecutablePath;
            if (string.IsNullOrEmpty(exe))
            {
                logger.Error("Failed to register because the application was not located.");
                return false;
            }

            //Prepare the command
            string command = null;
            if (info.UsingSteamApp)
            {
                //A steam command isntead
                command = $"xdg-open steam://rungameid/{info.SteamAppID}";
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

            string file = string.Format(desktopFileFormat, info.ApplicationID, command, info.ApplicationID);

            //Prepare the path
            string filename = $"/discord-{info.ApplicationID}.desktop";
            string filepath = home + "/.local/share/applications";
            var directory = Directory.CreateDirectory(filepath);
            if (!directory.Exists)
            {
                logger.Error("Failed to register because {0} does not exist", filepath);
                return false;
            }

            //Write the file
            File.WriteAllText(filepath + filename, file);

            //Register the Mime type
            if (!RegisterMime(info.ApplicationID))
            {
                logger.Error("Failed to register because the Mime failed.");
                return false;
            }

            logger.Trace("Registered {0}, {1}, {2}", filepath + filename, file, command);
            return true;
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
