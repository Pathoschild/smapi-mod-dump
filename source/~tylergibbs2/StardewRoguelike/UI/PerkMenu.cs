/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace StardewRoguelike.UI
{
    public class PerkMenu : IClickableMenu
    {
        public const int region_okButton = 101;

        public const int region_leftProfession = 102;

        public const int region_rightProfession = 103;

        public const int basewidth = 768;

        public const int baseheight = 512;

        public bool informationUp;

        public bool isActive;

        public bool hasUpdatedProfessions;

        private bool alreadyUsed = false;

        private Color leftPerkColor = Game1.textColor;

        private Color rightPerkColor = Game1.textColor;

        private MouseState oldMouseState;

        public ClickableTextureComponent starIcon = null!;

        public ClickableTextureComponent okButton;

        public ClickableComponent leftPerkComponent;

        public ClickableComponent rightPerkComponent;

        public Perks.PerkType? leftPerk;

        public Perks.PerkType? rightPerk;

        private List<string> extraInfoForLevel = new();

        private List<string> leftPerkDescription = new();

        private List<string> rightPerkDescription = new();

        private Perks.PerkRarity leftPerkRarity;

        private Perks.PerkRarity rightPerkRarity;

        private Rectangle sourceRectForLevelIcon;

        private string title { get => I18n.UI_PerkMenu_Title(); }

        private readonly List<TemporaryAnimatedSprite> littleStars = new();

        public bool hasMovedSelection;

        public PerkMenu()
            : base(Game1.uiViewport.Width / 2 - 384, Game1.uiViewport.Height / 2 - 256, 768, 512)
        {
            isActive = true;
            width = 1088;
            height = 640;
            okButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - 64 - borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 101
            };
            Game1.player.completelyStopAnimatingOrDoingAction();
            informationUp = true;

            sourceRectForLevelIcon = new Rectangle(128, 16, 16, 16);

            int newHeight = 0;
            height = newHeight + 256 + extraInfoForLevel.Count * 64 * 3 / 4;
            gameWindowSizeChanged(Rectangle.Empty, Rectangle.Empty);

            leftPerkComponent = new ClickableComponent(new Rectangle(xPositionOnScreen, yPositionOnScreen + 128, width / 2, height), "")
            {
                myID = region_leftProfession,
                rightNeighborID = region_rightProfession
            };
            rightPerkComponent = new ClickableComponent(new Rectangle(xPositionOnScreen + (width / 2), yPositionOnScreen + 128, width / 2, height), "")
            {
                myID = region_rightProfession,
                leftNeighborID = region_leftProfession
            };

            var (perk1, perk2) = Perks.GetTwoRandomUniquePerks();
            if (perk1.HasValue)
                leftPerk = perk1.Value;
            if (perk2.HasValue)
                rightPerk = perk2.Value;

            leftPerkDescription = GetPerkDescription(leftPerk);
            rightPerkDescription = GetPerkDescription(rightPerk);

            if (leftPerk.HasValue)
                leftPerkRarity = Perks.GetPerkRarity(leftPerk.Value);
            if (rightPerk.HasValue)
                rightPerkRarity = Perks.GetPerkRarity(rightPerk.Value);

            populateClickableComponentList();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(region_leftProfession);
            snapCursorToCurrentSnappedComponent();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - 256;
            RepositionOkButton();
        }

        public virtual void RepositionOkButton()
        {
            okButton.bounds = new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - 64 - borderWidth, 64, 64);
            if (okButton.bounds.Right > Game1.uiViewport.Width)
                okButton.bounds.X = Game1.uiViewport.Width - 64;

            if (okButton.bounds.Bottom > Game1.uiViewport.Height)
                okButton.bounds.Y = Game1.uiViewport.Height - 64;
        }


        public static List<string> GetPerkDescription(Perks.PerkType? perkType)
        {
            if (!perkType.HasValue)
                return new();

            return new()
            {
                Perks.GetPerkDisplayName(perkType.Value),
                Perks.GetPerkDescription(perkType.Value)
            };
        }

        public static Color GetRarityColor(Perks.PerkRarity perkRarity)
        {
            return perkRarity switch
            {
                Perks.PerkRarity.Common => new(3, 153, 63),
                Perks.PerkRarity.Uncommon => new(3, 157, 252),
                _ => new(157, 3, 252)
            };
        }

        public static string GetRarityText(Perks.PerkRarity perkRarity)
        {
            return perkRarity switch
            {
                Perks.PerkRarity.Common => I18n.UI_PerkMenu_Rarity_Common(),
                Perks.PerkRarity.Uncommon => I18n.UI_PerkMenu_Rarity_Uncommon(),
                _ => I18n.UI_PerkMenu_Rarity_Rare()
            };
        }

        public override void update(GameTime time)
        {
            if (!isActive)
            {
                exitThisMenu();
                return;
            }

            for (int i = littleStars.Count - 1; i >= 0; i--)
            {
                if (littleStars[i].update(time))
                    littleStars.RemoveAt(i);
            }

            if (Game1.random.NextDouble() < 0.03)
            {
                Vector2 position = new(0f, Game1.random.Next(yPositionOnScreen - 128, yPositionOnScreen - 4) / 20 * 4 * 5 + 32);
                if (Game1.random.NextDouble() < 0.5)
                    position.X = Game1.random.Next(xPositionOnScreen + width / 2 - 228, xPositionOnScreen + width / 2 - 132);
                else
                    position.X = Game1.random.Next(xPositionOnScreen + width / 2 + 116, xPositionOnScreen + width - 160);

                if (position.Y < yPositionOnScreen - 64 - 8)
                    position.X = Game1.random.Next(xPositionOnScreen + width / 2 - 116, xPositionOnScreen + width / 2 + 116);

                position.X = position.X / 20f * 4f * 5f;
                littleStars.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(364, 79, 5, 5), 80f, 7, 1, position, flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                {
                    local = true
                });
            }

            if (isActive)
            {
                leftPerkColor = Game1.textColor;
                rightPerkColor = Game1.textColor;

                Game1.player.completelyStopAnimatingOrDoingAction();
                if (Game1.getMouseY() > yPositionOnScreen + 192 && Game1.getMouseY() < yPositionOnScreen + height && !alreadyUsed)
                {
                    if (Game1.getMouseX() > xPositionOnScreen && Game1.getMouseX() < xPositionOnScreen + width / 2 && leftPerk.HasValue)
                    {
                        leftPerkColor = Color.Green;
                        if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)) && readyToClose())
                        {
                            Perks.AddPerk(leftPerk.Value);
                            isActive = false;
                            informationUp = false;
                            alreadyUsed = true;
                        }
                    }
                    else if (Game1.getMouseX() > xPositionOnScreen + width / 2 && Game1.getMouseX() < xPositionOnScreen + width && rightPerk.HasValue)
                    {
                        rightPerkColor = Color.Green;
                        if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)) && readyToClose())
                        {
                            Perks.AddPerk(rightPerk.Value);
                            isActive = false;
                            informationUp = false;
                            alreadyUsed = true;
                        }
                    }
                }
                height = 512;
            }
            oldMouseState = Game1.input.GetMouseState();
            if (isActive && !informationUp && starIcon is not null)
            {
                if (starIcon.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    starIcon.sourceRect.X = 294;
                else
                    starIcon.sourceRect.X = 310;
            }

            if (!isActive || !informationUp)
                return;

            Game1.player.completelyStopAnimatingOrDoingAction();
            if (okButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                okButton.scale = Math.Min(1.1f, okButton.scale + 0.05f);
                if ((oldMouseState.LeftButton == ButtonState.Pressed || Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A)) && readyToClose())
                    okButtonClicked();
            }
            else
                okButton.scale = Math.Max(1f, okButton.scale - 0.05f);
        }

        public void okButtonClicked()
        {
            isActive = false;
            informationUp = false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            foreach (TemporaryAnimatedSprite littleStar in littleStars)
                littleStar.draw(b);

            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + width / 2 - 116, yPositionOnScreen - 32 + 12), new Rectangle(363, 87, 58, 22), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            if (!informationUp && isActive && starIcon is not null)
                starIcon.draw(b);
            else
            {
                if (!informationUp)
                    return;

                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, speaker: false, drawOnlyBox: true);
                drawHorizontalPartition(b, yPositionOnScreen + 192);

                // perk vertical dividing lines
                if ((leftPerk.HasValue || rightPerk.HasValue) && !alreadyUsed)
                    drawVerticalIntersectingPartition(b, xPositionOnScreen + width / 2 - 32, yPositionOnScreen + 192);

                // Header
                Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2(xPositionOnScreen + spaceToClearSideBorder + borderWidth, yPositionOnScreen + spaceToClearTopBorder + 16), sourceRectForLevelIcon, Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.88f);
                b.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, yPositionOnScreen + spaceToClearTopBorder + 16), Game1.textColor);
                Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2(xPositionOnScreen + width - spaceToClearSideBorder - borderWidth - 64, yPositionOnScreen + spaceToClearTopBorder + 16), sourceRectForLevelIcon, Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.88f);
                string chooseProfession = I18n.UI_PerkMenu_DoPick();
                b.DrawString(Game1.smallFont, chooseProfession, new Vector2(xPositionOnScreen + width / 2 - Game1.smallFont.MeasureString(chooseProfession).X / 2f, yPositionOnScreen + 64 + spaceToClearTopBorder), Game1.textColor);

                // left perk
                if (leftPerk.HasValue && !alreadyUsed)
                {
                    b.DrawString(Game1.dialogueFont, leftPerkDescription[0], new Vector2(xPositionOnScreen + spaceToClearSideBorder + 32, yPositionOnScreen + spaceToClearTopBorder + 160), leftPerkColor);
                    b.DrawString(Game1.smallFont, Game1.parseText(GetRarityText(leftPerkRarity), Game1.smallFont, width / 3 - 64), new Vector2(xPositionOnScreen + spaceToClearSideBorder + 32, yPositionOnScreen + spaceToClearTopBorder + 208), GetRarityColor(leftPerkRarity));
                    b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + spaceToClearSideBorder + width / 2 - 112, yPositionOnScreen + spaceToClearTopBorder + 160 - 16), Perks.GetPerkSourceRect(leftPerk.Value), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    for (int j = 1; j < leftPerkDescription.Count; j++)
                        b.DrawString(Game1.smallFont, Game1.parseText(leftPerkDescription[j], Game1.smallFont, width / 3 - 64), new Vector2(-4 + xPositionOnScreen + spaceToClearSideBorder + 32, yPositionOnScreen + spaceToClearTopBorder + 128 + 8 + 64 * (j + 1)), leftPerkColor);
                }

                // right perk
                if (rightPerk.HasValue && !alreadyUsed)
                {
                    b.DrawString(Game1.dialogueFont, rightPerkDescription[0], new Vector2(xPositionOnScreen + spaceToClearSideBorder + width / 2, yPositionOnScreen + spaceToClearTopBorder + 160), rightPerkColor);
                    b.DrawString(Game1.smallFont, Game1.parseText(GetRarityText(rightPerkRarity), Game1.smallFont, width / 3 - 64), new Vector2(xPositionOnScreen + spaceToClearSideBorder + width / 2, yPositionOnScreen + spaceToClearTopBorder + 208), GetRarityColor(rightPerkRarity));
                    b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + spaceToClearSideBorder + width - 128, yPositionOnScreen + spaceToClearTopBorder + 160 - 16), Perks.GetPerkSourceRect(rightPerk.Value), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    for (int i = 1; i < rightPerkDescription.Count; i++)
                        b.DrawString(Game1.smallFont, Game1.parseText(rightPerkDescription[i], Game1.smallFont, width / 3 - 48), new Vector2(-4 + xPositionOnScreen + spaceToClearSideBorder + width / 2, yPositionOnScreen + spaceToClearTopBorder + 128 + 8 + 64 * (i + 1)), rightPerkColor);
                }

                if (!leftPerk.HasValue && !rightPerk.HasValue)
                {
                    string allUnlocked = I18n.UI_PerkMenu_HasAllPerks();
                    b.DrawString(Game1.smallFont, allUnlocked, new Vector2(xPositionOnScreen + width / 2 - Game1.smallFont.MeasureString(allUnlocked).X / 2f, yPositionOnScreen + 232 + spaceToClearTopBorder), Game1.textColor);
                }
                else if (alreadyUsed)
                {
                    string alreadyUsedText = I18n.UI_PerkMenu_AlreadyClaimed();
                    b.DrawString(Game1.smallFont, alreadyUsedText, new Vector2(xPositionOnScreen + width / 2 - Game1.smallFont.MeasureString(alreadyUsedText).X / 2f, yPositionOnScreen + 232 + spaceToClearTopBorder), Game1.textColor);
                }

                drawMouse(b);
            }
        }
    }
}
