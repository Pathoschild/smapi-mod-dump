/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewWebApi.Server;

namespace StardewWebApi.Game.World;

public static class WorldActions
{
    public static ActionResult PlaySound(string name, int? pitch = null)
    {
        try
        {
            var result = Game1.playSound(name, pitch);
            return new ActionResult(result);
        }
        catch (Exception ex)
        {
            SMAPIWrapper.LogError($"Error playing sound {name}: {ex.Message}");
            return new ActionResult(false, ex.Message);
        }
    }
}