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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BNWCore
{
    public class Assets_Editor
    {
        public static Texture2D ObjectsTexture;
        public static Texture2D ConstructionTexture;
        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            ObjectsTexture = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/objects.png");
            ConstructionTexture = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/construction.png");
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<int, string>().Data;
                    data.Add(735, $"Magic Net/50/-300/Crafting/{ModEntry.ModHelper.Translation.Get("translator_object_735_name")}/{ModEntry.ModHelper.Translation.Get("translator_object_735_description")}");
                    data.Add(738, $"Magic Bootle/50/-300/Basic -81/{ModEntry.ModHelper.Translation.Get("translator_object_737_name")}/{ModEntry.ModHelper.Translation.Get("translator_object_737_description")}");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/ExtraDialogue"))
            {
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.teste"))
                {
                    e.Edit(asset => {
                        var data = asset.AsDictionary<string, string>().Data;
                        data["Robin_UpgradeConstruction_Festival"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_start_construction");
                        data["Robin_UpgradeConstruction"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_start_construction");
                        data["Robin_NewConstruction_Festival"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_start_construction");
                        data["Robin_NewConstruction"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_start_construction");
                        data["Robin_Instant"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_instant_construction");
                        data["Robin_HouseUpgrade_Accepted"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_ugrade_house");
                    });
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Strings/Locations"))
            {
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.teste"))
                {
                    e.Edit(asset => {
                        var data = asset.AsDictionary<string, string>().Data;
                        data["ScienceHouse_CarpenterMenu"] = ModEntry.ModHelper.Translation.Get("translator_dialog_box_ScienceHouse_CarpenterMenu");
                    });
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add("earth_farming_blessing", $"{ModEntry.ModHelper.Translation.Get("translator_earth_farming_blessing_text")}[#]{ModEntry.ModHelper.Translation.Get("translator_earth_farming_blessing_title")}");
                    data.Add("nature_foraging_blessing", $"{ModEntry.ModHelper.Translation.Get("translator_nature_foraging_blessing_text")}[#]{ModEntry.ModHelper.Translation.Get("translator_nature_foraging_blessing_title")}");
                    data.Add("water_fishing_blessing", $"{ModEntry.ModHelper.Translation.Get("translator_water_fishing_blessing_text")}[#]{ModEntry.ModHelper.Translation.Get("translator_water_fishing_blessing_title")}");
                    data.Add("fire_mining_blessing", $"{ModEntry.ModHelper.Translation.Get("translator_fire_mining_blessing_text")}[#]{ModEntry.ModHelper.Translation.Get("translator_fire_mining_blessing_title")}");
                    data.Add("wind_combat_blessing", $"{ModEntry.ModHelper.Translation.Get("translator_wind_combat_blessing_text")}[#]{ModEntry.ModHelper.Translation.Get("translator_wind_combat_blessing_title")}");
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset => {
                    var editor = asset.AsImage();

                    editor.PatchImage(ObjectsTexture, targetArea: new Rectangle(240, 480, 80, 16), patchMode: PatchMode.Replace);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites/Cursors"))
            {
                if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.teste"))
                {
                    e.Edit(asset => {
                        var editor = asset.AsImage();
                        editor.PatchImage(ConstructionTexture, targetArea: new Rectangle(399, 262, 58, 43), patchMode: PatchMode.Replace);
                    });
                }
            }    
        }
    }
}