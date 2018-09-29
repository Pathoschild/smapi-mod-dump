using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using StardustCore;
namespace StarAI.MenuCore
{

    public class CropUtilityMenu : IClickableMenu
    {
        public static int widthToMoveActiveTab = Game1.tileSize / 8;
        private string descriptionText = "";
        private string hoverText = "";
        public List<ClickableTextureComponent> sideTabs = new List<ClickableTextureComponent>();
        public Dictionary<int, List<List<ClickableTextureComponent>>> collections = new Dictionary<int, List<List<ClickableTextureComponent>>>();
        public const int region_sideTabShipped = 7001;
        public const int region_sideTabFish = 7002;
        public const int region_sideTabArtifacts = 7003;
        public const int region_sideTabMinerals = 7004;
        public const int region_sideTabCooking = 7005;
        public const int region_sideTabAchivements = 7006;
        public const int region_forwardButton = 707;
        public const int region_backButton = 706;
        public const int organicsTab = 0;
        public const int fishTab = 1;
        public const int archaeologyTab = 2;
        public const int mineralsTab = 3;
        public const int cookingTab = 4;
        public const int achievementsTab = 5;
        public const int distanceFromMenuBottomBeforeNewPage = 128;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent forwardButton;
        private int currentTab;
        private int currentPage;
        private int value;

      

        public CropUtilityMenu(int x, int y, int width, int height)
          : base(x, y, width, height, false)
        {
            List<ClickableTextureComponent> sideTabs1 = this.sideTabs;
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4 + CollectionsPage.widthToMoveActiveTab, this.yPositionOnScreen + Game1.tileSize * 2, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Shipped"), Game1.mouseCursors, new Rectangle(640, 80, 16, 16), (float)Game1.pixelZoom, false);
            int num1 = 7001;
            textureComponent1.myID = num1;
            int num2 = 7002;
            textureComponent1.downNeighborID = num2;
            int num3 = 0;
            textureComponent1.rightNeighborID = num3;
            sideTabs1.Add(textureComponent1);
            this.collections.Add(0, new List<List<ClickableTextureComponent>>());
            List<ClickableTextureComponent> sideTabs2 = this.sideTabs;
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Fish"), Game1.mouseCursors, new Rectangle(640, 64, 16, 16), (float)Game1.pixelZoom, false);
            int num4 = 7002;
            textureComponent2.myID = num4;
            int num5 = 7001;
            textureComponent2.upNeighborID = num5;
            int num6 = 7003;
            textureComponent2.downNeighborID = num6;
            int num7 = 0;
            textureComponent2.rightNeighborID = num7;
            sideTabs2.Add(textureComponent2);
            this.collections.Add(1, new List<List<ClickableTextureComponent>>());
            List<ClickableTextureComponent> sideTabs3 = this.sideTabs;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 4, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Artifacts"), Game1.mouseCursors, new Rectangle(656, 64, 16, 16), (float)Game1.pixelZoom, false);
            int num8 = 7003;
            textureComponent3.myID = num8;
            int num9 = 7002;
            textureComponent3.upNeighborID = num9;
            int num10 = 7004;
            textureComponent3.downNeighborID = num10;
            int num11 = 0;
            textureComponent3.rightNeighborID = num11;
            sideTabs3.Add(textureComponent3);
            this.collections.Add(2, new List<List<ClickableTextureComponent>>());
            List<ClickableTextureComponent> sideTabs4 = this.sideTabs;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 5, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Minerals"), Game1.mouseCursors, new Rectangle(672, 64, 16, 16), (float)Game1.pixelZoom, false);
            int num12 = 7004;
            textureComponent4.myID = num12;
            int num13 = 7003;
            textureComponent4.upNeighborID = num13;
            int num14 = 7005;
            textureComponent4.downNeighborID = num14;
            int num15 = 0;
            textureComponent4.rightNeighborID = num15;
            sideTabs4.Add(textureComponent4);
            this.collections.Add(3, new List<List<ClickableTextureComponent>>());
            List<ClickableTextureComponent> sideTabs5 = this.sideTabs;
            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 6, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Cooking"), Game1.mouseCursors, new Rectangle(688, 64, 16, 16), (float)Game1.pixelZoom, false);
            int num16 = 7005;
            textureComponent5.myID = num16;
            int num17 = 7004;
            textureComponent5.upNeighborID = num17;
            int num18 = 7006;
            textureComponent5.downNeighborID = num18;
            int num19 = 0;
            textureComponent5.rightNeighborID = num19;
            sideTabs5.Add(textureComponent5);
            this.collections.Add(4, new List<List<ClickableTextureComponent>>());
            List<ClickableTextureComponent> sideTabs6 = this.sideTabs;
            ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 7, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Achievements"), Game1.mouseCursors, new Rectangle(656, 80, 16, 16), (float)Game1.pixelZoom, false);
            int num20 = 7006;
            textureComponent6.myID = num20;
            int num21 = 7005;
            textureComponent6.upNeighborID = num21;
            int num22 = 0;
            textureComponent6.rightNeighborID = num22;
            sideTabs6.Add(textureComponent6);
            this.collections.Add(5, new List<List<ClickableTextureComponent>>());
            CollectionsPage.widthToMoveActiveTab = Game1.tileSize / 8;
            ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + height - 20 * Game1.pixelZoom, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), (float)Game1.pixelZoom, false);
            int num23 = 706;
            textureComponent7.myID = num23;
            int num24 = -7777;
            textureComponent7.rightNeighborID = num24;
            this.backButton = textureComponent7;
            ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - Game1.tileSize / 2 - 15 * Game1.pixelZoom, this.yPositionOnScreen + height - 20 * Game1.pixelZoom, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), (float)Game1.pixelZoom, false);
            int num25 = 707;
            textureComponent8.myID = num25;
            int num26 = -7777;
            textureComponent8.leftNeighborID = num26;
            this.forwardButton = textureComponent8;
            int[] numArray = new int[this.sideTabs.Count];
            int num27 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int num28 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4;
            int num29 = 10;
            foreach (KeyValuePair<int, string> keyValuePair in Game1.objectInformation)
            {
                string str = keyValuePair.Value.Split('/')[3];
                bool drawShadow = false;
                int index;
                if (str.Contains("Arch"))
                {
                    index = 2;
                    if (Game1.player.archaeologyFound.ContainsKey(keyValuePair.Key))
                        drawShadow = true;
                }
                else if (str.Contains("Fish"))
                {
                    if (keyValuePair.Key < 167 || keyValuePair.Key >= 173)
                    {
                        index = 1;
                        if (Game1.player.fishCaught.ContainsKey(keyValuePair.Key))
                            drawShadow = true;
                    }
                    else
                        continue;
                }
                else if (str.Contains("Mineral") || str.Substring(str.Length - 3).Equals("-2"))
                {
                    index = 3;
                    if (Game1.player.mineralsFound.ContainsKey(keyValuePair.Key))
                        drawShadow = true;
                }
                else if (str.Contains("Cooking") || str.Substring(str.Length - 3).Equals("-7"))
                {
                    index = 4;
                    if (Game1.player.recipesCooked.ContainsKey(keyValuePair.Key))
                        drawShadow = true;
                    if (keyValuePair.Key == 217 || keyValuePair.Key == 772 || keyValuePair.Key == 773)
                        continue;
                }
                else if(keyValuePair.Key>0)
                {
                    index = 0;
                    StardewValley.Object obj = new StardewValley.Object(keyValuePair.Key, 1);
                    if (obj.getCategoryName()!="Seed") continue;
                    drawShadow = true;
                    
                }
                else
                    continue;
                int x1 = num27 + numArray[index] % num29 * (Game1.tileSize + 4);
                int y1 = num28 + numArray[index] / num29 * (Game1.tileSize + 4);
                if (y1 > this.yPositionOnScreen + height - 128)
                {
                    this.collections[index].Add(new List<ClickableTextureComponent>());
                    numArray[index] = 0;
                    x1 = num27;
                    y1 = num28;
                }
                if (this.collections[index].Count == 0)
                    this.collections[index].Add(new List<ClickableTextureComponent>());
                StardewValley.Object o = new StardewValley.Object(keyValuePair.Key, 1);
                if (o.getCategoryName() != "Seed") continue;
                List<ClickableTextureComponent> textureComponentList = this.collections[index].Last<List<ClickableTextureComponent>>();
                float scale= Game1.pixelZoom * (1 + UtilityCore.SeedCropUtility.getUtilityScaleValue(o.parentSheetIndex));
                if (UtilityCore.SeedCropUtility.getUtilityScaleValue(o.parentSheetIndex)==0) scale = 1.00f;
                ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(keyValuePair.Key.ToString() + " " + drawShadow.ToString(), new Rectangle(x1, y1, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, keyValuePair.Key, 16, 16),Game1.pixelZoom, drawShadow);
                textureComponent9.scale = scale;
                int count = this.collections[index].Last<List<ClickableTextureComponent>>().Count;
                textureComponent9.myID = count;
                int num30 = (this.collections[index].Last<List<ClickableTextureComponent>>().Count + 1) % num29 == 0 ? -1 : this.collections[index].Last<List<ClickableTextureComponent>>().Count + 1;
                textureComponent9.rightNeighborID = num30;
                int num31 = this.collections[index].Last<List<ClickableTextureComponent>>().Count % num29 == 0 ? 7001 : this.collections[index].Last<List<ClickableTextureComponent>>().Count - 1;
                textureComponent9.leftNeighborID = num31;
                int num32 = y1 + (Game1.tileSize + 4) > this.yPositionOnScreen + height - 128 ? -7777 : this.collections[index].Last<List<ClickableTextureComponent>>().Count + num29;
                textureComponent9.downNeighborID = num32;
                int num33 = this.collections[index].Last<List<ClickableTextureComponent>>().Count < num29 ? 12345 : this.collections[index].Last<List<ClickableTextureComponent>>().Count - num29;
                textureComponent9.upNeighborID = num33;
                int num34 = 1;
                textureComponent9.fullyImmutable = num34 != 0;
                textureComponentList.Add(textureComponent9);
                ++numArray[index];
            }
            if (this.collections[5].Count == 0)
                this.collections[5].Add(new List<ClickableTextureComponent>());
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            base.customSnapBehavior(direction, oldRegion, oldID);
            if (direction == 2)
            {
                if (this.currentPage > 0)
                    this.currentlySnappedComponent = this.getComponentWithID(706);
                else if (this.currentPage == 0 && this.collections[this.currentTab].Count > 1)
                    this.currentlySnappedComponent = this.getComponentWithID(707);
                this.backButton.upNeighborID = oldID;
                this.forwardButton.upNeighborID = oldID;
            }
            else if (direction == 3)
            {
                if (oldID != 707 || this.currentPage <= 0)
                    return;
                this.currentlySnappedComponent = this.getComponentWithID(706);
            }
            else
            {
                if (direction != 1 || oldID != 706 || this.collections[this.currentTab].Count <= this.currentPage + 1)
                    return;
                this.currentlySnappedComponent = this.getComponentWithID(707);
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            base.snapToDefaultClickableComponent();
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            for (int index = 0; index < this.sideTabs.Count; ++index)
            {
                if (this.sideTabs[index].containsPoint(x, y) && this.currentTab != index)
                {
                    Game1.playSound("smallSelect");
                    this.sideTabs[this.currentTab].bounds.X -= CollectionsPage.widthToMoveActiveTab;
                    this.currentTab = index;
                    this.currentPage = 0;
                    this.sideTabs[index].bounds.X += CollectionsPage.widthToMoveActiveTab;
                }
            }
            if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
            {
                this.currentPage = this.currentPage - 1;
                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
                if (Game1.options.snappyMenus && Game1.options.gamepadControls && this.currentPage == 0)
                {
                    this.currentlySnappedComponent = (ClickableComponent)this.forwardButton;
                    Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
                }
            }
            ClickableTextureComponent texture;
            foreach (ClickableTextureComponent textureComponent in this.collections[0][this.currentPage])
            {
                if (textureComponent.containsPoint(x, y))
                {
                    string[] s = textureComponent.name.Split(' ');
                    ModCore.CoreMonitor.Log(s[0]);
                    ModCore.CoreMonitor.Log("CLICKED A THING!");
                    UtilityCore.SeedCropUtility.updateUserUtilities(Convert.ToInt32(s[0]), 0.05f);
                    textureComponent.scale = Game1.pixelZoom * (1 + UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])));
                    if (UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0]))==0) textureComponent.scale = 1.00f;
                    texture = textureComponent;
                }
            }
                if (this.currentPage >= this.collections[this.currentTab].Count - 1 || !this.forwardButton.containsPoint(x, y))
                return;
            this.currentPage = this.currentPage + 1;
            Game1.playSound("shwip");
            this.forwardButton.scale = this.forwardButton.baseScale;
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.currentPage != this.collections[this.currentTab].Count - 1)
                return;
            this.currentlySnappedComponent = (ClickableComponent)this.backButton;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableTextureComponent textureComponent in this.collections[0][this.currentPage])
            {
                if (textureComponent.containsPoint(x, y))
                {
                    string[] s = textureComponent.name.Split(' ');
                    ModCore.CoreMonitor.Log(s[0]);
                    ModCore.CoreMonitor.Log("CLICKED A THING!");
                    UtilityCore.SeedCropUtility.updateUserUtilities(Convert.ToInt32(s[0]), -0.05f);
                    //ModCore.CoreMonitor.Log(textureComponent.scale.ToString());
                    textureComponent.scale = Game1.pixelZoom * (1 + UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])));
                    if (UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])) == 0) textureComponent.scale = 1.00f;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.descriptionText = "";
            this.hoverText = "";
            this.value = -1;
            int i = 0;
            foreach (ClickableTextureComponent sideTab in this.sideTabs)
            {
                if (i != 0) break;
                if (sideTab.containsPoint(x, y))
                {
                    this.hoverText = sideTab.hoverText;
                    return;
                }
                i++;
            }
            foreach (ClickableTextureComponent textureComponent in this.collections[this.currentTab][this.currentPage])
            {
                if (textureComponent.containsPoint(x, y))
                {
                    string[] s = textureComponent.name.Split(' ');

                    textureComponent.scale = Game1.pixelZoom * (1+ UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])));
                    if (UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])) == 0) textureComponent.scale = 1.00f;
                    if (Convert.ToBoolean(textureComponent.name.Split(' ')[1]) || this.currentTab == 5) {
                        this.hoverText = this.createDescription(Convert.ToInt32(textureComponent.name.Split(' ')[0]));
                        this.hoverText += "\n\nAI Utility Value: " +Math.Round(UtilityCore.SeedCropUtility.CropSeedUtilityDictionary[(Convert.ToInt32(s[0]))],3);
                        this.hoverText += "\n\nUser Utility Value: " +Math.Round(UtilityCore.SeedCropUtility.UserCropSeedUtilityDictionary[(Convert.ToInt32(s[0]))],3);
                        this.hoverText += "\n\nTotal Utility Value: " +Math.Round(UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])),3);
                    }
                    else
                        this.hoverText = "???";
                }
                else
                {
                    string[] s = textureComponent.name.Split(' ');
                    textureComponent.scale = Game1.pixelZoom * (1 + UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])));
                    if (UtilityCore.SeedCropUtility.getUtilityScaleValue(Convert.ToInt32(s[0])) == 0) textureComponent.scale = 1.00f;
                }
            }
            this.forwardButton.tryHover(x, y, 0.5f);
            this.backButton.tryHover(x, y, 0.5f);
        }

        public string createDescription(int index)
        {
            string str1 = "";
            string str2;
            if (this.currentTab == 5)
            {
                string[] strArray = Game1.achievements[index].Split('^');
                str2 = str1 + strArray[0] + Environment.NewLine + Environment.NewLine + strArray[1];
            }
            else
            {
                string[] strArray = Game1.objectInformation[index].Split('/');
                string str3 = strArray[4];
                string str4 = str1 + str3 + Environment.NewLine + Environment.NewLine + Game1.parseText(strArray[5], Game1.smallFont, Game1.tileSize * 4) + Environment.NewLine + Environment.NewLine;
                if (strArray[3].Contains("Arch"))
                {
                    string str5 = str4;
                    string str6;
                    if (!Game1.player.archaeologyFound.ContainsKey(index))
                        str6 = "";
                    else
                        str6 = Game1.content.LoadString("Strings\\UI:Collections_Description_ArtifactsFound", (object)Game1.player.archaeologyFound[index][0]);
                    str2 = str5 + str6;
                }
                else if (strArray[3].Contains("Cooking"))
                {
                    string str5 = str4;
                    string str6;
                    if (!Game1.player.recipesCooked.ContainsKey(index))
                        str6 = "";
                    else
                        str6 = Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", (object)Game1.player.recipesCooked[index]);
                    str2 = str5 + str6;
                }
                else if (strArray[3].Contains("Fish"))
                {
                    str2 = str4 + Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", (object)(Game1.player.fishCaught.ContainsKey(index) ? Game1.player.fishCaught[index][0] : 0));
                    if (Game1.player.fishCaught.ContainsKey(index) && Game1.player.fishCaught[index][1] > 0)
                        str2 = str2 + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Collections_Description_BiggestCatch", (object)(LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double)Game1.player.fishCaught[index][1] * 2.54) : (double)Game1.player.fishCaught[index][1]));
                }
                else if (strArray[3].Contains("Minerals") || strArray[3].Substring(strArray[3].Length - 3).Equals("-2"))
                    str2 = str4 + Game1.content.LoadString("Strings\\UI:Collections_Description_MineralsFound", (object)(Game1.player.mineralsFound.ContainsKey(index) ? Game1.player.mineralsFound[index] : 0));
                else
                    str2 = str4 + Game1.content.LoadString("Strings\\UI:Collections_Description_NumberShipped", (object)(Game1.player.basicShipped.ContainsKey(index) ? Game1.player.basicShipped[index] : 0));
                this.value = Convert.ToInt32(strArray[1]);
            }
            return str2;
        }

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            int i = 0;
            foreach (ClickableTextureComponent sideTab in this.sideTabs)
            {
                sideTab.draw(b);
                i++;
                if (i != 0) break;
            }
            if (this.currentPage > 0)
                this.backButton.draw(b);
            if (this.currentPage < this.collections[this.currentTab].Count - 1)
                this.forwardButton.draw(b);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            foreach (ClickableTextureComponent textureComponent in this.collections[0][this.currentPage])
            {
                bool boolean = textureComponent.scale.Equals(1.00f);
                textureComponent.draw(b, boolean ? Color.White * 0.6f : Color.White, 0.86f);
                if (this.currentTab == 5 & boolean)
                {
                    int num = new Random(Convert.ToInt32(textureComponent.name.Split(' ')[0])).Next(12);
                    b.Draw(Game1.mouseCursors, new Vector2((float)(textureComponent.bounds.X + 16 + Game1.tileSize / 4), (float)(textureComponent.bounds.Y + 20 + Game1.tileSize / 4)), new Rectangle?(new Rectangle(256 + num % 6 * Game1.tileSize / 2, 128 + num / 6 * Game1.tileSize / 2, Game1.tileSize / 2, Game1.tileSize / 2)), Color.White, 0.0f, new Vector2((float)(Game1.tileSize / 4), (float)(Game1.tileSize / 4)), textureComponent.scale, SpriteEffects.None, 0.88f);
                }
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            if (this.hoverText.Equals(""))
            {
                this.drawMouse(b);
                return;
            }
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, this.value, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            this.drawMouse(b);
        }
    }
}