/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal static class SpectatorHelper
{
    private static ModConfig config = null!;
    
    public static void Init(ModConfig _config)
    {
        config = _config;
    }
    
    // 旁观地点
    public static void SpectateLocation(string name)
    {
        var location = Game1.getLocationFromName(name);

        if (location is null)
        {
            Log.Info(I18n.UI_SpectateLocation_Fail(name));
            return;
        }

        Game1.activeClickableMenu = new SpectatorMenu(config, location);
        Log.Info(I18n.UI_SpectateLocation_Success(location.DisplayName));
    }

    // 旁观玩家
    public static void SpectateFarmer(string name)
    {
        if (!Context.HasRemotePlayers)
        {
            Log.Info(I18n.UI_NoPlayerOnline());
            return;
        }

        var farmer = Game1.otherFarmers.FirstOrDefault(x => x.Value.Name == name).Value;

        if (farmer is null)
        {
            Log.Info(I18n.UI_SpectatePlayer_Fail(name));
            return;
        }

        Game1.activeClickableMenu = new SpectatorMenu(config, farmer.currentLocation, farmer, true);
        Log.Info(I18n.UI_SpectatePlayer_Success(farmer.Name));
    }
}