/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using System.Globalization;

namespace SpecialOrdersExtended;

internal class Utilities
{
    public static List<string> ContextSort(IEnumerable<string> enumerable)
    {
        LocalizedContentManager contextManager = Game1.content;
        string langcode = contextManager.LanguageCodeString(contextManager.GetCurrentLanguage());
        List<string> outputlist = enumerable.ToList();
        outputlist.Sort(StringComparer.Create(new CultureInfo(langcode), true));
        return outputlist;
    }
}
