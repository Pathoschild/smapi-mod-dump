using StardewModdingAPI;

namespace TehPers.CoreMod.Api {
    public interface IApiHelper {
        /// <summary>The core API that owns this helper.</summary>
        ICoreApi CoreApi { get; }

        /// <summary>The mod that owns this core API object.</summary>
        IMod Owner { get; }

        /// <summary>Logs a message to the console.</summary>
        /// <param name="message">The message to display</param>
        /// <param name="severity">The severity of the message</param>
        void Log(string message, LogLevel severity = LogLevel.Debug);
    }
}