/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.ReflectionManager;
using AtraCore.Utilities;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.Framework;

/// <summary>
/// Class that handles placement of special fertilizers.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
public sealed class CanPlaceHandler : IMoreFertilizersAPI
{
    #region ModdataStrings

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
    public const string Joja = "atravita.MoreFertilizer.Joja";

    /// <summary>
    /// ModData string to track trees fertilized with tree fertilizers.
    /// </summary>
    public const string TreeFertilizer = "atravita.MoreFertilizer.TreeFertilizer";

    /// <summary>
    /// ModData string to track trees fertilized with the tree tapper fertilizer.
    /// </summary>
    public const string TreeTapperFertilizer = "atravita.MoreFertilizer.TreeTapper";

    /// <summary>
    /// ModData string for the Bountiful Bush fertilizer.
    /// </summary>
    public const string BountifulBush = "atravita.MoreFertilizer.BountifulBush";

    /// <summary>
    /// ModData string for the Rapid Bush fertilizer.
    /// </summary>
    public const string RapidBush = "atravita.MoreFertilizer.RapidBush";

    /// <summary>
    /// ModData string marking fertilized mushroom boxen.
    /// </summary>
    public const string MushroomFertilizer = "atravita.MoreFertilizer.MushroomFertilizer";

    /// <summary>
    /// ModData string marking miraculous beverages.
    /// </summary>
    public const string MiraculousBeverages = "atravita.MoreFertilizer.MiraculousBeverages";

    /// <summary>
    /// Marks the prismatic fertilizer.
    /// </summary>
    public const string PrismaticFertilizer = "atravita.MoreFertilizer.Prismatic";
    #endregion

    #region reflection

    /// <summary>
    /// Stardew's Bush::shake.
    /// </summary>
    private static readonly BushShakeDel BushShakeMethod = typeof(Bush)
        .GetCachedMethod("shake", ReflectionCache.FlagTypes.InstanceFlags)
        .CreateDelegate<BushShakeDel>();

    private delegate void BushShakeDel(
        Bush bush,
        Vector2 tileLocation,
        bool doEvenIfStillShaking);

    /// <summary>
    /// Stardew's Tree::shake.
    /// </summary>
    private static readonly TreeShakeDel TreeShakeMethod = typeof(Tree)
        .GetCachedMethod("shake", ReflectionCache.FlagTypes.InstanceFlags)
        .CreateDelegate<TreeShakeDel>();

    private delegate void TreeShakeDel(
        Tree tree,
        Vector2 tileLocation,
        bool doEvenIfStillShaking,
        GameLocation location);
    #endregion

    /// <inheritdoc />
    public bool CanPlaceFertilizer(SObject obj, GameLocation loc, Vector2 tile)
        => this.CanPlaceFertilizer(obj, loc, tile, false);

    /// <inheritdoc />
    public bool CanPlaceFertilizer(SObject obj, GameLocation loc, Vector2 tile, bool alert)
    {
        if (Utility.isPlacementForbiddenHere(loc) || !Context.IsPlayerFree)
        {
            return false;
        }

        Guard.IsNotNull(obj);
        if (obj.ParentSheetIndex == -1 || obj.bigCraftable.Value || obj.GetType() != typeof(SObject) || obj.Category != SObject.fertilizerCategory)
        {
            return false;
        }

        if (loc.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain))
        {
            if (terrain is HoeDirt dirt && dirt.crop is Crop crop && crop.programColored.Value && obj.ParentSheetIndex == ModEntry.PrismaticFertilizerID)
            {
                bool ret = !dirt.modData.ContainsKey(PrismaticFertilizer);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
            else if (terrain is FruitTree fruitTree
                && (obj.ParentSheetIndex == ModEntry.MiraculousBeveragesID
                    || (fruitTree.growthStage.Value != FruitTree.treeStage
                        && (obj.ParentSheetIndex == ModEntry.FruitTreeFertilizerID || obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID))))
            {
                bool ret = !fruitTree.modData.ContainsKey(FruitTreeFertilizer) && !fruitTree.modData.ContainsKey(MiraculousBeverages);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
            else if (terrain is Bush bush && (bush.size.Value == Bush.greenTeaBush || bush.size.Value == Bush.mediumBush) && !bush.townBush.Value
                && ((obj.ParentSheetIndex == ModEntry.RapidBushFertilizerID && bush.size.Value == Bush.greenTeaBush) || obj.ParentSheetIndex == ModEntry.BountifulBushID
                    || (obj.ParentSheetIndex == ModEntry.MiraculousBeveragesID && bush.size.Value == Bush.greenTeaBush)))
            {
                bool ret = !bush.modData.ContainsKey(BountifulBush) && !bush.modData.ContainsKey(RapidBush) && !bush.modData.ContainsKey(MiraculousBeverages);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
            else if (terrain is Tree tree && tree.growthStage.Value >= Tree.treeStage && tree.treeType.Value is not Tree.palmTree or Tree.palmTree2
                && obj.ParentSheetIndex == ModEntry.TreeTapperFertilizerID)
            {
                bool ret = !tree.modData.ContainsKey(TreeFertilizer) && !tree.modData.ContainsKey(TreeTapperFertilizer);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
        }

        if (loc.Objects.TryGetValue(tile, out SObject @object) && @object is IndoorPot pot)
        {
            if (pot.bush?.Value is Bush pottedBush && pottedBush.size?.Value == Bush.greenTeaBush
                    && (obj.ParentSheetIndex == ModEntry.RapidBushFertilizerID || obj.ParentSheetIndex == ModEntry.BountifulBushID || obj.ParentSheetIndex == ModEntry.MiraculousBeveragesID))
            {
                bool ret = !pottedBush.modData.ContainsKey(BountifulBush) && !pottedBush.modData.ContainsKey(RapidBush) && !pottedBush.modData.ContainsKey(MiraculousBeverages);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
            else if (pot.hoeDirt.Value is HoeDirt dirt && dirt.crop is Crop crop && crop.programColored.Value && obj.ParentSheetIndex == ModEntry.PrismaticFertilizerID)
            {
                bool ret = !dirt.modData.ContainsKey(PrismaticFertilizer);
                if (alert && !ret)
                {
                    AlertPlayer();
                }
                return ret;
            }
        }

        if (obj.ParentSheetIndex == ModEntry.BountifulBushID)
        {
            Rectangle pos = new((int)tile.X * 64, (int)tile.Y * 64, 16, 16);
            foreach (LargeTerrainFeature largeterrainfeature in loc.largeTerrainFeatures)
            {
                if (largeterrainfeature is Bush bigBush && !bigBush.townBush.Value && bigBush.getBoundingBox().Intersects(pos))
                {
                    bool ret = !bigBush.modData.ContainsKey(BountifulBush) && !bigBush.modData.ContainsKey(RapidBush) && !bigBush.modData.ContainsKey(MiraculousBeverages);
                    if (alert && !ret)
                    {
                        AlertPlayer();
                    }
                    return ret;
                }
            }
        }

        // fish food.
        if ((obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID)
            && loc.canFishHere() && loc.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") is not null)
        {
            bool ret = !loc.modData.ContainsKey(FishFood);
            if (alert && !ret)
            {
                AlertPlayer();
            }
            return ret;
        }

        if(loc is BuildableGameLocation buildableLoc && obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            foreach(Building b in buildableLoc.buildings)
            {
                if (b is FishPond && b.occupiesTile(tile))
                {
                    bool ret = !b.modData.ContainsKey(DomesticatedFishFood);
                    if (alert && !ret)
                    {
                        AlertPlayer();
                    }
                    return ret;
                }
            }
        }
        return false;
    }

    /// <inheritdoc />
    public bool TryPlaceFertilizer(SObject obj, GameLocation loc, Vector2 tile)
    {
        Guard.IsNotNull(obj);
        if (!this.CanPlaceFertilizer(obj, loc, tile, true))
        {
            return false;
        }
        if (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID)
        {
            loc.modData?.SetInt(FishFood, obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID ? 3 : 1);
            if (loc.IsUnsavedLocation())
            {
                int days = obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID ? 3 : 1;
                FishFoodHandler.UnsavedLocHandler.FishFoodLocationMap[Game1.currentLocation.NameOrUniqueName] = days;
                FishFoodHandler.BroadcastSingle(ModEntry.MultiplayerHelper, Game1.currentLocation.NameOrUniqueName, days);
            }
            return true;
        }
        if (loc.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain))
        {
            if (terrain is FruitTree fruitTree)
            {
                if (obj.ParentSheetIndex == ModEntry.FruitTreeFertilizerID || obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID)
                {
                    fruitTree.modData?.SetInt(FruitTreeFertilizer, obj.ParentSheetIndex == ModEntry.DeluxeFruitTreeFertilizerID ? 2 : 1);
                    fruitTree.shake(fruitTree.currentTileLocation, true, fruitTree.currentLocation);
                    return true;
                }
                if (obj.ParentSheetIndex == ModEntry.MiraculousBeveragesID)
                {
                    fruitTree.shake(fruitTree.currentTileLocation, true, fruitTree.currentLocation);
                    fruitTree.modData?.SetBool(MiraculousBeverages, true);
                    return true;
                }
            }
            if (terrain is Bush bush)
            {
                return ApplyTeaBushFertilizer(obj, bush);
            }
            if (terrain is Tree tree
                && (obj.ParentSheetIndex == ModEntry.TreeTapperFertilizerID))
            {
                TreeShakeMethod(tree, tile, true, loc);
                tree.modData?.SetBool(TreeTapperFertilizer, true);
                return true;
            }

            if (terrain is HoeDirt dirt
                && obj.ParentSheetIndex == ModEntry.PrismaticFertilizerID)
            {
                dirt.modData?.SetBool(PrismaticFertilizer, true);
                return true;
            }
        }

        if (loc.Objects.TryGetValue(tile, out SObject @object) && @object is IndoorPot pot)
        {
            if (pot.hoeDirt.Value is HoeDirt dirt
                && obj.ParentSheetIndex == ModEntry.PrismaticFertilizerID)
            {
                dirt.modData?.SetBool(PrismaticFertilizer, true);
                return true;
            }
            if (pot.bush?.Value is Bush pottedBush && pottedBush.size?.Value == Bush.greenTeaBush)
            {
                return ApplyTeaBushFertilizer(obj, pottedBush);
            }
        }

        if (obj.ParentSheetIndex == ModEntry.BountifulBushID)
        {
            Rectangle pos = new((int)tile.X * 64, (int)tile.Y * 64, 16, 16);
            foreach (LargeTerrainFeature largeterrainfeature in loc.largeTerrainFeatures)
            {
                if (largeterrainfeature is Bush bigBush && bigBush.size.Value == Bush.mediumBush && bigBush.getBoundingBox().Intersects(pos))
                {
                    BushShakeMethod(bigBush, bigBush.currentTileLocation, true);
                    bigBush.modData?.SetBool(BountifulBush, true);
                    return true;
                }
            }
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
    public void AnimateFertilizer(SObject obj, GameLocation loc, Vector2 tile)
    {
        Guard.IsNotNull(obj);
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

            Game1.playSound("throwDownITem"); // sic

            const float deltaY = -140f;
            const float gravity = 0.0025f;

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
                    func: static () => Game1.currentLocation.waterColor.Value = ModEntry.Config.WaterOverlayColor,
                    timer: (int)time);
            }
        }
    }

    private static bool ApplyTeaBushFertilizer(SObject obj, Bush teaBush)
    {
        if (obj.ParentSheetIndex == ModEntry.BountifulBushID)
        {
            BushShakeMethod(teaBush, teaBush.currentTileLocation, true);
            teaBush.modData?.SetBool(BountifulBush, true);
            return true;
        }
        if (obj.ParentSheetIndex == ModEntry.RapidBushFertilizerID)
        {
            BushShakeMethod(teaBush, teaBush.currentTileLocation, true);
            teaBush.modData?.SetBool(RapidBush, true);
            return true;
        }
        if (obj.ParentSheetIndex == ModEntry.MiraculousBeveragesID)
        {
            BushShakeMethod(teaBush, teaBush.currentTileLocation, true);
            teaBush.modData?.SetBool(MiraculousBeverages, true);
            return true;
        }
        return false;
    }

    private static void AlertPlayer()
        => Game1.showRedMessageUsingLoadString(@"Strings\StringsFromCSFiles:TreeFertilizer2");
}