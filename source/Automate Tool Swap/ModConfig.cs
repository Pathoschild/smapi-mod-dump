/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using StardewModdingAPI;

namespace AutomateToolSwap
{
    internal class ModConfig
    {
        public bool Enabled { get; set; } = true;

        public SButton ToggleKey { get; set; } = SButton.CapsLock;

        public SButton SwapKey { get; set; } = SButton.MouseLeft;

        public bool Pickaxe_greater_wcan { get; set; } = false;

        public bool Pickaxe_over_melee { get; set; } = false;

        public SButton LastToolButton { get; set; } = SButton.MouseMiddle;
        
        public bool Hoe_in_empty_soil { get; set; } = true;

        public bool Auto_switch_last_tool { get; set; } = false;
    }
}