/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace DecidedlyShared.Utilities;

public class Player
{
    public static bool TryAddMailFlag(string flag, Farmer player)
    {
        if (!player.hasOrWillReceiveMail(flag))
        {
            player.mailReceived.Add(flag);

            return true;
        }

        return false;
    }
}
