/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Xml.Serialization;

namespace BarkingUpTheRightTree.Tools
{
    /// <summary>Represents the bark remover tool.</summary>
    [XmlType("Mods_BURT_BarkRemover")] // this is required for SpaceCore to be able to serialise the tool
    public class BarkRemover : Tool
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        public BarkRemover()
            : base("Bark Remover", 0, 546, 572, false) { }

        /// <summary>Gets a new instance of <see cref="BarkRemover"/>.</summary>
        /// <returns>A new <see cref="BarkRemover"/> instance.</returns>
        public override Item getOne() => new BarkRemover();

        /// <summary>Whether the bark remover can be trashed.</summary>
        /// <returns><see langword="true"/>, meaning it can be trashed.</returns>
        public override bool canBeTrashed() => true;

        /// <summary>Performs the main action of the tool.</summary>
        /// <param name="location">The current game location.</param>
        /// <param name="x">The X hit position of the tool.</param>
        /// <param name="y">The Y hit position of the tool.</param>
        /// <param name="power">The tool power (unused).</param>
        /// <param name="who">The farmer performing the action (unused).</param>
        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            var tile = new Vector2(x / 64, y / 64);
            if (location.terrainFeatures.ContainsKey(tile))
                location.terrainFeatures[tile].performToolAction(this, 0, tile, location);
        }


        /*********
        ** Protected Methods
        *********/
        /// <summary>Gets the display name.</summary>
        /// <returns>The display name.</returns>
        protected override string loadDisplayName() => "Bark Remover";

        /// <summary>Gets the description.</summary>
        /// <returns>The description.</returns>
        protected override string loadDescription() => "Removes bark from specific trees.";
    }
}
