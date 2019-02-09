using System.Collections.Generic;
using TehPers.Core.Gui.Base.Units;

namespace TehPers.Core.Gui.SDV.Units {
    public static class GuiUnits  {
        
        /// <summary>Responsive unit which resolves to 0.</summary>
        public static ResponsiveUnit<GuiInfo> Zero { get; } = new ResponsiveUnit<GuiInfo>();

        /// <summary>Responsive unit equal to half the parent's unit of the same type along the same axis.</summary>
        public static ResponsiveUnit<GuiInfo> Half { get; } = new ResponsiveUnit<GuiInfo>(new PercentParentUnit(0.5f));

        /// <summary>Responsive unit centered in the parent along the same axis.</summary>
        public static ResponsiveUnit<GuiInfo> Centered { get; } = new ResponsiveUnit<GuiInfo>(new PercentParentUnit(1f), new PercentParentSizeUnit(0.5f), new PercentSizeUnit(-0.5f));

        /// <summary>Responsive unit equal to the parent's unit of the same type along the same axis.</summary>
        public static ResponsiveUnit<GuiInfo> SameAsParent { get; } = new ResponsiveUnit<GuiInfo>(new PercentParentUnit(1f));
    }
}
