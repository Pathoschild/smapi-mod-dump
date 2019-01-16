using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace Omegasis.BuyBackCollectables.Framework
{
    /// <summary>The clickable menu which lets the player buy back collectables.</summary>
    internal class BuyBackMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>The organics tab ID.</summary>
        private const int OrganicsTab = 0;

        /// <summary>The fish tab ID.</summary>
        private const int FishTab = 1;

        /// <summary>The archaeology tab ID.</summary>
        private const int ArchaeologyTab = 2;

        /// <summary>The minerals tab ID.</summary>
        private const int MineralsTab = 3;

        /// <summary>The cooking tab ID.</summary>
        private const int CookingTab = 4;

        /// <summary>The achievements tab ID.</summary>
        private const int AchievementsTab = 5;

        /// <summary>The offset to apply to the selected tab.</summary>
        private readonly int WidthToMoveActiveTab = Game1.tileSize / 8;

        /// <summary>The multiplier applied to the cost of buying back a collectable.</summary>
        private readonly double CostMultiplier;

        /// <summary>The back button.</summary>
        private readonly ClickableTextureComponent BackButton;

        /// <summary>The forward button.</summary>
        private readonly ClickableTextureComponent ForwardButton;

        /// <summary>The category tabs shown along the side.</summary>
        private readonly List<ClickableTextureComponent> SideTabs = new List<ClickableTextureComponent>();

        /// <summary>The text to display in a hover tooltip.</summary>
        private string HoverText = "";

        /// <summary>The selected tab.</summary>
        private int CurrentTab;

        /// <summary>The selected page.</summary>
        private int CurrentPage;

        /// <summary>The buttons to show for each tab.</summary>
        private readonly Dictionary<int, List<List<ClickableTextureComponent>>> Collections = new Dictionary<int, List<List<ClickableTextureComponent>>>();

        /// <summary>The cost to buy back the selected item.</summary>
        private int Value;

        /// <summary>The selected item.</summary>
        public Item NewItem;

        /// <summary>The cost to buy back the selected item.</summary>
        public int NewItemValue;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="costMultiplier">The multiplier applied to the cost of buying back a collectable.</param>
        public BuyBackMenu(double costMultiplier)
            : base(Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
        {
            // initialise
            this.CostMultiplier = costMultiplier;

            // create components
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4 + this.WidthToMoveActiveTab, this.yPositionOnScreen + Game1.tileSize * 2, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Shipped"), Game1.mouseCursors, new Rectangle(640, 80, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.OrganicsTab, new List<List<ClickableTextureComponent>>());
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Fish"), Game1.mouseCursors, new Rectangle(640, 64, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.FishTab, new List<List<ClickableTextureComponent>>());
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 4, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Artifacts"), Game1.mouseCursors, new Rectangle(656, 64, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.ArchaeologyTab, new List<List<ClickableTextureComponent>>());
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 5, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Minerals"), Game1.mouseCursors, new Rectangle(672, 64, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.MineralsTab, new List<List<ClickableTextureComponent>>());
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 6, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Cooking"), Game1.mouseCursors, new Rectangle(688, 64, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.CookingTab, new List<List<ClickableTextureComponent>>());
            this.SideTabs.Add(new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - Game1.tileSize * 3 / 4, this.yPositionOnScreen + Game1.tileSize * 7, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:Collections_Achievements"), Game1.mouseCursors, new Rectangle(656, 80, 16, 16), Game1.pixelZoom));
            this.Collections.Add(BuyBackMenu.AchievementsTab, new List<List<ClickableTextureComponent>>());
            this.BackButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize * 3 / 4, this.yPositionOnScreen + this.height - 20 * Game1.pixelZoom, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            this.ForwardButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - Game1.tileSize / 2 - 15 * Game1.pixelZoom, this.yPositionOnScreen + this.height - 20 * Game1.pixelZoom, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom);
            int[] array = new int[this.SideTabs.Count];
            int num = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4;
            int num3 = 10;
            foreach (KeyValuePair<int, string> entry in Game1.objectInformation)
            {
                string fields = entry.Value.Split('/')[3];
                bool drawShadow = false;
                int selectedTab;
                if (fields.Contains("Arch"))
                {
                    selectedTab = BuyBackMenu.ArchaeologyTab;
                    if (Game1.player.archaeologyFound.ContainsKey(entry.Key))
                        drawShadow = true;
                }
                else if (fields.Contains("Fish"))
                {
                    if (entry.Key >= 167 && entry.Key < 173)
                        continue;
                    selectedTab = BuyBackMenu.FishTab;
                    if (Game1.player.fishCaught.ContainsKey(entry.Key))
                        drawShadow = true;
                }
                else if (fields.Contains("Mineral") || fields.Substring(fields.Length - 3).Equals("-2"))
                {
                    selectedTab = BuyBackMenu.MineralsTab;
                    if (Game1.player.mineralsFound.ContainsKey(entry.Key))
                        drawShadow = true;
                }
                else if (fields.Contains("Cooking") || fields.Substring(fields.Length - 3).Equals("-7"))
                {
                    selectedTab = BuyBackMenu.CookingTab;
                    if (Game1.player.recipesCooked.ContainsKey(entry.Key))
                        drawShadow = true;
                    if (entry.Key == 217 || entry.Key == 772 || entry.Key == 773)
                        continue;
                }
                else
                {
                    if (!Object.isPotentialBasicShippedCategory(entry.Key, fields.Substring(fields.Length - 3)))
                        continue;
                    selectedTab = BuyBackMenu.OrganicsTab;
                    if (Game1.player.basicShipped.ContainsKey(entry.Key))
                        drawShadow = true;
                }
                int x2 = num + array[selectedTab] % num3 * (Game1.tileSize + 4);
                int num5 = num2 + array[selectedTab] / num3 * (Game1.tileSize + 4);
                if (num5 > this.yPositionOnScreen + this.height - 128)
                {
                    this.Collections[selectedTab].Add(new List<ClickableTextureComponent>());
                    array[selectedTab] = 0;
                    x2 = num;
                    num5 = num2;
                }
                if (this.Collections[selectedTab].Count == 0)
                    this.Collections[selectedTab].Add(new List<ClickableTextureComponent>());
                this.Collections[selectedTab].Last().Add(new ClickableTextureComponent(entry.Key + " " + drawShadow, new Rectangle(x2, num5, Game1.tileSize, Game1.tileSize), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, entry.Key, 16, 16), Game1.pixelZoom, drawShadow));
                array[selectedTab]++;
            }
            if (this.Collections[5].Count == 0)
                this.Collections[5].Add(new List<ClickableTextureComponent>());
            foreach (KeyValuePair<int, string> current2 in Game1.achievements)
            {
                bool flag = Game1.player.achievements.Contains(current2.Key);
                string[] array2 = current2.Value.Split('^');
                if (flag || (array2[2].Equals("true") && (array2[3].Equals("-1") || this.FarmerHasAchievements(array2[3]))))
                {
                    int x3 = num + array[5] % num3 * (Game1.tileSize + 4);
                    int y2 = num2 + array[5] / num3 * (Game1.tileSize + 4);
                    this.Collections[5][0].Add(new ClickableTextureComponent(current2.Key + " " + flag, new Rectangle(x3, y2, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f));
                    array[5]++;
                }
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the farmer has the given achievements.</summary>
        /// <param name="listOfAchievementNumbers">The achievement IDs as a space-separated list.</param>
        private bool FarmerHasAchievements(string listOfAchievementNumbers)
        {
            string[] array = listOfAchievementNumbers.Split(' ');
            foreach (string text in array)
            {
                if (!Game1.player.achievements.Contains(Convert.ToInt32(text)))
                    return false;
            }
            return true;
        }

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            for (int i = 0; i < this.SideTabs.Count; i++)
            {
                if (this.SideTabs[i].containsPoint(x, y) && this.CurrentTab != i)
                {
                    Game1.playSound("smallSelect");
                    ClickableTextureComponent curTab = this.SideTabs[this.CurrentTab];
                    curTab.bounds.X = curTab.bounds.X - this.WidthToMoveActiveTab;
                    this.CurrentTab = i;
                    this.CurrentPage = 0;
                    ClickableTextureComponent newTab = this.SideTabs[i];
                    newTab.bounds.X = newTab.bounds.X + this.WidthToMoveActiveTab;
                }
            }
            if (this.CurrentPage > 0 && this.BackButton.containsPoint(x, y))
            {
                this.CurrentPage--;
                Game1.playSound("shwip");
                this.BackButton.scale = this.BackButton.baseScale;
            }
            if (this.CurrentPage < this.Collections[this.CurrentTab].Count - 1 && this.ForwardButton.containsPoint(x, y))
            {
                this.CurrentPage++;
                Game1.playSound("shwip");
                this.ForwardButton.scale = this.ForwardButton.baseScale;
            }
            foreach (ClickableTextureComponent current2 in this.Collections[this.CurrentTab][this.CurrentPage])
            {
                if (current2.containsPoint(x, y) && this.NewItem != null && Game1.player.money >= this.Value)
                {
                    Game1.player.money -= this.Value;
                    Game1.playSound("coin");
                    Game1.player.addItemByMenuIfNecessary(this.NewItem);
                }
            }
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.NewItem != null && Game1.player.money >= this.Value)
            {
                Game1.player.money -= this.Value;
                Game1.player.addItemByMenuIfNecessary(this.NewItem);
                Game1.playSound("coin");
            }
        }

        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            this.HoverText = "";
            this.Value = -1;
            foreach (ClickableTextureComponent current in this.SideTabs)
            {
                if (current.containsPoint(x, y))
                {
                    this.HoverText = current.hoverText;
                    return;
                }
            }
            foreach (ClickableTextureComponent current2 in this.Collections[this.CurrentTab][this.CurrentPage])
            {
                if (current2.containsPoint(x, y))
                {
                    current2.scale = Math.Min(current2.scale + 0.02f, current2.baseScale + 0.1f);
                    if (Convert.ToBoolean(current2.name.Split(' ')[1]) || this.CurrentTab == 5)
                        this.HoverText = this.CreateDescription(Convert.ToInt32(current2.name.Split(' ')[0]));
                    else
                        this.HoverText = "???";
                }
                else
                {
                    current2.scale = Math.Max(current2.scale - 0.02f, current2.baseScale);
                }
            }
            this.ForwardButton.tryHover(x, y, 0.5f);
            this.BackButton.tryHover(x, y, 0.5f);
        }

        /// <summary>Generate the item description for an item ID.</summary>
        /// <param name="index">The item ID.</param>
        private string CreateDescription(int index)
        {
            string text = "";
            if (this.CurrentTab == 5)
            {
                string[] array = Game1.achievements[index].Split('^');
                text = text + array[0] + Environment.NewLine + Environment.NewLine;
                text += array[1];
                this.NewItem = null;
            }
            else
            {
                string[] array2 = Game1.objectInformation[index].Split('/');
                foreach (KeyValuePair<int, string> meh in Game1.objectInformation)
                {
                    string[] array3 = meh.Value.Split('/');
                    if (array3[0] == array2[0])
                    {
                        this.NewItem = new Object(Convert.ToInt32(meh.Key), 1);
                        if (this.NewItem.Name == "Stone" || this.NewItem.Name == "stone") this.NewItem = new Object(390, 1);
                    }
                }
                text = string.Concat(text, array2[0], Environment.NewLine, Environment.NewLine, Game1.parseText(array2[4], Game1.smallFont, Game1.tileSize * 4), Environment.NewLine, Environment.NewLine);
                if (array2[3].Contains("Arch"))
                {
                    text += (Game1.player.archaeologyFound.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_ArtifactsFound", Game1.player.archaeologyFound[index][0]) : "");
                }
                else if (array2[3].Contains("Cooking"))
                {
                    text += (Game1.player.recipesCooked.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", Game1.player.recipesCooked[index]) : "");
                }
                else if (array2[3].Contains("Fish"))
                {
                    text += Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", Game1.player.fishCaught.ContainsKey(index) ? Game1.player.fishCaught[index][0] : 0);
                    if (Game1.player.fishCaught.ContainsKey(index) && Game1.player.fishCaught[index][1] > 0)
                    {
                        text = text + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Collections_Description_BiggestCatch", Game1.player.fishCaught[index][1]);
                    }
                }
                else if (array2[3].Contains("Minerals") || array2[3].Substring(array2[3].Length - 3).Equals("-2"))
                {
                    text += Game1.content.LoadString("Strings\\UI:Collections_Description_MineralsFound", Game1.player.mineralsFound.ContainsKey(index) ? Game1.player.mineralsFound[index] : 0);
                }
                else
                {
                    text += Game1.content.LoadString("Strings\\UI:Collections_Description_NumberShipped", Game1.player.basicShipped.ContainsKey(index) ? Game1.player.basicShipped[index] : 0);
                }
                this.Value = Convert.ToInt32(array2[1]);
                this.Value = (int)(this.Value * this.CostMultiplier);
                this.NewItemValue = this.Value;
            }
            return text;
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            foreach (var tab in this.SideTabs)
                tab.draw(b);

            if (this.CurrentPage > 0)
                this.BackButton.draw(b);
            if (this.CurrentPage < this.Collections[this.CurrentTab].Count - 1)
                this.ForwardButton.draw(b);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            foreach (ClickableTextureComponent current in this.Collections[this.CurrentTab][this.CurrentPage])
            {
                bool flag = Convert.ToBoolean(current.name.Split(' ')[1]);
                current.draw(b, flag ? Color.White : (Color.Black * 0.2f), 0.86f);
                if (this.CurrentTab == 5 & flag)
                {
                    int num = new Random(Convert.ToInt32(current.name.Split(' ')[0])).Next(12);
                    b.Draw(Game1.mouseCursors, new Vector2(current.bounds.X + 16 + Game1.tileSize / 4, current.bounds.Y + 20 + Game1.tileSize / 4), new Rectangle(256 + num % 6 * Game1.tileSize / 2, 128 + num / 6 * Game1.tileSize / 2, Game1.tileSize / 2, Game1.tileSize / 2), Color.White, 0f, new Vector2(Game1.tileSize / 4, Game1.tileSize / 4), current.scale, SpriteEffects.None, 0.88f);
                }
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            if (!this.HoverText.Equals(""))
                IClickableMenu.drawHoverText(b, this.HoverText, Game1.smallFont, 0, 0, this.Value);
            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);

        }
    }
}
