using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;

namespace Sprint
{
    class ModConfig
    {
        public SButton SprintKey { get; set; } = SButton.LeftShift;
        public int SprintSpeed { get; set; } = 5;
        public bool DrainStamina { get; set; } = true;
        public float StaminaCost { get; set; } = 0.25f;
    }
}
