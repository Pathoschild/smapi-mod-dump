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
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TehPers.PowerGrid.Units;
using SObject = StardewValley.Object;

namespace TehPers.PowerGrid.World
{
    /// <summary>
    /// An object which can conduct power.
    /// </summary>
    public interface IEnergyConductor
    {
        /// <summary>
        /// Positions that this conductor can send power to.
        /// </summary>
        IEnumerable<Vector2> OutgoingConnections { get; }

        /// <summary>
        /// Updates this conductor's state.
        /// </summary>
        /// <param name="info">Information about the energy state of this conductor.</param>
        void Update(EnergyTickInfo info);
    }

    public interface IEnergyProducer : IEnergyConductor
    {
        /// <summary>
        /// The amount of power this producer creates.
        /// </summary>
        Power Production { get; }
    }

    public interface IEnergyConsumer : IEnergyConductor
    {
        /// <summary>
        /// The amount of power this consumer uses.
        /// </summary>
        Power Consumption { get; }
    }
}