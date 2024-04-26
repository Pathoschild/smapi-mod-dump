/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/BaitsDoTheirJobs
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace BaitsDoTheirJobs;

internal sealed class ModEntry : Mod
{
    private IModHelper? helper;
    private IGenericModConfigMenuApi? configMenu;
    private ModConfig? config;
    private Farmer? player;
    private int doConsume = -1;
    private int baitCount = -1;
    private BaitType? baitType = null;
    private string targetedFish = string.Empty;
    private BobberBar? bobberBar = null;

    public override void Entry(IModHelper helper)
    {
        this.helper = helper;
        config = helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.Display.MenuChanged += Display_MenuChanged;
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
            getValue: () => config!.MagnetBaitEnabled,
            setValue: value => config!.MagnetBaitEnabled = value,
            name: () => "Magnet bait enabled",
            tooltip: () => "Enable mod on magnet bait (treasure bait)"
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.WildBaitEnabled,
            setValue: value => config!.WildBaitEnabled = value,
            name: () => "Wild bait enabled",
            tooltip: () => "Enable mod on wild bait (double fish bait)"
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.TargetedBaitEnabled,
            setValue: value => config!.TargetedBaitEnabled = value,
            name: () => "Targeted bait enabled",
            tooltip: () => "Enable mod on targeted bait (specific fish bait)"
        );
    }

    private void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (helper == null || config == null || player == null) return;
        if (player.CurrentTool is not FishingRod) return;
        if (e.NewMenu is BobberBar bar) bobberBar = bar;
        else bobberBar = null;
    }

    private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _ = Task.Run(AssignPlayerAsync);
    }

    private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (helper == null || config == null || player == null) return;
        if (player.CurrentTool is not FishingRod rod) return;
        if (rod.isFishing && rod.GetBait() != null && baitCount < 0 && baitType == null)
        {
            Object tempBait = rod.GetBait();
            if (tempBait != null)
            {
                if (tempBait.ItemId.Contains("SpecificBait") && config.TargetedBaitEnabled)
                {
                    baitType = BaitType.Specific;
                    doConsume = -1;
                    targetedFish = tempBait.Name.Trim()[..^4].Trim();
                }
                else if (tempBait.ItemId.Contains("703") && config.MagnetBaitEnabled)
                {
                    baitType = BaitType.Magnet;
                    doConsume = 0;
                    targetedFish = string.Empty;
                }
                else if (tempBait.ItemId.Contains("774") && config.WildBaitEnabled)
                {
                    baitType = BaitType.Wild;
                    doConsume = -1;
                    targetedFish = string.Empty;
                }
                if (baitType != null) baitCount = tempBait.Stack;
            }
            else
            {
                baitType = null;
                doConsume = 1;
                targetedFish = string.Empty;
                baitCount = 0;
            }
        }
        if (rod.isReeling && bobberBar != null && baitCount > 0 && rod.GetBait() != null)
        {
            if (baitType == BaitType.Magnet && doConsume == 0)
            {
                if ((bobberBar.treasure || bobberBar.goldenTreasure) && bobberBar.treasureAppearTimer <= 0)
                {
                    doConsume = 1;
                    rod.GetBait().Stack = baitCount;
                }
                else if (rod.GetBait().Stack == baitCount && baitCount < 5) rod.GetBait().Stack++;
            }
            if (baitType == BaitType.Specific && doConsume == -1)
            {
                if (ItemRegistry.GetData(bobberBar.whichFish).InternalName.Contains(targetedFish)) doConsume = 1;
                else
                {
                    doConsume = 0;
                    if (baitCount < 5) rod.GetBait().Stack++;
                }
            }
            if (bobberBar.distanceFromCatching <= 0) doConsume = 1;
        }
        if (rod.pullingOutOfWater && rod.whichFish != null && baitCount > 0 && doConsume == -1 && rod.GetBait() != null)
        {
            if (rod.fromFishPond)
            {
                doConsume = 1;
                return;
            }
            if (baitType == BaitType.Specific)
            {
                if (rod.whichFish.GetParsedData().DisplayName.Contains(targetedFish)) doConsume = 1;
                else
                {
                    doConsume = 0;
                    if (baitCount < 5) rod.GetBait().Stack++;
                }
                return;
            }
            if (baitType == BaitType.Wild)
            {
                if (rod.numberOfFishCaught < 2)
                {
                    doConsume = 0;
                    if (baitCount < 5) rod.GetBait().Stack++;
                }
                else doConsume = 1;
                return;
            }
            doConsume = 1;
        }
        if (!rod.isFishing && !rod.isReeling && !rod.pullingOutOfWater && !rod.fishCaught && !rod.treasureCaught && (baitCount >= 0 || doConsume >= 0 || baitType != null || !string.IsNullOrEmpty(targetedFish)))
        {
            if (doConsume == 0 && rod.GetBait() != null) rod.GetBait().Stack = baitCount;
            if (baitCount >= 0) baitCount = -1;
            if (doConsume >= 0) doConsume = -1;
            if (baitType != null) baitType = null;
            if (!string.IsNullOrEmpty(targetedFish)) targetedFish = string.Empty;
        }
    }

    private async Task AssignPlayerAsync()
    {
        while (Game1.player == null) await Task.Delay(500);
        player = Game1.player;
    }
}

public enum BaitType
{
    Specific,
    Magnet,
    Wild,
}
