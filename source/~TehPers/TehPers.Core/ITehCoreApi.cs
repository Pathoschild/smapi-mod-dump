using StardewModdingAPI;

namespace TehPers.Core {
    public interface ITehCoreApi {
        /// <summary>The mod that this core API is linked to.</summary>
        IMod Owner { get; }

        /// <summary>Logging function used by core APIs.</summary>
        /// <param name="apiName">The name of the API.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The severity of the message.</param>
        void Log(string apiName, string message, LogLevel level);
    }
}
