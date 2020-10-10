/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Api.Items.Machines {
    public interface IMachine : IModObject {
        /// <summary>Called when this machine is placed down. The state of the machine can be initialized here.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        void Placed(IMachineInformation machineInfo);

        /// <summary>Called when this machine is removed from the world.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        void Removed(IMachineInformation machineInfo);

        /// <summary>Called every tick the machine is in the world.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        void UpdateTick(IMachineInformation machineInfo);

        /// <summary>Called after the machine is drawn.</summary>
        /// <param name="machineInformation">Information about the current machine.</param>
        /// <param name="spriteBatch">The batch used to draw this machine.</param>
        /// <param name="x">The x-coordinate the machine was drawn to.</param>
        /// <param name="y">The y-coordinate the machine was drawn to.</param>
        void AfterDraw(IMachineInformation machineInformation, SpriteBatch spriteBatch, int x, int y);

        /// <summary>Tries to accept an item into this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="items">The items that are available.</param>
        /// <param name="inputTile">The tile to input from (for directional input).</param>
        /// <param name="doAccept">A callback which performs the action of accepting an item. Until this method is called, the state of the machine should not actually change. This may be called multiple times if multiple payloads are returned.</param>
        /// <returns>The item payloads that were accepted by this machine. Each payload is an <see cref="IEnumerable{T}"/> containing requests that should be sent together. The return value is an <see cref="IEnumerable{T}"/> of payloads. If nothing should be accepted, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>.</returns>
        IEnumerable<IEnumerable<ObjectRequest>> Accept(IMachineInformation machineInfo, IEnumerable<ObjectRequest> items, Vector2 inputTile, out MachineAction doAccept);

        /// <summary>Tries to eject an item from this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="items">The items that can be ejected.</param>
        /// <param name="outputTile">The tile to output to (for directional output).</param>
        /// <param name="doEject">A callback which performs the action of ejecting an item. Until this method is called, the state of the machine should not actually change. This may be called multiple times if multiple payloads are returned.</param>
        /// <returns>The item payloads that were ejected by this machine. Each payload is an <see cref="IEnumerable{T}"/> containing requests that should be sent together. The return value is an <see cref="IEnumerable{T}"/> of payloads. If nothing should be ejected, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>.</returns>
        IEnumerable<IEnumerable<ObjectRequest>> Eject(IMachineInformation machineInfo, IEnumerable<ObjectRequest> items, Vector2 outputTile, out MachineAction doEject);

        /// <summary>Called whenever a farmer tries to place an item into this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="heldObject">The object held by the farmer when activating the machine.</param>
        /// <param name="inventory">The items in the farmer's inventory, including the held item.</param>
        /// <param name="source">The farmer who activated this machine.</param>
        /// <param name="doInsert">A callback which performs the action of inserting an item. Until this method is called, the state of the machine should not actually change. This callback should not remove the inserted items from the farmer.</param>
        /// <returns>The item payload that would be inserted into this machine. If nothing would be inserted, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>. This method should not modify the state of the machine or farmer, but should return the items that would be inserted as a payload instead.</returns>
        IEnumerable<ObjectRequest> InsertItem(IMachineInformation machineInfo, SObject heldObject, IEnumerable<Item> inventory, Farmer source, out Action doInsert);

        /// <summary>Called whenever a farmer tries to remove an item from this machine.</summary>
        /// <param name="machineInfo">Information about the current machine.</param>
        /// <param name="source">The farmer who activated this machine.</param>
        /// <param name="doRemove">A callback which performs the action of removing an item from this machine. Until this method is called, the state of the machine should not actually change. This callback should not add the removed items to the farmer.</param>
        /// <returns>The item payload that would be removed from this machine. If nothing would be removed, use <c>null</c> or <see cref="Enumerable.Empty{T}"/>. This method should not modify the state of the machine or farmer, but should return the items that would be removed as a payload instead.</returns>
        IEnumerable<ObjectRequest> RemoveItem(IMachineInformation machineInfo, Farmer source, out Action doRemove);
    }

    /// <summary>An action performed by a machine.</summary>
    /// <param name="payload">The payload being acted upon.</param>
    public delegate void MachineAction(IEnumerable<ObjectRequest> payload);
}