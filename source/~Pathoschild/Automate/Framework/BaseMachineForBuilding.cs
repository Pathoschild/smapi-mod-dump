/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace Pathoschild.Stardew.Automate.Framework
{
    /// <summary>A generic machine instance for a building.</summary>
    internal abstract class BaseMachineForBuilding<TMachine> : BaseMachine<TMachine> where TMachine : Building
    {
        /*********
        ** Protected methods
        *********/
        /// <inheritdoc />
        protected BaseMachineForBuilding(TMachine machine, GameLocation location, in Rectangle tileArea, string machineTypeId = null)
            : base(machine, location, in tileArea, machineTypeId) { }

        /// <summary>Get the player who 'owns' the machine.</summary>
        /// <remarks>See remarks on <see cref="GenericObjectMachine{TMachine}.GetOwner"/>.</remarks>
        protected Farmer GetOwner()
        {
            Farmer mainPlayer = Game1.player;

            long ownerId = this.Machine.owner.Value;
            return ownerId != 0
                ? Game1.getFarmer(ownerId) ?? mainPlayer
                : mainPlayer;
        }
    }
}
