/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.CoreMod.Api.Items.Machines {
    public interface IMachineInformation {
        /// <summary>The location this machine is located at.</summary>
        GameLocation Location { get; }

        /// <summary>The coordinates of this machine.</summary>
        Vector2 Position { get; }

        /// <summary>Gets the tracked state of the machine.</summary>
        /// <typeparam name="T">The type of state being tracked.</typeparam>
        /// <returns>The tracked state.</returns>
        T GetState<T>();

        /// <summary>Sets the tracked state of the machine.</summary>
        /// <typeparam name="T">The type of state being tracked.</typeparam>
        /// <param name="state">The new state of the machine.</param>
        void SetState<T>(T state);
    }
}