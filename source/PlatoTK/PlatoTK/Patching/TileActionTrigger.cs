/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatoTK.Patching
{
    internal class TileActionTrigger : ITileActionTrigger
    {
        public string Trigger { get; }

        public string[] Params { get; }

        public string Full { get; }

        public string LayerName { get; }

        public Point TileLocation { get; }

        public GameLocation Location { get; }

        public TileActionTrigger(string action, string layer, int x, int y, GameLocation location)
        {
            Full = action;
            LayerName = layer;
            Location = location;
            TileLocation = new Point(x, y);
            List<string> callString = action.Split(' ').ToList();
            Trigger = callString[0];
            callString.RemoveAt(0);
            Params = callString.ToArray();
        }
    }
}
