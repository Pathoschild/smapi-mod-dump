using StardewModdingAPI;
using TehPers.CoreMod.Api;

namespace TehPers.CoreMod {
    internal class ApiHelper : IApiHelper {
        private readonly string _apiName;

        /// <inheritdoc />
        public ICoreApi CoreApi { get; }

        /// <inheritdoc />
        public IMod Owner => this.CoreApi.Owner;

        public ApiHelper(ICoreApi coreApi, string apiName) {
            this.CoreApi = coreApi;
            this._apiName = apiName;
        }

        /// <inheritdoc />
        public void Log(string message, LogLevel severity = LogLevel.Debug) {
            this.Owner.Monitor.Log($"[{this._apiName}]\t{message}", severity);
        }
    }
}