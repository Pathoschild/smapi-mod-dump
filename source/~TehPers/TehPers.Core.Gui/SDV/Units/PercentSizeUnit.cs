using TehPers.Core.Gui.Base.Units;

namespace TehPers.Core.Gui.SDV.Units {
    public class PercentSizeUnit : IGuiUnit {
        /// <inheritdoc />
        public float Quantity { get; }

        public PercentSizeUnit(float quantity) {
            this.Quantity = quantity;
        }

        /// <inheritdoc />
        public float Resolve(GuiInfo info) {
            return this.Quantity * info.ResolvedSize;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Negate() {
            return new PercentSizeUnit(-this.Quantity);
        }

        /// <inheritdoc />
        public bool TryAdd(IUnit<GuiInfo> other, out IUnit<GuiInfo> sum) {
            if (other is PercentSizeUnit) {
                sum = new PercentSizeUnit(this.Quantity + other.Quantity);
                return true;
            }

            sum = default;
            return false;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Multiply(float scalar) {
            return new PercentSizeUnit(this.Quantity * scalar);
        }
    }
}