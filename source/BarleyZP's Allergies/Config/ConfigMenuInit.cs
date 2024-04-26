/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.Apis;
using StardewModdingAPI;

namespace BZP_Allergies.Config
{
    internal class ConfigMenuInit
    {
        public static void SetupMenuUI(IGenericModConfigMenuApi configMenu, IManifest modManifest)
        {
            // add a link to config
            configMenu.AddPageLink(modManifest, "BarleyZP.BzpAllergies_Farmer", () => "Farmer Allergies");

            // switch to page
            configMenu.AddPage(modManifest, "BarleyZP.BzpAllergies_Farmer", () => "Farmer Allergies");

            // add some config options
            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => "Farmer Allergies"
            );
            configMenu.AddParagraph(
                mod: modManifest,
                text: () => "Eating a food containing an allergen results in a loss of energy and debuffs. Both raw ingredients and derived items may cause reactions."
            );


            List<string> mainAllergies = new()
            {
                "egg", "wheat", "fish", "shellfish", "treenuts", "dairy", "mushroom"
            };
            mainAllergies.Sort();

            foreach (string id in mainAllergies)
            {
                string displayName = AllergenManager.GetAllergenDisplayName(id);
                configMenu.AddBoolOption(
                    mod: modManifest,
                    name: () => displayName,
                    tooltip: () => "Your farmer will be allergic to any foods containing " + displayName.ToLower() + ".",
                    getValue: () => ModEntry.Config.Farmer.Allergies.GetValueOrDefault(id, false),
                    setValue: value => ModEntry.Config.Farmer.Allergies[id] = value
                );
            }
        }

        public static void SetupContentPackConfig(IGenericModConfigMenuApi configMenu, IManifest modManifest, IContentPack pack)
        {
            // switch to farmer allergies page
            configMenu.AddPage(modManifest, "BarleyZP.BzpAllergies_Farmer", () => "Farmer Allergies");

            // add a link to config
            configMenu.AddPageLink(modManifest, pack.Manifest.UniqueID, () => pack.Manifest.Name);

            // switch to page
            configMenu.AddPage(modManifest, pack.Manifest.UniqueID, () => "Farmer Allergies");

            // add a link back
            configMenu.AddPageLink(modManifest, "BarleyZP.BzpAllergies_Farmer", () => "Back");

            // title
            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => pack.Manifest.Name
            );

            // register options
            GenericAllergenConfig farmerConfig = ModEntry.Config.Farmer;
            List<string> sortedAllergens = AllergenManager.ALLERGEN_CONTENT_PACK[pack.Manifest.UniqueID].ToList();
            sortedAllergens.Sort();

            foreach (var allergen in sortedAllergens)
            {
                string displayName = AllergenManager.GetAllergenDisplayName(allergen);
                configMenu.AddBoolOption(
                    mod: modManifest,
                    name: () => displayName,
                    tooltip: () => "Your farmer will be allergic to any foods containing " + displayName.ToLower() + ".",
                    getValue: () => ModEntry.Config.Farmer.Allergies.GetValueOrDefault(allergen, false),
                    setValue: value => ModEntry.Config.Farmer.Allergies[allergen] = value
                );
            }
        }
    }
}
