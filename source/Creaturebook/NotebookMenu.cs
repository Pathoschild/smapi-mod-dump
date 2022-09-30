/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewModdingAPI.Utilities;
using System;
using System.IO;

namespace Creaturebook
{
    public class NotebookMenu : IClickableMenu
    {
        ClickableTextureComponent RightArrow; ClickableTextureComponent LeftArrow;
        ClickableTextureComponent SearchButton; ClickableTextureComponent CloseButton;
        ClickableTextureComponent Button_1; ClickableTextureComponent Button_2; ClickableTextureComponent Button_3;

        public int currentID = 0;
        public int actualID = 0;
        public int currentChapter = 0;

        string convertedID;
        string fullCreatureID;
        string modID;

        TextBox textBox;

        // sets the corner, The lower Y, the lower left-up corner
        Vector2 TopLeftCorner = Utility.getTopLeftPositionForCenteringOnScreen(960, 520);

        bool willSearch = false;
        bool IsFirstActive = true;
        bool IsSecondActive = false;
        bool IsThirdActive = false;
        bool IsHeaderPage = true;
        bool WasHeaderPage = false;

        string latinName;
        string description;
        string localizedName;
        string unknownLabel;
        string unknownDesc;
        string authorchapterTitle;
        string creatureAmount;
        string setsAndPages = "Creature Sets\n";

        Texture2D NotebookTexture;
        Texture2D ButtonTexture;
        Texture2D CreatureTexture;
        Texture2D CreatureTexture_2;
        Texture2D CreatureTexture_3;

        string[] menuTexts;
        Texture2D[] MenuTextures;
        public NotebookMenu()
        {
            NotebookTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "NotebookTexture"));
            CreatureTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ModEntry.creatures[actualID].FromContentPack.Manifest.UniqueID + "." + ModEntry.chapterModels[currentChapter].CreatureNamePrefix + "_" + ModEntry.creatures[actualID].ID + "_Image1"));
            ButtonTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "SearchButton"));

            this.MenuTextures = new Texture2D[] { NotebookTexture, CreatureTexture, ButtonTexture, CreatureTexture_2, CreatureTexture_3 };

            if (ModEntry.creatures[actualID].HasScientificName)
            {
                latinName = ModEntry.Helper.Translation.Get("CB.LatinName") + ModEntry.creatures[actualID].ScientificName;
            }
            else
            {
                latinName = null;
            }
            if (ModEntry.creatures[actualID].HasFunFact)
            {
                description = ModEntry.creatures[actualID].Desc;
            }
            if (ModEntry.creatures[actualID].HasExtraImages)
            {
                Button_2 = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X + 50, (int)TopLeftCorner.Y + 50, 50, 50), Game1.mouseCursors, new Rectangle(528, 128, 8, 8), 4f);
                if (File.Exists(PathUtilities.NormalizeAssetName(ModEntry.creatures[actualID].directory + "\\book-image_3.png")))
                {
                    Button_3 = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X + 50, (int)TopLeftCorner.Y + 100, 50, 50), Game1.mouseCursors, new Rectangle(520, 128, 8, 8), 4f);
                }
                else
                {
                    Button_3 = null;
                }
            }
            else
            {
                Button_1 = null;
                Button_2 = null;
                Button_3 = null;
            }

            localizedName = ModEntry.creatures[actualID].Name;
            unknownLabel = ModEntry.Helper.Translation.Get("CB.UnknownLabel");
            unknownDesc = ModEntry.Helper.Translation.Get("CB.UnknownDesc");
            authorchapterTitle = ModEntry.Helper.Translation.Get("CB.Chapter") + Convert.ToString(currentChapter + 1) + ": " + ModEntry.chapterModels[currentChapter].ChapterTitle + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ModEntry.chapterModels[currentChapter].Author;
            creatureAmount = ModEntry.Helper.Translation.Get("CB.CreatureAmount") + Convert.ToString(ModEntry.chapterModels[currentChapter].CreatureAmount);

            RightArrow = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X + 1282 - 372, (int)TopLeftCorner.Y + 718 - 242, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
            LeftArrow = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X, (int)TopLeftCorner.Y + 718 - 242, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
            SearchButton = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X + 1282 - 300, (int)TopLeftCorner.Y + 718 - 220, 68, 64), MenuTextures[2], new Rectangle(0, 0, 17, 16), 4f);
            CloseButton = new ClickableTextureComponent(new Rectangle((int)TopLeftCorner.X + 1282 - 300, (int)TopLeftCorner.Y, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

            textBox = new TextBox(Game1.content.Load<Texture2D>(PathUtilities.NormalizePath("LooseSprites\\textBox")), null, Game1.smallFont, Color.Black);
            textBox.X = (int)TopLeftCorner.X + 1282 - 300;
            textBox.Y = (int)TopLeftCorner.Y + 718 - 300;
            textBox.Width = (int)TopLeftCorner.X + 1282 - 250;

            menuTexts = new string[] { latinName, description, localizedName, unknownLabel, unknownDesc, authorchapterTitle, creatureAmount, setsAndPages };

            textBox.OnEnterPressed += textBoxEnter;
            Game1.keyboardDispatcher.Subscriber = textBox;
            updateNotebookPage();
        }
        public override void draw(SpriteBatch b)
        {
            //draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(MenuTextures[0], TopLeftCorner, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth: 0.5f);
            
            CloseButton.draw(b);
            if (!IsHeaderPage)
            {
                SearchButton.draw(b);
                if (Game1.player.modData[ModEntry.MyModID + "_" + modID + "." + fullCreatureID] != "null")
                {
                    string Date = Game1.player.modData[ModEntry.MyModID + "_" + ModEntry.creatures[currentID].FromContentPack.Manifest.UniqueID + "." + fullCreatureID];
                    int count = Convert.ToInt32(Date);
                    SDate convertedDate = SDate.FromDaysSinceStart(count);
                    string translatedDate = convertedDate.ToLocaleString();
                    string dateDiscovered = ModEntry.Helper.Translation.Get("CB.dateDiscovered") + translatedDate;
                    if (Button_2 != null)
                    {
                        Button_1.draw(b);
                        Button_2.draw(b);
                        if (Button_3 != null)
                        {
                            Button_3.draw(b);
                        }
                    }
                    if (IsFirstActive)
                    {
                        b.Draw(CreatureTexture, TopLeftCorner, null, Color.White, 0f, new Vector2(0 + ModEntry.creatures[actualID].OffsetX, 0 + ModEntry.creatures[actualID].OffsetY), ModEntry.creatures[actualID].Scale_1, SpriteEffects.None, layerDepth: 0.5f);
                    }
                    else if (IsSecondActive)
                    {
                        b.Draw(CreatureTexture_2, TopLeftCorner, null, Color.White, 0f, new Vector2(0 + ModEntry.creatures[actualID].OffsetX_2, 0 + ModEntry.creatures[actualID].OffsetY_2), ModEntry.creatures[actualID].Scale_2, SpriteEffects.None, layerDepth: 0.5f);
                    }
                    else if (IsThirdActive)
                    {
                        b.Draw(CreatureTexture_3, TopLeftCorner, null, Color.White, 0f, new Vector2(0 + ModEntry.creatures[actualID].OffsetX_3, 0 + ModEntry.creatures[actualID].OffsetY_3), ModEntry.creatures[actualID].Scale_3, SpriteEffects.None, layerDepth: 0.5f);
                    }
                    b.DrawString(Game1.smallFont, menuTexts[2], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 310), Color.Black);

                    if (ModEntry.modConfig.ShowScientificNames && ModEntry.creatures[actualID].HasScientificName)
                    {
                        b.DrawString(Game1.smallFont, menuTexts[0], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 350), Color.Black);
                    }

                    if (ModEntry.modConfig.ShowDiscoveryDates)
                    {
                        b.DrawString(Game1.smallFont, dateDiscovered, new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 390), Color.Black);
                    }

                    if (ModEntry.creatures[actualID].HasFunFact)
                    {
                        SpriteText.drawString(b, menuTexts[1], (int)TopLeftCorner.X + 910 - 371, (int)TopLeftCorner.Y + 254 - 230, width: 420, height: 490);
                    }
                }
                else if (Game1.player.modData[ModEntry.MyModID + "_" + modID + "." + fullCreatureID] == "null")
                {
                    b.Draw(CreatureTexture, TopLeftCorner, null, Color.Black * 0.8f, 0f, new Vector2(0 + ModEntry.creatures[actualID].OffsetX, 0 + ModEntry.creatures[actualID].OffsetY), ModEntry.creatures[actualID].Scale_1, SpriteEffects.None, layerDepth: 0.5f);

                    b.DrawString(Game1.smallFont, menuTexts[3], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 310), Color.Black);
                    b.DrawString(Game1.smallFont, menuTexts[4], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 350), Color.Black);
                }
            }
            else
            {
                if (ModEntry.chapterModels[currentChapter].EnableSets)
                {
                    b.DrawString(Game1.smallFont, menuTexts[7], new Vector2(TopLeftCorner.X + 910 - 371, TopLeftCorner.Y + 100), Color.Black);
                }
                else
                {
                    b.DrawString(Game1.smallFont, menuTexts[6], new Vector2(TopLeftCorner.X + 910 - 371, TopLeftCorner.Y + 100), Color.Black);
                }
                SpriteText.drawString(b, menuTexts[5], (int)TopLeftCorner.X + 30, (int)TopLeftCorner.Y + 54, width: 420, height: 490, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
            }
            if (actualID == 0 && IsHeaderPage && currentID == 0)
            {
                LeftArrow.visible = false;
                RightArrow.draw(b);
            }
            else if (actualID == ModEntry.creatures.Count - 1)
            {
                RightArrow.visible = false;
                LeftArrow.draw(b);
            }
            else
            {
                LeftArrow.visible = true;
                RightArrow.visible = true;
                LeftArrow.draw(b);
                RightArrow.draw(b);
            }

            if (willSearch)
            {
                textBox.Draw(b);
            }
            // draw cursor at last
            drawMouse(b);
        }
        public void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= 1 && willSearch)
            {
                int result = -1;
                if (int.TryParse(sender.Text, out result))
                {
                    if (-1 < Convert.ToInt32(sender.Text) && Convert.ToInt32(sender.Text) < ModEntry.chapterModels[currentChapter].CreatureAmount)
                    {
                        currentID = Convert.ToInt32(sender.Text);
                        willSearch = false;
                        textBox.Text = "";
                        if (currentChapter is 0 && currentID != 0)
                        {
                            actualID = currentID;
                        }
                        else if (currentChapter is 0 && currentID == 0)
                        {
                            actualID = 0;
                        }
                        else
                        {
                            actualID = 0;
                            for (int i = 0; i < currentChapter-1; i++)
                            {
                                actualID += ModEntry.chapterModels[i].CreatureAmount;
                            }
                            actualID += currentID + ModEntry.chapterModels.Count-1;
                        }
                        updateNotebookPage();
                    }
                }
                else if (!int.TryParse(sender.Text, out result))
                {
                    foreach (var item in ModEntry.creatures)
                    {
                        if (item.ScientificName is not null or "")
                        {
                            if (item.ScientificName.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                currentID = item.ID;
                                willSearch = false;
                                textBox.Text = "";
                                if (currentChapter is 0 && currentID != 0)
                                {
                                    actualID = currentID;
                                }
                                else if (currentChapter is 0 && currentID == 0)
                                {
                                    actualID = 0;
                                }
                                else if (currentChapter > 0 && currentID > 0)
                                {
                                    actualID = 0;
                                    for (int a = 0; a < currentChapter - 1; a++)
                                    {
                                        actualID += ModEntry.chapterModels[a].CreatureAmount;
                                    }
                                    actualID += currentID + ModEntry.chapterModels.Count -1;
                                }
                                updateNotebookPage();
                                break;
                            }
                        }
                        if (item.Name.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            currentID = item.ID;
                            willSearch = false;
                            textBox.Text = "";
                            if (currentChapter is 0 && currentID != 0)
                            {
                                actualID = currentID;
                            }
                            else if (currentChapter is 0 && currentID == 0)
                            {
                                actualID = 0;
                            }
                            else if (currentChapter > 0 && currentID > 0)
                            {
                                actualID = 0;
                                for (int a = 0; a < currentChapter - 1; a++)
                                {
                                    actualID += ModEntry.chapterModels[a].CreatureAmount;
                                }
                                actualID += currentID + ModEntry.chapterModels.Count - 1;
                            }
                            updateNotebookPage();
                            break;
                        }
                    }
                }
            }
        }
        public override void receiveKeyPress(Keys key)
        {
            //Leave this here and empty so map menu and char menu doesn't get mad at you
        }
        // 5. The method invoked when the player left-clicks on the menu.
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CloseButton.containsPoint(x,y))
            {
                Game1.activeClickableMenu.exitThisMenu();
            }
            if (SearchButton.containsPoint(x, y) && willSearch && textBox.Text == "")
            {
                willSearch = false;
            }
            else if (SearchButton.containsPoint(x, y) && !willSearch)
            {
                willSearch = true;
            }
            if (Button_2 != null)
            {
                if (Button_2.containsPoint(x, y))
                {
                    IsFirstActive = false;
                    IsSecondActive = true;
                    IsThirdActive = false;
                    updateNotebookPage();
                }
                else if (Button_1.containsPoint(x, y))
                {
                    IsFirstActive = true;
                    IsSecondActive = false;
                    IsThirdActive = false;
                    updateNotebookPage();
                }
                else if (Button_3 is not null)
                {
                    if (Button_3.containsPoint(x, y))
                    {
                        IsFirstActive = false;
                        IsSecondActive = false;
                        IsThirdActive = true;
                        updateNotebookPage();
                    }
                }
            }
            if (LeftArrow.containsPoint(x, y) && LeftArrow.visible && !willSearch)
            {
                if (IsHeaderPage)
                    IsHeaderPage = false;
                else if (actualID > 0)
                {
                    actualID--;
                    if (currentID == 0 && currentChapter != 0)
                    {
                        currentChapter--;
                        currentID = ModEntry.chapterModels[currentChapter].CreatureAmount - 1;
                        WasHeaderPage = true;
                        IsHeaderPage = true;     
                    }
                    else if (currentID > 0)
                        currentID--;
                }
                else if (currentID == 0)
                    IsHeaderPage = true;
                updateNotebookPage();
            }
            else if (RightArrow.containsPoint(x, y) && RightArrow.visible && !willSearch)
            {
                if (IsHeaderPage)
                    IsHeaderPage = false;
                else if (actualID + 1 != ModEntry.creatures.Count)
                {
                    actualID++;
                    if (currentID + 1 == ModEntry.chapterModels[currentChapter].CreatureAmount && currentChapter < ModEntry.chapterModels.Count)
                    {
                        currentChapter++;
                        currentID = 0;
                        IsHeaderPage = true;
                    }
                    else
                        currentID++;
                    updateNotebookPage();
                }
            }
            textBox.Update();
            Game1.keyboardDispatcher.Subscriber = textBox;
            Game1.playSound("shwip");
        }
        private void updateNotebookPage()
        {
            convertedID = Convert.ToString(currentID);
            fullCreatureID = ModEntry.chapterModels[currentChapter].CreatureNamePrefix + "_" + convertedID;
            modID = ModEntry.creatures[actualID].FromContentPack.Manifest.UniqueID;
            menuTexts[2] = ModEntry.creatures[actualID].Name;
            menuTexts[1] = ModEntry.creatures[actualID].Desc;
            CreatureTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ModEntry.creatures[actualID].FromContentPack.Manifest.UniqueID + "." + ModEntry.chapterModels[currentChapter].CreatureNamePrefix + "_" + ModEntry.creatures[actualID].ID + "_Image1"));
            menuTexts[5] = authorchapterTitle = ModEntry.Helper.Translation.Get("CB.Chapter") + Convert.ToString(currentChapter + 1) + ": " + ModEntry.chapterModels[currentChapter].ChapterTitle + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ModEntry.chapterModels[currentChapter].Author;
            menuTexts[6] = ModEntry.Helper.Translation.Get("CB.CreatureAmount") + Convert.ToString(ModEntry.chapterModels[currentChapter].CreatureAmount);
            if (ModEntry.chapterModels[currentChapter].EnableSets)
            {
                menuTexts[7] = "";
                foreach (var set in ModEntry.chapterModels[currentChapter].setsAndIDs)
                {
                    int a_1 = set.Value.IndexOf('-');
                    string a = set.Value.Remove(a_1);

                    int b_1 = set.Value.LastIndexOf('-');
                    string b = set.Value.Substring(b_1);

                    menuTexts[7] += set.Key + "  " + Convert.ToString(actualID + Convert.ToInt32(a)) + Convert.ToString(actualID + Convert.ToInt32(b)) + "\n";
                }
            }
            if (ModEntry.creatures[actualID].HasScientificName)
            {
                menuTexts[0] = ModEntry.Helper.Translation.Get("CB.LatinName") + ModEntry.creatures[actualID].ScientificName;
            }
            else
            {
                latinName = "";
            }
            if (WasHeaderPage)
            {
                menuTexts[5] = authorchapterTitle = ModEntry.Helper.Translation.Get("CB.Chapter") + Convert.ToString(currentChapter + 2) + ": " + ModEntry.chapterModels[currentChapter + 1].ChapterTitle + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ModEntry.chapterModels[currentChapter + 1].Author;
                menuTexts[6] = ModEntry.Helper.Translation.Get("CB.CreatureAmount") + Convert.ToString(ModEntry.chapterModels[currentChapter + 1].CreatureAmount);
                WasHeaderPage = false;
            }
            if (ModEntry.creatures[actualID].HasExtraImages)
            {
                CreatureTexture_2 = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ModEntry.creatures[actualID].FromContentPack.Manifest.UniqueID + "." + ModEntry.chapterModels[currentChapter].CreatureNamePrefix + "_" + ModEntry.creatures[actualID].ID + "_Image2"));
                if (File.Exists(PathUtilities.NormalizeAssetName(ModEntry.creatures[actualID].directory + "\\book-image_3.png")))
                {
                    CreatureTexture_3 = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ModEntry.creatures[actualID].FromContentPack.Manifest.UniqueID + "." + ModEntry.chapterModels[currentChapter].CreatureNamePrefix + "_" + ModEntry.creatures[actualID].ID + "_Image3"));
                }
                else
                {
                    CreatureTexture_3 = null;
                }
            }
            else
            {
                CreatureTexture_2 = null;
            }
        }
    }
}
