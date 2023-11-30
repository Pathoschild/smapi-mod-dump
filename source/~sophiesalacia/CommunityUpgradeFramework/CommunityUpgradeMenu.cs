/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace CommunityUpgradeFramework;

internal class CommunityUpgradeMenu : IClickableMenu
{
    private Dictionary<string, CommunityUpgrade> AvailableCommunityUpgrades;
    private string LocationKey;

    public readonly Texture2D MenuTexture = Game1.mouseCursors;

    public ClickableTextureComponent LeftArrow;
    public ClickableTextureComponent RightArrow;
    public ClickableTextureComponent PurchaseButton;

    private Rectangle MenuBounds;

    private static readonly Rectangle MenuBorderTexture = new Rectangle(384, 373, 18, 18);

    private const int DesiredWidth = 1024;
    private const int DesiredHeight = 768;
    private const int ThumbnailWidth = 512;
    private const int ThumbnailHeight = 512;
    private readonly Vector2 ScreenCenterPosition;

    private KeyValuePair<string, CommunityUpgrade> SelectedUpgrade;
    private const int TextSpacer = 20;

    public CommunityUpgradeMenu(string loc) : base(Game1.uiViewport.Width / 2 - (DesiredWidth + borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (DesiredHeight + borderWidth * 2) / 2, DesiredWidth + borderWidth * 2, DesiredHeight + borderWidth * 2, showUpperRightCloseButton: true)
    {
        LocationKey = loc;
        AvailableCommunityUpgrades =
            new Dictionary<string, CommunityUpgrade>(Game1.content.Load<Dictionary<string, CommunityUpgrade>>(Globals.CommunityUpgradesPath)
                .Where(cu => cu.Value.Location == LocationKey));

        if (AvailableCommunityUpgrades.Any())
        {
            SelectedUpgrade = AvailableCommunityUpgrades.First();
        }

        // logging
        foreach (var communityUpgrade in AvailableCommunityUpgrades)
        {
            Log.Trace(communityUpgrade.Value);
        }

        MenuBounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);

        LeftArrow = new ClickableTextureComponent(
            bounds: new Rectangle(xPositionOnScreen, yPositionOnScreen / 2, 32, 32),
            texture: MenuTexture,
            sourceRect: new Rectangle(353, 494, 12, 11),
            scale: 4f
        )
        {
            myID = 106,
            leftNeighborID = 97865,
            rightNeighborID = 3546
        };

        RightArrow = new ClickableTextureComponent(
            bounds: new Rectangle(xPositionOnScreen + width, yPositionOnScreen / 2, 32, 32),
            texture: MenuTexture,
            sourceRect: new Rectangle(365, 494, 12, 11),
            scale: 4f
        )
        {
            myID = 97865,
            leftNeighborID = 3546,
            rightNeighborID = 106
        };

        ScreenCenterPosition = new Vector2(Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2);
    }

    public override void draw(SpriteBatch b)
    {
        b.Draw(texture: Game1.fadeToBlackRect, destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds, color: Color.Black * 0.75f);

        // main menu
        drawTextureBox(b: b,
            texture: MenuTexture,
            sourceRect: MenuBorderTexture,
            x: xPositionOnScreen,
            y: yPositionOnScreen,
            width: width,
            height: height,
            color: Color.White,
            scale: 4f
        );

        // thumbnail box
        drawTextureBox(b: b,
            texture: MenuTexture,
            sourceRect: MenuBorderTexture,
            x: xPositionOnScreen,
            y: yPositionOnScreen,
            width: ThumbnailWidth + borderWidth * 2,
            height: ThumbnailHeight + borderWidth * 2,
            color: Color.White,
            scale: 4f
        );

        if (SelectedUpgrade.Key is not null)
        {
            CommunityUpgrade cu = SelectedUpgrade.Value;

            Vector2 textDrawPos = new(x: xPositionOnScreen + ThumbnailWidth + borderWidth * 2f + TextSpacer,
                y: yPositionOnScreen + 1.5f * TextSpacer);

            string titleText = Game1.parseText(text: cu.Name, whichFont: Game1.dialogueFont, width: ThumbnailWidth);

            Utility.drawTextWithShadow(
                b: b,
                text: titleText,
                font: Game1.dialogueFont,
                position: textDrawPos,
                color: Game1.textColor,
                scale: 1.5f
            );

            textDrawPos += new Vector2(x: 0f, y: 6 * TextSpacer);

            Utility.drawTextWithShadow(
                b: b,
                text: "Costs:",
                font: Game1.dialogueFont,
                position: textDrawPos,
                color: Game1.textColor,
                scale: 1f
            );

            textDrawPos += new Vector2(x: 0f, y: 2 * TextSpacer);

            foreach (var cost in cu.ItemPriceDict)
            {
                // draw icon
                b.Draw(
                    texture: Game1.objectSpriteSheet,
                    position: textDrawPos,
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.objectSpriteSheet, tilePosition: cost.Key, width: 16, height: 16),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 2f,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );

                textDrawPos += new Vector2(40f, -7f);

                Utility.drawTextWithShadow(
                    b: b,
                    text: cost.Value.ToString(),
                    font: Game1.dialogueFont,
                    position: textDrawPos,
                    color: Game1.textColor,
                    scale: 1f
                );

                textDrawPos += new Vector2(-40f, 47f);
            }

            textDrawPos += new Vector2(0f, TextSpacer);

            foreach (var cost in cu.CurrencyPriceDict)
            {
                switch (cost.Key)
                {
                    case "Gold":
                    {
                        b.Draw(
                            texture: Game1.mouseCursors,
                            position: textDrawPos + new Vector2(4f, 0f),
                            sourceRectangle: new Rectangle(280, 411, 16, 16),
                            color: Color.White,
                            rotation: 0f,
                            origin: Vector2.Zero,
                            scale: 2f,
                            effects: SpriteEffects.None,
                            layerDepth: 0f
                        );

                        textDrawPos += new Vector2(40f, -7f);

                        Utility.drawTextWithShadow(
                            b: b,
                            text: cost.Value.ToString(),
                            font: Game1.dialogueFont,
                            position: textDrawPos,
                            color: Game1.textColor,
                            scale: 1f
                        );

                        textDrawPos += new Vector2(-40f, 47f);

                        break;
                    }
                }
            }

            textDrawPos += new Vector2(0f, TextSpacer);
        }

        if (AvailableCommunityUpgrades.Count > 1)
        {
            RightArrow.draw(b);
            LeftArrow.draw(b);
        }
        else if (AvailableCommunityUpgrades.Count == 0)
        {
            drawTextureBox(
                b: b,
                texture: MenuTexture,
                sourceRect: MenuBorderTexture,
                x: xPositionOnScreen,
                y: yPositionOnScreen,
                width: width,
                height: height,
                color: Color.White,
                scale: 4f
            );

            Utility.drawBoldText(
                b: b,
                text: "There are no community upgrades available at the moment.",
                font: Game1.dialogueFont,
                position: ScreenCenterPosition - new Vector2(x: Game1.dialogueFont.MeasureString(text: "There are no community upgrades available at the moment.").X / 2, y: 0f),
                color: Color.Black,
                scale: 1f
            );
        }

        upperRightCloseButton.draw(b);

        drawMouse(b);
    }
}
