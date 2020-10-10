/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmAutomation.Common
{
    [DebuggerDisplay("{Location} : {Object.Name} => {Chest.Name}")]
    public class ConnectedTile
    {
        public Object Object { get; set; }
        public Chest Chest { get; set; }
        public Vector2 Location { get; set; }

        public bool Equals(ConnectedTile obj)
        {
            return Location.Equals(obj?.Location);
        }
    }
}
