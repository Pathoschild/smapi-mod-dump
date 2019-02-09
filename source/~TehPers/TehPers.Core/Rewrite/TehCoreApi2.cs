using StardewModdingAPI;

namespace TehPers.Core.Rewrite {
    internal class TehCoreApi2 : ITehCoreApi {
        /// <inheritdoc />
        public IMod Owner { get; }

        internal TehCoreApi2(IMod owner) {
            this.Owner = owner;
        }

        /// <inheritdoc />
        public void Log(string apiName, string message, LogLevel level) {
            this.Owner.Monitor.Log($"[TehPers.Core - {apiName}] {message}", level);
        }
    }
}
