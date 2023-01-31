/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using xTile;
using static BuildableGreenhouse.Framework.ModExtension.ModExtension;
using static BuildableGreenhouse.Framework.ModTranslation;

namespace BuildableGreenhouse
{
    public class BuildableGreenhouse : Mod
    {
        public override void Entry(IModHelper helper)
        {
            InitializeTranslations(helper);
            InitializeExtensions(helper, Monitor, ModManifest);

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Maps\\GreenhouseMap"))
            {
                e.Edit(asset =>
                {
                    Map greenhouseIndoorMap = asset.AsMap().Data;
                    greenhouseIndoorMap.Properties["IsGreenhouse"] = true;
                });
            }
        }
    }
}
