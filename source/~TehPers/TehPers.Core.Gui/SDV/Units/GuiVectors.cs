using TehPers.Core.Gui.Base.Units;

namespace TehPers.Core.Gui.SDV.Units {
    public static class GuiVectors {
        /// <summary>Responsive vector equal to the zero vector.</summary>
        public static ResponsiveVector2<GuiInfo> Zero { get; } = new ResponsiveVector2<GuiInfo>();

        /// <summary>Responsive vector equal to half the parent's units of the same type.</summary>
        public static ResponsiveVector2<GuiInfo> Half { get; } = new ResponsiveVector2<GuiInfo>(GuiUnits.Half, GuiUnits.Half);

        /// <summary>Responsive vector centered in the parent.</summary>
        public static ResponsiveVector2<GuiInfo> Centered { get; } = new ResponsiveVector2<GuiInfo>(GuiUnits.Centered, GuiUnits.Centered);

        /// <summary>Responsive vector equal to the parent's units of the same type.</summary>
        public static ResponsiveVector2<GuiInfo> SameAsParent { get; } = new ResponsiveVector2<GuiInfo>(GuiUnits.SameAsParent, GuiUnits.SameAsParent);
    }
}