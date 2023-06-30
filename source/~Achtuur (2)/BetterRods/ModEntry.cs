/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Patches;
using BetterRods.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace BetterRods;

public class ModEntry : Mod
{
    internal static ModEntry Instance;
    internal ModConfig Config;

    internal static bool FarmerHoldsFishingRod()
    {
        if (!Context.IsWorldReady || Game1.player.CurrentTool is null)
            return false;

        Type toolType = Game1.player.CurrentTool.GetType();

        // Make sure tool type is fishing rod
        // This _should_ always be true when fishing menu is active
        return toolType == typeof(StardewValley.Tools.FishingRod);
    }

    /// <summary>
    /// Returns multiplier based on held tool
    /// </summary>
    /// <param name="enabled">Whether multiplier is enabled. If false, returns <c>1f</c></param>
    /// <param name="bambooMultiplier">Multiplier used when holding a bamboo (or training) pole</param>
    /// <param name="fiberglassMultiplier">Multiplier used when holding a fiberglass rod</param>
    /// <param name="iridiumMultiplier">Multiplier used when holding an iridium rod</param>
    /// <returns></returns>
    internal static float GetMultiplier(bool enabled, float bambooMultiplier, float fiberglassMultiplier, float iridiumMultiplier)
    {
        // if no rod, or this setting disabled, do 'nothing' by returning 1
        if (!FarmerHoldsFishingRod() || !enabled)
            return 1f;

        // Compare tool name to check what tier of fishing rod is used
        string toolName = Game1.player.CurrentTool.Name.ToLower();

        if (toolName.Contains("iridium"))
            return iridiumMultiplier;

        else if (toolName.Contains("fiberglass"))
            return fiberglassMultiplier;

        else // Training rod + Bamboo Pole
            return bambooMultiplier;
    }

    internal static float GetGravityMultiplier()
    {
        return GetMultiplier(
            enabled: ModEntry.Instance.Config.enableSpeedMultiplier,
            bambooMultiplier: ModEntry.Instance.Config.bambooSpeedMultiplier,
            fiberglassMultiplier: ModEntry.Instance.Config.fiberglassSpeedMultiplier,
            iridiumMultiplier: ModEntry.Instance.Config.iridiumSpeedMultiplier
        );
    }

    internal static float GetDistanceGainMultiplier()
    {
        return GetMultiplier(
            enabled: ModEntry.Instance.Config.enableDistanceGainMultiplier,
            bambooMultiplier: ModEntry.Instance.Config.bambooDistanceGainMultiplier,
            fiberglassMultiplier: ModEntry.Instance.Config.fiberglassDistanceGainMultiplier,
            iridiumMultiplier: ModEntry.Instance.Config.iridiumDistanceGainMultiplier
        );
    }

    internal static float GetDistanceLossMultiplier()
    {
        return GetMultiplier(
            enabled: ModEntry.Instance.Config.enableDistanceLossMultiplier,
            bambooMultiplier: ModEntry.Instance.Config.bambooDistanceLossMultiplier,
            fiberglassMultiplier: ModEntry.Instance.Config.fiberglassDistanceLossMultiplier,
            iridiumMultiplier: ModEntry.Instance.Config.iridiumDistanceLossMultiplier
        );
    }

    internal static float GetNibbleTimeMultiplier()
    {
        return GetMultiplier(
            enabled: ModEntry.Instance.Config.enableNibbleTimeMultiplier,
            bambooMultiplier: ModEntry.Instance.Config.bambooNibbleTimeMultiplier,
            fiberglassMultiplier: ModEntry.Instance.Config.fiberglassNibbleTimeMultiplier,
            iridiumMultiplier: ModEntry.Instance.Config.iridiumNibbleTimeMultiplier
        );
    }

    public override void Entry(IModHelper helper)
    {

        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new BobberBarPatch(),
            new TimeUntilFishingBitePatch()
        );

        this.Config = this.Helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.Config.createMenu();
    }
}
