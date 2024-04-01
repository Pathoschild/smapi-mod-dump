/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace Skateboard
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public SButton RideButton { get; set; } = SButton.MouseRight;
        public float MaxSpeed { get; set; } = 10;
        public float Acceleration { get; set; } = 1;
        public float Deceleration { get; set; } = 0.5f;
        public string CraftingRequirements { get; set; } = "709 4 337 1 335 1 338 1 92 10";
    }
}
