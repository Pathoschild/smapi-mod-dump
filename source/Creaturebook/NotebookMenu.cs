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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace Creaturebook
{
    public class NotebookMenu : IClickableMenu
    {
        readonly ClickableTextureComponent RightArrow = new(new((int)TopLeftCorner.X + 1282 - 372, (int)TopLeftCorner.Y + 718 - 242, 48, 44), Game1.mouseCursors, new(365, 495, 12, 11), 4f);
        readonly ClickableTextureComponent LeftArrow = new(new((int)TopLeftCorner.X, (int)TopLeftCorner.Y + 718 - 242, 48, 44), Game1.mouseCursors, new(352, 495, 12, 11), 4f);

        readonly ClickableTextureComponent CloseButton = new(new((int)TopLeftCorner.X + 982, (int)TopLeftCorner.Y, 48, 48), Game1.mouseCursors, new(337, 494, 12, 12), 4f);

        readonly ClickableTextureComponent Button_1;
        readonly ClickableTextureComponent Button_2;
        readonly ClickableTextureComponent Button_3;

        readonly ClickableTextureComponent SwitchToNormal_1 = new(new((int)TopLeftCorner.X + 365, (int)TopLeftCorner.Y + 10, 64, 64), Game1.mouseCursors, new(162, 440, 16, 16), 4f);
        readonly ClickableTextureComponent SwitchToNormal_2 = new(new((int)TopLeftCorner.X + 880, (int)TopLeftCorner.Y + 10, 64, 64), Game1.mouseCursors, new(162, 440, 16, 16), 4f);
        readonly ClickableTextureComponent ShowSetView = new(new((int)TopLeftCorner.X + 982, (int)TopLeftCorner.Y + 200, 48, 48), Game1.mouseCursors, new(337, 494, 12, 12), 4f);

        ClickableComponent Sticky_Purple; ClickableComponent Sticky_Yellow;
        ClickableComponent Sticky_Blue; ClickableComponent Sticky_Green;

        static string[] PagesWithStickies = new string[4];

        public int currentID = 0;
        public int currentChapter = 0;
        public int currentSetPage = 0;

        static Chapter ChapterYoureIn = ModEntry.Chapters[0];
        static Creature CurrentCreature = ChapterYoureIn.Creatures[0];

        readonly TextBox textBox = new(Game1.content.Load<Texture2D>(PathUtilities.NormalizePath("LooseSprites\\textBox")), null, Game1.smallFont, Color.Black)
        {
            X = (int)TopLeftCorner.X + 1282 - 300,
            Y = (int)TopLeftCorner.Y + 718 - 300,
            Width = (int)TopLeftCorner.X + 1282 - 250
        };
        float Stickyrotation;

        // sets the corner, The lower Y, the lower left-up corner
        static readonly Vector2 TopLeftCorner = Utility.getTopLeftPositionForCenteringOnScreen(960, 520);

        bool willSearch = false;
        bool IsFirstActive = true;
        bool IsSecondActive = false;
        bool IsThirdActive = false;
        bool showSetPaging = ChapterYoureIn.EnableSets;
        bool IsHeaderPage = true;
        bool WasHeaderPage = false;
        bool lastSetOnItsOwn = false;
        bool wasOnSecondPage = false;
        IDictionary<Set,Set> pageOrder = new Dictionary<Set, Set>();

        readonly string unknownLabel = ModEntry.Helper.Translation.Get("CB.UnknownLabel");
        readonly string unknownDesc = ModEntry.Helper.Translation.Get("CB.UnknownDesc");

        string latinName;
        string description;
        string localizedName;
        string authorchapterTitle = ModEntry.Helper.Translation.Get("CB.Chapter") + "1: " + ChapterYoureIn.Title + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ChapterYoureIn.Author;

        static readonly Texture2D NotebookTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "NotebookTexture"));
        static readonly Texture2D ButtonTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "SearchButton"));
        static readonly Texture2D Stickies = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "Stickies"));

        static readonly ClickableTextureComponent SearchButton = new(new((int)TopLeftCorner.X + 982, (int)TopLeftCorner.Y + 718 - 220, 68, 64), ButtonTexture, new(0, 0, 17, 16), 4f);

        Texture2D CreatureTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_0_Image1"));
        Texture2D CreatureTexture_2;
        Texture2D CreatureTexture_3;

        readonly string[] menuTexts;
        readonly Texture2D[] MenuTextures;

        public NotebookMenu()
        {
            currentID = 0;
            currentChapter = 0;
            currentSetPage = 0;
            ChapterYoureIn = ModEntry.Chapters[0];
            showSetPaging = ChapterYoureIn.EnableSets;
            MenuTextures = new Texture2D[] { NotebookTexture, CreatureTexture, ButtonTexture, CreatureTexture_2, CreatureTexture_3, Stickies };
            
            localizedName = string.IsNullOrEmpty(CurrentCreature.NameKey) ? ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.CreatureNamePrefix + "_" + CurrentCreature.ID.ToString() + "_name") : ChapterYoureIn.FromContentPack.Translation.Get(CurrentCreature.NameKey);

            if (ModEntry.modConfig.EnableStickies)
            {
                PagesWithStickies = ModEntry.Helper.Data.ReadJsonFile<string[]>(PathUtilities.NormalizeAssetName("localData/stickies.json"));
                if (PagesWithStickies == null)
                {
                    PagesWithStickies = new string[4];
                    Sticky_Yellow = new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 200, 240, 84), "");
                    Sticky_Green = new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 260, 240, 84), "");
                    Sticky_Blue = new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 320, 240, 84), "");
                    Sticky_Purple = new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 380, 240, 84), "");
                }
                else
                {
                    Sticky_Yellow = PagesWithStickies[0] is null ? new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 200, 240, 84), "") : new(new((int)TopLeftCorner.X - 150, (int)TopLeftCorner.Y + 200, 240, 84), "");
                    Sticky_Green = PagesWithStickies[1] is null ? new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 260, 240, 84), "") : new(new((int)TopLeftCorner.X - 150, (int)TopLeftCorner.Y + 260, 240, 84), "");
                    Sticky_Blue = PagesWithStickies[2] is null ? new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 320, 240, 84), "") : new(new((int)TopLeftCorner.X - 150, (int)TopLeftCorner.Y + 320, 240, 84), "");
                    Sticky_Purple = PagesWithStickies[3] is null ? new(new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 380, 240, 84), "") : new(new((int)TopLeftCorner.X - 150, (int)TopLeftCorner.Y + 380, 240, 84), "");
                }
            }
            if (CurrentCreature.HasScientificName)
                latinName = ModEntry.Helper.Translation.Get("CB.LatinName") + CurrentCreature.ScientificName;
            else
                latinName = null;
            if (CurrentCreature.HasFunFact)
                description = string.IsNullOrEmpty(CurrentCreature.DescKey) ? ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.CreatureNamePrefix + "_" + CurrentCreature.ID.ToString() + "_desc") : ChapterYoureIn.FromContentPack.Translation.Get(CurrentCreature.DescKey);
            if (CurrentCreature.HasExtraImages)
            {
                Button_2 = new(new((int)TopLeftCorner.X + 50, (int)TopLeftCorner.Y + 50, 50, 50), Game1.mouseCursors, new(528, 128, 8, 8), 4f);
                if (File.Exists(PathUtilities.NormalizeAssetName(CurrentCreature.Directory + "\\book-image_3.png")))
                    Button_3 = new(new((int)TopLeftCorner.X + 50, (int)TopLeftCorner.Y + 100, 50, 50), Game1.mouseCursors, new(520, 128, 8, 8), 4f);
                else
                    Button_3 = null;
            }
            else
            {
                Button_1 = null;
                Button_2 = null;
                Button_3 = null;
            }

            menuTexts = new string[] { latinName, description, localizedName, unknownLabel, unknownDesc, authorchapterTitle };

            textBox.OnEnterPressed += TextBoxEnter;
            Game1.keyboardDispatcher.Subscriber = textBox;
            UpdateNotebookPage();
            if (ChapterYoureIn.EnableSets)
                CalculateSetPages(ChapterYoureIn.Sets);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            if (PagesWithStickies[2] is not null && PagesWithStickies[2] != ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() && ModEntry.modConfig.EnableStickies)
                b.Draw(MenuTextures[5], new(Sticky_Blue.bounds.X, Sticky_Blue.bounds.Y), new(0, 42, 60, 21), Color.White, Stickyrotation, Vector2.Zero, 3f, SpriteEffects.None, layerDepth: 0.5f);

            b.Draw(MenuTextures[0], TopLeftCorner, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth: 0.5f);

            if (PagesWithStickies[2] == ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() || PagesWithStickies[2] is null && ModEntry.modConfig.EnableStickies)
                b.Draw(MenuTextures[5], new(Sticky_Blue.bounds.X, Sticky_Blue.bounds.Y), new(0, 42, 60, 21), Color.White, Stickyrotation, Vector2.Zero, 3f, SpriteEffects.None, layerDepth: 0.5f);

            Sticky_Yellow.visible = true;
            Sticky_Green.visible = true;
            Sticky_Blue.visible = true;
            Sticky_Purple.visible = true;

            CloseButton.draw(b);
            if (!IsHeaderPage)
            {
                if (!showSetPaging)
                {
                    SearchButton.draw(b);
                    if (ChapterYoureIn.EnableSets)
                        ShowSetView.draw(b);
                    
                    if (!Game1.player.modData[ModEntry.MyModID + "_" + ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString()].Equals("null"))
                    {
                        string Date = Game1.player.modData[ModEntry.MyModID + "_" + ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString()];
                        int count = Convert.ToInt32(Date);
                        SDate convertedDate = SDate.FromDaysSinceStart(count);
                        string translatedDate = convertedDate.ToLocaleString();
                        string dateDiscovered = ModEntry.Helper.Translation.Get("CB.dateDiscovered") + translatedDate;
                        if (Button_2 is not null)
                        {
                            Button_1.draw(b);
                            Button_2.draw(b);
                            if (Button_3 != null)
                                Button_3.draw(b);
                        }
                        if (IsFirstActive)
                            b.Draw(MenuTextures[1], TopLeftCorner, null, Color.White, 0f, new(CurrentCreature.ImageOffsets[0].X, CurrentCreature.ImageOffsets[0].Y), CurrentCreature.ImageScales[0], SpriteEffects.None, layerDepth: 0.5f);

                        else if (IsSecondActive)
                            b.Draw(MenuTextures[2], TopLeftCorner, null, Color.White, 0f, new(CurrentCreature.ImageOffsets[1].X, CurrentCreature.ImageOffsets[1].Y), CurrentCreature.ImageScales[1], SpriteEffects.None, layerDepth: 0.5f);

                        else if (IsThirdActive)
                            b.Draw(MenuTextures[3], TopLeftCorner, null, Color.White, 0f, new(CurrentCreature.ImageOffsets[2].X, CurrentCreature.ImageOffsets[2].Y), CurrentCreature.ImageScales[2], SpriteEffects.None, layerDepth: 0.5f);

                        b.DrawString(Game1.smallFont, menuTexts[2], new(TopLeftCorner.X + 15, TopLeftCorner.Y + 310), Color.Black);

                        if (ModEntry.modConfig.ShowScientificNames && CurrentCreature.HasScientificName)
                            b.DrawString(Game1.smallFont, menuTexts[0], new(TopLeftCorner.X + 15, TopLeftCorner.Y + 350), Color.Black);

                        if (ModEntry.modConfig.ShowDiscoveryDates)
                            b.DrawString(Game1.smallFont, dateDiscovered, new(TopLeftCorner.X + 15, TopLeftCorner.Y + 390), Color.Black);

                        if (CurrentCreature.HasFunFact)
                            SpriteText.drawString(b, menuTexts[1], (int)TopLeftCorner.X + 910 - 371, (int)TopLeftCorner.Y + 254 - 230, width: 420, height: 490);
                    }
                    else
                    {
                        b.Draw(CreatureTexture, TopLeftCorner, null, Color.Black * 0.8f, 0f, new Vector2(CurrentCreature.ImageOffsets[0].X, CurrentCreature.ImageOffsets[0].Y), CurrentCreature.ImageScales[0], SpriteEffects.None, layerDepth: 0.5f);
                        b.DrawString(Game1.smallFont, menuTexts[3], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 310), Color.Black);
                        b.DrawString(Game1.smallFont, menuTexts[4], new Vector2(TopLeftCorner.X + 15, TopLeftCorner.Y + 350), Color.Black);
                    }
                }
                else
                {
                    if (currentSetPage < ChapterYoureIn.Sets.Length - 1)
                    {
                        DrawCreatureIcons(b, currentSetPage);
                        lastSetOnItsOwn = false;
                    }
                    else if (currentSetPage == ChapterYoureIn.Sets.Length - 1 || lastSetOnItsOwn)
                    {
                        DrawCreatureIcons(b, currentSetPage);
                        lastSetOnItsOwn = true;
                    }
                }
            }
            else
            {
                SpriteText.drawString(b, menuTexts[5], (int)TopLeftCorner.X + 30, (int)TopLeftCorner.Y + 54, width: 420, height: 490, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
            }
            if (currentID == 0 && IsHeaderPage && currentID == 0)
            {
                LeftArrow.visible = false;
                RightArrow.draw(b);
            }
            else if ((currentID == ModEntry.Chapters[^1].Creatures.Length - 1 && currentChapter == ModEntry.Chapters.Count - 1) || (currentSetPage == ModEntry.Chapters[^1].Sets.Length - 1 && lastSetOnItsOwn))
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
                textBox.Draw(b);
            drawMouse(b);
        }
        public void TextBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= 1 && willSearch)
            {
                int result = -1;
                if (int.TryParse(sender.Text, out result))
                {
                    if (-1 < Convert.ToInt32(sender.Text) && Convert.ToInt32(sender.Text) < ChapterYoureIn.Creatures.Length)
                    {
                        currentID = Convert.ToInt32(sender.Text);
                        willSearch = false;
                        textBox.Text = "";
                        UpdateNotebookPage();
                    }
                }
                else if (!int.TryParse(sender.Text, out result))
                {
                    foreach (var item in ChapterYoureIn.Creatures)
                    {
                        string name = ChapterYoureIn.FromContentPack.Translation.Get(item.NameKey);
                        if (!string.IsNullOrEmpty(item.ScientificName))
                        {
                            if (item.ScientificName.Contains(sender.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                currentID = item.ID;
                                willSearch = false;
                                textBox.Text = "";
                                UpdateNotebookPage();
                                break;
                            }
                        }
                        if (name.Contains(sender.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            currentID = item.ID;
                            willSearch = false;
                            textBox.Text = "";
                            UpdateNotebookPage();
                            break;
                        }
                    }
                }
            }
        }
        public override void receiveKeyPress(Keys key) { }
        //Leave this here and empty so M and E buttons don't yeet your menu
        public override void performHoverAction(int x, int y)
        {
            //Patch tools then use the same image to create 
            //an empty ClickableTextureComponent hehe
        }
        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.DPadLeft)
                ArrowBehaivor(1);
            
            if (b == Buttons.DPadRight)
                ArrowBehaivor(0);

            if (b == Buttons.Back)
            {
                ModEntry.Helper.Data.WriteJsonFile(PathUtilities.NormalizeAssetName("localData/stickies.json"), PagesWithStickies);
                Game1.activeClickableMenu.exitThisMenu();
            }

            if (Button_2 != null)
            {
                IsFirstActive = b == Buttons.DPadUp && IsSecondActive;
                IsSecondActive = (IsFirstActive && b == Buttons.DPadDown) || (IsThirdActive && b == Buttons.DPadUp);
                if (Button_3 != null)
                    IsThirdActive = IsSecondActive && b == Buttons.DPadDown;
            }

            if (b == Buttons.RightTrigger && showSetPaging && SwitchToNormal_2.visible)
                SetButtons(1);
            if (b == Buttons.LeftTrigger && showSetPaging && SwitchToNormal_1.visible)
                SetButtons(0);
            if (b == Buttons.RightShoulder && !showSetPaging && ChapterYoureIn.EnableSets)
                SetButtons(2);

            willSearch = b == Buttons.RightStick && !willSearch && textBox.Text == "";

            if (willSearch && !string.IsNullOrEmpty(textBox.Text) && b == Buttons.LeftShoulder)
                TextBoxEnter(textBox);

            textBox.Update();
            Game1.keyboardDispatcher.Subscriber = textBox;
            Game1.playSound("shwip");
            UpdateNotebookPage();
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Sticky_Blue.containsPoint(x, y))
                UpdateStickies(2);
            
            if (SwitchToNormal_1.containsPoint(x, y) && !IsHeaderPage && !WasHeaderPage)
                SetButtons(0);
            
            else if (SwitchToNormal_2.containsPoint(x, y) && !IsHeaderPage && !WasHeaderPage)
                SetButtons(1);
            
            else if (ShowSetView.containsPoint(x, y) && ChapterYoureIn.EnableSets)
                SetButtons(2);
            
            if (CloseButton.containsPoint(x, y))
            {
                ModEntry.Helper.Data.WriteJsonFile(PathUtilities.NormalizeAssetName("localData/stickies.json"), PagesWithStickies);
                Game1.activeClickableMenu.exitThisMenu();
            }
            willSearch = SearchButton.containsPoint(x, y) && !willSearch && textBox.Text == "";

            if (Button_2 != null)
            {
                IsFirstActive = Button_1.containsPoint(x, y);
                IsSecondActive = Button_2.containsPoint(x, y);
                if (Button_3 != null)
                IsThirdActive = Button_3.containsPoint(x, y);
            }

            int a = 2;
            if (!willSearch)
            {
                a = LeftArrow.containsPoint(x, y) && LeftArrow.visible ? 1 : a;
                a = RightArrow.containsPoint(x, y) && RightArrow.visible ? 0 : a;
                a = !RightArrow.containsPoint(x, y) && !LeftArrow.containsPoint(x, y) ? 2 : a;
                ArrowBehaivor(a);
            }

            textBox.Update();
            Game1.keyboardDispatcher.Subscriber = textBox;
            Game1.playSound("shwip");
            UpdateNotebookPage();
        }
        public void UpdateNotebookPage()
        {
            CurrentCreature = ChapterYoureIn.Creatures[currentID];

            CreatureTexture = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() + "_Image1"));

            MenuTextures[1] = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() + "_Image1"));
            menuTexts[5] = ModEntry.Helper.Translation.Get("CB.Chapter") + Convert.ToString(currentChapter + 1) + ": " + ChapterYoureIn.Title + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ChapterYoureIn.Author;

            if (CurrentCreature.HasScientificName)
                menuTexts[0] = ModEntry.Helper.Translation.Get("CB.LatinName") + CurrentCreature.ScientificName;
            else
                latinName = "";

            if (CurrentCreature.HasScientificName)
                menuTexts[1] = string.IsNullOrEmpty(CurrentCreature.DescKey) ? ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() + "_desc") : ChapterYoureIn.FromContentPack.Translation.Get(CurrentCreature.DescKey);
            else
                menuTexts[1] = "";
            menuTexts[2] = string.IsNullOrEmpty(CurrentCreature.NameKey) ? ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString() + "_name") : ChapterYoureIn.FromContentPack.Translation.Get(CurrentCreature.NameKey);
            if (WasHeaderPage)
            {
                menuTexts[5] = authorchapterTitle = ModEntry.Helper.Translation.Get("CB.Chapter") + Convert.ToString(currentChapter + 2) + ": " + ModEntry.Chapters[currentChapter + 1].Title + "\n" + ModEntry.Helper.Translation.Get("CB.ChapterAuthorBy") + ModEntry.Chapters[currentChapter + 1].Author;
                WasHeaderPage = false;
            }
            if (CurrentCreature.HasExtraImages)
            {
                MenuTextures[2] = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + CurrentCreature.ID + "_Image2"));
                if (File.Exists(PathUtilities.NormalizeAssetName(CurrentCreature.Directory + "\\book-image_3.png")))
                    MenuTextures[3] = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + CurrentCreature.ID + "_Image3"));
                else
                    MenuTextures[3] = null;
            }
            else
                MenuTextures[2] = null;
        }
        public void CalculateSetPages(Set[] sets)
        {
            pageOrder = new Dictionary<Set,Set>();
            for (int i = 0; i < sets.Length; i++)
            {
                if (i % 2 == 0)
                    pageOrder.Add(sets[i], sets[i + 1]);
                else if (i % 2 == 1 && i == sets.Length - 1)
                    pageOrder.Add(sets[i], new Set(true));
            }
        }
        public void DrawCreatureIcons(SpriteBatch b, int i)
        {
            Texture2D creature;
            
            for (int l = 0; l < ChapterYoureIn.Sets[i].OffsetsInMenu.Length; l++)
            {
                creature = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + ChapterYoureIn.Sets[i].CreaturesBelongingToThisSet[l].ToString() + "_Image1"));
                bool color = !Game1.player.modData[ModEntry.MyModID + "_" + ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + ChapterYoureIn.Sets[i].CreaturesBelongingToThisSet[l]].Equals("null");
                if (color)
                    b.Draw(creature, new((int)TopLeftCorner.X + ChapterYoureIn.Sets[i].OffsetsInMenu[l].X, (int)TopLeftCorner.Y + ChapterYoureIn.Sets[i].OffsetsInMenu[l].Y), null, Color.White, 0f, Vector2.Zero, ChapterYoureIn.Sets[i].ScalesInMenu[l], SpriteEffects.None, layerDepth: 0.5f);
                else
                    b.Draw(creature, new((int)TopLeftCorner.X + ChapterYoureIn.Sets[i].OffsetsInMenu[l].X, (int)TopLeftCorner.Y + ChapterYoureIn.Sets[i].OffsetsInMenu[l].Y), null, Color.Black * 0.8f, 0f, Vector2.Zero, ChapterYoureIn.Sets[i].ScalesInMenu[l], SpriteEffects.None, layerDepth: 0.5f);
                SwitchToNormal_1.draw(b);
                SpriteText.drawString(b, ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.Sets[i].DisplayNameKey), (int)TopLeftCorner.X + 20, (int)TopLeftCorner.Y + 10);
            }
            if (i != ChapterYoureIn.Sets.Length - 1)
            {
                Texture2D creature_2;
                for (int l = 0; l < ChapterYoureIn.Sets[i + 1].OffsetsInMenu.Length; l++)
                {
                    creature_2 = ModEntry.Helper.GameContent.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + ChapterYoureIn.Sets[i + 1].CreaturesBelongingToThisSet[l].ToString() + "_Image1"));
                    if (Game1.player.modData[ModEntry.MyModID + "_" + ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + ChapterYoureIn.Sets[i + 1].CreaturesBelongingToThisSet[l]] is not "null")
                        b.Draw(creature_2, new((int)TopLeftCorner.X + 530 + ChapterYoureIn.Sets[i + 1].OffsetsInMenu[l].X, (int)TopLeftCorner.Y + ChapterYoureIn.Sets[i + 1].OffsetsInMenu[l].Y), null, Color.White, 0f, Vector2.Zero, ChapterYoureIn.Sets[i + 1].ScalesInMenu[l], SpriteEffects.None, layerDepth: 0.5f);
                    else
                        b.Draw(creature_2, new((int)TopLeftCorner.X + 530 + ChapterYoureIn.Sets[i + 1].OffsetsInMenu[l].X, (int)TopLeftCorner.Y + ChapterYoureIn.Sets[i + 1].OffsetsInMenu[l].Y), null, Color.Black * 0.8f, 0f, Vector2.Zero, ChapterYoureIn.Sets[i + 1].ScalesInMenu[l], SpriteEffects.None, layerDepth: 0.5f);
                    SwitchToNormal_2.draw(b);
                    SpriteText.drawString(b, ChapterYoureIn.FromContentPack.Translation.Get(ChapterYoureIn.Sets[i + 1].DisplayNameKey), (int)TopLeftCorner.X + 530, (int)TopLeftCorner.Y + 10);
                }
            }
        }
        private void UpdateStickies(int WhichSticky)
        {
            if (string.IsNullOrEmpty(PagesWithStickies[WhichSticky]) && !IsHeaderPage && !WasHeaderPage)
            {
                Sticky_Yellow.bounds = WhichSticky is 0 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 200, 240, 84) : Sticky_Yellow.bounds;
                Sticky_Green.bounds = WhichSticky is 1 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 260, 240, 84) : Sticky_Green.bounds;
                Sticky_Blue.bounds = WhichSticky is 2 ? new((int)TopLeftCorner.X, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Blue.bounds;
                Sticky_Purple.bounds = WhichSticky is 3 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Purple.bounds;
                PagesWithStickies[WhichSticky] = ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString();
            }
            else if (PagesWithStickies[WhichSticky].Equals(ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString()) && !IsHeaderPage && !WasHeaderPage)
            {
                Sticky_Yellow.bounds = WhichSticky is 0 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 200, 240, 84) : Sticky_Yellow.bounds;
                Sticky_Yellow.bounds = WhichSticky is 1 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 260, 240, 84) : Sticky_Green.bounds;
                Sticky_Blue.bounds = WhichSticky is 2 ? new((int)TopLeftCorner.X, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Blue.bounds;
                Sticky_Yellow.bounds = WhichSticky is 3 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Purple.bounds;
                PagesWithStickies[WhichSticky] = null;
            }
            else if (!string.IsNullOrEmpty(PagesWithStickies[WhichSticky]) && PagesWithStickies[WhichSticky].Equals(ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString()))
                FindPageAndCheckSides(PagesWithStickies[WhichSticky], true, 5);
        }
        private void FindPageAndCheckSides(string CreatureID, bool WillTravel, int SentFromWhichSticky)
        {
            if (string.IsNullOrEmpty(CreatureID))
                return;
            for (int i = 0; i < ModEntry.Chapters.Count; i++)
            {
                for (int c = 0; c < ChapterYoureIn.Creatures.Length; c++)
                {
                    if (CreatureID.Contains(ChapterYoureIn.FromContentPack.Manifest.UniqueID) && CreatureID.Contains(ChapterYoureIn.CreatureNamePrefix) && CreatureID.Contains(c.ToString()))
                    {
                        if (WillTravel)
                        {
                            currentID = c;
                            currentChapter = i;
                            
                            IsHeaderPage = false;
                            WasHeaderPage = false;
                            UpdateNotebookPage();
                            return;
                        }
                        else
                        {
                            if (i < currentChapter || (i == currentChapter && c < currentID) || CreatureID == ChapterYoureIn.FromContentPack.Manifest.UniqueID + "." + ChapterYoureIn.CreatureNamePrefix + "_" + currentID.ToString())
                                Stickyrotation = (float)Math.PI;

                            else if (i > currentChapter || (i == currentChapter && c > currentID) && SentFromWhichSticky is not 5)
                            {
                                Stickyrotation = 0f;
                                Sticky_Yellow.bounds = SentFromWhichSticky is 0 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 200, 240, 84) : Sticky_Yellow.bounds;
                                Sticky_Green.bounds = SentFromWhichSticky is 1 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 260, 240, 84) : Sticky_Green.bounds;
                                Sticky_Blue.bounds = SentFromWhichSticky is 2 ? new(NotebookTexture.Width + 50, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Blue.bounds;
                                Sticky_Purple.bounds = SentFromWhichSticky is 3 ? new((int)TopLeftCorner.X - 200, (int)TopLeftCorner.Y + 320, 240, 84) : Sticky_Purple.bounds;
                            }
                        }
                    }
                }
            }
        }
        private void ArrowBehaivor (int whichArrow)
        {
            switch (whichArrow)
            {
                case 0: //right arrow
                    if (IsHeaderPage)
                        IsHeaderPage = false;
                    else if ((currentID + 1 != ModEntry.Chapters[^1].Creatures.Length || currentChapter != ModEntry.Chapters.Count - 1) && !showSetPaging)
                    {
                        if (currentID == ChapterYoureIn.Creatures.Length - 1 && currentChapter < ModEntry.Chapters.Count - 1)
                        {
                            currentChapter++;
                            currentID = 0;
                            ChapterYoureIn = ModEntry.Chapters[currentChapter];
                            IsHeaderPage = true;
                            currentSetPage = 0;
                            if (ChapterYoureIn.EnableSets)
                                CalculateSetPages(ChapterYoureIn.Sets);
                            showSetPaging = ChapterYoureIn.EnableSets;
                        }
                        else
                            currentID++;
                    }
                    else if (showSetPaging)
                    {
                        /*for (int i = 0; i < pageOrder.Count; i++)
                        {
                            Set emptySet = new Set(true);
                            if (pageOrder.Values.Contains(emptySet))
                            {

                            }
                        }*/
                        if (currentSetPage <= ChapterYoureIn.Sets.Length - 1)
                        {
                            if (0 == ChapterYoureIn.Sets.Length % 2 && currentSetPage == ChapterYoureIn.Sets.Length - 2 && !(currentSetPage == pageOrder.Count - 3))
                            {
                                currentID += ChapterYoureIn.Sets[currentSetPage].CreaturesBelongingToThisSet.Length;
                                currentSetPage++;
                            }
                            else if (currentSetPage == ChapterYoureIn.Sets.Length - 1 && currentChapter != ModEntry.Chapters.Count - 1)
                            {
                                currentChapter++;
                                currentID = 0;
                                IsHeaderPage = true;
                                currentSetPage = 0;
                                ChapterYoureIn = ModEntry.Chapters[currentChapter];
                                CalculateSetPages(ChapterYoureIn.Sets);
                                showSetPaging = ChapterYoureIn.EnableSets;
                            }
                            else if (currentSetPage != ChapterYoureIn.Sets.Length - 2)
                            {
                                currentID += ChapterYoureIn.Sets[currentSetPage].CreaturesBelongingToThisSet.Length;
                                currentID += ChapterYoureIn.Sets[currentSetPage + 1].CreaturesBelongingToThisSet.Length;
                                currentSetPage++; currentSetPage++;
                                if (currentSetPage != ChapterYoureIn.Sets.Length - 2 && currentChapter == ModEntry.Chapters.Count - 1)
                                {
                                    lastSetOnItsOwn = true;
                                }
                            }
                        }
                    }
                    break;

                case 1: //left arrow
                    if (IsHeaderPage)
                        IsHeaderPage = false;
                    else if (currentID > 0 && !showSetPaging)
                    {
                        if (currentID == 0 && currentChapter != 0)
                        {
                            currentChapter--;
                            ChapterYoureIn = ModEntry.Chapters[currentChapter];
                            currentID = ChapterYoureIn.Creatures.Length - 1;
                            WasHeaderPage = true;
                            IsHeaderPage = true;
                            if (ChapterYoureIn.EnableSets)
                            {
                                CalculateSetPages(ChapterYoureIn.Sets);
                                currentSetPage = ChapterYoureIn.Sets.Length - 1;
                            }
                            showSetPaging = ChapterYoureIn.EnableSets;
                        }
                        currentID--;
                    }
                    else if (showSetPaging)
                    {
                        if (currentSetPage > 0)
                        {
                            if (0 == currentSetPage % 2)
                            {
                                currentID -= ChapterYoureIn.Sets[currentSetPage - 1].CreaturesBelongingToThisSet.Length;
                                currentSetPage--;
                            }
                            else if (currentSetPage is not 1 || currentSetPage == pageOrder.Count - 1)
                            {
                                currentID -= ChapterYoureIn.Sets[currentSetPage - 1].CreaturesBelongingToThisSet.Length;
                                currentID -= ChapterYoureIn.Sets[currentSetPage - 2].CreaturesBelongingToThisSet.Length;
                                currentSetPage--;
                                currentSetPage--;
                            }
                            lastSetOnItsOwn = false;
                        }
                        else if (currentSetPage == 0 && currentChapter != 0)
                        {
                            currentChapter--;
                            ChapterYoureIn = ModEntry.Chapters[currentChapter];
                            currentID = ChapterYoureIn.Creatures.Length - 1;
                            WasHeaderPage = true;
                            IsHeaderPage = true;
                            if (ChapterYoureIn.EnableSets)
                            {
                                CalculateSetPages(ChapterYoureIn.Sets);
                                currentSetPage = ChapterYoureIn.Sets.Length - 1;
                            }
                            showSetPaging = ChapterYoureIn.EnableSets;
                        }
                    }
                    else if (currentID == 0)
                        IsHeaderPage = true;
                    break;
            }
            for (int i = 0; i < 4; i++)
                FindPageAndCheckSides(PagesWithStickies[i], false, i);
            UpdateNotebookPage();
        }
        private void SetButtons(int whichButton)
        {
            switch (whichButton)
            {
                case 0: //Switch 1
                    showSetPaging = false;
                    if (!currentID.Equals(ChapterYoureIn.Sets[currentSetPage].CreaturesBelongingToThisSet[0]))
                        currentID = ChapterYoureIn.Sets[currentSetPage].CreaturesBelongingToThisSet[0];
                    
                    if (lastSetOnItsOwn)
                        lastSetOnItsOwn = false;
                    
                    break;
                case 1: //Switch 2

                    showSetPaging = false;
                    if (currentID != ChapterYoureIn.Sets[currentSetPage + 1].CreaturesBelongingToThisSet[0] || currentSetPage != 0)
                        currentID = ChapterYoureIn.Sets[currentSetPage + 1].CreaturesBelongingToThisSet[0];

                    else if (currentSetPage == 0)
                        currentID += ChapterYoureIn.Sets[currentSetPage].CreaturesBelongingToThisSet.Length;
                    
                    wasOnSecondPage = true;
                    UpdateNotebookPage();
                    break;
                case 2: //ShowSetView
                    showSetPaging = true;
                    for (int i = 0; i < ChapterYoureIn.Sets.Length; i++)
                    {
                        foreach (var item in ChapterYoureIn.Sets[i].CreaturesBelongingToThisSet)
                        {
                            if (currentID == item)
                            {
                                currentSetPage = i;
                            }
                        }
                    }
                    if (lastSetOnItsOwn)
                    {
                        currentSetPage = ChapterYoureIn.Sets.Length - 1;
                    }
                    else if (wasOnSecondPage && currentSetPage > 0)
                    {
                        currentID = ChapterYoureIn.Sets[currentSetPage - 1].CreaturesBelongingToThisSet[0];
                        currentSetPage--;
                        wasOnSecondPage = false;
                    }
                    else if (wasOnSecondPage && currentSetPage == 0)
                        currentID = 0;
                    break; 
            }
            UpdateNotebookPage();
        }
    }
}
