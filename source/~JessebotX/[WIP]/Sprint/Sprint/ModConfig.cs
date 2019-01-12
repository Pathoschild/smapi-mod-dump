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
        public SButton PrimarySprintKey { get; set; } = SButton.LeftShift;
        public SButton SecondarySprintKey { get; set; } = SButton.RightShift;
        public SButton ControllerSprintButton { get; set; } = SButton.LeftStick;

        public SButton SlowDownKey { get; set; } = SButton.LeftControl;
    }
}
