using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace catGifts
{
    class ModConfig
    {
        public int THRESHOLD_1 { get; set; } = 300;
        public int THRESHOLD_2 { get; set; } = 1000;
        public int THRESHOLD_3 { get; set; } = 1000;
        public int GIFT_CHANCE_1 { get; set; } = 20;
        public int GIFT_CHANCE_2 { get; set; } = 30;
        public int GIFT_CHANCE_3 { get; set; } = 40;
        public int FARMHAND_CHANCE { get; set; } = 30;
        public int LOW_CHANCE { get; set; } = 40;
        public int MID_CHANCE { get; set; } = 40;
        public int HI_CHANCE { get; set; } = 20;
        public int MAX_WEEKLY_GIFTS { get; set; } = 3;
    }
}
