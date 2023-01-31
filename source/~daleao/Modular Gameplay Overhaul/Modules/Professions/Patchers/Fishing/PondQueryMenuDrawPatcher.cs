/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuDrawPatcher"/> class.</summary>
    internal PondQueryMenuDrawPatcher()
    {
        this.Target = this.RequireMethod<PondQueryMenu>(nameof(PondQueryMenu.draw), new[] { typeof(SpriteBatch) });
        this.Prefix!.priority = Priority.HigherThanNormal;
        this.Prefix!.after = new[] { OverhaulModule.Ponds.Namespace };
    }

    private delegate void DrawHorizontalPartitionDelegate(
        IClickableMenu instance, SpriteBatch b, int yPosition, bool small = false, int red = -1, int green = -1, int blue = -1);

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyAfter("DaLion.Overhaul.Modules.Ponds")]
    private static bool PondQueryMenuDrawPrefix(
        PondQueryMenu __instance,
        float ____age,
        Rectangle ____confirmationBoxRectangle,
        string ____confirmationText,
        bool ___confirmingEmpty,
        string ___hoverText,
        SObject ____fishItem,
        FishPond ____pond,
        SpriteBatch b)
    {
        try
        {
            if (!____pond.GetOwner().HasProfession(Profession.Aquarist) && !(ProfessionsModule.Config.LaxOwnershipRequirements &&
                                                                             Game1.game1.DoesAnyPlayerHaveProfession(
                                                                                 Profession.Aquarist, out _)))
            {
                return true; // run original logic
            }

            var fishPondData = Reflector
                .GetUnboundFieldGetter<FishPond, FishPondData?>(____pond, "_fishPondData")
                .Invoke(____pond);
            var populationGates = fishPondData?.PopulationGates;
            var isLegendaryPond = ____fishItem.HasContextTag("fish_legendary");
            if (!isLegendaryPond && populationGates is not null &&
                ____pond.lastUnlockedPopulationGate.Value < populationGates.Keys.Max())
            {
                return true; // run original logic
            }

            if (Game1.globalFade)
            {
                __instance.drawMouse(b);
                return false; // don't run original logic
            }

            // draw stuff
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            var hasUnresolvedNeeds = ____pond.neededItem.Value is not null && ____pond.HasUnresolvedNeeds() &&
                                     !____pond.hasCompletedRequest.Value;
            var pondNameText = Game1.content.LoadString(
                PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Name"),
                ____fishItem.DisplayName);
            var textSize = Game1.smallFont.MeasureString(pondNameText);
            Game1.DrawBox(
                (int)((Game1.uiViewport.Width / 2) - ((textSize.X + 64f) * 0.5f)),
                __instance.yPositionOnScreen - 4 + 128,
                (int)(textSize.X + 64f),
                64);
            Utility.drawTextWithShadow(
                b,
                pondNameText,
                Game1.smallFont,
                new Vector2(
                    (Game1.uiViewport.Width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen - 4 + 160f - (textSize.Y * 0.5f)),
                Color.Black);
            var displayedText = Reflector
                .GetUnboundMethodDelegate<Func<PondQueryMenu, string>>(__instance, "getDisplayedText")
                .Invoke(__instance);
            var extraHeight = 0;
            if (hasUnresolvedNeeds)
            {
                extraHeight += 116;
            }

            var extraTextHeight = Reflector
                .GetUnboundMethodDelegate<Func<PondQueryMenu, string, int>>(__instance, "measureExtraTextHeight")
                .Invoke(__instance, displayedText);
            Game1.drawDialogueBox(
                __instance.xPositionOnScreen,
                __instance.yPositionOnScreen + 128,
                PondQueryMenu.width,
                PondQueryMenu.height - 128 + extraHeight + extraTextHeight,
                false,
                true);
            var populationText = Game1.content.LoadString(
                PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_Population"),
                string.Concat(____pond.FishCount),
                ____pond.maxOccupants.Value);
            textSize = Game1.smallFont.MeasureString(populationText);
            Utility.drawTextWithShadow(
                b,
                populationText,
                Game1.smallFont,
                new Vector2(
                    __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128),
                Game1.textColor);

            // draw fish
            int x = 0, y = 0;
            var slotsToDraw = ____pond.maxOccupants.Value;
            var columns = (int)Math.Ceiling(slotsToDraw / 2f);
            var slotSpacing = 18 - columns;
            for (var i = 0; i < slotsToDraw; i++)
            {
                var yOffset = (float)Math.Sin(____age + (x * 0.75f) + (y * 0.25f)) * 2f;
                var yPos = __instance.yPositionOnScreen + (int)(yOffset * 4f) + (y * slotSpacing * 4f) + 275.2f;
                var xPos = __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (columns * slotSpacing * 2f) +
                           (x * slotSpacing * 4f);
                if (i < ____pond.FishCount)
                {
                    ____fishItem.drawInMenu(
                        b,
                        new Vector2(xPos, yPos),
                        0.75f,
                        1f,
                        0f,
                        StackDrawType.Hide,
                        Color.White,
                        false);
                }
                else
                {
                    ____fishItem.drawInMenu(
                        b,
                        new Vector2(xPos, yPos),
                        0.75f,
                        0.35f,
                        0f,
                        StackDrawType.Hide,
                        Color.Black,
                        false);
                }

                if (++x != columns)
                {
                    continue;
                }

                x = 0;
                y++;
            }

            // draw more stuff
            textSize = Game1.smallFont.MeasureString(displayedText);
            Utility.drawTextWithShadow(
                b,
                displayedText,
                Game1.smallFont,
                new Vector2(
                    __instance.xPositionOnScreen + (PondQueryMenu.width / 2) - (textSize.X * 0.5f),
                    __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - (hasUnresolvedNeeds ? 32 : 48) - textSize.Y),
                Game1.textColor);

            if (hasUnresolvedNeeds)
            {
                Reflector.GetUnboundMethodDelegate<DrawHorizontalPartitionDelegate>(__instance, "drawHorizontalPartition").Invoke(
                    __instance,
                    b,
                    (int)(__instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight - 48f));
                Utility.drawWithShadow(
                    b,
                    Game1.mouseCursors,
                    new Vector2(
                        __instance.xPositionOnScreen + 60 + (8f * Game1.dialogueButtonScale / 10f),
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 28),
                    new Rectangle(412, 495, 5, 4),
                    Color.White,
                    (float)Math.PI / 2f,
                    Vector2.Zero);

                var bringText =
                    Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/UI:PondQuery_StatusRequest_Bring"));
                textSize = Game1.smallFont.MeasureString(bringText);
                var leftX = __instance.xPositionOnScreen + 88;
                float textX = leftX;
                var iconX = textX + textSize.X + 4f;
                if (LocalizedContentManager.CurrentLanguageCode.IsIn(
                        LocalizedContentManager.LanguageCode.ja,
                        LocalizedContentManager.LanguageCode.ko,
                        LocalizedContentManager.LanguageCode.tr))
                {
                    iconX = leftX - 8;
                    textX = leftX + 76;
                }

                Utility.drawTextWithShadow(
                    b,
                    bringText,
                    Game1.smallFont,
                    new Vector2(
                        textX,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 24),
                    Game1.textColor);

                b.Draw(
                    Game1.objectSpriteSheet,
                    new Vector2(
                        iconX,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 4),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        16,
                        16),
                    Color.Black * 0.4f,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                b.Draw(
                    Game1.objectSpriteSheet,
                    new Vector2(
                        iconX + 4f,
                        __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        ____pond.neededItem.Value?.ParentSheetIndex ?? 0,
                        16,
                        16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                if (____pond.neededItemCount.Value > 1)
                {
                    Utility.drawTinyDigits(
                        ____pond.neededItemCount.Value,
                        b,
                        new Vector2(
                            iconX + 48f,
                            __instance.yPositionOnScreen + PondQueryMenu.height + extraTextHeight + 48),
                        3f,
                        1f,
                        Color.White);
                }
            }

            __instance.okButton.draw(b);
            __instance.emptyButton.draw(b);
            __instance.changeNettingButton.draw(b);
            if (___confirmingEmpty)
            {
                b.Draw(
                    Game1.fadeToBlackRect,
                    Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    Color.Black * 0.75f);

                const int padding = 16;
                ____confirmationBoxRectangle.Width += padding;
                ____confirmationBoxRectangle.Height += padding;
                ____confirmationBoxRectangle.X -= padding / 2;
                ____confirmationBoxRectangle.Y -= padding / 2;
                Game1.DrawBox(
                    ____confirmationBoxRectangle.X,
                    ____confirmationBoxRectangle.Y,
                    ____confirmationBoxRectangle.Width,
                    ____confirmationBoxRectangle.Height);

                ____confirmationBoxRectangle.Width -= padding;
                ____confirmationBoxRectangle.Height -= padding;
                ____confirmationBoxRectangle.X += padding / 2;
                ____confirmationBoxRectangle.Y += padding / 2;
                b.DrawString(
                    Game1.smallFont,
                    ____confirmationText,
                    new Vector2(____confirmationBoxRectangle.X, ____confirmationBoxRectangle.Y),
                    Game1.textColor);

                __instance.yesButton.draw(b);
                __instance.noButton.draw(b);
            }
            else if (!string.IsNullOrEmpty(___hoverText))
            {
                IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
            }

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
