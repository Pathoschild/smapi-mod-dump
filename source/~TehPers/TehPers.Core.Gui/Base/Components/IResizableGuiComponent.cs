using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Units;

namespace TehPers.Core.Gui.Base.Components {
    public interface IResizableGuiComponent : IGuiComponent {
        /// <inheritdoc cref="IGuiComponent.Location"/>
        new ResponsiveVector2<GuiInfo> Location { get; set; }

        /// <inheritdoc cref="IGuiComponent.Size"/>
        new ResponsiveVector2<GuiInfo> Size { get; set; }
    }
}
