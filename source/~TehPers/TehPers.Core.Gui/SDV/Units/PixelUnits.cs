using TehPers.Core.Gui.Base.Units;

namespace TehPers.Core.Gui.SDV.Units {
    public class PixelUnits : IUnit<GuiInfo> {
        /// <inheritdoc />
        public float Quantity { get; }

        public PixelUnits(float quantity) {
            this.Quantity = quantity;
        }

        /// <inheritdoc />
        public float Resolve(GuiInfo info) {
            return this.Quantity;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Negate() {
            return new PixelUnits(-this.Quantity);
        }

        /// <inheritdoc />
        public bool TryAdd(IUnit<GuiInfo> other, out IUnit<GuiInfo> sum) {
            if (other is PixelUnits) {
                sum = new PixelUnits(this.Quantity + other.Quantity);
                return true;
            }

            sum = default;
            return false;
        }

        /// <inheritdoc />
        public IUnit<GuiInfo> Multiply(float scalar) {
            return new PixelUnits(this.Quantity * scalar);
        }
    }
}