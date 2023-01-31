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
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ItemManagement;
using AtraCore.Framework.ReflectionManager;
using AtraCore.Models;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewValley.Objects;

namespace AtraCore.HarmonyPatches.DrawPrismaticPatches;

#warning - finish this.

/// <summary>
/// Draws things with a prismatic tint or overlay.
/// </summary>
[HarmonyPatch]
internal static class DrawPrismatic
{
    private static readonly SortedList<ItemTypeEnum, Dictionary<int, Lazy<Texture2D>>> PrismaticMasks = new();
    private static readonly SortedList<ItemTypeEnum, HashSet<int>> PrismaticFull = new();

    #region LOADDATA

    /// <summary>
    /// Load the prismatic data.
    /// Called on SaveLoaded.
    /// </summary>
    internal static void LoadPrismaticData()
    {
        Dictionary<string, DrawPrismaticModel>? models = AssetManager.GetPrismaticModels();
        if (models is null)
        {
            return;
        }

        PrismaticFull.Clear();
        PrismaticMasks.Clear();

        foreach (DrawPrismaticModel? model in models.Values)
        {
            if (!int.TryParse(model.Identifier, out int id))
            {
                id = DataToItemMap.GetID(model.ItemType, model.Identifier);
                if (id == -1)
                {
                    ModEntry.ModMonitor.Log($"Could not resolve {model.ItemType}, {model.Identifier}, skipping.", LogLevel.Warn);
                    continue;
                }
            }

            // Handle the full prismatics.
            if (string.IsNullOrWhiteSpace(model.Mask))
            {
                if (!PrismaticFull.TryGetValue(model.ItemType, out HashSet<int>? set))
                {
                    set = new();
                }
                set.Add(id);
                PrismaticFull[model.ItemType] = set;
            }
            else
            {
                // handle the ones that have masks.
                if (!PrismaticMasks.TryGetValue(model.ItemType, out Dictionary<int, Lazy<Texture2D>>? masks))
                {
                    masks = new();
                }
                if (!masks.TryAdd(id, new(() => Game1.content.Load<Texture2D>(model.Mask))))
                {
                    ModEntry.ModMonitor.Log($"{model.ItemType} - {model.Identifier} appears to be a duplicate, ignoring", LogLevel.Warn);
                }
                PrismaticMasks[model.ItemType] = masks;
            }
        }
    }
    #endregion

    #region Helpers
    [MethodImpl(TKConstants.Hot)]
    private static bool ShouldDrawAsFullColored(this Item item)
    => item.GetItemType() is ItemTypeEnum type && PrismaticFull.TryGetValue(type, out HashSet<int>? set)
        && set.Contains(item.ParentSheetIndex);

    [MethodImpl(TKConstants.Hot)]
    private static Color ReplaceDrawColorForItem(Color prevcolor, Item item)
        => item.ShouldDrawAsFullColored() ? Utility.GetPrismaticColor() : prevcolor;

    [MethodImpl(TKConstants.Hot)]
    private static Color ReplaceDrawColorForItem(Color prevcolor, ItemTypeEnum type, int parentSheetIndex)
        => PrismaticFull.TryGetValue(type, out HashSet<int>? set) && set.Contains(parentSheetIndex) ? Utility.GetPrismaticColor() : prevcolor;

    [MethodImpl(TKConstants.Hot)]
    private static Texture2D? GetColorMask(this Item item)
        => item.GetItemType() is ItemTypeEnum type && PrismaticMasks.TryGetValue(type, out Dictionary<int, Lazy<Texture2D>>? masks)
            && masks.TryGetValue(item.ParentSheetIndex, out Lazy<Texture2D>? mask) ? mask.Value : null;

    [MethodImpl(TKConstants.Hot)]
    private static void DrawColorMask(Item item, SpriteBatch b, Rectangle position, float drawDepth)
    {
        if (item.GetColorMask() is Texture2D texture)
        {
            b.Draw(
                texture,
                destinationRectangle: position,
                sourceRectangle: null,
                color: Utility.GetPrismaticColor(),
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: drawDepth);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private static void DrawColorMask(ItemTypeEnum type, int parentSheetIndex, SpriteBatch b, int x, int y, float drawDepth)
    {
        if (PrismaticMasks.TryGetValue(type, out Dictionary<int, Lazy<Texture2D>>? masks) && masks.TryGetValue(parentSheetIndex, out Lazy<Texture2D>? mask))
        {
            b.Draw(
                texture: mask.Value,
                new Vector2(x, y),
                sourceRectangle: null,
                color: Utility.GetPrismaticColor(),
                rotation: 0,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: drawDepth);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private static void DrawSObjectAndAlsoColorMask(
        SpriteBatch b,
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth,
        SObject obj)
    {
        b.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        if (obj.GetColorMask() is Texture2D tex)
        {
            b.Draw(tex, position, null, Utility.GetPrismaticColor(), rotation, origin, scale, effects, layerDepth);
        }
    }
    #endregion

    #region SOBJECT

    /// <summary>
    /// Prefixes SObject's drawInMenu function in order to draw things prismatic-ally.
    /// </summary>
    /// <param name="__instance">SObject instance.</param>
    /// <param name="color">Color to make things.</param>
    [UsedImplicitly]
    [HarmonyPrefix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(SObject), nameof(SObject.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PrefixSObjectDrawInMenu(SObject __instance, ref Color color)
    {
        try
        {
            if (__instance.ShouldDrawAsFullColored())
            {
                color = Utility.GetPrismaticColor();
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic item\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(SObject), nameof(SObject.drawInMenu))]
    private static IEnumerable<CodeInstruction>? TranspileSObjectDrawInMenu(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.AdjustUtilityTextColor();

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(SObject), nameof(SObject.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixSObjectDrawInMenu(
        SObject __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth)
    {
        try
        {
            if (__instance.GetColorMask() is Texture2D texture)
            {
                spriteBatch.Draw(
                    texture: texture,
                    position: location + (new Vector2(32f, 32f) * scaleSize),
                    sourceRectangle: new Rectangle(0, 0, 16, 16),
                    color: Utility.GetPrismaticColor() * transparency,
                    rotation: 0f,
                    origin: new Vector2(8f, 8f) * scaleSize,
                    scale: scaleSize * 4f,
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic mask\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.drawMenuView))]
    private static IEnumerable<CodeInstruction>? TranspileCraftingRecipes(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty("White", ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, (int)ItemTypeEnum.BigCraftable),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(CraftingRecipe).GetCachedMethod(nameof(CraftingRecipe.getIndexOfMenuView), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, ItemTypeEnum, int>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.drawWithShadow), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, (int)ItemTypeEnum.BigCraftable),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(CraftingRecipe).GetCachedMethod(nameof(CraftingRecipe.getIndexOfMenuView), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldarg_1), // spritebatch
                new(OpCodes.Ldarg_2), // x
                new(OpCodes.Ldarg_3), // y
                new(OpCodes.Ldarga_S, 4),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod(nameof(DrawColorMask), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(ItemTypeEnum), typeof(int), typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) } )),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty("White", ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, (int)ItemTypeEnum.SObject),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(CraftingRecipe).GetCachedMethod(nameof(CraftingRecipe.getIndexOfMenuView), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, ItemTypeEnum, int>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.drawWithShadow), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4, (int)ItemTypeEnum.SObject),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(CraftingRecipe).GetCachedMethod(nameof(CraftingRecipe.getIndexOfMenuView), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Ldarg_1), // spritebatch
                new(OpCodes.Ldarg_2), // x
                new(OpCodes.Ldarg_3), // y
                new(OpCodes.Ldarga_S, 4),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod(nameof(DrawColorMask), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(ItemTypeEnum), typeof(int), typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) } )),
            });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(SObject), nameof(SObject.drawWhenHeld))]
    private static IEnumerable<CodeInstruction>? TranspileSObjectDrawWhenHeld(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            // lots of places things are drawn here.
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            // first one is the bigcraftable, second one is the nonbigcraftable
            for (int i = 0; i < 2; i++)
            {
                helper.FindNext(new CodeInstructionWrapper[]
                {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                })
                .Advance(1)
                .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, Item>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
                })
                .FindNext(new CodeInstructionWrapper[]
                {
                    (OpCodes.Callvirt, typeof(SpriteBatch).GetCachedMethod(nameof(SpriteBatch.Draw), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) } )),
                })
                .ReplaceInstruction(
                    instruction: new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod(nameof(DrawSObjectAndAlsoColorMask), ReflectionCache.FlagTypes.StaticFlags)),
                    keepLabels: true)
                .Insert(new CodeInstruction[] { new(OpCodes.Ldarg_0) });
            }

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyBefore("Digus.ProducerFrameworkMod")]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Used for matching only.")]
    [HarmonyPatch(typeof(SObject), nameof(SObject.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) } )]
    private static IEnumerable<CodeInstruction>? TranspileSObjectDraw(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            // lots of places things are drawn here.
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            // bigcraftables block is first. Look for the single alone draw statement NOT in a conditional.
            // and bracket it.
            helper.FindNext(new CodeInstructionWrapper[]
            { // if (base.parentSheetIndex == 272)
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(Item).GetCachedField(nameof(Item.parentSheetIndex), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // op_Implicit
                (OpCodes.Ldc_I4, 272),
                OpCodes.Bne_Un,
            })
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .Advance(-1)
            .FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_1,
                (OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.bigCraftableSpriteSheet), ReflectionCache.FlagTypes.StaticFlags)),
                SpecialCodeInstructionCases.LdLoc,
            })
            .Advance(2);

            CodeInstruction? destination = helper.CurrentInstruction.Clone();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, Item>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                SpecialCodeInstructionCases.LdLoc,
                (OpCodes.Callvirt, typeof(SpriteBatch).GetCachedMethod(nameof(SpriteBatch.Draw), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) } )),
            });

            CodeInstruction? layerDepth = helper.CurrentInstruction.Clone();

            helper.Advance(2)
            .Insert(new CodeInstruction[]
            {
                new (OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                destination,
                layerDepth,
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Item, SpriteBatch, Rectangle, float>(nameof(DrawColorMask), ReflectionCache.FlagTypes.StaticFlags)),
            });

            // alright! Now to deal with normal SObjects
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(SObject).GetCachedField(nameof(SObject.fragility), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call,
                OpCodes.Ldc_I4_2,
                OpCodes.Beq,
            })
            .Advance(4)
            .StoreBranchDest()
            .AdvanceToStoredLabel();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, Item>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Callvirt, typeof(SpriteBatch).GetCachedMethod(nameof(SpriteBatch.Draw), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) } )),
            })
            .ReplaceInstruction(
                instruction: new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod(nameof(DrawSObjectAndAlsoColorMask), ReflectionCache.FlagTypes.StaticFlags)),
                keepLabels: true)
            .Insert(new CodeInstruction[] { new(OpCodes.Ldarg_0) });

            // and the held item. Kill me now.
            helper.FindNext(new CodeInstructionWrapper[]
            { // must skip past the sprinkler section first.
                OpCodes.Ldarg_0,
                (OpCodes.Callvirt, typeof(SObject).GetCachedMethod(nameof(SObject.IsSprinkler), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse,
            })
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(SObject).GetCachedField(nameof(SObject.heldObject), ReflectionCache.FlagTypes.InstanceFlags)),
                (OpCodes.Callvirt, typeof(NetFieldBase<SObject, NetRef<SObject>>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                OpCodes.Brfalse,
            })
            .Copy(3, out IEnumerable<CodeInstruction>? codes)
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Call, typeof(Color).GetCachedProperty(nameof(Color.White), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
            })
            .Advance(1)
            .Insert(codes.ToArray())
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod<Color, Item>(nameof(ReplaceDrawColorForItem), ReflectionCache.FlagTypes.StaticFlags)),
            })
            .FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Callvirt, typeof(SpriteBatch).GetCachedMethod(nameof(SpriteBatch.Draw), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) } )),
            })
            .ReplaceInstruction(
                instruction: new(OpCodes.Call, typeof(DrawPrismatic).GetCachedMethod(nameof(DrawSObjectAndAlsoColorMask), ReflectionCache.FlagTypes.StaticFlags)),
                keepLabels: true)
            .Insert(codes.Select(c => c.Clone()).ToArray());

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    #endregion

    #region RING

    [UsedImplicitly]
    [HarmonyPrefix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(Ring), nameof(Ring.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PrefixRingDrawInMenu(Ring __instance, ref Color color)
    {
        try
        {
            if (__instance.ShouldDrawAsFullColored())
            {
                color = Utility.GetPrismaticColor();
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic ring\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(Ring), nameof(Ring.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixRingDrawInMenu(
        Ring __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth)
    {
        try
        {
            if (__instance.GetColorMask() is Texture2D texture)
            {
                spriteBatch.Draw(
                    texture: texture,
                    position: location + (new Vector2(32f, 32f) * scaleSize),
                    sourceRectangle: new Rectangle(0, 0, 16, 16),
                    color: Utility.GetPrismaticColor() * transparency,
                    rotation: 0f,
                    origin: new Vector2(8f, 8f) * scaleSize,
                    scale: scaleSize * 4f,
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic mask\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    #endregion

    #region BOOTS

    [UsedImplicitly]
    [HarmonyPrefix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(Boots), nameof(Boots.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PrefixBootsDrawInMenu(Boots __instance, ref Color color)
    {
        try
        {
            if (__instance.ShouldDrawAsFullColored())
            {
                color = Utility.GetPrismaticColor();
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic boots\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(Boots), nameof(Boots.drawInMenu))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixBootsDrawInMenu(
        Ring __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth)
    {
        try
        {
            if (__instance.GetColorMask() is Texture2D texture)
            {
                spriteBatch.Draw(
                    texture: texture,
                    position: location + (new Vector2(32f, 32f) * scaleSize),
                    sourceRectangle: new Rectangle(0, 0, 16, 16),
                    color: Utility.GetPrismaticColor() * transparency,
                    rotation: 0f,
                    origin: new Vector2(8f, 8f) * scaleSize,
                    scale: scaleSize * 4f,
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in drawing prismatic mask\n\n{ex}", LogLevel.Error);
        }
        return;
    }

    #endregion
}
