/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/BaitsDoTheirJobs
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Constants;
using StardewValley.GameData.Locations;
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
    private readonly PseudoBobber pseudoBobber = new();
    private bool doubleFishFlag = false;
    private Item? replacingFish = null;

    public override void Entry(IModHelper helper)
    {
        this.helper = helper;
        config = helper.ReadConfig<ModConfig>();
        helper.Events.Display.Rendered += Display_Rendered;
        helper.Events.Display.MenuChanged += Display_MenuChanged;
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
    }

    private void Display_Rendered(object? sender, RenderedEventArgs e)
    {
        if (helper == null || config == null || player == null) return;
        if (bobberBar == null || replacingFish == null) return;
        int num = (bobberBar.xPositionOnScreen > Game1.viewport.Width * 0.75f) ? (bobberBar.xPositionOnScreen - 80) : (bobberBar.xPositionOnScreen + 216);
        bool flag = num < bobberBar.xPositionOnScreen;
        e.SpriteBatch.Draw(Game1.mouseCursors_1_6, new Vector2(num - 12, bobberBar.yPositionOnScreen + 40) + bobberBar.everythingShake, new Rectangle(227, 6, 29, 24), Color.White, 0f, new Vector2(10f, 10f), 4f, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.88f);
        replacingFish.drawInMenu(e.SpriteBatch, new Vector2(num, bobberBar.yPositionOnScreen) + new Vector2(flag ? (-8) : (-4), 4f) * 4f + bobberBar.everythingShake, 1f);
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
            getValue: () => config!.ForceTreasure,
            setValue: value => config!.ForceTreasure = value,
            name: () => "Force treasure",
            tooltip: () => "Force treasure to appear when equipping magnet (excluding fishing during festival)"
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
            getValue: () => config!.ForceDoubleFish,
            setValue: value => config!.ForceDoubleFish = value,
            name: () => "Force double fish",
            tooltip: () => "Force wild bait to catch double fish (excluding legendary fish)"
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.TargetedBaitEnabled,
            setValue: value => config!.TargetedBaitEnabled = value,
            name: () => "Targeted bait enabled",
            tooltip: () => "Enable mod on targeted bait (specific fish bait)"
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.ForceTargetedFish,
            setValue: value => config!.ForceTargetedFish = value,
            name: () => "Force targeted fish",
            tooltip: () => "Force targeted bait to catch targeted fish if possible"
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
            FishingAddOn(rod, config);
        if (rod.isReeling && bobberBar != null && baitCount > 0 && rod.GetBait() != null)
            ReelingAddOn(rod, bobberBar, config);
        if (rod.pullingOutOfWater && rod.whichFish != null && baitCount > 0 && doConsume == -1 && rod.GetBait() != null)
            PullingAddOn(rod, config);
        if (!rod.isFishing && !rod.isReeling && !rod.pullingOutOfWater && !rod.fishCaught && !rod.treasureCaught && (baitCount >= 0 || doConsume >= 0 || baitType != null || !string.IsNullOrEmpty(targetedFish) || doubleFishFlag || !pseudoBobber.IsInitialized() || replacingFish != null))
            Finalize(rod);
        if (doubleFishFlag) rod.numberOfFishCaught = 2;
    }

    private async Task AssignPlayerAsync()
    {
        while (Game1.player == null) await Task.Delay(500);
        player = Game1.player;
    }

    private static async Task DelayPullingFish(FishingRod rod, PseudoBobber pseudoBobber)
    {
        await Task.Delay(100);
        rod.pullFishFromWater(pseudoBobber.whichFish, pseudoBobber.fishSize, pseudoBobber.fishQuality, pseudoBobber.difficulty, pseudoBobber.isTreasure, pseudoBobber.isPerfect, false, null, false, 1);
    }

    private void FishingAddOn(FishingRod rod, ModConfig config)
    {
        Object tempBait = rod.GetBait();
        if (tempBait != null)
        {
            if (tempBait.QualifiedItemId.Contains("(O)SpecificBait", StringComparison.OrdinalIgnoreCase) && config.TargetedBaitEnabled)
            {
                baitType = BaitType.Specific;
                doConsume = -1;
                targetedFish = "(O)" + rod.GetBait().preservedParentSheetIndex.Value;
            }
            else if (tempBait.QualifiedItemId.Contains("(O)703", StringComparison.OrdinalIgnoreCase) && config.MagnetBaitEnabled)
            {
                baitType = BaitType.Magnet;
                doConsume = 0;
                targetedFish = string.Empty;
            }
            else if (tempBait.QualifiedItemId.Contains("(O)774", StringComparison.OrdinalIgnoreCase) && config.WildBaitEnabled)
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

    private void ReelingAddOn(FishingRod rod, BobberBar bobberBar, ModConfig config)
    {
        if (baitType == BaitType.Magnet && doConsume == 0)
        {
            if (config.ForceTreasure && !Game1.isFestival() && !bobberBar.treasure && !bobberBar.goldenTreasure)
            {
                bool treasure = true;
                bool goldenTreasure = Game1.player.stats.Get(StatKeys.Mastery(1)) != 0 && Game1.random.NextDouble() < 0.25 + Game1.player.team.AverageDailyLuck();
                int timer = 1000;
                bobberBar.treasure = treasure;
                bobberBar.goldenTreasure = goldenTreasure;
                bobberBar.treasureAppearTimer = timer;
            }
            if ((bobberBar.treasure || bobberBar.goldenTreasure) && bobberBar.treasureAppearTimer <= 0)
            {
                doConsume = 1;
                rod.GetBait().Stack = baitCount;
            }
            else if (rod.GetBait().Stack == baitCount && baitCount < 5) rod.GetBait().Stack++;
        }
        if (baitType == BaitType.Specific && doConsume == -1)
        {
            if (config.ForceTargetedFish && ItemRegistry.GetData(bobberBar.whichFish).QualifiedItemId != targetedFish)
            {
                List<string> availableFish = GetAvailableFish(rod);
                if (availableFish.Contains(targetedFish)) ReplaceTargetedFish(rod, bobberBar, targetedFish);
            }
            if (ItemRegistry.GetData(bobberBar.whichFish).QualifiedItemId == targetedFish) doConsume = 1;
            else
            {
                doConsume = 0;
                if (baitCount < 5) rod.GetBait().Stack++;
            }
        }
        if (bobberBar.distanceFromCatching <= 0) doConsume = 1;
        if (config.ForceDoubleFish) pseudoBobber.Update(bobberBar);
    }

    private void PullingAddOn(FishingRod rod, ModConfig config)
    {
        if (rod.fromFishPond)
        {
            doConsume = 1;
            return;
        }
        if (baitType == BaitType.Specific)
        {
            if (rod.whichFish.QualifiedItemId == targetedFish) doConsume = 1;
            else
            {
                doConsume = 0;
                if (baitCount < 5) rod.GetBait().Stack++;
            }
            return;
        }
        if (baitType == BaitType.Wild)
        {
            if (config.ForceDoubleFish)
            {
                if (rod.whichFish.GetParsedData().Category == Object.FishCategory && !rod.bossFish)
                {
                    if (rod.numberOfFishCaught < 2)
                    {
                        doubleFishFlag = true;
                        _ = Task.Run(() => DelayPullingFish(rod, pseudoBobber));
                    }
                    doConsume = 1;
                }
                else
                {
                    doConsume = 0;
                    if (baitCount < 5) rod.GetBait().Stack++;
                }
            }
            else
            {
                if (rod.numberOfFishCaught < 2)
                {
                    doConsume = 0;
                    if (baitCount < 5) rod.GetBait().Stack++;
                }
                else doConsume = 1;
            }
            return;
        }
        doConsume = 1;
    }

    private void Finalize(FishingRod rod)
    {
        if (doConsume == 0 && rod.GetBait() != null) rod.GetBait().Stack = baitCount;
        if (baitCount >= 0) baitCount = -1;
        if (doConsume >= 0) doConsume = -1;
        if (baitType != null) baitType = null;
        if (!string.IsNullOrEmpty(targetedFish)) targetedFish = string.Empty;
        if (doubleFishFlag) doubleFishFlag = false;
        if (!pseudoBobber.IsInitialized()) pseudoBobber.Clear();
        if (replacingFish != null) replacingFish = null;
    }

    private List<string> GetAvailableFish(FishingRod rod)
    {
        // I am too lazy to comment and explain this
        // It basically check for a bunch of condition to see if a fish can be fished at the player position
        // The field, method and variable names are kinda self explained
        List<string> availableFishId = new();
        List<SpawnFishData> availableFish = player!.currentLocation.GetData().Fish;
        foreach (SpawnFishData fish in availableFish)
        {
            bool canAdd = true;
            if (string.IsNullOrWhiteSpace(fish.ItemId)) canAdd = false;
            if (canAdd && !string.IsNullOrWhiteSpace(fish.Condition)) canAdd = GameStateQuery.CheckConditions(fish.Condition);
            if (canAdd && fish.Season.HasValue) canAdd = fish.Season == Game1.season;
            if (canAdd && fish.PlayerPosition.HasValue) canAdd = fish.PlayerPosition.GetValueOrDefault().Contains(player!.TilePoint.X, player!.TilePoint.Y);
            if (canAdd && fish.BobberPosition.HasValue) canAdd = fish.PlayerPosition.GetValueOrDefault().Contains((int)rod.bobber.Value.X, (int)rod.bobber.Value.Y);
            if (canAdd && fish.CatchLimit >= 0)
            {
                if (player!.fishCaught.TryGetValue(fish.ItemId, out int[]? fishCaught)) canAdd = fishCaught[0] < fish.CatchLimit;
                else canAdd = false;
            }
            if (canAdd) canAdd = player!.FishingLevel >= fish.MinFishingLevel;
            if (canAdd) canAdd = rod.clearWaterDistance >= fish.MinDistanceFromShore;
            if (canAdd && fish.MaxDistanceFromShore > -1) canAdd = rod.clearWaterDistance <= fish.MaxDistanceFromShore;
            if (canAdd)
            {
                Dictionary<string, string> allFishData = DataLoader.Fish(Game1.content);
                string unqualifiedId = ItemRegistry.GetData(fish.ItemId).ItemId;
                if (!allFishData.TryGetValue(unqualifiedId, out string? rawData)) canAdd = false;
                else
                {
                    string[] splitBySlash = rawData.Split('/');
                    if (!ArgUtility.TryGet(splitBySlash, 5, out string? timeSpan, out _)) canAdd = false;
                    else
                    {
                        string[] splitBySpace = ArgUtility.SplitBySpace(timeSpan);
                        bool validTime = false;
                        for (int i = 0; i < splitBySpace.Length; i += 2)
                        {
                            if (!ArgUtility.TryGetInt(splitBySpace, i, out int startTime, out _) || !ArgUtility.TryGetInt(splitBySpace, i + 1, out int endTime, out _))
                            {
                                validTime = false;
                                break;
                            }
                            else
                            {
                                if (Game1.timeOfDay >= startTime && Game1.timeOfDay < endTime)
                                {
                                    validTime = true;
                                    break;
                                }
                            }
                        }
                        canAdd = validTime;
                    }
                }
            }
            if (canAdd && !availableFishId.Contains(fish.ItemId)) availableFishId.Add(fish.ItemId);
        }
        return availableFishId;
    }

    private void ReplaceTargetedFish(FishingRod rod, BobberBar bar, string fishQualifiedId)
    {
        Dictionary<string, string> fishDictionary = DataLoader.Fish(Game1.content);
        Item targetedFish = ItemRegistry.Create(fishQualifiedId);
        bool haveBlessing = false;
        bool haveSonar = false;
        if (fishDictionary.TryGetValue(bar.whichFish, out string? oldFishData) && bar.difficulty < Convert.ToInt32(oldFishData.Split('/')[1])) haveBlessing = true;
        if (targetedFish.TryGetTempData("IsBossFish", out bool bossFish)) bar.bossFish = bossFish;
        else bar.bossFish = false;
        bar.whichFish = targetedFish.ItemId;
        if (fishDictionary.TryGetValue(bar.whichFish, out string? newFishData))
        {
            string[] splitBySlash = newFishData.Split('/');
            bar.difficulty = Convert.ToInt32(splitBySlash[1]);
            switch (splitBySlash[2].ToLower())
            {
                case "mixed":
                    bar.motionType = 0;
                    break;
                case "dart":
                    bar.motionType = 1;
                    break;
                case "smooth":
                    bar.motionType = 2;
                    break;
                case "floater":
                    bar.motionType = 4;
                    break;
                case "sinker":
                    bar.motionType = 3;
                    break;
            }
            bar.minFishSize = Convert.ToInt32(splitBySlash[3]);
            bar.maxFishSize = Convert.ToInt32(splitBySlash[4]);
            float fishSize = 1f;
            fishSize *= rod.clearWaterDistance / 5f;
            int sizeModifier = 1 + player!.FishingLevel / 2;
            fishSize *= Game1.random.Next(sizeModifier, Math.Max(6, sizeModifier)) / 5f;
            if (rod.favBait) fishSize *= 1.2f;
            fishSize *= 1f + Game1.random.Next(-10, 11) / 100f;
            fishSize = Math.Max(0f, Math.Min(1f, fishSize));
            bar.fishSize = (int)(bar.minFishSize + (bar.maxFishSize - bar.minFishSize) * fishSize);
            bar.fishSize++;
            bar.perfect = true;
            bar.fishQuality = ((!(bar.fishSize < 0.33)) ? ((bar.fishSize < 0.66) ? 1 : 2) : 0);
            bar.fishSizeReductionTimer = 800;
            foreach (Object tackle in rod.GetTackle())
            {
                if (tackle.QualifiedItemId.Contains("(O)877", StringComparison.OrdinalIgnoreCase))
                {
                    bar.fishQuality++;
                    if (bar.fishQuality > 2) bar.fishQuality = 4;
                }
                if (tackle.QualifiedItemId.Contains("(O)SonarBobber", StringComparison.OrdinalIgnoreCase)) haveSonar = true;
            }
            if (haveBlessing)
            {
                if (bar.difficulty > 20f)
                {
                    if (bar.bossFish) bar.difficulty *= 0.75f;
                    else bar.difficulty /= 2f;
                }
                bar.distanceFromCatchPenaltyModifier = 0.5f;
            }
            if (haveSonar) replacingFish = targetedFish;
        }
    }
}
