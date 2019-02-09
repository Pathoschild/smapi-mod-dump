using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.CoreMod.Api.Items.Machines;

namespace TehPers.CoreMod.Items.Machines {
    internal class MachineInformation : IMachineInformation {
        /// <inheritdoc />
        public GameLocation Location { get; }

        /// <inheritdoc />
        public Vector2 Position { get; }

        private object _state;

        public MachineInformation(GameLocation location, Vector2 position) {
            this.Location = location;
            this.Position = position;
        }

        /// <inheritdoc />
        public T GetState<T>() {
            return this._state is T state ? state : default;
        }

        /// <inheritdoc />
        public void SetState<T>(T state) {
            this._state = state;
        }
    }
}