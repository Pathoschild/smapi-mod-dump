/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace StackEverythingRedux
{
    public class ModConfig
    {
        public bool EnableStackSplitRedux { get; set; } = true;
        public int MaxStackingNumber { get; set; } = 999;
        public int DefaultCraftingAmount { get; set; } = 1;
        public int DefaultShopAmount { get; set; } = 5;
        public bool DebuggingMode { get; set; } = false;
    }

    /// <summary>
    /// This class containe "tunables" that should not be user-editable
    /// </summary>
    /// <remarks>Gathered here so we can tune the mod from one place, instead of hunting down config knobs everywhere</remarks>
    internal static class StaticConfig
    {
        /// <summary>Valid modifier keys to hold while RightClick-ing</summary>
        internal static readonly SButton[] ModifierKeys = [SButton.LeftShift, SButton.RightShift];

        /// <summary>Delay between new menu appearing & our handler beginning</summary>
        /// <remarks>To allow time for other mods to manipulate inventories</remarks>
        internal static readonly int SplitMenuOpenDelayTicks = 2;

        /// <summary>Text color when the text is highlighted. This should contrast with HighlightColor.</summary>
        internal static readonly Color HighlightTextColor = Color.White;

        /// <summary>The background color of the highlighted text.</summary>
        internal static readonly Color HighlightColor = Color.Blue;
    }
}
