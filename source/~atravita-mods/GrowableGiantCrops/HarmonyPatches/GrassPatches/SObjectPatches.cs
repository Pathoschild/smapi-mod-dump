/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using FastExpressionCompiler.LightExpression;

using GrowableGiantCrops.Framework;
using GrowableGiantCrops.Framework.Assets;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.HarmonyPatches.GrassPatches;

/// <summary>
/// Patches on SObject for grass.
/// </summary>
[HarmonyPatch(typeof(SObject))]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "Reviewed.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named for Harmony.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Reviewed.")]
internal static class SObjectPatches
{
    /// <summary>
    /// A mod data key used to mark custom grass types.
    /// </summary>
    internal const string ModDataKey = "atravita.GrowableGiantCrop.GrassType";

    /// <summary>
    /// The ParentSheetIndex of a grass starter.
    /// </summary>
    internal const int GrassStarterIndex = 297;

    private static readonly Api Api = new();

    #region delegates

    private static readonly Lazy<Func<SObject, bool>?> isMoreGrassStarter = new(() =>
    {
        Type? moreGrass = AccessTools.TypeByName("MoreGrassStarters.GrassStarterItem");
        if (moreGrass is null)
        {
            return null;
        }

        ParameterExpression? obj = Expression.ParameterOf<SObject>("obj");
        TypeBinaryExpression? express = Expression.TypeIs(obj, moreGrass);
        return Expression.Lambda<Func<SObject, bool>>(express, obj).CompileFast();
    });

    /// <summary>
    /// Gets whether an item is a MoreGrassStarter grass starter.
    /// </summary>
    internal static Func<SObject, bool>? IsMoreGrassStarter => isMoreGrassStarter.Value;

    private static readonly Lazy<Func<int, SObject>?> instantiateMoreGrassStarter = new(() =>
    {
        Type? moreGrass = AccessTools.TypeByName("MoreGrassStarters.GrassStarterItem");
        if (moreGrass is null)
        {
            return null;
        }

        ParameterExpression which = Expression.ParameterOf<int>("which");
        ConstructorInfo constructor = moreGrass.GetCachedConstructor<int>(ReflectionCache.FlagTypes.InstanceFlags);
        NewExpression newObj = Expression.New(constructor, which);
        return Expression.Lambda<Func<int, SObject>>(newObj, which).CompileFast();
    });

    /// <summary>
    /// Instantiates an instance of an More Grass grass starter.
    /// </summary>
    internal static Func<int, SObject>? InstantiateMoreGrassStarter => instantiateMoreGrassStarter.Value;

    private static Lazy<Func<Grass, bool>?> isMoreGrassGrass = new(() =>
    {
        Type? moreGrass = AccessTools.TypeByName("MoreGrassStarters.CustomGrass");
        if (moreGrass is null)
        {
            return null;
        }

        ParameterExpression? obj = Expression.ParameterOf<Grass>("grass");
        TypeBinaryExpression? express = Expression.TypeIs(obj, moreGrass);
        return Expression.Lambda<Func<Grass, bool>>(express, obj).CompileFast();
    });

    /// <summary>
    /// Checks to see if a Grass instance is a More Grass grass.
    /// </summary>
    internal static Func<Grass, bool>? IsMoreGrassGrass => isMoreGrassGrass.Value;

    private static Lazy<Func<int, Grass>?> instantiateMoreGrassGrass = new(() =>
    {
        Type? moreGrass = AccessTools.TypeByName("MoreGrassStarters.CustomGrass");
        if (moreGrass is null)
        {
            return null;
        }

        ParameterExpression which = Expression.ParameterOf<int>("which");
        ConstantExpression numberOfWeeds = Expression.ConstantInt(1);
        ConstructorInfo constructor = moreGrass.GetCachedConstructor<int, int>(ReflectionCache.FlagTypes.InstanceFlags);
        NewExpression newObj = Expression.New(constructor, which, numberOfWeeds);
        return Expression.Lambda<Func<int, Grass>>(newObj, which).CompileFast();
    });

    /// <summary>
    /// Instantiates an instance of a MoreGrass grass.
    /// </summary>
    internal static Func<int, Grass>? InstantiateMoreGrassGrass => instantiateMoreGrassGrass.Value;

    private static Lazy<Func<SObject, int?>?> getMoreGrassStarterIndex = new(() =>
    {
        Type? moreGrass = AccessTools.TypeByName("MoreGrassStarters.GrassStarterItem");
        if (moreGrass is null)
        {
            return null;
        }

        ParameterExpression? obj = Expression.ParameterOf<SObject>("obj");
        TypeBinaryExpression? isInst = Expression.TypeIs(obj, moreGrass);
        ParameterExpression ret = Expression.ParameterOf<int?>("ret");

        ConstantExpression retnull = Expression.ConstantNull<int>();
        MethodInfo whichGrassGetter = moreGrass.GetCachedProperty("whichGrass", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()
                                       ?? ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>("whichgrass getter");

        UnaryExpression casted = Expression.TypeAs(obj, moreGrass);
        BinaryExpression assign = Expression.Assign(ret, Expression.Call(casted, whichGrassGetter));

        ConditionalExpression ifStatement = Expression.IfThenElse(isInst, assign, retnull);

        BlockExpression block = Expression.Block(typeof(int?), new List<ParameterExpression>() { ret }, ifStatement, ret);
        return Expression.Lambda<Func<SObject, int?>>(block, obj).CompileFast();
    });

    /// <summary>
    /// Gets the internal index of a more grass starter.
    /// </summary>
    internal static Func<SObject, int?>? GetMoreGrassStarterIndex => getMoreGrassStarterIndex.Value;

    #endregion

    #region draw patches

    private static AssetHolder? texture;
    private static Dictionary<string, int> offsets = new()
    {
        ["spring"] = 0,
        ["summer"] = 20,
        ["fall"] = 40,
        ["winter"] = 80, // remember that desert/indoors should use spring instead.
        ["2"] = 60,
        ["3"] = 80,
        ["4"] = 100,
        ["5"] = 120,
        ["6"] = 140,
    };

    [MethodImpl(TKConstants.Hot)]
    private static bool GetDrawParts(SObject obj, [NotNullWhen(true)] out Texture2D? tex, out int offset)
    {
        tex = null;
        offset = 0;
        if (obj.ParentSheetIndex != GrassStarterIndex || obj.modData?.TryGetValue(ModDataKey, out string? idx) != true)
        {
            return false;
        }

        texture ??= AssetCache.Get("TerrainFeatures/grass");
        if (texture?.Get() is not Texture2D temptex)
        {
            return false;
        }
        tex = temptex;

        if (idx == "1")
        {
            GameLocation loc = Game1.currentLocation;
            idx = loc is not Desert && loc is not IslandLocation && loc.IsOutdoors ? Game1.GetSeasonForLocation(loc) : "spring";
        }

        return offsets.TryGetValue(idx, out offset);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) } )]
    private static bool PrefixDraw(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        if (!GetDrawParts(__instance, out Texture2D? tex, out int offset))
        {
            return true;
        }

        Vector2 position = Game1.GlobalToLocal(
            Game1.viewport,
            new Vector2(x * Game1.tileSize, y * Game1.tileSize));
        float draw_layer = Math.Max(
            0f,
            ((y * Game1.tileSize) + 40) / 10000f) + (x * 1E-05f);

        spriteBatch.Draw(
            texture: tex,
            position,
            sourceRectangle: new Rectangle(30, offset, 15, 20),
            color: Color.White * alpha,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: Vector2.One * Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth: draw_layer);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.drawWhenHeld))]
    private static bool PrefixDrawWhenHeld(SObject __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
        if (!GetDrawParts(__instance, out Texture2D? tex, out int offset))
        {
            return true;
        }

        spriteBatch.Draw(
            texture: tex,
            position: objectPosition - new Vector2(0, 8),
            sourceRectangle: new Rectangle(30, offset, 15, 20),
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            scale: 4f,
            effects: SpriteEffects.None,
            layerDepth: Math.Max(0f, (f.getStandingY() + 3) / 10000f));

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.drawInMenu))]
    private static bool PrefixDrawInMenu(SObject __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (!GetDrawParts(__instance, out Texture2D? tex, out int offset))
        {
            return true;
        }

        spriteBatch.Draw(
            texture: tex,
            position: location + new Vector2(32, 48),
            sourceRectangle: new Rectangle(30, offset, 15, 20),
            color: color * transparency,
            rotation: 0f,
            new Vector2(8f, 16f),
            scale: scaleSize * Game1.pixelZoom,
            effects: SpriteEffects.None,
            layerDepth);
        if (((drawStackNumber == StackDrawType.Draw && __instance.maximumStackSize() > 1 && __instance.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive)
            && scaleSize > 0.3f && __instance.Stack != int.MaxValue)
        {
            Utility.drawTinyDigits(
                toDraw: __instance.Stack,
                b: spriteBatch,
                position: location + new Vector2(64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize) + (3f * scaleSize), 64f - (18f * scaleSize) + 2f),
                scale: 3f * scaleSize,
                layerDepth: 1f,
                c: Color.White);
        }
        return false;
    }

    #endregion

    #region placement patch

    private static Grass GetMatchingGrass(SObject obj) => Api.GetMatchingGrass(obj) ?? new Grass(Grass.springGrass, 4);

    [HarmonyPatch(nameof(SObject.placementAction))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldc_I4_1,
                OpCodes.Ldc_I4_4,
                (OpCodes.Newobj, typeof(Grass).GetCachedConstructor<int, int>(ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .GetLabels(out IList<Label>? labels)
            .Remove(3)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(SObjectPatches).GetCachedMethod(nameof(GetMatchingGrass), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
    #endregion
}
