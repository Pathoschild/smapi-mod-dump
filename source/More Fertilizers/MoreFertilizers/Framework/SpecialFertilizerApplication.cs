/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles applying special fertilizers.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class SpecialFertilizerApplication
{
    private static readonly CanPlaceHandler PlaceHandler = new();

    /// <summary>
    /// Handles applying a fertilizer from an input button press.
    /// </summary>
    /// <param name="e">Button press event arguments.</param>
    /// <param name="helper">SMAPI's input helper.</param>
    internal static void ApplyFertilizer(ButtonPressedEventArgs e, IInputHelper helper)
    {
        if (!Context.IsWorldReady || !(e.Button.IsUseToolButton() || e.Button.IsActionButton())
            || Game1.player.ActiveObject is not SObject obj || obj.bigCraftable.Value
            || !Utility.withinRadiusOfPlayer(((int)e.Cursor.GrabTile.X * 64) + 32, ((int)e.Cursor.GrabTile.Y * 64) + 32, 3, Game1.player)
            || !PlaceHandler.CanPlaceFertilizer(obj, Game1.currentLocation, e.Cursor.GrabTile))
        {
            return;
        }

        // Handle the special case of tossing the fish food fertilizer.
        if (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID || obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            Vector2 placementtile = (e.Cursor.GrabTile * 64f) + new Vector2(32f, 32f);
            if (obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID && Game1.currentLocation is BuildableGameLocation loc)
            {
                foreach (Building b in loc.buildings)
                {
                    if (b is FishPond fishPond && b.occupiesTile(e.Cursor.GrabTile))
                    {
                        placementtile = fishPond.GetCenterTile() * 64f;
                        break;
                    }
                }
            }
            Game1.player.FaceFarmerTowardsPosition(placementtile);
            Game1.playSound("throwDownITem");

            Vector2 delta = placementtile - Game1.player.Position;
            float gravity = 0.0025f;
            float velocity = -0.08f;
            if (delta.Y < -80)
            {
                // Ensure the initial velocity is sufficiently fast to make it all the way up.
                velocity -= MathF.Sqrt(2 * MathF.Abs(delta.Y + 80) * gravity);
            }
            float time = (MathF.Sqrt(Math.Max((velocity * velocity) + (gravity * (delta.Y + 128) * 2f), 0)) / gravity) - (velocity / gravity);

            Multiplayer mp = MultiplayerHelpers.GetMultiplayer();
            mp.broadcastSprites(
                Game1.currentLocation,
                new TemporaryAnimatedSprite(
                    textureName: Game1.objectSpriteSheetName,
                    sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, 16, 16),
                    position: Game1.player.position - new Vector2(0, 128f),
                    flipped: false,
                    alphaFade: 0f,
                    color: Color.White)
                {
                    scale = Game1.pixelZoom,
                    layerDepth = 1f,
                    totalNumberOfLoops = 1,
                    interval = time,
                    acceleration = new Vector2(0f, gravity),
                    motion = new Vector2(delta.X / time, velocity),
                    timeBasedMotion = true,
                });

            GameLocationUtils.DrawWaterSplash(Game1.currentLocation, placementtile, mp, (int)time);

            DelayedAction.playSoundAfterDelay("waterSlosh", (int)time, Game1.player.currentLocation);
            if (obj.ParentSheetIndex != ModEntry.DomesticatedFishFoodID)
            {
                DelayedAction.functionAfterDelay(
                    () => Game1.currentLocation.waterColor.Value = FedFishWaterColor(),
                    (int)time);
            }

            if (PlaceHandler.TryPlaceFertilizer(obj, Game1.currentLocation, e.Cursor.GrabTile))
            {
                Game1.player.reduceActiveItemByOne();
                helper.Suppress(e.Button);
                return;
            }
        }

        // Handle placing the other fertilizers.
        if (PlaceHandler.TryPlaceFertilizer(obj, Game1.currentLocation, e.Cursor.GrabTile))
        {
            Game1.player.reduceActiveItemByOne();
            helper.Suppress(e.Button);
            return;
        }
    }

    /// <summary>
    /// Gets the color to tint the water for fed fish.
    /// </summary>
    /// <returns>The color.</returns>
    internal static Color FedFishWaterColor()
    {
        Color color = Color.MediumPurple;
        color.A = 155;
        return color;
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(nameof(Utility.playerCanPlaceItemHere))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixPlayerCanPlaceItemHere(GameLocation location, Item item, int x, int y, Farmer f, ref bool __result)
    {
        try
        {
            Vector2 tile = new(MathF.Floor(x / 64f), MathF.Floor(y / 64f));
            if (item is SObject obj && PlaceHandler.CanPlaceFertilizer(obj, location, tile) &&
                Utility.withinRadiusOfPlayer(x, y, 2, f))
            {
                __result = true;
                return false;
            }
            else if (item is SObject fert && ModEntry.SpecialFertilizerIDs.Contains(fert.ParentSheetIndex))
            {
                __result = false;
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Attempt to prefix Utility.playerCanPlaceItemHere has failed:\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}