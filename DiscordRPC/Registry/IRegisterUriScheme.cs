using DiscordRPC.Logging;

namespace DiscordRPC.Registry
{
    /// <summary>
    /// Interface for registering a URI scheme.
    /// </summary>
    public interface IRegisterUriScheme
    {
        /// <summary>
        /// Registers the URI scheme. If Steam ID is passed, the application will be launched through steam instead of directly.
        /// <para>Additional arguments can be supplied if required.</para>
        /// </summary>
        /// <param name="info">The register context.</param>
        bool Register(SchemeInfo info);
    }
}
