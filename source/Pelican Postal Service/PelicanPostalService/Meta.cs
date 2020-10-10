/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TobinWilson/PelicanPostalService
**
*************************************************/

using Pelican.Config;
using StardewModdingAPI;

namespace Pelican
{
    public class Meta
    {
        public static ModConfig Config { get; internal set; }
        public static IMonitor Console { get; internal set; }
        public static ITranslationHelper Lang { get; internal set; }
    }
}
