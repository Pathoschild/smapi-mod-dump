/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using AtraShared.Menuing;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;
using StopRugRemoval.Configuration;
using xTile.Dimensions;

namespace StopRugRemoval.HarmonyPatches.Confirmations;

/// <summary>
/// Holds patches about confirming warps.
/// </summary>
[HarmonyPatch]
internal static class ConfirmWarp
{
    /// <summary>
    /// Whether or not the warp has been confirmed.
    /// </summary>
    internal static readonly PerScreen<bool> HaveConfirmed = new(createNewState: () => false);

#pragma warning disable SA1602 // Enumeration items should be documented. Reviewed
    /// <summary>
    /// The location to warp to. IDs are the ParentSheetIndex of the totem.
    /// </summary>
    internal enum WarpLocation
    {
        /// <summary>
        /// No warp location found. Do nothing.
        /// </summary>
        None = -1,

        /// <summary>
        /// Warp to farm.
        /// </summary>
        Farm = 688,

        /// <summary>
        /// Warp to mountain.
        /// </summary>
        Mountain = 689,
        Beach = 690,
        Desert = 261,
        IslandSouth = 886,
    }
#pragma warning restore SA1602 // Enumeration items should be documented

    /// <summary>
    /// Applies the patch to the wand.
    /// </summary>
    /// <param name="harmony">Harmony instance.</param>
    /// <remarks>Seperate so these patches are not applied if player is using Better Return Scepter.</remarks>
    internal static void ApplyWandPatches(Harmony harmony)
    {
        harmony.Patch(
            original: typeof(Wand).InstanceMethodNamed(nameof(Wand.DoFunction)),
            prefix: new HarmonyMethod(typeof(ConfirmWarp), nameof(PrefixWand)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SObject), nameof(SObject.performUseAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool PrefixTotemWarp(SObject __instance, GameLocation location, ref bool __result)
    {
        if (Game1.eventUp || Game1.isFestival() || Game1.fadeToBlack || Game1.player.swimming.Value || Game1.player.onBridge.Value || !ModEntry.Config.Enabled)
        {
            return true;
        }
        if (!Enum.IsDefined((WarpLocation)__instance.ParentSheetIndex) || __instance.bigCraftable.Value)
        { // Not an attempt to warp.
            return true;
        }

        WarpLocation locationEnum = (WarpLocation)__instance.ParentSheetIndex;

        if (Game1.getLocationFromName(locationEnum.ToString()) is not GameLocation loc)
        { // Something went very wrong. I cannot find the location at all....
            ModEntry.ModMonitor.Log($"Failed to find {locationEnum}!", LogLevel.Error);
            return true;
        }

        if (loc.IsBeforeFestivalAtLocation(ModEntry.ModMonitor, alertPlayer: true))
        { // Festival. Can't warp anyways.
            __result = false;
            return false;
        }

        if (!HaveConfirmed.Value
            && (IsLocationConsideredDangerous(location) ? ModEntry.Config.WarpsInDangerousAreas : ModEntry.Config.WarpsInSafeAreas)
                .HasFlag(Context.IsMultiplayer ? ConfirmationEnum.InMultiplayerOnly : ConfirmationEnum.NotInMultiplayer))
        {
            ModEntry.InputHelper.SurpressClickInput();
            List<Response> responses = new()
            {
                new Response("WarpsYes", I18n.Yes()).SetHotKey(Keys.Y),
                new Response("WarpsNo", I18n.No()).SetHotKey(Keys.Escape),
            };

            List<Action?> actions = new()
            {
                () =>
                {
                    HaveConfirmed.Value = true;
                    __instance.performUseAction(location);
                    Game1.player.reduceActiveItemByOne();
                    HaveConfirmed.Value = false;
                },
            };

            __result = false;
            Game1.activeClickableMenu = new DialogueAndAction(I18n.ConfirmWarps(), responses, actions);
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(Building), nameof(Building.doAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixBuildingAction(Building __instance, Vector2 tileLocation, Farmer who, ref bool __result)
    {
        if (Game1.eventUp || Game1.isFestival() || Game1.fadeToBlack || Game1.player.swimming.Value || Game1.player.onBridge.Value || !ModEntry.Config.Enabled)
        {
            return true;
        }
        if (!__instance.occupiesTile(tileLocation) || !__instance.buildingType.Value.EndsWith("Obelisk", StringComparison.OrdinalIgnoreCase) || !who.IsLocalPlayer)
        {
            return true;
        }
        WarpLocation location = __instance.buildingType.Value switch
        {
            "Earth Obelisk" => WarpLocation.Mountain,
            "Water Obelisk" => WarpLocation.Beach,
            "Desert Obelisk" => WarpLocation.Desert,
            "Island Obelisk" => WarpLocation.IslandSouth,
            _ => WarpLocation.None,
        };

        if (location is WarpLocation.None || Game1.getLocationFromName(location.ToString()) is not GameLocation loc)
        { // Something went very wrong. I cannot find the location at all....
            return true;
        }

        if (loc.IsBeforeFestivalAtLocation(ModEntry.ModMonitor, alertPlayer: true))
        { // Festival. Can't warp anyways.
            __result = false;
            return false;
        }

        if (!HaveConfirmed.Value
            && (IsLocationConsideredDangerous(who.currentLocation) ? ModEntry.Config.WarpsInDangerousAreas : ModEntry.Config.WarpsInSafeAreas)
                .HasFlag(Context.IsMultiplayer ? ConfirmationEnum.InMultiplayerOnly : ConfirmationEnum.NotInMultiplayer))
        {
            ModEntry.InputHelper.SurpressClickInput();
            List<Response> responses = new()
            {
                new Response("WarpsYes", I18n.Yes()).SetHotKey(Keys.Y),
                new Response("WarpsNo", I18n.No()).SetHotKey(Keys.Escape),
            };

            List<Action?> actions = new()
            {
                () =>
                {
                    HaveConfirmed.Value = true;
                    __instance.doAction(tileLocation, who);
                    HaveConfirmed.Value = false;
                },
            };

            __result = false;
            Game1.activeClickableMenu = new DialogueAndAction(I18n.ConfirmWarps(), responses, actions);
            return false;
        }
        return true;
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixWand(Wand __instance, GameLocation location, int x, int y, int power, Farmer who)
    {
        if (!who.IsLocalPlayer)
        {
            return true;
        }
        if (Game1.eventUp || Game1.isFestival() || Game1.fadeToBlack || Game1.player.swimming.Value || Game1.player.onBridge.Value || !ModEntry.Config.Enabled)
        {
            return true;
        }
        if (!HaveConfirmed.Value
             && (IsLocationConsideredDangerous(location) ? ModEntry.Config.ReturnScepterInDangerousAreas : ModEntry.Config.ReturnScepterInSafeAreas)
                 .HasFlag(Context.IsMultiplayer ? ConfirmationEnum.InMultiplayerOnly : ConfirmationEnum.NotInMultiplayer))
        {
            ModEntry.InputHelper.SurpressClickInput();
            List<Response> responses = new()
            {
                new Response("WarpsYes", I18n.Yes()).SetHotKey(Keys.Y),
                new Response("WarpsNo", I18n.No()).SetHotKey(Keys.Escape),
            };

            List<Action?> actions = new()
            {
                () =>
                {
                    HaveConfirmed.Value = true;
                    __instance.DoFunction(location, x, y, power, who);
                    HaveConfirmed.Value = false;
                },
            };
            Game1.activeClickableMenu = new DialogueAndAction(I18n.ConfirmWarps(), responses, actions);
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(IslandWest), nameof(IslandWest.performAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixIslandWest(IslandWest __instance, string action, Farmer who, Location tileLocation)
    {
        if (Game1.eventUp || Game1.isFestival() || Game1.fadeToBlack || Game1.player.swimming.Value || Game1.player.onBridge.Value || !ModEntry.Config.Enabled)
        {
            return true;
        }
        if (action == "FarmObelisk" && !HaveConfirmed.Value
            && (IsLocationConsideredDangerous(__instance) ? ModEntry.Config.WarpsInDangerousAreas : ModEntry.Config.WarpsInSafeAreas)
                 .HasFlag(Context.IsMultiplayer ? ConfirmationEnum.InMultiplayerOnly : ConfirmationEnum.NotInMultiplayer))
        {
            List<Response> responses = new()
            {
                new Response("WarpsYes", I18n.Yes()).SetHotKey(Keys.Y),
                new Response("WarpsNo", I18n.No()).SetHotKey(Keys.Escape),
            };

            List<Action?> actions = new()
            {
                () =>
                {
                    HaveConfirmed.Value = true;
                    __instance.performAction(action, who, tileLocation);
                    HaveConfirmed.Value = false;
                },
            };
            Game1.activeClickableMenu = new DialogueAndAction(I18n.ConfirmWarps(), responses, actions);
            return false;
        }
        return true;
    }

    private static bool IsLocationConsideredDangerous(GameLocation location)
        => ModEntry.Config.SafeLocationMap.TryGetValue(location.NameOrUniqueName, out IsSafeLocationEnum val)
            ? (val == IsSafeLocationEnum.Dangerous) || (val == IsSafeLocationEnum.Dynamic && location.IsDangerousLocation())
            : location.IsDangerousLocation();
}