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