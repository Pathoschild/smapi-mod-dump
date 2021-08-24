/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace Common.Integrations.JsonAssets
{
    internal class JsonAssetsIntegration : ModIntegration<IJsonAssetsAPI>
    {
        public JsonAssetsIntegration(IModRegistry modRegistry)
            : base(modRegistry, "spacechase0.JsonAssets")
        {
        }
    }
}