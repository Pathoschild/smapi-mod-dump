/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

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