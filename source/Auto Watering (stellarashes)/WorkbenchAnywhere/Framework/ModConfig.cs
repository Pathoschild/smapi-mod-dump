/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewModdingAPI;

namespace WorkbenchAnywhere.Framework
{
    public class ModConfig
    {
        public bool AllowRemoteDeposit { get; set; } = true;
        public bool ReplaceCraftMenu { get; set; } = true;
        public bool ReplaceWorkbench { get; set; } = false;
        public bool CarpenterUsesMaterialChests { get; set; } = true;
        public string DepositKey { get; set; } = SButton.V.ToString();
        public string ConfigReloadKey { get; set; } = SButton.F5.ToString();
        public string[] MaterialItemNames { get; set; } = new string[0];
        public int[] MaterialItemCategories { get; set; } = new[] { -15, -16, -28 };
    }
}
