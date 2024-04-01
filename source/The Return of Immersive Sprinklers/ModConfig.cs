/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lucaskfreitas/ImmersiveSprinklers
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace ImmersiveSprinklers
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool ShowRangeWhenPlacing { get; set; } = true;
        public float Scale { get; set; } = 4;
        public float Alpha { get; set; } = 1;
        public int DrawOffsetX { get; set; } = 0;
        public int DrawOffsetY { get; set; } = 0;
        public int DrawOffsetZ { get; set; } = 0;
        public SButton PickupButton { get; set; } = SButton.E;
        public SButton ActivateButton { get; set; } = SButton.Enter;
        public SButton ShowRangeButton { get; set; } = SButton.LeftControl;
        public Dictionary<string, int> SprinklerRadii { get; set; } = new()
        {
            { "Sprinkler", 0 },
            { "Quality Sprinkler", 1 },
            { "Iridium Sprinkler", 2 }
        };
    }
}
