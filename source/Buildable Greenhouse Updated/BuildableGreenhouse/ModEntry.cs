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
using static BuildableGreenhouse.ModExtension.ModExtension;

namespace BuildableGreenhouse
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            InitializeExtensions(helper, Monitor, ModManifest);

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Maps\\Greenhouse"))
            {
                e.Edit(asset =>
                {
                    Map greenhouseIndoorMap = asset.AsMap().Data;
                    greenhouseIndoorMap.Properties["IsGreenhouse"] = true;
                });
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            
        }
    }
}
