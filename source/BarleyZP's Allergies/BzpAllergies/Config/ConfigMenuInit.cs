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
            ITranslationHelper translation = ModEntry.Instance.Translation;
            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => translation.Get("config.general-settings")
            );

            configMenu.AddKeybind(
                mod: modManifest,
                name: () => translation.Get("config.allergy-menu-kb"),
                getValue: () => ModEntry.Instance.Config.AllergyPageButton,
                setValue: value => ModEntry.Instance.Config.AllergyPageButton = value,
                tooltip: () => translation.Get("config.allergy-menu-kb-tt")
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.skills-tab"),
                tooltip: () => translation.Get("config.skills-tab-tt"),
                getValue: () => ModEntry.Instance.Config.EnableTab,
                setValue: value => ModEntry.Instance.Config.EnableTab = value
            );


            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => translation.Get("config.difficulty")
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.eating-hint"),
                tooltip: () => translation.Get("config.eating-hint-tt"),
                getValue: () => ModEntry.Instance.Config.HintBeforeEating,
                setValue: value => ModEntry.Instance.Config.HintBeforeEating = value
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.holding-reaction"),
                tooltip: () => translation.Get("config.holding-reaction-tt"),
                getValue: () => ModEntry.Instance.Config.HoldingReaction,
                setValue: value => ModEntry.Instance.Config.HoldingReaction = value
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.cooking-reaction"),
                tooltip: () => translation.Get("config.cooking-reaction-tt"),
                getValue: () => ModEntry.Instance.Config.CookingReaction,
                setValue: value => ModEntry.Instance.Config.CookingReaction = value
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.nausea"),
                tooltip: () => translation.Get("config.nausea-tt"),
                getValue: () => ModEntry.Instance.Config.EnableNausea,
                setValue: value => ModEntry.Instance.Config.EnableNausea = value
            );

            configMenu.AddNumberOption(
                mod: modManifest,
                name: () => translation.Get("config.debuff-len"),
                tooltip: () => translation.Get("config.debuff-len-tt"),
                getValue: () => ModEntry.Instance.Config.EatingDebuffLengthSeconds,
                setValue: value => ModEntry.Instance.Config.EatingDebuffLengthSeconds = value
            );

            configMenu.AddNumberOption(
                mod: modManifest,
                name: () => translation.Get("config.debuff-sev"),
                tooltip: () => translation.Get("config.debuff-sev-tt"),
                getValue: () => ModEntry.Instance.Config.DebuffSeverityMultiplier,
                setValue: value => ModEntry.Instance.Config.DebuffSeverityMultiplier = value
            );

            configMenu.AddSectionTitle(
                mod: modManifest,
                text: () => translation.Get("config.random")
            );

            configMenu.AddBoolOption(
                mod: modManifest,
                name: () => translation.Get("config.random-hint"),
                tooltip: () => translation.Get("config.random-hint-tt"),
                getValue: () => ModEntry.Instance.Config.AllergenCountHint,
                setValue: value => ModEntry.Instance.Config.AllergenCountHint = value
            );

            configMenu.AddNumberOption(
                mod: modManifest,
                name: () => translation.Get("config.random-num"),
                tooltip: () => translation.Get("config.random-num-tt"),
                getValue: () => ModEntry.Instance.Config.NumberRandomAllergies,
                setValue: value =>
                {
                    if (value < -1) value = -1;
                    if (value > AllergenManager.ALLERGEN_DATA_ASSET.Count) value = AllergenManager.ALLERGEN_DATA_ASSET.Count;
                    ModEntry.Instance.Config.NumberRandomAllergies = value;
                }
            );

            configMenu.AddParagraph(modManifest, () => translation.Get("config.random-neg-1"));
        }
    }
}
