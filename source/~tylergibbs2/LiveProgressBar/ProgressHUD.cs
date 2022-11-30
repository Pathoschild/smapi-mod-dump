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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace LiveProgressBar
{
    class ProgressHUD : IClickableMenu
    {
        private float Progress { get; set; }

        private bool ShowExtra { get; set; } = false;
        private bool IsVisible { get; set; } = true;

        private int extraWidth;
        private int extraHeight;

        public ProgressHUD(float progress)
        {
            width = 200;
            height = 175;

            extraWidth = 500;
            extraHeight = 600;

            Progress = progress;
            CalculatePositions();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            CalculatePositions();
        }

        private void CalculatePositions()
        {
            xPositionOnScreen = Game1.uiViewport.Width - width;
            yPositionOnScreen = (Game1.uiViewport.Height / 2) - height;
        }
        public void SetProgress(float progress)
        {
            Progress = progress;
        }

        public void SetVisible(bool state)
        {
            IsVisible = state;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            ShowExtra = !ShowExtra;
        }

        private static List<string> GetExtraStrings()
        {
            List<string> strings = new();

            float ItemsShippedPrct = Math.Min(0f + Utility.GetFarmCompletion((Farmer farmer) => Utility.getFarmerItemsShippedPercent(farmer)).Value, 1f);
            int ObelisksBuilt = Math.Min(Utility.numObelisksOnFarm(), 4);
            float SlayerQuestsPrct = Math.Min(GetMonsterQuestPercent(), 1f);
            float MaxFriendshipPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getMaxedFriendshipPercent(farmer)).Value, 1f);
            int Level = (int)Math.Min(Utility.GetFarmCompletion((Farmer farmer) => farmer.Level).Value, 25);
            int StardropsFound = Math.Min(GetStardropsFound(), 7);
            float CookedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCookedRecipesPercent(farmer)).Value, 1f);
            float CraftedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCraftedRecipesPercent(farmer)).Value, 1f);
            float FishCaughtPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getFishCaughtPercent(farmer)).Value, 1f);
            int walnutsFound = Math.Min(Game1.netWorldState.Value.GoldenWalnutsFound.Value, 130);
            string GoldClock = Game1.getFarm().isBuildingConstructed("Gold Clock") ? "Yes" : "No";

            strings.Add($"Items Shipped: {ItemsShippedPrct:P0}");
            strings.Add($"Obelisks Built: {ObelisksBuilt}/4");
            strings.Add($"Gold Clock Built: {GoldClock}");
            strings.Add($"Slayer Quests: {SlayerQuestsPrct:P0}");
            strings.Add($"Max Friendships: {MaxFriendshipPrct:P0}");
            strings.Add($"Farmer Level: {Level}/25");
            strings.Add($"Stardrops Found: {StardropsFound}/7");
            strings.Add($"Cooked Recipes: {CookedRecipesPrct:P0}");
            strings.Add($"Crafted Recipes: {CraftedRecipesPrct:P0}");
            strings.Add($"Fish Caught: {FishCaughtPrct:P0}");
            strings.Add($"Golden Walnuts: {walnutsFound}/130");

            return strings;
        }

        private static int GetStardropsFound()
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

        private static float GetMonsterQuestPercent()
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

        public override void draw(SpriteBatch b)
        {
            if (!IsVisible)
                return;

            // removes the leading space
            System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.NumberFormat.PercentPositivePattern = 1;  // Avoid putting a space between a number and its percentage
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;

            string percentString = Progress > 1f ? "100%" : $"{Progress:P2}";
            Vector2 textPos = new(xPositionOnScreen + (width / 3) - 25, yPositionOnScreen + (height / 2) + 10);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            Utility.drawTextWithShadow(b, percentString, Game1.dialogueFont, textPos, Game1.textColor);

            if (ShowExtra)
            {
                int startingX = xPositionOnScreen - extraWidth;
                int startingY = (Game1.uiViewport.Height / 2) - height;
                Game1.drawDialogueBox(startingX, startingY, extraWidth, extraHeight, false, true);
                textPos = new Vector2(startingX + 40, startingY + 100);
                foreach (string stat in GetExtraStrings())
                {
                    Utility.drawTextWithShadow(b, stat, Game1.dialogueFont, textPos, Game1.textColor);
                    textPos.Y += 42;
                }
            }

            drawMouse(b);
        }
    }
}
