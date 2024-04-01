/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

//using Harmony;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MultiStoryFarmhouse
{
    public class FloorsData
    {
        public List<Floor> floors = new List<Floor>();
        public FloorsData()
        {
        }
    }

    public class Floor
    {
        public string name;
        public string mapPath;
        public Vector2 stairsStart;
        public List<Rectangle> floors = new List<Rectangle>();
        public List<Rectangle> walls = new List<Rectangle>();
    }
}