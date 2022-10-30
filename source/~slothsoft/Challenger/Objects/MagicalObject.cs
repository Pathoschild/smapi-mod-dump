/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Menus;
using Slothsoft.Challenger.Models;
using StardewModdingAPI.Events;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Objects;

/// <summary>
/// This class patches <code>StardewValley.Object</code>.
/// </summary>
public static class MagicalObject {

    public const int ObjectId = 570745037;
    internal const string ObjectName = "Magical Object";
    private const bool ObjectBigCraftable = true;

    private static readonly HashSet<SObject> MagicalObjects = new();

    internal static void PatchObject(string uniqueId) {
        var helper = ChallengerMod.Instance.Helper;
        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.Input.ButtonPressed += OnButtonPressed;

        var harmony = new Harmony(uniqueId);

        MagicalDebris.PatchObject(harmony);
        MagicalGame1.PatchObject(harmony);
        MagicalWorldState.PatchObject(harmony);

        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.draw),
                new[] {
                    typeof(SpriteBatch),
                    typeof(int),
                    typeof(int),
                    typeof(float),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.draw),
                new[] {
                    typeof(SpriteBatch),
                    typeof(int),
                    typeof(int),
                    typeof(float),
                    typeof(float),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.drawAsProp),
                new[] {
                    typeof(SpriteBatch),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.drawInMenu),
                new[] {
                    typeof(SpriteBatch),
                    typeof(Vector2),
                    typeof(float),
                    typeof(float),
                    typeof(float),
                    typeof(StackDrawType),
                    typeof(Color),
                    typeof(bool),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.drawWhenHeld),
                new[] {
                    typeof(SpriteBatch),
                    typeof(Vector2),
                    typeof(Farmer),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );

        harmony.Patch(
            original: AccessTools.Method(
                typeof(SObject),
                nameof(SObject.isActionable),
                new[] {
                    typeof(Farmer),
                }),
            prefix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeVanillaObject)),
            postfix: new HarmonyMethod(typeof(MagicalObject), nameof(MakeMagicalObject))
        );
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
        if (e.Name.StartsWith("Data/BigCraftablesInformation")) {
            // See documentation: https://stardewvalleywiki.com/Modding:Items#Big_craftables
            // name / price / edibility / type and category / description / outdoors / indoors / fragility / is lamp / display name
            e.Edit(
                asset => {
                    var helper = ChallengerMod.Instance.Helper;
                    var description = helper.Translation.Get("MagicalObject.Description");
                    var name = helper.Translation.Get("MagicalObject.Name");
                    
                    var data = asset.AsDictionary<int, string>().Data;
                    data.Add(
                        ObjectId,
                        $"{ObjectName}/0/-300/Crafting -9/{description}/true/true/0//{name}"
                    );
                });
        }
    }

    private static void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
        // we don't interact with anything here
        if (!Context.IsPlayerFree) {
            return;
        }

        // Filter out all objects that are not magical or still in use
        var pos = CommonHelpers.GetCursorTile(1);
        if (!Game1.currentLocation.Objects.TryGetValue(pos, out var obj)
            || !IsMagicalObject(obj)
            || obj.heldObject.Value is not null
            || obj.MinutesUntilReady > 0) {
            return;
        }

        // we have a magical object on our hands
        try {
            MakeVanillaObject(obj);
            if (obj.ParentSheetIndex == MagicalReplacement.Default.ParentSheetIndex) {
                if (!e.Button.IsUseToolButton()) {
                    // we have no special item for this challenge -> open challenge Menu
                    if (Game1.activeClickableMenu == null && (e.Button.IsActionButton() || e.Button.IsUseToolButton())) 
                        Game1.activeClickableMenu = new ChallengeMenu();
                }
            } else if (e.Button.IsUseToolButton() && Game1.player.CurrentItem != null) {
                if (Game1.player.CurrentItem.getOne() is SObject heldItem && obj.performObjectDropInAction(heldItem, false, Game1.player)) {
                    Game1.player.reduceActiveItemByOne();
                }
            }
        } finally {
            MakeMagicalObject(obj);
        }
    }

    private static void MakeVanillaObject(SObject __instance) {
        if (!IsMagicalObject(__instance) || !ChallengerMod.Instance.IsInitialized())
            return;

        MagicalObjects.Add(__instance);

        var magicalReplacement = ChallengerMod.Instance.GetApi()!.ActiveChallengeMagicalReplacement;
        __instance.ParentSheetIndex = magicalReplacement.ParentSheetIndex;
        __instance.Name = magicalReplacement.Name;
    }

    internal static bool IsMagicalObject(SObject instance) {
        return instance is { bigCraftable.Value: ObjectBigCraftable, ParentSheetIndex: ObjectId };
    }

    private static void MakeMagicalObject(SObject __instance) {
        if (!MagicalObjects.Contains(__instance))
            return;

        __instance.ParentSheetIndex = ObjectId;
        __instance.Name = ObjectName;
    }
}