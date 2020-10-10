/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace NoSoilDecayRedux
{
    public class SaveTiles
    {
        public string location { get; set; } = "Farm";
        public List<Vector2> tiles = new List<Vector2>();

        public SaveTiles()
        {

        }

        public SaveTiles(string location, List<Vector2> tiles)
        {
            this.location = location;
            this.tiles = tiles;
        }
    }
}
