using DiscordRPC.Logging;

namespace DiscordRPC.Registry
{
    interface IUriSchemeCreator
    {
        /// <summary>
        /// Registers the URI scheme. If Steam ID is passed, the application will be launched through steam instead of directly.
        /// <para>Additional arguments can be supplied if required.</para>
        /// </summary>
        /// <param name="logger">The logger to report messages too.</param>
        /// <param name="appid">The ID of the discord application</param>
        /// <param name="steamid">Optional field to indicate if this game should be launched through steam instead</param>
        void RegisterUriScheme(ILogger logger, string appid, string steamid = null);

    }
}
