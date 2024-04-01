/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Services.Patchers;

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models.Events;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.FuryCore;
using StardewMods.SpritePatcher.Framework.Enums;
using StardewMods.SpritePatcher.Framework.Enums.Patches;
using StardewMods.SpritePatcher.Framework.Interfaces;
using StardewMods.SpritePatcher.Framework.Models;
using StardewMods.SpritePatcher.Framework.Services.Factory;

/// <summary>Base class for texture patches.</summary>
internal abstract class BasePatcher : BaseService, ISpritePatcher
{
    private static readonly CodeInstruction BackgroundDrawMethod = new(OpCodes.Ldc_I4_0);
    private static readonly CodeInstruction ConstructionDrawMethod = new(OpCodes.Ldc_I4_1);
    private static readonly CodeInstruction HeldDrawMethod = new(OpCodes.Ldc_I4_2);
    private static readonly CodeInstruction MenuDrawMethod = new(OpCodes.Ldc_I4_3);
    private static readonly CodeInstruction ShadowDrawMethod = new(OpCodes.Ldc_I4_4);
    private static readonly CodeInstruction WorldDrawMethod = new(OpCodes.Ldc_I4_5);

    private static readonly Dictionary<MethodInfo, CodeInstruction> ReplacementMethods = new()
    {
        {
            AccessTools.DeclaredMethod(
                typeof(SpriteBatch),
                nameof(SpriteBatch.Draw),
                [
                    typeof(Texture2D),
                    typeof(Vector2),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(float),
                    typeof(SpriteEffects),
                    typeof(float),
                ]),
            CodeInstruction.Call(
                typeof(BasePatcher),
                nameof(BasePatcher.DrawCustom1),
                [
                    typeof(SpriteBatch),
                    typeof(Texture2D),
                    typeof(Vector2),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(float),
                    typeof(SpriteEffects),
                    typeof(float),
                    typeof(object),
                    typeof(DrawMethod),
                ])
        },
        {
            AccessTools.DeclaredMethod(
                typeof(SpriteBatch),
                nameof(SpriteBatch.Draw),
                [
                    typeof(Texture2D),
                    typeof(Rectangle),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(SpriteEffects),
                    typeof(float),
                ]),
            CodeInstruction.Call(
                typeof(BasePatcher),
                nameof(BasePatcher.DrawCustom2),
                [
                    typeof(SpriteBatch),
                    typeof(Texture2D),
                    typeof(Rectangle),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(SpriteEffects),
                    typeof(float),
                    typeof(object),
                    typeof(DrawMethod),
                ])
        },
        {
            AccessTools.DeclaredMethod(
                typeof(SpriteBatch),
                nameof(SpriteBatch.Draw),
                [
                    typeof(Texture2D),
                    typeof(Vector2),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(Vector2),
                    typeof(SpriteEffects),
                    typeof(float),
                ]),
            CodeInstruction.Call(
                typeof(BasePatcher),
                nameof(BasePatcher.DrawCustom3),
                [
                    typeof(SpriteBatch),
                    typeof(Texture2D),
                    typeof(Vector2),
                    typeof(Rectangle),
                    typeof(Color),
                    typeof(float),
                    typeof(Vector2),
                    typeof(Vector2),
                    typeof(SpriteEffects),
                    typeof(float),
                    typeof(object),
                    typeof(DrawMethod),
                ])
        },
    };

    private static BasePatcher instance = null!;

    private readonly ConfigManager config;
    private readonly SpriteFactory spriteFactory;

    /// <summary>Initializes a new instance of the <see cref="BasePatcher" /> class.</summary>
    /// <param name="configManager">Dependency used for managing config data.</param>
    /// <param name="eventSubscriber">Dependency used for subscribing to events.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="spriteFactory">Dependency used for getting managed objects.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="patchManager">Dependency used for managing patches.</param>
    protected BasePatcher(
        ConfigManager configManager,
        IEventSubscriber eventSubscriber,
        ILog log,
        SpriteFactory spriteFactory,
        IManifest manifest,
        IPatchManager patchManager)
        : base(log, manifest)
    {
        BasePatcher.instance = this;
        this.config = configManager;
        this.spriteFactory = spriteFactory;
        this.Patches = patchManager;
        eventSubscriber.Subscribe<ConfigChangedEventArgs<DefaultConfig>>(this.OnConfigChanged);
    }

    /// <inheritdoc />
    public string Id => this.ModId + "." + this.Type.ToStringFast();

    /// <inheritdoc />
    public abstract AllPatches Type { get; }

    /// <summary>Gets the dependency used for managing patches.</summary>
    protected IPatchManager Patches { get; }

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> Draw(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.WorldDrawMethod);

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> DrawBackground(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.BackgroundDrawMethod);

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> DrawInConstruction(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.ConstructionDrawMethod);

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> DrawInMenu(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.MenuDrawMethod);

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> DrawShadow(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.ShadowDrawMethod);

    /// <summary>Transpiles the given set of code instructions by replacing calls to a specific draw method.</summary>
    /// <param name="instructions">The original set of code instructions.</param>
    /// <returns>A new set of code instructions with the replaced draw method calls.</returns>
    protected static IEnumerable<CodeInstruction> DrawWhenHeld(IEnumerable<CodeInstruction> instructions) =>
        BasePatcher.DrawTranspiler(instructions, BasePatcher.HeldDrawMethod);

    private static void DrawCustom1(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth,
        object entity,
        DrawMethod drawMethod)
    {
        var managedObject = BasePatcher.instance.spriteFactory.GetOrAdd(entity);
        managedObject.Draw(
            spriteBatch,
            texture,
            position,
            sourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth,
            drawMethod);
    }

    private static void DrawCustom2(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle destinationRectangle,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteEffects effects,
        float layerDepth,
        IHaveModData entity,
        DrawMethod drawMethod)
    {
        var managedObject = BasePatcher.instance.spriteFactory.GetOrAdd(entity);
        managedObject.Draw(
            spriteBatch,
            texture,
            destinationRectangle,
            sourceRectangle,
            color,
            rotation,
            origin,
            effects,
            layerDepth,
            drawMethod);
    }

    private static void DrawCustom3(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects effects,
        float layerDepth,
        object entity,
        DrawMethod drawMethod)
    {
        var managedObject = BasePatcher.instance.spriteFactory.GetOrAdd(entity);
        managedObject.Draw(
            spriteBatch,
            texture,
            position,
            sourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth,
            drawMethod);
    }

    private static IEnumerable<CodeInstruction> DrawTranspiler(
        IEnumerable<CodeInstruction> instructions,
        CodeInstruction drawMethod)
    {
        foreach (var instruction in instructions)
        {
            var replaced = false;
            foreach (var (oldMethod, newMethod) in BasePatcher.ReplacementMethods)
            {
                if (!instruction.Calls(oldMethod))
                {
                    continue;
                }

                replaced = true;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return drawMethod;
                yield return newMethod;

                break;
            }

            if (!replaced)
            {
                yield return instruction;
            }
        }
    }

    private void OnConfigChanged(ConfigChangedEventArgs<DefaultConfig> obj)
    {
        if (this.config.GetValue(this.Type))
        {
            this.Patches.Patch(this.Id);
        }
        else
        {
            this.Patches.Unpatch(this.Id);
        }
    }
}