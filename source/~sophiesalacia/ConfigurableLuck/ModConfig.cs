/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace ConfigurableLuck;

internal class ModConfig
{
    public bool Enabled = true;
    public double LuckValue = 0.0f;

    internal void ApplyConfigChangesToGame()
    {
        if (!Context.IsWorldReady)
            return;

        LuckManager.SetLuck(Game1.player, LuckValue);
        Log.Trace($"Updated luck value to {LuckValue}");
    }
}
