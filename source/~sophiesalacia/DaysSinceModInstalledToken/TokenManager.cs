/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace DaysSinceModInstalledToken;

internal class TokenManager
{
    internal static void RegisterToken()
    {
        Globals.ContentPatcherApi?.RegisterToken(Globals.Manifest!, "DaysSinceModInstalled", new DaysSinceModInstalledToken());
    }

}
