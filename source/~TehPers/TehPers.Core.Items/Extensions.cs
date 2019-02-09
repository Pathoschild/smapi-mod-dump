using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Items.Delegators;

namespace TehPers.Core.Items {
    public static class Extensions {
        private static readonly Dictionary<ITehCoreApi, IItemApi> _itemApis = new Dictionary<ITehCoreApi, IItemApi>();

        public static IItemApi GetItemApi(this ITehCoreApi core) {
            return Extensions._itemApis.GetOrAdd(core, () => {
                DrawingDelegator.PatchIfNeeded();
                ItemDelegator.PatchIfNeeded();

                IItemApi api = new ItemApi(core);
                core.Log("Items", "Item API created", LogLevel.Debug);
                return api;
            });
        }
    }
}
