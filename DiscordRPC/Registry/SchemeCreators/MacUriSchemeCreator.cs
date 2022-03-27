using System.IO;
using DiscordRPC.Logging;

namespace DiscordRPC.Registry.SchemeCreators
{
    internal class MacUriSchemeCreator : IUriSchemeCreator
    {
        private readonly ILogger _logger;
        
        public MacUriSchemeCreator(ILogger logger)
        {
            _logger = logger;
        }

        public bool RegisterUriScheme(UriSchemeRegister register)
        {
            // TODO: Log Error

            var exe = register.ExecutablePath;
            if (string.IsNullOrEmpty(exe))
            {
                _logger.Error("Failed to register because the application could not be located.");
                return false;
            }
            
            _logger.Trace("Registering Steam Command");

            // Prepare the command
            var command = exe;
            if (register.UsingSteamApp) command = "steam://rungameid/" + register.SteamAppId;
            else _logger.Warning("This library does not fully support MacOS URI Scheme Registration.");

            // Get the folder ready
            const string filePath = "~/Library/Application Support/discord/games";
            var directory = Directory.CreateDirectory(filePath);
            if (!directory.Exists)
            {
                _logger.Error("Failed to register because {0} does not exist", filePath);
                return false;
            }

            // Write the contents to file
            File.WriteAllText($"{filePath}/{register.ApplicationId}.json", "{ \"command\": \"" + command + "\" }");
            _logger.Trace("Registered {0}, {1}", $"{filePath}/{register.ApplicationId}.json", command);
            return true;
        }
        
    }
}