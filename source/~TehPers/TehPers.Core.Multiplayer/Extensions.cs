using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Multiplayer {
    public static class Extensions {
        private static readonly Dictionary<ITehCoreApi, IMultiplayerApi> _multiplayerApis = new Dictionary<ITehCoreApi, IMultiplayerApi>();

        public static IMultiplayerApi GetMultiplayerApi(this ITehCoreApi core) {
            return Extensions._multiplayerApis.GetOrAdd(core, () => {
                MessageDelegator.PatchIfNeeded();

                MultiplayerApi api = new MultiplayerApi(core);
                core.Log("Multiplayer", "Multiplayer API created", LogLevel.Debug);
                return api;
            });
        }
    }
}
