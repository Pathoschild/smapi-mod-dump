/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/HorseFluteAutoMount
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace HorseFluteAutoMount;

internal sealed class ModEntry : Mod
{
    private IModHelper? helper;
    private IGenericModConfigMenuApi? configMenu;
    private ModConfig? config;
    private Farmer? player;
    private bool isTracking;

    public override void Entry(IModHelper helper)
    {
        this.helper = helper;
        config = helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
    }

    private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu == null) return;
        configMenu.Register(
            mod: ModManifest,
            reset: () => config = new ModConfig(),
            save: () => helper!.WriteConfig(config!)
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.EnableMod,
            setValue: value => config!.EnableMod = value,
            name: () => "Enable mod"
        );
    }

    private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _ = Task.Run(AssignPlayerAsync);
    }

    private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (helper == null || config == null || player == null) return;
        if (!config.EnableMod) return;
        if (CanUseHorseFlute(player) && player.ActiveObject != null && player.ActiveObject.QualifiedItemId == "(O)911" && player.freezePause > 0 && player.mount == null && !isTracking) isTracking = true;
        if (isTracking && player.freezePause <= 0 && player.CanMove)
        {
            isTracking = false;
            _ = DelayMounting(player, Utility.findHorseForPlayer(player.UniqueMultiplayerID));
        }
    }

    private async Task AssignPlayerAsync()
    {
        while (Game1.player == null) await Task.Delay(500);
        player = Game1.player;
    }

    // Wait for the horse to appear next to player (currently hard coded 50 milliseconds, please suggest a better method)
    // Pet the horse before mounting if possible
    private static async Task DelayMounting(Farmer player, Horse horse)
    {
        int retry = 0;
        await Task.Delay(50);
        while (retry < 10 && !horse.mounting.Value)
        {
            retry++;
            if (player.CanMove) horse.checkAction(player, Game1.currentLocation);
            await Task.Delay(50);
        }
    }

    // Based on Farmer.IsBusyDoingSomething(), modified to be used for horse flute
    private static bool CanUseHorseFlute(Farmer player)
    {
        if (Game1.eventUp) return false;
        if (Game1.isFestival()) return false;
        if (Game1.fadeToBlack) return false;
        if (Game1.fadeIn) return false;
        if (Game1.currentMinigame != null) return false;
        if (Game1.activeClickableMenu != null) return false;
        if (Game1.isWarping) return false;
        if (Game1.killScreen) return false;
        if (player.UsingTool) return false;
        if (player.usingSlingshot) return false;
        if (player.temporarilyInvincible) return false;
        return true;
    }
}
