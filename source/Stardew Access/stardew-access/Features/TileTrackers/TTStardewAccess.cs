/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Features.Tracker;

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

internal class TTStardewAccess : TileTrackerBase
{

    private readonly string[] ignored_categories = { "animal" };

    public TTStardewAccess(object? arg = null) : base(arg) {
            
    }

    public override void FindObjects(object? arg) {

        Dictionary<Vector2, (string name, string category)> scannedTiles = Radar.SearchLocation();

        /* Categorise the scanned tiles into groups
         *
         * This method uses breadth first search so the first item is the closest item, no need to reorder or check for closest item
         */
        foreach (var tile in scannedTiles) {

            string category = tile.Value.category;

            if (ignored_categories.Contains(category)) continue;

            AddFocusableObject(tile.Value.category, tile.Value.name, tile.Key);
        }

        base.FindObjects(arg);
    }

}