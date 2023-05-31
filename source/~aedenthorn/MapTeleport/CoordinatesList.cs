/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MapTeleport
{
    public class CoordinatesList
    {
        public List<Coordinates> coordinates = new List<Coordinates>();
    }
    public class Coordinates
    {
        public string name;
        public string mapName;
        public int x;
        public int y;
        public int id;
        public bool enabled = true;
    }
}