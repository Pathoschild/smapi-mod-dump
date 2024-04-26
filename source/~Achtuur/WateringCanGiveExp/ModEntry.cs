/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using WateringCanGiveExp.Patches;

namespace WateringCanGiveExp;

public class ModEntry : Mod
{
    public const int FarmingSkillID = 0;

    internal static ModEntry Instance;
    internal ModConfig Config;
    private PerScreen<float> wateringExpTotal = new PerScreen<float>();

    public override void Entry(IModHelper helper)
    {

        HarmonyPatcher.ApplyPatches(this,
            new CropHarvestPatcher()
        );

        I18n.Init(helper.Translation);
        ModEntry.Instance = this;
        this.Config = this.Helper.ReadConfig<ModConfig>();
        this.wateringExpTotal.Value = 0f;

        AchtuurCore.Events.EventPublisher.FinishedWateringSoil += OnFinishedWateringSoil;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.Config.createMenu();
    }

    private void OnFinishedWateringSoil(object sender, WateringFinishedArgs e)
    {
        // Quit if world is not loaded
        if (!Context.IsWorldReady)
            return;

        // Only add exp if farmer who watered is current player
        if (!e.farmer.IsLocalPlayer)
            return;

        // Get integer part of total watering exp to not 'lose' exp due to rounding
        this.wateringExpTotal.Value += this.Config.ExpforWateringSoil;
        int floored_total = (int)Math.Floor(wateringExpTotal.Value);


        Logger.DebugLog(ModEntry.Instance.Monitor, $"{this.wateringExpTotal.Value}");

        // Add integer part to farmer
        e.farmer.gainExperience(FarmingSkillID, floored_total);

        // Subtract integer part from total as it has already been received
        this.wateringExpTotal.Value -= floored_total;
    }
}
