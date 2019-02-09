using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui {
    public static class Extensions {
        private static readonly Dictionary<ITehCoreApi, IGuiApi> _guiApis = new Dictionary<ITehCoreApi, IGuiApi>();

        public static IGuiApi GetGuiApi(this ITehCoreApi coreApi) {
            return Extensions._guiApis.GetOrAdd(coreApi, () => {
                GuiApi api = new GuiApi(coreApi);
                coreApi.Log("Gui", "Gui API created", LogLevel.Debug);
                return api;
            });
        }
    }
}
