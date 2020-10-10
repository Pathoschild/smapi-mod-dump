/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;

namespace QiExchanger
{
    internal class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.F9;
        public int ExchangeRate { get; set; } = 10;
    }
}
