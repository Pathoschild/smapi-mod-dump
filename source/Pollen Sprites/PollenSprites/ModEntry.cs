using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace PollenSprites
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //do nothing; currently, this mod only implements the PollenSprite class, allowing it to be accessed by Farm Type Manager's reflection code
        }
    }
}
