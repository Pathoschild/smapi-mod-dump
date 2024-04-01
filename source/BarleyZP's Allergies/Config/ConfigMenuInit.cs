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
            
            // add some config options
            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => "Farmer Allergies"
            );
            configMenu.AddParagraph(
                mod: modManifest,
                text: () => "Eating a food containing an allergen results in a loss of energy and debuffs. Both raw ingredients and cooked foods may cause reactions."
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Eggs",
                tooltip: () => "Your farmer will be allergic to any foods containing eggs.",
                getValue: () => ModEntry.Config.Farmer.EggAllergy,
                setValue: value => ModEntry.Config.Farmer.EggAllergy = value
            );
            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Wheat",
                tooltip: () => "Your farmer will be allergic to any foods containing wheat.",
                getValue: () => ModEntry.Config.Farmer.WheatAllergy,
                setValue: value => ModEntry.Config.Farmer.WheatAllergy = value
            );
            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Fish",
                tooltip: () => "Your farmer will be allergic to any foods containing fish.",
                getValue: () => ModEntry.Config.Farmer.FishAllergy,
                setValue: value => ModEntry.Config.Farmer.FishAllergy = value
            );
            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Shellfish",
                tooltip: () => "Your farmer will be allergic to any foods containing shellfish.",
                getValue: () => ModEntry.Config.Farmer.ShellfishAllergy,
                setValue: value => ModEntry.Config.Farmer.ShellfishAllergy = value
            );
            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Tree Nuts",
                tooltip: () => "Your farmer will be allergic to any foods containing tree nuts.",
                getValue: () => ModEntry.Config.Farmer.TreenutAllergy,
                setValue: value => ModEntry.Config.Farmer.TreenutAllergy = value
            );
            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => "Dairy",
                tooltip: () => "Your farmer will be allergic to any foods containing dairy.",
                getValue: () => ModEntry.Config.Farmer.DairyAllergy,
                setValue: value => ModEntry.Config.Farmer.DairyAllergy = value
            );
        }
    }
}
