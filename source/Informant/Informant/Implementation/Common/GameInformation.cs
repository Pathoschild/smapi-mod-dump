/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

namespace Slothsoft.Informant.Implementation.Common; 

internal static class GameInformation {
    
    internal static string GetObjectDisplayName(int parentSheetIndex) {
        Game1.objectInformation.TryGetValue(parentSheetIndex, out var str);
        return string.IsNullOrEmpty(str) ? "???" : str.Split('/')[4];
    }
}