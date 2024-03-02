/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Stardew.NameMapping
{
    public interface INameMapper
    {
        string GetEnglishName(string internalName);
        string GetInternalName(string englishName);
    }
}