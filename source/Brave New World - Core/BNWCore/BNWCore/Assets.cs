/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BNWCore
{
    public class AssetEditor
    {
        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<int, string>().Data;
                    data.Add(735, $"Magic Net/50/-300/Crafting/{ModEntry.ModHelper.Translation.Get("BNWCoreMagicNetname")}/{ModEntry.ModHelper.Translation.Get("BNWCoreMagicNetdescription")}");
                    data.Add(738, $"Magic Bootle/50/-300/Basic -81/{ModEntry.ModHelper.Translation.Get("BNWCoreMagicBootlename")}/{ModEntry.ModHelper.Translation.Get("BNWCoreMagicBootledescription")}");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset => {
                    var editor = asset.AsImage();

                    editor.PatchImage(ModEntry.ObjectsTexture, targetArea: new Rectangle(240, 480, 80, 16), patchMode: PatchMode.Replace);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites/Cursors"))
            {
                e.Edit(asset => {
                    var editor = asset.AsImage();
                    editor.PatchImage(ModEntry.ConstructionTexture, targetArea: new Rectangle(399, 262, 58, 43), patchMode: PatchMode.Replace);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add("earth_farming_blessing", "earth[#]earth");
                    data.Add("nature_foraging_blessing", "nature[#]nature");
                    data.Add("water_fishing_blessing", "water[#]water");
                    data.Add("fire_mining_blessing", "fire[#]fire");
                    data.Add("wind_combat_blessing", "wind[#]wind");
                });
            }
        }
    }
}