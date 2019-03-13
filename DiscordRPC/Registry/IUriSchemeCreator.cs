using DiscordRPC.Logging;

namespace DiscordRPC.Registry
{
    internal interface IUriSchemeCreator
    {
        /// <summary>
        /// Registers the URI scheme. If Steam ID is passed, the application will be launched through steam instead of directly.
        /// <para>Additional arguments can be supplied if required.</para>
        /// </summary>
        /// <param name="register">The register context.</param>
        bool RegisterUriScheme(UriSchemeRegister register);
    }
}
