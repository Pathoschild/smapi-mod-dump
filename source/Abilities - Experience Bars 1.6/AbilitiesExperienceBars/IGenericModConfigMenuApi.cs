/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sandman534/Abilities-Experience-Bars
**
*************************************************/

using System;
using StardewModdingAPI;

namespace AbilitiesExperienceBars
{
    // https://github.com/spacechase0/StardewValleyMods/blob/5b80730bb3f9b830bb8912ff9ae386da63af61c6/GenericModConfigMenu/IGenericModConfigMenuApi.cs#L12
    public interface IGenericModConfigMenuApi
    {
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

    }
}
