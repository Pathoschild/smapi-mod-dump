/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

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