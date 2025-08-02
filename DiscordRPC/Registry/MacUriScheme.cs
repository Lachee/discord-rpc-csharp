using DiscordRPC.Logging;
using System.IO;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// Registers a URI scheme on MacOS.
    /// </summary>
    public sealed class MacUriScheme : IRegisterUriScheme
    {
        private ILogger logger;

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
            string filepath = "~/Library/Application Support/discord/games";
            var directory = Directory.CreateDirectory(filepath);
            if (!directory.Exists)
            {
                logger.Error("Failed to register because {0} does not exist", filepath);
                return false;
            }

            //Write the contents to file
            string applicationSchemeFilePath = $"{filepath}/{info.ApplicationID}.json";
            File.WriteAllText(applicationSchemeFilePath, "{ \"command\": \"" + command + "\" }");
            logger.Trace("Registered {0}, {1}", applicationSchemeFilePath, command);
            return true;
        }

    }
}
