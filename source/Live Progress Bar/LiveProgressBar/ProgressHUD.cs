/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/LiveProgressBar
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace LiveProgressBar
{
    class ProgressHUD : IClickableMenu
    {
        private float progress;

        private bool showExtra = false;
        private bool isVisible = true;

        private int extraWidth;
        private int extraHeight;

        private ClickableComponent percentClick;

        public ProgressHUD(float progress)
        {
            this.width = 200;
            this.height = 175;

            this.extraWidth = 500;
            this.extraHeight = 600;

            this.progress = progress;
            this.CalcPositions();
            this.percentClick = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), "percent");
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.CalcPositions();
            this.percentClick = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), "percent");
        }

        private void CalcPositions()
        {
            this.xPositionOnScreen = Game1.uiViewport.Width - this.width;
            this.yPositionOnScreen = (Game1.uiViewport.Height / 2) - this.height;
        }
        public void SetProgress(float progress)
        {
            this.progress = progress;
        }

        public void SetVisible(bool state)
        {
            this.isVisible = state;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.showExtra = !this.showExtra;
        }

        private List<string> GetExtraStrings()
        {
            List<string> strings = new List<string>();

            float ItemsShippedPrct = Math.Min(0f + Utility.GetFarmCompletion((Farmer farmer) => Utility.getFarmerItemsShippedPercent(farmer)).Value, 1f);
            int ObelisksBuilt = Math.Min(Utility.numObelisksOnFarm(), 4);
            float SlayerQuestsPrct = Math.Min(this.GetMonsterQuestPercent(), 1f);
            float MaxFriendshipPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getMaxedFriendshipPercent(farmer)).Value, 1f);
            int Level = (int)Math.Min(Utility.GetFarmCompletion((Farmer farmer) => farmer.Level).Value, 25);
            int StardropsFound = Math.Min(this.GetStardropsFound(), 7);
            float CookedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCookedRecipesPercent(farmer)).Value, 1f);
            float CraftedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCraftedRecipesPercent(farmer)).Value, 1f);
            float FishCaughtPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getFishCaughtPercent(farmer)).Value, 1f);
            int walnutsFound = Math.Min((int)Game1.netWorldState.Value.GoldenWalnutsFound, 130);

            string GoldClock;
            if (Game1.getFarm().isBuildingConstructed("Gold Clock"))
                GoldClock = "Yes";
            else
                GoldClock = "No";

            strings.Add(string.Format("Items Shipped: {0:P0}", ItemsShippedPrct));
            strings.Add(string.Format("Obelisks Built: {0}/4", ObelisksBuilt));
            strings.Add(string.Format("Gold Clock Built: {0}", GoldClock));
            strings.Add(string.Format("Slayer Quests: {0:P0}", SlayerQuestsPrct));
            strings.Add(string.Format("Max Friendships: {0:P0}", MaxFriendshipPrct));
            strings.Add(string.Format("Farmer Level: {0}/25", Level));
            strings.Add(string.Format("Stardrops Found: {0}/7", StardropsFound));
            strings.Add(string.Format("Cooked Recipes: {0:P0}", CookedRecipesPrct));
            strings.Add(string.Format("Crafted Recipes: {0:P0}", CraftedRecipesPrct));
            strings.Add(string.Format("Fish Caught: {0:P0}", FishCaughtPrct));
            strings.Add(string.Format("Golden Walnuts: {0}/130", walnutsFound));

            return strings;
        }

        public override void draw(SpriteBatch b)
        {
            if (!this.isVisible)
                return;

            // removes the leading space
            System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.NumberFormat.PercentPositivePattern = 1;  // Avoid putting a space between a number and its percentage
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;

            String percentString;
            if (this.progress >= 1f)
                percentString = "100%";
            else
                percentString = string.Format("{0:P2}", this.progress);
    
            Vector2 textPos = new Vector2(this.xPositionOnScreen + (this.width / 3) - 25, this.yPositionOnScreen + (this.height / 2) + 10);

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            Utility.drawTextWithShadow(b, percentString, Game1.dialogueFont, textPos, Game1.textColor);

            if (this.showExtra)
            {
                int startingX = this.xPositionOnScreen - this.extraWidth;
                int startingY = (Game1.uiViewport.Height / 2) - this.height;
                Game1.drawDialogueBox(startingX, startingY, this.extraWidth, this.extraHeight, false, true);
                textPos = new Vector2(startingX + 40, startingY + 100);
                foreach (string stat in this.GetExtraStrings())
                {
                    Utility.drawTextWithShadow(b, stat, Game1.dialogueFont, textPos, Game1.textColor);
                    textPos.Y += 42;
                }
            }

            drawMouse(b);
        }

        private int GetStardropsFound()
        {
            Farmer who = Game1.player;

            int found = 0;

            if (who.hasOrWillReceiveMail("CF_Fair"))
                found += 1;
            if (who.hasOrWillReceiveMail("CF_Fish"))
                found += 1;
            if (who.hasOrWillReceiveMail("CF_Mines"))
                found += 1;
            if (who.hasOrWillReceiveMail("CF_Sewer"))
                found += 1;
            if (who.hasOrWillReceiveMail("museumComplete"))
                found += 1;
            if (who.hasOrWillReceiveMail("CF_Spouse"))
                found += 1;
            if (who.hasOrWillReceiveMail("CF_Statue"))
                found += 1;

            return found;
        }

        private float GetMonsterQuestPercent()
        {
            int num = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge") + Game1.stats.getMonstersKilled("Tiger Slime");
            int shadowsKilled = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute") + Game1.stats.getMonstersKilled("Shadow Sniper");
            int skeletonsKilled = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
            int crabsKilled = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
            int caveInsectsKilled = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
            int batsKilled = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat") + Game1.stats.getMonstersKilled("Iridium Bat");
            int duggyKilled = Game1.stats.getMonstersKilled("Duggy") + Game1.stats.getMonstersKilled("Magma Duggy");
            Game1.stats.getMonstersKilled("Metal Head");
            Game1.stats.getMonstersKilled("Stone Golem");
            int dustSpiritKilled = Game1.stats.getMonstersKilled("Dust Spirit");
            int mummiesKilled = Game1.stats.getMonstersKilled("Mummy");
            int dinosKilled = Game1.stats.getMonstersKilled("Pepper Rex");
            int serpentsKilled = Game1.stats.getMonstersKilled("Serpent") + Game1.stats.getMonstersKilled("Royal Serpent");
            int flameSpiritsKilled = Game1.stats.getMonstersKilled("Magma Sprite") + Game1.stats.getMonstersKilled("Magma Sparker");

            float total = 12f;
            float completed = total;

            if (num < 1000)
                completed -= 1f;
            if (shadowsKilled < 150)
                completed -= 1f;
            if (skeletonsKilled < 50)
                completed -= 1f;
            if (caveInsectsKilled < 125)
                completed -= 1f;
            if (batsKilled < 200)
                completed -= 1f;
            if (duggyKilled < 30)
                completed -= 1f;
            if (dustSpiritKilled < 500)
                completed -= 1f;
            if (crabsKilled < 60)
                completed -= 1f;
            if (mummiesKilled < 100)
                completed -= 1f;
            if (dinosKilled < 50)
                completed -= 1f;
            if (serpentsKilled < 250)
                completed -= 1f;
            if (flameSpiritsKilled < 150)
                completed -= 1f;

            return completed / total;
        }
    }
}
