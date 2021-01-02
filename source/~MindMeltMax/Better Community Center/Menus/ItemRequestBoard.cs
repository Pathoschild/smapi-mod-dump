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
using StardewValley.Menus;
using System;
using SObject = StardewValley.Object;
using System.Collections.Generic;

namespace BCC.Menus
{
    public class ItemRequestBoard : IClickableMenu
    {
        public static Item item;

        public string RequestStandard = $"Request Item";
        public string RequestString = $"";
        public bool hasRequested3Items = false;
        public bool hasRequestedAnything = false;

        public ClickableComponent RequestItemButton;
        public ClickableTextureComponent smallItemTexture;
        public List<ClickableTextureComponent> textures = new List<ClickableTextureComponent>();

        public Texture2D BackgroundTexture;

        public IMonitor Monitor;

        public ItemRequestBoard(IMonitor monitor) : base(0, 0, 0, 0, true)
        {
            hasRequestedAnything = Requests.RequestList.Count != 0 ? true : false;
            hasRequested3Items = Requests.RequestList.Count >= 3 ? true : false;

            Monitor = monitor;
            BackgroundTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard");
            width = 338 * 4;
            height = 792;
            Vector2 CenterScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)CenterScreen.X;
            yPositionOnScreen = (int)CenterScreen.Y;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            RequestItemButton = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 128, yPositionOnScreen + height - 128, (int)Game1.dialogueFont.MeasureString(RequestStandard).X + 24, (int)Game1.dialogueFont.MeasureString(RequestStandard).Y + 24), "");
            RequestItemButton.myID = 0;
            if (hasRequested3Items)
                RequestItemButton.visible = false;
            int index = 0;
            foreach (Request r in Requests.RequestList)
            {
                string[] indexedArray = Game1.objectInformation[r.itemIndex].Split('/');
                smallItemTexture = new ClickableTextureComponent(Util.getItemName(r.itemIndex), new Rectangle(xPositionOnScreen + 352, yPositionOnScreen + 320 + index, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, r.itemIndex, 16, 16), 4f, true);
                textures.Add(smallItemTexture);
                index += 96;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            foreach (Request r in Requests.RequestList)
            {
                if (r.itemIndex == Util.CompletedRequestIndex)
                {
                    string name = Game1.objectInformation[r.itemIndex].Split('/')[0];
                    foreach (ClickableTextureComponent component in textures)
                    {
                        if (component.name == name && Game1.player.Money >= r.totalPrice && component.containsPoint(x, y))
                        {
                            SObject tempObject = new SObject(r.itemIndex, 1);
                            for (int i = 0; i < r.itemCount; i++)
                            {
                                Item oneTempObject = tempObject.getOne();
                                Game1.player.addItemToInventory(oneTempObject);
                            }
                            Game1.player.Money -= r.totalPrice;
                            Game1.activeClickableMenu = null;
                            break;
                        }
                        else if (Game1.player.Money < r.totalPrice)
                        {
                            Game1.activeClickableMenu = (IClickableMenu)new DialogueBox("You do not have enough money to claim this item");
                            break;
                        }
                        else
                            break;
                    }
                }
            }
            Util.TryToRemoveRequests();


            if (!RequestItemButton.visible || !RequestItemButton.containsPoint(x, y))
                return;
            else
                Game1.activeClickableMenu = (IClickableMenu)new ItemSelectionMenu("Select It4m", Monitor);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            foreach (ClickableTextureComponent component in textures)
            {
                if (component.containsPoint(x, y))
                    component.scale = Math.Min(component.scale + 0.02f, component.baseScale + 0.2f);
                else
                    component.scale = Math.Max(component.scale - 0.02f, component.baseScale);
            }
            if (RequestItemButton.visible)
            {
                float scale = RequestItemButton.scale;
                RequestItemButton.scale = RequestItemButton.bounds.Contains(x, y) ? 1.5f : 1f;
                if ((double)RequestItemButton.scale > (double)scale)
                    Game1.playSound("bigDeSelect");
            }
            else
                return;
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            b.Draw(BackgroundTexture, new Vector2((float)xPositionOnScreen, (float)yPositionOnScreen), new Rectangle(0, 0, 338, 198), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            if (!hasRequestedAnything)
                b.DrawString(Game1.dialogueFont, "No requests posted right now", new Vector2((float)xPositionOnScreen + 384, (float)yPositionOnScreen + 320), Game1.textColor);
            if (RequestItemButton.visible)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), RequestItemButton.bounds.X, RequestItemButton.bounds.Y, RequestItemButton.bounds.Width, RequestItemButton.bounds.Height, (double)RequestItemButton.scale > 1.0 ? Color.LightBlue : Color.White, 4f * RequestItemButton.scale);
                Utility.drawTextWithShadow(b, RequestStandard, Game1.dialogueFont, new Vector2((float)RequestItemButton.bounds.X + 12, (float)RequestItemButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
            }

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            int index1 = 72;
            int index2 = 0;
            foreach (ClickableTextureComponent component in textures)
            {
                int itemCount = 0;
                bool hasBeen7Days = false;
                bool drawCompletionString = false;
                RequestString = "";
                foreach (Request r in Requests.RequestList)
                {
                    string itemName = Game1.objectInformation[r.itemIndex].Split('/')[0];
                    if (itemName == component.name)
                    {
                        itemCount = r.itemCount;
                        if (r.itemIndex == Util.CompletedRequestIndex)
                            drawCompletionString = true;
                        if (Game1.Date.TotalDays - r.CreationDate > 7)
                            hasBeen7Days = true;
                    }
                }
                if (!drawCompletionString && !hasBeen7Days)
                {
                    if (itemCount == 1)
                        RequestString = $"I'm looking for {itemCount} {component.name.ToLower()}";
                    else
                        RequestString = $"I'm looking for {itemCount} {component.name.ToLower()}s";
                }
                else if (drawCompletionString)
                {
                    RequestString = Util.CompletionString;
                }
                else if (hasBeen7Days)
                    RequestString = "Request not filled";
                else
                    RequestString = "null";
                component.draw(b, hasBeen7Days ? Color.Black * 0.2f : Color.White, 0.86f, 0);
                b.DrawString(Game1.dialogueFont, RequestString, new Vector2((float)xPositionOnScreen + 352 + index1, (float)yPositionOnScreen + 328 + index2), drawCompletionString ? Color.LightGreen : Game1.textColor);

                index2 += 96;
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            base.draw(b);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }
    }
}
