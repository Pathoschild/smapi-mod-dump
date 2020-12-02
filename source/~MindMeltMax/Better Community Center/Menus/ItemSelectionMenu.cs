/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using BCC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BCC.Menus
{
    public class ItemSelectionMenu : IClickableMenu
    {
        private string hoverText = "";
        private List<List<ClickableTextureComponent>> Items = new List<List<ClickableTextureComponent>>();
        private ClickableTextureComponent backButton;
        private ClickableTextureComponent forwardButton;
        private int Page;
        private int LastPage;
        private string Title;

        private IMonitor Monitor;

        public ItemSelectionMenu(string title, IMonitor monitor) : base(0, 0, 0, 0, true)
        {
            Monitor = monitor;
            Title = title;
            width = 700 + borderWidth * 2;
            height = 600 + borderWidth * 2;

            xPositionOnScreen = Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2;

            CollectionsPage.widthToMoveActiveTab = 12;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 48, yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 7601,
                rightNeighborID = -7777
            };
            forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 60, yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 7602,
                leftNeighborID = -7777
            };

            int[] numArray1 = new int[12];
            int n1 = xPositionOnScreen + borderWidth + spaceToClearSideBorder;
            int n2 = yPositionOnScreen + borderWidth + spaceToClearTopBorder - 16;
            int n3 = 10;

            List<KeyValuePair<int, string>> ValueList = new List<KeyValuePair<int, string>>(Game1.objectInformation);
            List<KeyValuePair<int, string>> removableValues = new List<KeyValuePair<int, string>>();

            ValueList.Sort((a, b) => a.Key.CompareTo(b.Key));
            int index = 0;

            foreach(KeyValuePair<int, string> value in ValueList)
            {
                string str1 = value.Value.Split('/')[3];
                string[] str2 = str1.Split(' ');
                if (str2.Length <= 1 || (str2.Length == 2 && str2[1] == "-300" || str2[1] == "-1" || str2[1] == "-24" || str2[1] == "-20" || str2[1] == "-74" || str2[1] == "-28" || str2[1] == "-9" || str2[1] == "-8"))
                    removableValues.Add(value);
            }

            foreach(KeyValuePair<int, string> value in removableValues)
            {
                if (ValueList.Contains(value))
                    ValueList.Remove(value);
            }

            foreach(KeyValuePair<int, string> value in ValueList)
            {
                string str1 = value.Value.Split('/')[3];
                bool MarkOut = false;
                int key = value.Key;
                foreach (int cat in Util.RequestableItemCategories)
                {
                    if (str1.Contains(cat.ToString()) && Util.IsRequestableItem(key))
                    {
                        MarkOut = true;
                    }
                }

                int x = n1 + index % n3 * 68;
                int y = n2 + index / n3 * 68;
                if(y > yPositionOnScreen + height - 128)
                {
                    Items.Add(new List<ClickableTextureComponent>());
                    index = 0;
                    x = n1;
                    y = n2;
                }
                if (Items.Count == 0)
                    Items.Add(new List<ClickableTextureComponent>());
                List<ClickableTextureComponent> textureComponents = Items.Last();
                ClickableTextureComponent component = new ClickableTextureComponent(value.Key.ToString() +"/"+ MarkOut.ToString(), new Rectangle(x, y, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, value.Key, 16, 16), 4f, MarkOut)
                {
                    myID = Items.Last().Count,
                    rightNeighborID = (Items.Last().Count + 1) % n3 == 0 ? -1 : Items.Last().Count + 1,
                    leftNeighborID = Items.Last().Count % n3 == 0 ? 7001 : Items.Last().Count - 1,
                    downNeighborID = y + 68 > yPositionOnScreen + height - 128 ? -7777 : Items.Last().Count + n3,
                    upNeighborID = Items.Last().Count < n3 ? 11223 : Items.Last().Count - n3,
                    fullyImmutable = true
                };
                textureComponents.Add(component);
                index++;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            foreach (ClickableTextureComponent component in Items[Page])
            {
                var isUnLocked = Convert.ToBoolean(component.name.Split('/')[1]);
                if (component.containsPoint(x, y) && isUnLocked)
                {
                    Game1.activeClickableMenu = (IClickableMenu)new ItemRequestMenu(Convert.ToInt32(component.name.Split('/')[0]), Util.getItemName(Convert.ToInt32(component.name.Split('/')[0])), Monitor);
                }
                else if (component.containsPoint(x, y) && !isUnLocked)
                {
                    Game1.activeClickableMenu = (IClickableMenu)new DialogueBox("Item not yet unlocked");
                }
                else
                    continue;
            }

            if (Page > 0 && backButton.containsPoint(x, y))
            {
                --Page;
                Game1.playSound("shwip");
                backButton.scale = backButton.baseScale;
            }
            if (Page < Items.Count - 1 && forwardButton.containsPoint(x, y))
            {
                ++Page;
                Game1.playSound("shwip");
                forwardButton.scale = forwardButton.baseScale;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            hoverText = "";
            foreach(ClickableTextureComponent clickableTexture in Items[Page])
            {
                if (clickableTexture.containsPoint(x, y))
                {
                    clickableTexture.scale = Math.Min(clickableTexture.scale + 0.02f, clickableTexture.baseScale + 0.1f);
                    if (!Util.IsRequestableItem(Convert.ToInt32(clickableTexture.name.Split('/')[0])))
                        hoverText = "???";
                    else
                        hoverText = Util.getItemName(Convert.ToInt32(clickableTexture.name.Split('/')[0]));
                }
                else
                    clickableTexture.scale = Math.Max(clickableTexture.scale - 0.02f, clickableTexture.baseScale);
            }
            forwardButton.tryHover(x, y, 0.5f);
            backButton.tryHover(x, y, 0.5f);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            base.draw(b);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            if (Page > 0)
                backButton.draw(b);
            if (Page < Items.Count - 1)
                forwardButton.draw(b);
            SpriteText.drawStringWithScrollCenteredAt(b, Title, Game1.viewport.Width / 2 - 50, Game1.viewport.Height / 2 - 310, Title, 1f, -1, 0, 0.88f, false);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            foreach(ClickableTextureComponent component in Items[Page])
            {
                bool blacken = Convert.ToBoolean(component.name.Split('/')[1]);
                component.draw(b, blacken ? Color.White : Color.Black*0.2f, 0.86f, 0);
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            if (!hoverText.Equals(""))
                drawHoverText(b, hoverText, Game1.smallFont, 0, 0);

            drawMouse(b);
        }
    }
}
