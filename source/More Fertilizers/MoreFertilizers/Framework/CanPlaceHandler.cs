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
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.Framework;

/// <summary>
/// Class that handles placement of special fertilizers.
/// </summary>
public sealed class CanPlaceHandler : IMoreFertilizersAPI
{
    /// <summary>
    /// ModData string for the organic fertilizer.
    /// </summary>
    public const string Organic = "atravita.MoreFertilizer.Organic";

    /// <summary>
    /// ModData string for the Fruit Tree Fertilizers.
    /// </summary>
    public const string FruitTreeFertilizer = "atravita.MoreFertilizer.FruitTree";

    /// <summary>
    /// ModData string for the Fish Food fertilizers.
    /// </summary>
    public const string FishFood = "atravita.MoreFertilizer.FishFood";

    /// <summary>
    /// ModData string for the Domesticated Fish Food.
    /// </summary>
    public const string DomesticatedFishFood = "atravita.MoreFertilizer.DomesticatedFishFood";

    /// <summary>
    /// ModData string for joja crops.
    /// </summary>
    internal const string Joja = "atravita.MoreFertilizer.Joja";

    /// <inheritdoc />
    public bool CanPlaceFertilizer(SObject obj, GameLocation loc, Vector2 tile)
    {
        if (obj.ParentSheetIndex == -1 || obj.bigCraftable.Value || Utility.isPlacementForbiddenHere(loc) || !Context.IsPlayerFree)
        {
            return false;
        }

        if (loc.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain) && terrain is FruitTree tree
            && (obj.ParentSheetIndex == ModEntry.FruitTreeFertilizerID || obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID))
        {
            return !tree.modData.ContainsKey(FruitTreeFertilizer);
        }

        if (loc.canFishHere() && loc.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") is not null
            && (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID))
        {
            return !loc.modData.ContainsKey(FishFood);
        }

        if(loc is BuildableGameLocation buildableLoc && obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            foreach(Building b in buildableLoc.buildings)
            {
                if (b is FishPond && b.occupiesTile(tile))
                {
                    return !b.modData.ContainsKey(DomesticatedFishFood);
                }
            }
        }
        return false;
    }

    /// <inheritdoc />
    public bool TryPlaceFertilizer(SObject obj, GameLocation loc, Vector2 tile)
    {
        if (!this.CanPlaceFertilizer(obj, loc, tile))
        {
            return false;
        }
        if (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID)
        {
            loc.modData?.SetInt(FishFood, obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID ? 3 : 1);
            if (loc is MineShaft or VolcanoDungeon)
            {
                FishFoodHandler.UnsavedLocHandler.FishFoodLocationMap[Game1.currentLocation.NameOrUniqueName] = obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID ? 3 : 1;
                FishFoodHandler.BroadcastHandler(ModEntry.MultiplayerHelper);
            }
            return true;
        }
        if (loc.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain) && terrain is FruitTree tree &&
            (obj.ParentSheetIndex == ModEntry.FruitTreeFertilizerID || obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID))
        {
            tree.modData?.SetInt(FruitTreeFertilizer, obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID ? 2 : 1);
            return true;
        }

        if (loc is BuildableGameLocation buildableLoc && obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            foreach (Building b in buildableLoc.buildings)
            {
                if (b is FishPond && b.occupiesTile(tile))
                {
                    b.modData?.SetBool(DomesticatedFishFood, true);
                    return true;
                }
            }
        }

        return false;
    }

    /// <inheritdoc />
    public void AnimateFertilizer(StardewValley.Object obj, GameLocation loc, Vector2 tile)
    {
        if (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID || obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            Vector2 placementtile = (tile * 64f) + new Vector2(32f, 32f);
            if (obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID && Game1.currentLocation is BuildableGameLocation buildable)
            {
                foreach (Building b in buildable.buildings)
                {
                    if (b is FishPond pond && b.occupiesTile(tile))
                    {
                        placementtile = pond.GetCenterTile() * 64f;
                        break;
                    }
                }
            }

            Game1.playSound("throwDownITem");

            float deltaY = -140f;
            float gravity = 0.0025f;
            float velocity = -0.08f - MathF.Sqrt(2 * 60f * gravity);
            float time = (MathF.Sqrt((velocity * velocity) - (gravity * deltaY * 2f)) / gravity) - (velocity / gravity);

            Multiplayer mp = MultiplayerHelpers.GetMultiplayer();
            mp.broadcastSprites(
                Game1.currentLocation,
                new TemporaryAnimatedSprite(
                    textureName: Game1.objectSpriteSheetName,
                    sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, 16, 16),
                    position: placementtile + new Vector2(0, deltaY),
                    flipped: false,
                    alphaFade: 0f,
                    color: Color.White)
                {
                    scale = Game1.pixelZoom,
                    layerDepth = 1f,
                    totalNumberOfLoops = 1,
                    interval = time,
                    acceleration = new Vector2(0f, gravity),
                    motion = new Vector2(0f, velocity),
                    timeBasedMotion = true,
                });

            GameLocationUtils.DrawWaterSplash(Game1.currentLocation, placementtile, mp, (int)time);
            DelayedAction.playSoundAfterDelay("waterSlosh", (int)time, Game1.player.currentLocation);
            if (obj.ParentSheetIndex != ModEntry.DomesticatedFishFoodID)
            {
                DelayedAction.functionAfterDelay(
                    () => Game1.currentLocation.waterColor.Value = SpecialFertilizerApplication.FedFishWaterColor(),
                    (int)time);
            }
        }
    }
}