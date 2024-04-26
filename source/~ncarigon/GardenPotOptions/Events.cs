/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;

namespace GardenPotOptions {
    internal static class Events {
        public static void Register() {
            if (ModEntry.Instance is not null) {
                ModEntry.Instance.Helper.Events.Content.AssetRequested += (_, e)
                    => Content_AssetRequested(e);
            }
        }

        public static void Content_AssetRequested(AssetRequestedEventArgs e) {
            if (e.NameWithoutLocale.Name.Equals("Data/Mail", StringComparison.InvariantCultureIgnoreCase)) {
                e.Edit(a => {
                    var d = a.AsDictionary<string, string>().Data;
                    d["NCarigon.GardenPotOptions_Pot"] = $"{ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/mail_text_pot") ?? "null"}%item id (BC)62 %%[#]{ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/mail_title_pot") ?? "null"}";
                    d["NCarigon.GardenPotOptions_Recipe"] = $"{ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/mail_text_recipe") ?? "null"}%item craftingRecipe Garden_Pot %%[#]{ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/mail_title_recipe") ?? "null"}";
                });
            } else if (e.NameWithoutLocale.Name.Equals("Data/Events/Farm", StringComparison.InvariantCultureIgnoreCase)) {
                e.Edit(a => {
                    var d = a.AsDictionary<string, string>().Data;
                    var hp = ModEntry.Instance?.ModConfig?.HeartsForGardenPot ?? -1;
                    var hr = ModEntry.Instance?.ModConfig?.HeartsForRecipe ?? -1;
                    if (hp > -1) {
                        d[$"NCarigon.GardenPotOptions_Pot_Send/f Evelyn {hp * 250}/k 900553/sendmail NCarigon.GardenPotOptions_Pot"] = "null";
                        if (hr >= hp) {
                            d[$"NCarigon.GardenPotOptions_Recipe_Send/f Evelyn {hr * 250}/k 900553/e NCarigon.GardenPotOptions_Pot_Send/sendmail NCarigon.GardenPotOptions_Recipe"] = "null";
                        }
                        // add new default with requirement
                        d["900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny/k NCarigon.GardenPotOptions_Pot_Send"] = d["900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny"];
                        // add only recipe event
                        d["900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny/e NCarigon.GardenPotOptions_Pot_Send/k NCarigon.GardenPotOptions_Recipe_Send"] = ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/event_text_recipe") ?? "null";
                        // remove default
                        d.Remove("900553/t 600 1130/Hn ccPantry/A cc_Greenhouse/w sunny");
                    }
                });
            }
        }
    }
}
