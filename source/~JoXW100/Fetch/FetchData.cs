/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace Fetch
{
    public class FetchData
    {
        public List<Vector2> path;
        public int nextTile;
        public bool isFetching;
        public bool isBringing;
        public Farmer fetchee;
        public Debris fetched;

    }
}