/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;

namespace DecidedlyShared.Utilities;

public class Sound
{
    public static bool TryPlaySound(string soundCue)
    {
        try
        {
            Game1.soundBank.GetCue(soundCue);
        }
        catch (Exception e)
        {

            return false;
        }

        Game1.playSound(soundCue);
        return true;
    }
}
