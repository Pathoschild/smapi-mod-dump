/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

#nullable enable
namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Extensions;
using Utility;

using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

#endregion using directives

// ReSharper disable PossibleLossOfFraction
[UsedImplicitly]
internal class PondQueryMenuDrawPatch : BasePatch
{
    private const int PADDING_I = 16;

    private const float AQUARIST_SLOT_SPACING_F = 12f,
        LEGENDARY_SLOT_SPACING_F = 14f,
        LEGENDARY_X_OFFSET_F = 88f;

    private static readonly FieldInfo _FishPondData = typeof(FishPond).Field("_fishPondData");
    private static readonly MethodInfo _GetDisplayedText = typeof(PondQueryMenu).MethodNamed("getDisplayedText");
    private static readonly MethodInfo _MeasureExtraTextHeight = typeof(PondQueryMenu).MethodNamed("measureExtraTextHeight");
    private static readonly MethodInfo _DrawHorizontalPartition = typeof(PondQueryMenu).MethodNamed("drawHorizontalPartition");

    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuDrawPatch()
    {
        Original = RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] {typeof(SpriteBatch)});
    }

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPrefix]
    private static bool PondQueryMenuDrawPrefix(PondQueryMenu __instance, float ____age,
        Rectangle ____confirmationBoxRectangle, string ____confirmationText, bool ___confirmingEmpty,
        string ___hoverText, SObject ____fishItem, FishPond ____pond, SpriteBatch b)
    {
        try
        {
            var owner = Game1.getFarmerMaybeOffline(____pond.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession(Profession.Aquarist)) return true; // run original logic

            var fishPondData = (FishPondData?) _FishPondData.GetValue(____pond);
            if (fishPondData is null) return true; // run original logic

            var populationGates = fishPondData.PopulationGates;
            var isLegendaryPond = ____fishItem.HasContextTag("fish_legendary");
            if (____fishItem.IsAlgae() || populationGates is not null &&
                ____pond.lastUnlockedPopulationGate.Value < populationGates.Keys.Max() && !isLegendaryPond)
                return true; // run original logic

            if (!Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                         !____pond.hasCompletedRequest.Value;
                var pondNameText = Game1.content.LoadString(
                    PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
                    ____fishItem.DisplayName);
                var textSize = Game1.smallFont.MeasureString(pondNameText);
                Game1.DrawBox((int) (Game1.uiViewport.Width / 2 - (textSize.X + 64f) * 0.5f),
                    __instance.yPositionOnScreen - 4 + 128, (int) (textSize.X + 64f), 64);
                SUtility.drawTextWithShadow(b, pondNameText, Game1.smallFont,
                    new(Game1.uiViewport.Width / 2 - textSize.X * 0.5f,
                        __instance.yPositionOnScreen - 4 + 160f - textSize.Y * 0.5f), Color.Black);
                var displayedText = (string) _GetDisplayedText.Invoke(__instance, null)!;
                var extraHeight = 0;
                if (hasUnresolvedNeeds)
                    extraHeight += 116;

                var extraTextHeight = (int) _MeasureExtraTextHeight.Invoke(__instance, new object?[] {displayedText})!;
                Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen + 128,
                    PondQueryMenu.width, PondQueryMenu.height - 128 + extraHeight + extraTextHeight, false, true);
                var populationText = Game1.content.LoadString(
                    PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Population"),
                    string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
                textSize = Game1.smallFont.MeasureString(populationText);
                SUtility.drawTextWithShadow(b, populationText, Game1.smallFont,
                    new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                        __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                    Game1.textColor);
                
                var slotsToDraw = ____pond.maxOccupants.Value;
                var x = 0;
                var y = 0;
                var slotSpacing = isLegendaryPond ? LEGENDARY_SLOT_SPACING_F : AQUARIST_SLOT_SPACING_F;
                for (var i = 0; i < slotsToDraw; ++i)
                {
                    var yOffset = (float) Math.Sin(____age * 1f + x * 0.75f + y * 0.25f) * 2f;
                    var xPos = __instance.xPositionOnScreen - 20 + PondQueryMenu.width / 2 -
                        slotSpacing * Math.Min(slotsToDraw, 5) * 4f * 0.5f + slotSpacing * 4f * x - 12f;
                    if (isLegendaryPond) xPos += LEGENDARY_X_OFFSET_F;
                    var yPos = __instance.yPositionOnScreen + (int) (yOffset * 4f) + y * 4 * slotSpacing + 275.2f;

                    if (i < ____pond.FishCount)
                        ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 1f, 0f, StackDrawType.Hide, Color.White,
                            false);
                    else
                        ____fishItem.drawInMenu(b, new(xPos, yPos), 0.75f, 0.35f, 0f, StackDrawType.Hide, Color.Black,
                            false);

                    ++x;
                    if (x != (isLegendaryPond ? 3 : 6)) continue;

                    x = 0;
                    ++y;
                }

                textSize = Game1.smallFont.MeasureString(displayedText);
                SUtility.drawTextWithShadow(b, displayedText, Game1.smallFont,
                    new(__instance.xPositionOnScreen + PondQueryMenu.width / 2 - textSize.X * 0.5f,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight -
                        (hasUnresolvedNeeds ? 32 : 48) - textSize.Y), Game1.textColor);
                if (hasUnresolvedNeeds)
                {
                    _DrawHorizontalPartition.Invoke(__instance, new object?[]
                    {
                        b, (int) (__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f), false,
                        -1, -1, -1
                    });
                    SUtility.drawWithShadow(b, Game1.mouseCursors,
                        new(__instance.xPositionOnScreen + 60 + 8f * Game1.dialogueButtonScale / 10f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28),
                        new(412, 495, 5, 4), Color.White, (float) Math.PI / 2f, Vector2.Zero);
                    var bringText =
                        Game1.content.LoadString(
                            PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_StatusRequest_Bring"));
                    textSize = Game1.smallFont.MeasureString(bringText);
                    var leftX = __instance.xPositionOnScreen + 88;
                    float textX = leftX;
                    var iconX = textX + textSize.X + 4f;
                    if (LocalizedContentManager.CurrentLanguageCode.IsAnyOf(LocalizedContentManager.LanguageCode.ja,
                            LocalizedContentManager.LanguageCode.ko, LocalizedContentManager.LanguageCode.tr))
                    {
                        iconX = leftX - 8;
                        textX = leftX + 76;
                    }

                    SUtility.drawTextWithShadow(b, bringText, Game1.smallFont,
                        new(textX,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24),
                        Game1.textColor);
                    b.Draw(Game1.objectSpriteSheet,
                        new(iconX,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4),
                        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            ____pond.neededItem.Value?.ParentSheetIndex ?? 0, 16, 16), Color.Black * 0.4f, 0f,
                        Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    b.Draw(Game1.objectSpriteSheet,
                        new(iconX + 4f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight),
                        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            ____pond.neededItem.Value?.ParentSheetIndex ?? 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f,
                        SpriteEffects.None, 1f);
                    if (____pond.neededItemCount.Value > 1)
                        SUtility.drawTinyDigits(____pond.neededItemCount.Value, b,
                            new(iconX + 48f,
                                __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48), 3f, 1f,
                            Color.White);
                }

                __instance.okButton.draw(b);
                __instance.emptyButton.draw(b);
                __instance.changeNettingButton.draw(b);
                if (___confirmingEmpty)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds,
                        Color.Black * 0.75f);
                    ____confirmationBoxRectangle.Width += PADDING_I;
                    ____confirmationBoxRectangle.Height += PADDING_I;
                    ____confirmationBoxRectangle.X -= PADDING_I / 2;
                    ____confirmationBoxRectangle.Y -= PADDING_I / 2;
                    Game1.DrawBox(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y,
                        ____confirmationBoxRectangle.Width, ____confirmationBoxRectangle.Height);
                    ____confirmationBoxRectangle.Width -= PADDING_I;
                    ____confirmationBoxRectangle.Height -= PADDING_I;
                    ____confirmationBoxRectangle.X += PADDING_I / 2;
                    ____confirmationBoxRectangle.Y += PADDING_I / 2;
                    b.DrawString(Game1.smallFont, ____confirmationText,
                        new(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y),
                        Game1.textColor);
                    __instance.yesButton.draw(b);
                    __instance.noButton.draw(b);
                }
                else if (!string.IsNullOrEmpty(___hoverText))
                {
                    IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
                }
            }

            if (!ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveFishPonds") || ___confirmingEmpty)
                __instance.drawMouse(b);

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}