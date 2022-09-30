/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewRoguelike;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewValley.Menus
{
    public class GambaMenu : IClickableMenu
    {
        private enum SpinResult
        {
            Gold,
            Monsters,
            Sword,
            Boots,
            Food,
            Ring,
            Death,
            Respin
        }

        public new const int width = 640;

        public new const int height = 448;

        public double arrowRotation;

        public double arrowRotationVelocity;

        public double arrowRotationDeceleration;

        private int timerBeforeStart;

        private SparklingText resultText;

        private SpinResult result;

        private bool doneSpinning;

        private float spinButtonScale = 1f;

        private bool spinButtonPressed = false;

        private Color spinButtonColor = Game1.textColor;

        private int spinsLeft = 3;

        private ClickableComponent spinButton;

        private ClickableComponent spinsLeftBox;

        public static readonly Rectangle textureBoxRect = new(0, 256, 60, 60);

        public GambaMenu()
            : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height / 2 - 224, 640, 448)
        {
            SetSpinParameters();
            SetUpPositions();

            if (Game1.options.gamepadControls)
                snapToDefaultClickableComponent();
        }

        public void SetSpinParameters()
        {
            arrowRotationVelocity = Math.PI / 16.0;
            arrowRotationVelocity += Game1.random.Next(0, 20) * Math.PI / 256.0;
            arrowRotationDeceleration = -0.00062831853071795862;

            if (Game1.random.NextDouble() < 0.1)
                arrowRotationVelocity += Math.PI / 64.0;
            else if (Game1.random.NextDouble() < 0.4)
                arrowRotationVelocity += Math.PI / 96.0;
            else if (Game1.random.NextDouble() < 0.8)
                arrowRotationVelocity += Math.PI / 128.0;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            SetUpPositions();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = spinButton;
            snapCursorToCurrentSnappedComponent();
        }

        private void SetUpPositions()
        {
            string spinText = "Spin";
            Vector2 spinTextSize = Game1.smallFont.MeasureString(spinText);
            spinButton = new(new(xPositionOnScreen + (width / 2) - (int)(spinTextSize.X / 2), yPositionOnScreen + height - 32, (int)spinTextSize.X, (int)spinTextSize.Y), "spinButton", spinText)
            {
                myID = 101
            };

            string spinsLeftText = $"Spins Left: {spinsLeft}";
            Vector2 spinsLeftTextSize = Game1.smallFont.MeasureString(spinsLeftText);
            spinsLeftBox = new(new(xPositionOnScreen + (width / 2) - (int)(spinsLeftTextSize.X / 2), yPositionOnScreen + height + 32, (int)spinsLeftTextSize.X, (int)spinsLeftTextSize.Y), "spinsLeft", spinsLeftText);
        }

        private bool CanSpin()
        {
            return !spinButtonPressed && resultText is null && spinsLeft > 0;
        }

        private void PerformResult()
        {
            if (Game1.player.currentLocation is not MineShaft || result == SpinResult.Respin)
                return;

            MineShaft mine = (MineShaft)Game1.player.currentLocation;
            MerchantFloor merchantFloor = Merchant.GetNextMerchantFloor(mine);

            switch (result)
            {
                case SpinResult.Gold:
                    Game1.player.addItemByMenuIfNecessary(new Object(384, 15));
                    break;
                case SpinResult.Monsters:

                    int monstersToSpawn = 7;
                    if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                        monstersToSpawn += 2;

                    Vector2 playerLocation = Game1.player.getTileLocation();

                    while (monstersToSpawn > 0)
                    {
                        Vector2 randomTile = new(
                            playerLocation.X + Game1.random.Next(-6, 6),
                            playerLocation.Y + Game1.random.Next(-6, 6)
                        );
                        if (!mine.isTileLocationTotallyClearAndPlaceable((int)randomTile.X, (int)randomTile.Y))
                            continue;

                        Monster monster = mine.BuffMonsterIfNecessary(mine.getMonsterForThisLevel(mine.mineLevel, (int)randomTile.X, (int)randomTile.Y));
                        Roguelike.AdjustMonster(mine, ref monster);
                        mine.characters.Add(monster);
                        monstersToSpawn--;
                    }

                    Game1.exitActiveMenu();

                    break;
                case SpinResult.Sword:
                    MeleeWeapon sword = merchantFloor.PickAnySword();
                    Game1.player.addItemByMenuIfNecessary(sword);
                    break;
                case SpinResult.Boots:
                    Boots boots = merchantFloor.PickAnyBoots();
                    Game1.player.addItemByMenuIfNecessary(boots);
                    break;
                case SpinResult.Food:
                    Object food = merchantFloor.PickAnyFood();
                    Game1.player.addItemByMenuIfNecessary(food);
                    break;
                case SpinResult.Ring:
                    Ring ring = merchantFloor.PickAnyRing();
                    Game1.player.addItemByMenuIfNecessary(ring);
                    break;
                case SpinResult.Death:
                    Game1.exitActiveMenu();
                    Game1.player.takeDamage(Game1.player.health * 2, true, null);
                    break;
                default:
                    throw new NotImplementedException("whoopsies");
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (timerBeforeStart <= 0 && spinButtonPressed)
            {
                arrowRotationVelocity += arrowRotationDeceleration;

                if (arrowRotationVelocity <= 0.0 && !doneSpinning)
                {
                    doneSpinning = true;
                    arrowRotationDeceleration = 0.0;
                    arrowRotationVelocity = 0.0;

                    double degrees = arrowRotation * (180 / Math.PI);

                    if (degrees > 315)
                    {
                        result = SpinResult.Gold;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Free Gold!", Color.Lime, Color.White);
                    }
                    else if (degrees > 270)
                    {
                        doneSpinning = false;
                        SetSpinParameters();
                        timerBeforeStart = 1000;

                        result = SpinResult.Respin;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Respin!", Color.Lime, Color.White);
                        return;
                    }
                    else if (degrees > 225)
                    {
                        result = SpinResult.Monsters;
                        resultText = new SparklingText(Game1.dialogueFont, "Monsters!", Color.Red, Color.Transparent);
                        Game1.playSound("fishEscape");
                    }
                    else if (degrees > 180)
                    {
                        result = SpinResult.Sword;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Free Sword!", Color.Lime, Color.White);
                    }
                    else if (degrees > 135)
                    {
                        result = SpinResult.Boots;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Free Boots!", Color.Lime, Color.White);
                    }
                    else if (degrees > 90)
                    {
                        result = SpinResult.Food;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Free Food!", Color.Lime, Color.White);
                    }
                    else if (degrees > 45)
                    {
                        result = SpinResult.Ring;
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, "Free Ring!", Color.Lime, Color.White);
                    }
                    else
                    {
                        result = SpinResult.Death;
                        resultText = new SparklingText(Game1.dialogueFont, "Death!", Color.Red, Color.Transparent);
                        Game1.playSound("fishEscape");
                    }

                    spinButtonPressed = false;
                    spinsLeft--;
                    SetUpPositions();
                }

                double num = arrowRotation;
                arrowRotation += arrowRotationVelocity;
                if (num % (Math.PI / 4.0) > arrowRotation % (Math.PI / 4.0))
                    Game1.playSound("Cowboy_gunshot");

                arrowRotation %= Math.PI * 2.0;
            }
            else if (timerBeforeStart >= 0)
            {
                timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
                if (timerBeforeStart <= 0)
                    Game1.playSound("cowboy_monsterhit");
            }

            if (resultText is not null && resultText.update(time))
            {
                PerformResult();
                resultText = null;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (spinButton != null && spinButton.containsPoint(x, y) && CanSpin())
            {
                if (playSound)
                    Game1.playSound("bigSelect");
                spinButtonPressed = true;
                timerBeforeStart = 500;
                doneSpinning = false;
                SetSpinParameters();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            if (spinButton.containsPoint(x, y) && CanSpin())
            {
                spinButtonScale = Math.Min(spinButtonScale + 0.04f, 1f + 0.25f);
                spinButtonColor = Game1.unselectedOptionColor;
            }
            else
            {
                spinButtonScale = Math.Max(spinButtonScale - 0.04f, 1f);
                spinButtonColor = Game1.textColor;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            Keys menuKey = Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
            Keys journalKey = Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.journalButton);
            if ((key == menuKey || key == journalKey) && (CanSpin() || spinsLeft == 0))
                Game1.exitActiveMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);

            Keys key = Utility.mapGamePadButtonToKey(b);
            receiveKeyPress(key);

            if (currentlySnappedComponent is null)
                snapToDefaultClickableComponent();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(128, 1184, 160, 112), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.95f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 320, yPositionOnScreen + 224 + 4), new Rectangle(120, 1234, 8, 16), Color.White, (float)arrowRotation, new Vector2(4f, 15f), 4f, SpriteEffects.None, 0.96f);

            drawTextureBox(
                b,
                Game1.menuTexture,
                textureBoxRect,
                spinButton.bounds.X - 16,
                spinButton.bounds.Y - 16,
                spinButton.bounds.Width + 32,
                spinButton.bounds.Height + 24,
                Color.White,
                drawShadow: false,
                scale: spinButtonScale
            );

            Utility.drawBoldText(
                b,
                spinButton.label,
                Game1.smallFont,
                new Vector2(
                    spinButton.bounds.X,
                    spinButton.bounds.Y
                ),
                spinButtonColor
            );

            drawTextureBox(
                b,
                Game1.menuTexture,
                textureBoxRect,
                spinsLeftBox.bounds.X - 16,
                spinsLeftBox.bounds.Y - 16,
                spinsLeftBox.bounds.Width + 32,
                spinsLeftBox.bounds.Height + 24,
                Color.White,
                drawShadow: false
            );

            Utility.drawTextWithShadow(
                b,
                spinsLeftBox.label,
                Game1.smallFont,
                new Vector2(
                    spinsLeftBox.bounds.X,
                    spinsLeftBox.bounds.Y
                ),
                Game1.textColor
            );

            if (resultText is not null)
                resultText.draw(b, new(xPositionOnScreen + (width / 2) - (resultText.textWidth / 2), yPositionOnScreen));

            drawMouse(b);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
