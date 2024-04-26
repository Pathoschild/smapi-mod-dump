/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chsiao58/EvenBetterArtisanGoodIcons
**
*************************************************/

using System.Collections.Generic;

namespace BetterArtisanGoodIcons.Content
{
    /// <summary> Provides texture and source ingredient information.</summary>
    internal class CustomTextureData
    {
        public List<string> Fruits { get; set; } = null;
        public List<string> Vegetables { get; set; } = null;
        public List<string> Flowers { get; set; } = null;
        public List<string> Mushrooms { get; set; } = null;


        public string Jelly { get; set; } = null;
        public string Pickles { get; set; } = null;
        public string Wine { get; set; } = null;
        public string Juice { get; set; } = null;
        public string Honey { get; set; } = null;
        public string DriedMushrooms { get; set; } = null;

    }
}
