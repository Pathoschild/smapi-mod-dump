/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using StardewValley;
using SuperMode;

#endregion using directives

public static class Localization
{
    /// <summary>Get the localized pronoun for the currently registered Super Mode buff.</summary>
    public static string GetBuffPronoun()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
            case LocalizedContentManager.LanguageCode.es:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite.female");

            case LocalizedContentManager.LanguageCode.fr:
            case LocalizedContentManager.LanguageCode.pt:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite" +
                                                          (ModEntry.PlayerState.Value.SuperMode is PoacherColdBlood
                                                              ? ".male"
                                                              : ".female"));

            default:
                return string.Empty;
        }
    }
}