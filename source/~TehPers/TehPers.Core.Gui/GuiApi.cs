using StardewValley.Menus;
using TehPers.Core.Gui.Base.Components;

namespace TehPers.Core.Gui {
    internal class GuiApi : IGuiApi {
        private readonly ITehCoreApi _coreApi;

        public GuiApi(ITehCoreApi coreApiApi) {
            this._coreApi = coreApiApi;
        }

        public IClickableMenu ConvertMenu(IGuiComponent menu) {
            return new GuiClickableMenu(menu, this._coreApi);
        }
    }
}