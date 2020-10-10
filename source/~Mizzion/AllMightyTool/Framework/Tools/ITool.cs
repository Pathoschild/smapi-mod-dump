/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace AllMightyTool.Framework.Tools
{
    internal interface ITool
    {
        ///<summary>Activate the tool</summary>
        /// <param name="tile">The tile to modify.</param>
        /// <param name="obj">The object on the tile.</param>
        /// <param name="terrain">The feature on the tile.</param>
        /// <param name="who">The current player.</param>
        /// <param name="tool">The tool selected by the player (if any).</param>
        /// <param name="item">The item selected by the player (if any).</param>
        /// <param name="location">The current location.</param>
        bool Activate(Vector2 tile, SObject obj, TerrainFeature terrain, Farmer who, Tool tool, Item item,
            GameLocation location);
    }
}
