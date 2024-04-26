/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace AutomateToolSwap
{
    internal class ModConfig
    {
        public bool Enabled { get; set; } = true;

        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("CapsLock");

        public bool UseDifferentSwapKey { get; set; } = false;

        public KeybindList SwapKey { get; set; } = KeybindList.Parse("MouseLeft");

        public KeybindList LastToolKey { get; set; } = KeybindList.Parse("MouseMiddle");

        public string DetectionMethod { get; set; } = "Cursor";

        public bool WeaponOnMonsters { get; set; } = true;

        public bool AlternativeWeaponOnMonsters { get; set; } = false;

        public int MonsterRangeDetection { get; set; } = 3;

        public bool FishingRodOnWater { get; set; } = true;

        public bool IgnoreGrowingTrees { get; set; } = false;

        public bool AutoReturnToLastTool { get; set; } = false;

        public bool HoeForEmptySoil { get; set; } = true;

        public bool ScytheForGrass { get; set; } = false;

        public bool PickaxeOverWateringCan { get; set; } = false;

        public bool AnyToolForWeeds { get; set; } = false;

        public bool DisableTractorSwap { get; set; } = false;
    }
}