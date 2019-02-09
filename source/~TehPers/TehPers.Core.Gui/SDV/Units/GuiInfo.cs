namespace TehPers.Core.Gui.SDV.Units {
    public class GuiInfo {
        /// <summary>The resolved units of the parent component of the same type along the same axis.</summary>
        public float ParentUnits { get; }

        /// <summary>The resolved size of the parent component along the same axis.</summary>
        public float ParentSize { get; }

        /// <summary>This component's resolved size along the same axis, or <c>0</c> if not calculated yet.</summary>
        public float ResolvedSize { get; }

        public GuiInfo(float parentUnits, float parentSize) : this(parentUnits, parentSize, 0) { }
        public GuiInfo(float parentUnits, float parentSize, float resolvedSize) {
            this.ParentUnits = parentUnits;
            this.ParentSize = parentSize;
            this.ResolvedSize = resolvedSize;
        }
    }
}
