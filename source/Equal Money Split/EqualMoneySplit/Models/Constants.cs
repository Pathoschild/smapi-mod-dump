/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

namespace EqualMoneySplit.Models
{
    /// <summary>
    /// Constant values used throughout mod
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Mod ID for EqualMoneySplit
        /// </summary>
        public static string ModId { get; private set; } = EqualMoneyMod.SMAPI.ModRegistry.ModID;

        /// <summary>
        /// Base address mods send/listen to
        /// </summary>
        public static string MoneySplitListenerAddress { get; private set; } = "EqualMoneySplit.Address.MoneySplit";
    }
}
