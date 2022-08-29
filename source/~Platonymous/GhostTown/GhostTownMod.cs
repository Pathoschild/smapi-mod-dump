/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace GhostTown
{
    public class GhostTownMod : Mod
    {
        internal static Config config;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            helper.Events.Content.AssetRequested += new Ghostify(helper).OnAssetRequested;
        }
    }
}
