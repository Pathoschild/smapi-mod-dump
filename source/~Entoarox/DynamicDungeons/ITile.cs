/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using xTile.Layers;

namespace Entoarox.DynamicDungeons
{
    internal interface ITile
    {
        /*********
        ** Accessors
        *********/
        int X { get; set; }
        int Y { get; set; }
        Layer Layer { get; set; }


        /*********
        ** Methods
        *********/
        xTile.Tiles.Tile Get();
    }
}
