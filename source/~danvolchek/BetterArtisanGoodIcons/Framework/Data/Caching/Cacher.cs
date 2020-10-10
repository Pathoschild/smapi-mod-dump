/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterArtisanGoodIcons.Framework.Data.Format.Cached;
using StardewModdingAPI;

namespace BetterArtisanGoodIcons.Framework.Data.Caching
{
    class Cacher
    {
        private readonly IMonitor monitor;

        public Cacher(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        public IDictionary<int, CachedDefinition> Cache(Format.Loaded.LoadedDefinition[] definitions)
        {
            IDictionary<int, CachedDefinition> positions = new Dictionary<int, CachedDefinition>();

            foreach (Format.Loaded.LoadedDefinition def in definitions)
            {
                if (!def.ArtisanGood.TryLoad(out int artisanGoodId))
                {
                    def.ArtisanGood.LoadErrorMessage(this.monitor, "artisan good");
                    continue;
                }
                positions[artisanGoodId] = new CachedDefinition(this.monitor, def.Texture, def.SourceItems);
            }

            return positions;
        }
    }
}
