using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Igorious.StardewValley.ColoredChestsMod
{
    public class ColoredChestsModConfig : DynamicConfiguration
    {
        public class MenuConfigInfo
        {
            public int ColorsInRow { get; set; }

            [JsonProperty(Required = Required.Always)]
            public List<string> Colors { get; set; } = new List<string>();
        }

        public MenuConfigInfo MenuConfig { get; set; }

        public override void CreateDefaultConfiguration()
        {
            MenuConfig = new MenuConfigInfo
            {
                ColorsInRow = 4,
                Colors = new List<string>
                {
                    nameof(Color.White), nameof(Color.AliceBlue), "#FF56A5", nameof(Color.IndianRed),
                    nameof(Color.Red), nameof(Color.DarkOrange), nameof(Color.Gold), nameof(Color.Khaki),
                    nameof(Color.YellowGreen), nameof(Color.ForestGreen), nameof(Color.Teal), nameof(Color.DeepSkyBlue),
                    nameof(Color.RoyalBlue), nameof(Color.BlueViolet), nameof(Color.Purple), nameof(Color.DarkGray),
                },
            };
        }
    }
}
