/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterArtisanGoodIcons.Framework.Data.Format.Unloaded
{
    /// <summary> Provides texture and source ingredient information.</summary>
    internal class UnloadedData
    {
        public ItemIndicator[] Fruits { get; set; } = null;
        public ItemIndicator[] Vegetables { get; set; } = null;
        public ItemIndicator[] Flowers { get; set; } = null;

        public string Jelly { get; set; } = null;
        public string Pickles { get; set; } = null;
        public string Wine { get; set; } = null;
        public string Juice { get; set; } = null;
        public string Honey { get; set; } = null;

        public UnloadedDefinitions[] CustomArtisanGoods { get; set; } = null;

        public bool CanBeOverwritten { get; set; } = false;
    }
}
