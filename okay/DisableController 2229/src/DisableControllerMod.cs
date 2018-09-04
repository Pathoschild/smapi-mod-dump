using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace DisableControllerMod
{
        public class ModEntry : Mod {
                public override void Entry(IModHelper helper)
            {
                Microsoft.Xna.Framework.Input.DisableGamePad.DisableGamePads();
            }
            
            
            
        }
}
