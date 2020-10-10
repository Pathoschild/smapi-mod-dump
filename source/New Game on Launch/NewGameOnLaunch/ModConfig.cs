/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/4ppleCracker/NewGameOnLaunch
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGameOnLaunch
{
    class ModConfig
    {
        public string CharacterName { get; set; } = "NewGameOnLaunch Mod";
        public string FarmName { get; set; } = "NewGameOnLaunch Mod's";
        public string FavoriteThing { get; set; } = "Stardew Valley Mods";
        public int FarmType { get; set; } = 0;
        public int SkinColor { get; set; } = 0;
        public int HairStyle { get; set; } = 0;
        public int ShirtType { get; set; } = 0;
        public int Accessory { get; set; } = -1;
        public bool Male { get; set; } = true;
        public Color EyeColor { get; set; } = new Color(122, 68, 52);
        public Color HairColor { get; set; } = new Color(193, 90, 50);
        public Color PantsColor { get; set; } = new Color(46, 85, 183);
        public bool CatPerson { get; set; } = true;
    }
}
