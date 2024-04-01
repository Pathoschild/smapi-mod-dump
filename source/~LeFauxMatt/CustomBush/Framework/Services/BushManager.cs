/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush.Framework.Services;

using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.CustomBush.Framework.Models;
using StardewValley.Extensions;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

/// <summary>Responsible for handling tea logic.</summary>
internal sealed class BushManager : BaseService
{
    private static BushManager instance = null!;
    private readonly MethodInfo checkItemPlantRules;
    private readonly Lazy<Dictionary<string, BushModel>> data;

    private readonly IGameContentHelper gameContentHelper;
    private readonly string modDataId;
    private readonly string modDataItem;
    private readonly string modDataQuality;
    private readonly string modDataStack;

    /// <summary>Initializes a new instance of the <see cref="BushManager" /> class.</summary>
    /// <param name="gameContentHelper">Dependency used for loading game assets.</param>
    /// <param name="getBushModels">Function which returns the bush model data.</param>
    /// <param name="harmony">Dependency used to patch external code.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    public BushManager(
        IGameContentHelper gameContentHelper,
        Func<Dictionary<string, BushModel>> getBushModels,
        Harmony harmony,
        ILog log,
        IManifest manifest)
        : base(log, manifest)
    {
        BushManager.instance = this;
        this.modDataId = this.ModId + "/Id";
        this.modDataItem = this.ModId + "/ShakeOff";
        this.modDataQuality = this.ModId + "/Quality";
        this.modDataStack = this.ModId + "/Stack";
        this.gameContentHelper = gameContentHelper;
        this.data = new Lazy<Dictionary<string, BushModel>>(getBushModels);
        this.checkItemPlantRules =
            typeof(GameLocation).GetMethod("CheckItemPlantRules", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MethodAccessException("Unable to access CheckItemPlantRules");

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.draw), [typeof(SpriteBatch)]),
            new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_draw_prefix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.GetShakeOffItem)),
            postfix: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_GetShakeOffItem_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.inBloom)),
            postfix: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_inBloom_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.performToolAction)),
            transpiler: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_performToolAction_transpiler)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.setUpSourceRect)),
            postfix: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_setUpSourceRect_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Bush), nameof(Bush.shake)),
            transpiler: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Bush_shake_transpiler)));

        harmony.Patch(
            typeof(GameLocation).GetMethod("CheckItemPlantRules", BindingFlags.Public | BindingFlags.Instance),
            postfix: new HarmonyMethod(
                typeof(BushManager),
                nameof(BushManager.GameLocation_CheckItemPlantRules_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(HoeDirt), nameof(HoeDirt.canPlantThisSeedHere)),
            postfix: new HarmonyMethod(typeof(BushManager), nameof(BushManager.HoeDirt_canPlantThisSeedHere_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(IndoorPot), nameof(IndoorPot.performObjectDropInAction)),
            postfix: new HarmonyMethod(
                typeof(BushManager),
                nameof(BushManager.IndoorPot_performObjectDropInAction_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsTeaSapling)),
            postfix: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Object_IsTeaSapling_postfix)));

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.placementAction)),
            transpiler: new HarmonyMethod(typeof(BushManager), nameof(BushManager.Object_placementAction_transpiler)));
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    private static Bush AddModData(Bush bush, SObject obj)
    {
        if (!BushManager.instance.data.Value.ContainsKey(obj.QualifiedItemId))
        {
            return bush;
        }

        bush.modData[BushManager.instance.modDataId] = obj.QualifiedItemId;
        bush.setUpSourceRect();
        return bush;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static bool Bush_draw_prefix(
        Bush __instance,
        SpriteBatch spriteBatch,
        float ___shakeRotation,
        NetRectangle ___sourceRect,
        float ___yDrawOffset)
    {
        if (!__instance.modData.TryGetValue(BushManager.instance.modDataId, out var id)
            || !BushManager.instance.data.Value.TryGetValue(id, out var bushModel))
        {
            return true;
        }

        var x = (__instance.Tile.X * 64) + 32;
        var y = (__instance.Tile.Y * 64) + 64 + ___yDrawOffset;
        if (__instance.drawShadow.Value)
        {
            spriteBatch.Draw(
                Game1.shadowTexture,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x, y - 4)),
                Game1.shadowTexture.Bounds,
                Color.White,
                0,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                4,
                SpriteEffects.None,
                1E-06f);
        }

        var path = !__instance.IsSheltered()
            ? bushModel.Texture
            : !string.IsNullOrWhiteSpace(bushModel.IndoorTexture)
                ? bushModel.IndoorTexture
                : bushModel.Texture;

        var texture = BushManager.instance.gameContentHelper.Load<Texture2D>(path);

        spriteBatch.Draw(
            texture,
            Game1.GlobalToLocal(Game1.viewport, new Vector2(x, y)),
            ___sourceRect.Value,
            Color.White,
            ___shakeRotation,
            new Vector2(8, 32),
            4,
            __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            ((__instance.getBoundingBox().Center.Y + 48) / 10000f) - (__instance.Tile.X / 1000000f));

        return false;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void Bush_GetShakeOffItem_postfix(Bush __instance, ref string __result)
    {
        if (__instance.modData.TryGetValue(BushManager.instance.modDataItem, out var itemId)
            && !string.IsNullOrWhiteSpace(itemId))
        {
            __result = itemId;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void Bush_inBloom_postfix(Bush __instance, ref bool __result)
    {
        if (__instance.modData.TryGetValue(BushManager.instance.modDataItem, out var itemId)
            && !string.IsNullOrWhiteSpace(itemId))
        {
            __result = true;
            return;
        }

        if (!__instance.modData.TryGetValue(BushManager.instance.modDataId, out var id)
            || !BushManager.instance.data.Value.TryGetValue(id, out var bushModel))
        {
            return;
        }

        var season = __instance.Location.GetSeason();
        var dayOfMonth = Game1.dayOfMonth;
        var age = __instance.getAge();

        // Fails basic conditions
        if (age < bushModel.AgeToProduce || dayOfMonth < bushModel.DayToBeginProducing)
        {
            BushManager.instance.Log.Trace(
                "{0} will not produce. Age: {1} < {2} , Day: {3} < {4}",
                id,
                age.ToString(CultureInfo.InvariantCulture),
                bushModel.AgeToProduce.ToString(CultureInfo.InvariantCulture),
                dayOfMonth.ToString(CultureInfo.InvariantCulture),
                bushModel.DayToBeginProducing.ToString(CultureInfo.InvariantCulture));

            __result = false;
            return;
        }

        BushManager.instance.Log.Trace(
            "{0} passed basic conditions. Age: {1} >= {2} , Day: {3} >= {4}",
            id,
            age.ToString(CultureInfo.InvariantCulture),
            bushModel.AgeToProduce.ToString(CultureInfo.InvariantCulture),
            dayOfMonth.ToString(CultureInfo.InvariantCulture),
            bushModel.DayToBeginProducing.ToString(CultureInfo.InvariantCulture));

        // Fails default season conditions
        if (!bushModel.Seasons.Any() && season == Season.Winter && !__instance.IsSheltered())
        {
            BushManager.instance.Log.Trace(
                "{0} will not produce. Season: {1} and plant is outdoors.",
                id,
                season.ToString());

            __result = false;
            return;
        }

        if (!bushModel.Seasons.Any())
        {
            BushManager.instance.Log.Trace(
                "{0} passed default season condition. Season: {1} or plant is indoors.",
                id,
                season.ToString());
        }

        // Fails custom season conditions
        if (bushModel.Seasons.Any() && !bushModel.Seasons.Contains(season) && !__instance.IsSheltered())
        {
            BushManager.instance.Log.Trace(
                "{0} will not produce. Season: {1} not in {2} and plant is outdoors.",
                id,
                season.ToString(),
                string.Join(',', bushModel.Seasons));

            __result = false;
            return;
        }

        if (bushModel.Seasons.Any())
        {
            BushManager.instance.Log.Trace(
                "{0} passed custom season conditions. Season: {1} in {2} or plant is indoors.",
                id,
                season.ToString(),
                string.Join(',', bushModel.Seasons));
        }

        // Try to produce item
        BushManager.instance.Log.Trace("{0} attempting to produce random item.", id);
        if (!BushManager.instance.TryToProduceRandomItem(__instance, bushModel, out var item))
        {
            BushManager.instance.Log.Trace("{0} will not produce. No item was produced.", id);
            __result = false;
            return;
        }

        BushManager.instance.Log.Trace(
            "{0} selected {1} to grow with quality {2} and quantity {3}.",
            id,
            item.QualifiedItemId,
            item.Quality,
            item.Stack);

        __result = true;
        __instance.modData[BushManager.instance.modDataItem] = item.QualifiedItemId;
        __instance.modData[BushManager.instance.modDataQuality] = item.Quality.ToString(CultureInfo.InvariantCulture);
        __instance.modData[BushManager.instance.modDataStack] = item.Stack.ToString(CultureInfo.InvariantCulture);
    }

    private static IEnumerable<CodeInstruction> Bush_performToolAction_transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var method = AccessTools
            .GetDeclaredMethods(typeof(ItemRegistry))
            .First(method => method.Name == nameof(ItemRegistry.Create) && !method.IsGenericMethod);

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(method))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return CodeInstruction.Call(typeof(BushManager), nameof(BushManager.CreateBushItem));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void Bush_setUpSourceRect_postfix(Bush __instance, NetRectangle ___sourceRect)
    {
        if (!__instance.modData.TryGetValue(BushManager.instance.modDataId, out var id)
            || !BushManager.instance.data.Value.TryGetValue(id, out var bushModel))
        {
            return;
        }

        var age = __instance.getAge();
        var growthPercent = (float)age / bushModel.AgeToProduce;
        var x = (Math.Min(2, (int)(2 * growthPercent)) + __instance.tileSheetOffset.Value) * 16;
        var y = bushModel.TextureSpriteRow * 16;
        ___sourceRect.Value = new Rectangle(x, y, 16, 32);
    }

    private static IEnumerable<CodeInstruction> Bush_shake_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(
                AccessTools.DeclaredMethod(
                    typeof(Game1),
                    nameof(Game1.createObjectDebris),
                    [
                        typeof(string),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                        typeof(float),
                        typeof(GameLocation),
                    ])))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return CodeInstruction.Call(typeof(BushManager), nameof(BushManager.CreateObjectDebris));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void GameLocation_CheckItemPlantRules_postfix(
        GameLocation __instance,
        ref bool __result,
        string itemId,
        bool isGardenPot,
        bool defaultAllowed,
        ref string deniedMessage)
    {
        var metadata = ItemRegistry.GetMetadata(itemId);
        if (metadata is null
            || !BushManager.instance.data.Value.TryGetValue(metadata.QualifiedItemId, out var bushModel))
        {
            return;
        }

        var parameters = new object[] { bushModel.PlantableLocationRules, isGardenPot, defaultAllowed, null! };
        __result = (bool)BushManager.instance.checkItemPlantRules.Invoke(__instance, parameters)!;
        deniedMessage = (string)parameters[3];
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    private static Item CreateBushItem(string itemId, int amount, int quality, bool allowNull, Bush bush)
    {
        if (bush.modData.TryGetValue(BushManager.instance.modDataId, out var bushId))
        {
            itemId = bushId;
        }

        return ItemRegistry.Create(itemId, amount, quality, allowNull);
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    private static void CreateObjectDebris(
        string id,
        int xTile,
        int yTile,
        int groundLevel,
        int itemQuality,
        float velocityMultiplier,
        GameLocation? location,
        Bush bush)
    {
        if (bush.modData.TryGetValue(BushManager.instance.modDataItem, out var itemId)
            && !string.IsNullOrWhiteSpace(itemId))
        {
            if (!bush.modData.TryGetValue(BushManager.instance.modDataQuality, out var quality)
                || !int.TryParse(quality, out itemQuality))
            {
                itemQuality = 1;
            }

            if (!bush.modData.TryGetValue(BushManager.instance.modDataStack, out var stack)
                || !int.TryParse(stack, out var itemStack))
            {
                itemStack = 1;
            }

            for (var i = 0; i < itemStack; i++)
            {
                Game1.createObjectDebris(itemId, xTile, yTile, groundLevel, itemQuality, velocityMultiplier, location);
            }

            bush.modData.Remove(BushManager.instance.modDataItem);
            bush.modData.Remove(BushManager.instance.modDataQuality);
            bush.modData.Remove(BushManager.instance.modDataStack);
            return;
        }

        bush.modData.Remove(BushManager.instance.modDataItem);
        bush.modData.Remove(BushManager.instance.modDataQuality);
        bush.modData.Remove(BushManager.instance.modDataStack);

        if (!bush.modData.TryGetValue(BushManager.instance.modDataId, out var bushId)
            || !BushManager.instance.data.Value.TryGetValue(bushId, out var bushModel)
            || !BushManager.instance.TryToProduceRandomItem(bush, bushModel, out var item))
        {
            Game1.createObjectDebris(id, xTile, yTile, groundLevel, itemQuality, velocityMultiplier, location);
            return;
        }

        Game1.createObjectDebris(
            item.QualifiedItemId,
            xTile,
            yTile,
            groundLevel,
            item.Quality,
            velocityMultiplier,
            location);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void HoeDirt_canPlantThisSeedHere_postfix(string itemId, ref bool __result)
    {
        if (!__result || !BushManager.instance.data.Value.ContainsKey($"(O){itemId}"))
        {
            return;
        }

        __result = false;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void IndoorPot_performObjectDropInAction_postfix(
        IndoorPot __instance,
        Item dropInItem,
        bool probe,
        ref bool __result)
    {
        if (!BushManager.instance.data.Value.ContainsKey(dropInItem.QualifiedItemId)
            || __instance.hoeDirt.Value.crop != null)
        {
            return;
        }

        if (!probe)
        {
            __instance.bush.Value = new Bush(__instance.TileLocation, 3, __instance.Location)
            {
                modData =
                {
                    [BushManager.instance.modDataId] = dropInItem.QualifiedItemId,
                },
            };

            if (!__instance.Location.IsOutdoors)
            {
                __instance.bush.Value.inPot.Value = true;
                __instance.bush.Value.loadSprite();
                Game1.playSound("coin");
            }
        }

        __result = true;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "RedundantAssignment", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void Object_IsTeaSapling_postfix(SObject __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (BushManager.instance.data.Value.ContainsKey(__instance.QualifiedItemId))
        {
            __result = true;
        }
    }

    private static IEnumerable<CodeInstruction> Object_placementAction_transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.Is(
                OpCodes.Newobj,
                AccessTools.Constructor(
                    typeof(Bush),
                    [typeof(Vector2), typeof(int), typeof(GameLocation), typeof(int)])))
            {
                yield return instruction;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return CodeInstruction.Call(typeof(BushManager), nameof(BushManager.AddModData));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private bool TryToProduceRandomItem(Bush bush, BushModel bushModel, [NotNullWhen(true)] out Item? item)
    {
        foreach (var drop in bushModel.ItemsProduced)
        {
            item = this.TryToProduceItem(bush, drop);
            if (item is null)
            {
                continue;
            }

            return true;
        }

        item = null;
        return false;
    }

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Harmony")]
    private Item? TryToProduceItem(Bush bush, DropsModel drop)
    {
        if (!Game1.random.NextBool(drop.Chance))
        {
            return null;
        }

        if (drop.Condition != null
            && !GameStateQuery.CheckConditions(
                drop.Condition,
                bush.Location,
                null,
                null,
                null,
                null,
                bush.Location.SeedsIgnoreSeasonsHere() ? GameStateQuery.SeasonQueryKeys : null))
        {
            BushManager.instance.Log.Trace(
                "{0} did not select {1}. Failed: {2}",
                bush.modData[BushManager.instance.modDataId],
                drop.Id,
                drop.Condition);

            return null;
        }

        if (drop.Season.HasValue
            && bush.Location.SeedsIgnoreSeasonsHere()
            && drop.Season != Game1.GetSeasonForLocation(bush.Location))
        {
            BushManager.instance.Log.Trace(
                "{0} did not select {1}. Failed: {2}",
                bush.modData[BushManager.instance.modDataId],
                drop.Id,
                drop.Season.ToString());

            return null;
        }

        var item = ItemQueryResolver.TryResolveRandomItem(
            drop,
            new ItemQueryContext(bush.Location, null, null),
            false,
            null,
            null,
            null,
            delegate(string query, string error)
            {
                this.Log.Error(
                    "{0} failed parsing item query {1} for item {2}: {3}",
                    bush.modData[BushManager.instance.modDataId],
                    query,
                    drop.Id,
                    error);
            });

        return item;
    }
}