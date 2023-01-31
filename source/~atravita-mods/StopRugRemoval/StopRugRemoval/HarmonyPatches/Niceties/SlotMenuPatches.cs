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
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Menuing;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Patch that replaces the token purchase station with some more options.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class TokenPurchasePatch
{
    private static void AttemptBuyTokens(int tokens)
    {
        try
        {
            if (Game1.player.Money >= tokens * 10)
            {
                Game1.player.Money -= tokens * 10;
                Game1.player.currentLocation.localSound("Pickup_Coin15");
                Game1.player.clubCoins += tokens;
            }
            else
            {
                Game1.drawObjectDialogue(Game1.content.LoadString(@"Strings\StringsFromCSFiles:GameLocation.cs.8715"));
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while buying club coins?\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPatch(nameof(GameLocation.performAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool Prefix(string action, Farmer who, ref bool __result)
    {
        if (who.IsLocalPlayer && ModEntry.Config.Enabled && action == "BuyQiCoins")
        {
            try
            {
                List<Response> responses = new(5);
                List<Action?> actions = new(5);
                for (int i = 10; i <= 10000; i *= 10)
                {
                    if (Game1.player.Money < i * 10)
                    {
                        break;
                    }
                    int copy = i; // prevent accidental capture. There's no explicit notation for that in C#

                    // TODO: fix this to use the player's locale here.
                    responses.Add(new Response(i.ToString(), i.ToString()));
                    actions.Add(() => AttemptBuyTokens(copy));
                }

                responses.Add(new Response("No", Game1.content.LoadString(@"Strings\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape));

                Game1.activeClickableMenu = new DialogueAndAction(I18n.BuyCasino(), responses, actions, ModEntry.InputHelper);
                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Mod failed while prefixing GameLocation.performAction.\n\n{ex}", LogLevel.Error);
            }
        }
        return true;
    }
}

/// <summary>
/// Patches against the slot menu.
/// </summary>
[HarmonyPatch(typeof(Slots))]
internal static class SlotMenuPatches
{
    private const int HEIGHT = 52;

    private static readonly Lazy<Func<Slots, bool>> SpinningGetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("spinning", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldGetter<Slots, bool>());

    private static readonly Lazy<Action<Slots, bool>> SpinningSetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("spinning", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldSetter<Slots, bool>());

    private static readonly Lazy<Func<Slots, List<float>>> SlotResultsGetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("slotResults", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldGetter<Slots, List<float>>());

    private static readonly Lazy<Action<Slots, int>> CurrentBetSetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("currentBet", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldSetter<Slots, int>());

    private static readonly Lazy<Action<Slots, int>> SlotsFinishedSetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("slotsFinished", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldSetter<Slots, int>());

    private static readonly Lazy<Action<Slots, int>> SpinsCountSetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("spinsCount", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldSetter<Slots, int>());

    private static readonly Lazy<Action<Slots, bool>> ShowResultSetterLazy = new(
        () => typeof(Slots)
            .GetCachedField("showResult", ReflectionCache.FlagTypes.InstanceFlags)
            .GetInstanceFieldSetter<Slots, bool>());

    private static readonly PerScreen<ClickableComponent?> Bet1000 = new();

    private static readonly PerScreen<ClickableComponent?> Bet10000 = new();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Slots.receiveLeftClick))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void HandleClick(Slots __instance, ref bool ___spinning, int x, int y)
    {
        if (___spinning)
        {
            return;
        }
        if (Game1.player.clubCoins >= 1000 && Bet1000.Value?.bounds.Contains(x, y) == true)
        {
            CurrentBetSetterLazy.Value(__instance, 1000);
            StartSpin(__instance);
            Game1.player.clubCoins -= 1000;
        }
        else if (Game1.player.clubCoins >= 10000 && Bet10000.Value?.bounds.Contains(x, y) == true)
        {
            CurrentBetSetterLazy.Value(__instance, 10000);
            StartSpin(__instance);
            Game1.player.clubCoins -= 10000;
        }
        return;
    }

    private static void StartSpin(Slots slots)
    {
        Club.timesPlayedSlots++;
        slots.setSlotResults(SlotResultsGetterLazy.Value(slots));
        SpinningSetterLazy.Value(slots, true);
        Game1.playSound("bigSelect");
        SlotsFinishedSetterLazy.Value(slots, 0);
        SpinsCountSetterLazy.Value(slots, 0);
        ShowResultSetterLazy.Value(slots, false);
    }

    private static int ButtonOffset()
        => ModEntry.Config.BetIcons && ModEntry.Config.Enabled ? 288 : 160;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Slots), MethodType.Constructor, new[] { typeof(int), typeof(bool) })]
    private static void PostfixConstructor()
    {
        if (ModEntry.Config.BetIcons && ModEntry.Config.Enabled)
        {
            Vector2 position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 104, HEIGHT, -16, 160);
            Bet1000.Value = new ClickableComponent(new Rectangle((int)position.X, (int)position.Y, 104, HEIGHT), I18n.Bet1k());

            position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 124, HEIGHT, -16, 224);
            Bet10000.Value = new ClickableComponent(new Rectangle((int)position.X, (int)position.Y, 124, HEIGHT), I18n.Bet10k());
        }
    }

    // Move the DONE button down to make room for the bet 1k and bet 10k buttons.
#pragma warning disable SA1116 // Split parameters should start on line after declaration
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Slots), MethodType.Constructor, new[] { typeof(int), typeof(bool) })]
    private static IEnumerable<CodeInstruction>? TranspileConstructor(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldc_I4, 160),
                new(OpCodes.Call, typeof(Utility).GetCachedMethod(nameof(Utility.getTopLeftPositionForCenteringOnScreen), ReflectionCache.FlagTypes.StaticFlags, new[] { typeof(xTile.Dimensions.Rectangle), typeof(int), typeof(int), typeof(int), typeof(int) })),
            })
            .ReplaceInstruction(OpCodes.Call, typeof(SlotMenuPatches).GetCachedMethod(nameof(ButtonOffset), ReflectionCache.FlagTypes.StaticFlags), keepLabels: true);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling Slots.ctor!\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }

    // Stick my draw at the right location in the middle of this function. Gotta do this since the spritebatch is opened and closed in this function.
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Slots.draw))]
    private static IEnumerable<CodeInstruction>? TranspileDraw(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Slots).GetCachedField("showResult", ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Brfalse_S),
            })
            .Advance(2)
            .StoreBranchDest()
            .AdvanceToStoredLabel()
            .GetLabels(out IList<Label>? labels, clear: true)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, typeof(SlotMenuPatches).StaticMethodNamed(nameof(DrawImpl))),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling Slots.draw!\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
#pragma warning restore SA1116 // Split parameters should start on line after declaration

    private static void DrawImpl(Slots slots, SpriteBatch b)
    {
        if (Bet1000.Value is not null)
        {
            b.Draw(
                texture: AssetEditor.BetIcon,
                position: new Vector2(Bet1000.Value.bounds.X, Bet1000.Value.bounds.Y),
                sourceRectangle: new Rectangle(0, 0, 26, 13),
                color: Color.White * (!SpinningGetterLazy.Value(slots) && Game1.player.clubCoins >= 1000 ? 1f : 0.5f),
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom * Bet1000.Value.scale,
                effects: SpriteEffects.None,
                layerDepth: 0.99f);
        }

        if (Bet10000.Value is not null)
        {
            b.Draw(
                texture: AssetEditor.BetIcon,
                position: new Vector2(Bet10000.Value.bounds.X, Bet10000.Value.bounds.Y),
                sourceRectangle: new Rectangle(0, 13, 31, 13),
                color: Color.White * ((!SpinningGetterLazy.Value(slots) && Game1.player.clubCoins >= 10000) ? 1f : 0.5f),
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Game1.pixelZoom * Bet10000.Value.scale,
                effects: SpriteEffects.None,
                layerDepth: 0.99f);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Slots.unload))]
    private static void BeforeQuit()
    {
        Bet1000.Value = null;
        Bet10000.Value = null;
    }
}
