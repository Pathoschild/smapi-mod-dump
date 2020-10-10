/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FarmCleaner.Framework.Configs
{
    internal class FcConfig
    {
       // public List<string> LocationsToClear { get; set; } = new List<string>(){"Farm", "Mountain"};
        public bool GrassRemoval { get; set; } = true;
        public bool WeedRemoval { get; set; } = true;
        public bool StoneRemoval { get; set; } = true;
        public bool TwigRemoval { get; set; } = true;
        public bool StumpRemoval { get; set; } = false;
        public bool LargeLogRemoval { get; set; } = false;

        public bool LargeStoneRemoval { get; set; } = false;

        public Vector2 ChestLocation { get; set; } = new Vector2(58, 16);

        public TreeConfig TreeConfigs { get; set; } = new TreeConfig();

        /*
        public bool SaplingRemoval { get; set; }
        public int MaxTreeStage { get; set; }*/
    }
}
