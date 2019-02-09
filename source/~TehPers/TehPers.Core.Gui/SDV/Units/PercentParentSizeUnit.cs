using TehPers.Core.Gui.Base.Units;

namespace TehPers.Core.Gui.SDV.Units {
    public class PercentParentSizeUnit : IGuiUnit {
        /// <inheritdoc />
        public float Quantity { get; }

        public PercentParentSizeUnit(float quantity) {
            this.Quantity = quantity;
        }

        /// <inheritdoc />
        public float Resolve(GuiInfo info) {
            return this.Quantity * info.ParentSize;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Negate() {
            return new PercentParentSizeUnit(-this.Quantity);
        }

        /// <inheritdoc />
        public bool TryAdd(IUnit<GuiInfo> other, out IUnit<GuiInfo> sum) {
            if (other is PercentParentSizeUnit) {
                sum = new PercentParentSizeUnit(this.Quantity + other.Quantity);
                return true;
            }

            sum = default;
            return false;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Multiply(float scalar) {
            return new PercentParentSizeUnit(this.Quantity * scalar);
        }
    }
}