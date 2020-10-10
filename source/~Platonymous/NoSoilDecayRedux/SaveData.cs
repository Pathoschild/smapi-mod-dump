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
using StardewValley;
using System.Collections.Generic;

namespace NoSoilDecayRedux
{
    public class SaveData
    {

        public List<SaveTiles> data { get; set; } = new List<SaveTiles>();

        public SaveData()
        {

        }

        public SaveData(Dictionary<GameLocation, List<Vector2>> data)
        {
            this.data = new List<SaveTiles>();
            foreach (KeyValuePair<GameLocation, List<Vector2>> l in data)
                this.data.Add(new SaveTiles(l.Key.name, l.Value));
        }

    }
}
