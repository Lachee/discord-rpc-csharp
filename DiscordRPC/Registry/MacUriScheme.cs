using DiscordRPC.Logging;
using System;
using System.IO;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// Registers a URI scheme on MacOS.
    /// </summary>
    public sealed class MacUriScheme : IRegisterUriScheme
    {
        private ILogger logger;

        private static readonly string[] DiscordClientFolders = new string[]
        {
            "discord",
            "discordptb",
            "discordcanary",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MacUriScheme"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public MacUriScheme(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public bool Register(SchemeInfo info)
        {
            string exe = info.ExecutablePath;
            if (string.IsNullOrEmpty(exe))
            {
                logger.Error("Failed to register because the application could not be located.");
                return false;
            }

            logger.Trace("Registering Steam Command");

            //Prepare the command
            string command = exe;
            if (info.UsingSteamApp) command = $"steam://rungameid/{info.SteamAppID}";
            else logger.Warning("This library does not fully support MacOS URI Scheme Registration.");

            //get the folder ready
            foreach (var folder in DiscordClientFolders)
            {
                string discordDirectory = Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    "Library/Application Support/",
                    folder
                );

                if (Directory.Exists(discordDirectory))
                {
                    logger.Trace("Discord client folder exists: {0}", discordDirectory);
                    RegisterSchemeForClient(discordDirectory, info.ApplicationID, command);
                }
                else
                {
                    logger.Trace("Discord client folder does not exist: {0}", discordDirectory);
                }
            }

            return true;
        }

        private void RegisterSchemeForClient(string discordDirectory, string appId, string command)
        {
            string filepath = Path.Combine(discordDirectory, "games", $"{appId}.json");
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            //Write the contents to file
            string applicationSchemeFilePath = filepath;
            File.WriteAllText(applicationSchemeFilePath, $"{{ \"command\": \"{command}\" }}");
        }
    }
}
