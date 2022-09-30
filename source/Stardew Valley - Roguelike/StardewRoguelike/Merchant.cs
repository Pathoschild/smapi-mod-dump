/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewRoguelike
{
    public static class Merchant
    {
        public static readonly List<MerchantFloor> MerchantFloors = PopulateMerchantFloors();

        public static ShopMenu CurrentShop = null;

        public static string GetMapPath(MineShaft mine)
        {
            string result;
            int level = Roguelike.GetLevelFromMineshaft(mine);
            if (DebugCommands.ForcedFortuneTeller)
                result = "custom-merchant-curses";
            else if (level == 1 || level == Roguelike.ScalingOrder[0])
                result = "custom-merchant";
            else if (level == Roguelike.ScalingOrder[1])
                result = "custom-merchant-curses";
            else
                result = Roguelike.FloorRng.NextDouble() < 0.5 ? "custom-merchant-curses" : "custom-merchant";

            return result + (Roguelike.HardMode ? "-hard" : "");
        }

        public static bool IsMerchantFloor(MineShaft mine)
        {
            return IsMerchantFloor(Roguelike.GetLevelFromMineshaft(mine));
        }

        public static bool IsMerchantFloor(int level)
        {
            return level != 0 && (level % 6 == 0 || level == 1);
        }

        public static void PlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft mine && IsMerchantFloor(mine))
            {
                int level = Roguelike.GetLevelFromMineshaft(mine);
                Vector2 dialogueLocation;

                if (level == 1)
                {
                    dialogueLocation = new Vector2(14, 9) * 64f;
                    dialogueLocation.X += 32;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, "Welcome to The Abyss", 400);
                }
                else if (level == Roguelike.ScalingOrder[0])
                {
                    dialogueLocation = new Vector2(19, 9) * 64f;
                    dialogueLocation.X -= 16;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, "Looking to level up?", 400);

                }
                else if (level == Roguelike.ScalingOrder[1])
                {
                    dialogueLocation = new Vector2(23, 9) * 64f;
                    dialogueLocation.X -= 16;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, "Care for a little boost?", 400);
                }
                else if (level == Roguelike.ScalingOrder[^1])
                {
                    SpawnQi(mine);
                    PopulateQiDialogue(mine);
                }
            }
        }

        public static List<string> GetMusicTracks()
        {
            return new() { "Saloon1", "MarlonsTheme" };
        }

        public static void PopulateQiDialogue(GameLocation mine)
        {
            NPC qi = mine.getCharacterFromName("Mister Qi");
            if (qi is null)
                return;

            qi.CurrentDialogue.Clear();

            qi.setNewDialogue(
                "Congratulations on beating Nadith! With this accomplishment, I feel that you're finally ready to see the summit.#$b#" +
                "Interested in seeing how far down you can go? Continue on into the unknown, only a ladder away."
            );
        }

        public static void SpawnQi(MineShaft mine)
        {
            NPC qi = mine.getCharacterFromName("Mister Qi");
            if (qi is null)
            {
                qi = Game1.getCharacterFromName("Mister Qi");
                qi = qi.ShallowClone();
                qi.setTileLocation(new(9, 11));
                mine.addCharacter(qi);
            }
        }

        public static void SpawnMarlon(MineShaft mine)
        {
            NPC marlon = Game1.getCharacterFromName("Marlon");
            marlon.setTileLocation(new(14, 11));
            mine.addCharacter(marlon);
        }

        public static void DespawnGil(MineShaft mine)
        {
            mine.removeTileProperty(18, 10, "Buildings", "Action");
            mine.removeTileProperty(19, 10, "Buildings", "Action");
            mine.removeTileProperty(18, 11, "Buildings", "Action");
            mine.removeTileProperty(19, 11, "Buildings", "Action");

            mine.removeTile(18, 10, "Front");
            mine.removeTile(19, 10, "Front");
            mine.removeTile(18, 11, "Buildings");
            mine.removeTile(19, 11, "Buildings");
        }

        public static bool ShouldSpawnGil(int level)
        {
            return level > 1 || DebugCommands.ForcedGil;
        }

        public static MerchantFloor GetNextMerchantFloor(MineShaft mine)
        {
            int level = Roguelike.GetLevelFromMineshaft(mine);
            return GetNextMerchantFloor(level);
        }

        public static MerchantFloor GetNextMerchantFloor(int level)
        {
            int index = (level / 6) + 1;
            if (index > MerchantFloors.Count - 1)
                index = MerchantFloors.Count - 1;

            return MerchantFloors[index];
        }

        public static bool AnswerDialogueAction(MineShaft mine, string questionAndAnswer, string[] questionParams)
        {
            int hpNeeded = 15;
            int goldNeeded = 500;

            bool paid = false;

            if (questionAndAnswer == "cursePurchase_YesHP")
            {
                if (Game1.player.maxHealth > hpNeeded)
                {
                    Roguelike.TrueMaxHP -= hpNeeded;
                    paid = true;
                }
                else
                    Game1.drawObjectDialogue("You do not have enough HP.");
            }
            else if (questionAndAnswer == "cursePurchase_YesGold")
            {
                if (Game1.player.Money >= goldNeeded)
                {
                    Game1.player.Money -= goldNeeded;
                    paid = true;
                }
                else
                    Game1.drawObjectDialogue("You do not have enough money.");
            }

            if (paid)
            {
                Curse.AddRandomCurse();
                Game1.playSound("debuffSpell");
                mine.set_MineShaftUsedFortune(true);
            }

            return false;
        }

        public static bool PerformAction(MineShaft mine, string action, Farmer who, Location tileLocation)
        {
            if (action == "Curses" && !mine.get_MineShaftUsedFortune())
            {
                if (Curse.HasAllCurses())
                {
                    Game1.drawObjectDialogue("You have every otherworldly power known to mankind.");
                    return true;
                }

                int hpNeeded = 15;
                int goldNeeded = 500;

                var responses = new Response[3]
                {
                    new Response("YesGold", $"Yes [{goldNeeded}G]"),
                    new Response("YesHP", $"Yes [{hpNeeded} Max HP]"),
                    new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
                };

                mine.createQuestionDialogue("Would you like me to grant you an otherworldly ability for a low price?", responses, "cursePurchase");
                return true;
            }

            return false;
        }

        public static Dictionary<ISalable, int[]> GetMerchantStock(float priceAdjustment = 1f)
        {
            Dictionary<ISalable, int[]> stock = new();

            if (Perks.HasPerk(Perks.PerkType.Discount))
                priceAdjustment *= 0.9f;

            if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[0])         // Floor 1 Shop
                MerchantFloors[0].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[1])    // Floor 10 Shop
                MerchantFloors[1].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[2])    // Floor 20 Shop
                MerchantFloors[2].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[3])    // Floor 30 Shop
                MerchantFloors[3].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[4])    // Floor 40 Shop
                MerchantFloors[4].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[5])    // Floor 50 Shop
                MerchantFloors[5].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[6])    // Floor 60 Shop
                MerchantFloors[6].AddToStock(stock, priceAdjustment);
            else if (Roguelike.CurrentLevel < Roguelike.ScalingOrder[7])    // Floor 70 Shop
                MerchantFloors[7].AddToStock(stock, priceAdjustment);
            else                                                            // Floor 80+ Shop
                MerchantFloors[8].AddToStock(stock, priceAdjustment);

            int foodQuantity = Curse.HasCurse(CurseType.CheaperMerchant) ? 1 : 3;
            int foodPriceOffset = Curse.HasCurse(CurseType.CheaperMerchant) ? -100 : 0;

            Utility.AddStock(stock, new StardewValley.Object(194, foodQuantity), buyPrice: (int)((200 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Fried Egg
            Utility.AddStock(stock, new StardewValley.Object(196, foodQuantity), buyPrice: (int)((250 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Salad
            Utility.AddStock(stock, new StardewValley.Object(773, foodQuantity), buyPrice: (int)((300 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Life Elixir

            int totalMilksInGame = (Roguelike.MaxHP - Roguelike.StartingHP) / 25;
            int milkQuantity = totalMilksInGame - Roguelike.MilksBought;
            if (milkQuantity > 0)
                Utility.AddStock(stock, new StardewValley.Object(803, milkQuantity), buyPrice: (int)(500 * priceAdjustment), limitedQuantity: milkQuantity);  // Iridium Milk

            return stock;
        }

        // Should only be called once
        public static List<MerchantFloor> PopulateMerchantFloors()
        {
            List<MerchantFloor> result = new();

            int currentIndex = 0;
            while (true)
            {
                MerchantFloor floor;
                if (currentIndex == 0)                          // Floor 1 Shop
                {
                    floor = new(3, 0, 0, 0, 0);

                    floor.AddSword(12, 50, 50, 1);              // Wooden Blade
                    floor.AddSword(11, 50, 50, 1);              // Steel Smallsword
                    floor.AddSword(21, 50, 50, 1);              // Crystal Dagger
                }
                else if (currentIndex == 1)                     // Floor 10 Shop
                {
                    floor = new(3, 2, 0, 1, 0);

                    floor.AddSword(43, 400, 525, 1);            // Pirate's Sword
                    floor.AddSword(1, 400, 525, 1);             // Silver Saber
                    floor.AddSword(15, 650, 800, 1);            // Forest Sword
                    floor.AddSword(6, 700, 750, 1);             // Iron Edge
                    floor.AddSword(19, 325, 450, 1);            // Shadow Dagger
                    floor.AddSword(51, 325, 500, 1);            // Broken Trident

                    floor.AddRing(522, 700, 800, 1);            // Vampire Ring
                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring

                    floor.AddBoots(515, 500, 600, 1);           // Cowboy Boots
                    floor.AddBoots(513, 600, 700, 1);           // Genie Shoes
                    floor.AddBoots(508, 500, 600, 1);           // Combat Boots
                }
                else if (currentIndex == 2)                     // Floor 20 Shop
                {
                    floor = new(3, 2, 1, 2, 0);

                    floor.AddSword(5, 700, 850, 1);             // Bone Sword
                    floor.AddSword(14, 700, 850, 1);            // Neptune's Glaive
                    floor.AddSword(8, 825, 1025, 1);             // Obsidian Edge
                    floor.AddSword(60, 825, 1025, 1);            // Ossified Blade
                    floor.AddSword(45, 600, 775, 1);            // Wicked Kris
                    floor.AddSword(24, 750, 900, 1);            // Wood Club

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(522, 700, 800, 1);     // Vampire Ring
                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring

                    floor.AddBoots(513, 600, 700, 1);           // Genie Shoes
                    floor.AddBoots(511, 600, 700, 1);           // Dark Boots
                    floor.AddBoots(512, 600, 700, 1);           // Fire Walker Boots
                    floor.AddBoots(878, 800, 900, 1);           // Crystal Shoes
                }
                else if (currentIndex == 3)                     // Floor 30 Shop
                {
                    floor = new(3, 2, 2, 2, 0);

                    floor.AddSword(48, 800, 950, 1);            // Yeti Tooth
                    floor.AddSword(50, 825, 975, 1);            // Steel Falchion
                    floor.AddSword(2, 1000, 1225, 1);            // Dark Sword
                    floor.AddSword(9, 950, 1225, 1);            // Lava Katana
                    floor.AddSword(23, 600, 750, 1);            // Galaxy Dagger
                    floor.AddSword(56, 700, 800, 1);            // Dwarf Dagger
                    floor.AddSword(27, 825, 1000, 1);            // Wood Mallet

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(522, 700, 800, 1);     // Vampire Ring
                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(839, 400, 500, 1);     // Thorns Ring
                    floor.AddSpecialRing(521, 600, 700, 1);     // Warrior Ring

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                }
                else if (currentIndex == 4)                     // Floor 40 Shop
                {
                    floor = new(3, 2, 2, 2, 0);

                    floor.AddSword(2, 800, 1025, 1);             // Dark Sword
                    floor.AddSword(9, 750, 1025, 1);             // Lava Katana
                    floor.AddSword(4, 1000, 1200, 1);            // Galaxy Sword
                    floor.AddSword(54, 1000, 1200, 1);           // Dwarf Sword
                    floor.AddSword(59, 850, 1075, 1);            // Dragontooth Shiv
                    floor.AddSword(61, 925, 1075, 1);            // Iridium Needle
                    floor.AddSword(46, 825, 1100, 1);           // Kudgel

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(863, 600, 700, 1);     // Phoenix Ring
                    floor.AddSpecialRing(524, 800, 900, 1);     // Ring of Yoba

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                    floor.AddBoots(854, 800, 1100, 1);          // Mermaid Boots
                    floor.AddBoots(853, 800, 1100, 1);          // Cinderclown Shoes
                }
                else if (currentIndex == 5)                     // Floor 50 Shop
                {
                    floor = new(3, 2, 2, 2, 1);

                    floor.AddSword(2, 800, 1025, 1);             // Dark Sword
                    floor.AddSword(4, 1100, 1225, 1);           // Galaxy Sword
                    floor.AddSword(54, 1150, 1275, 1);          // Dwarf Sword
                    floor.AddSword(59, 1000, 1175, 1);           // Dragontooth Shiv
                    floor.AddSword(28, 1275, 1350, 1);          // Slammer

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(863, 600, 700, 1);     // Phoenix Ring
                    floor.AddSpecialRing(524, 800, 900, 1);     // Ring of Yoba
                    floor.AddSpecialRing(839, 600, 700, 1);     // Thorns Ring
                    floor.AddSpecialRing(521, 600, 700, 1);     // Warrior Ring
                    floor.AddSpecialRing(887, 600, 700, 1);     // Immunity Band

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                    floor.AddBoots(854, 800, 1100, 1);          // Mermaid Boots
                    floor.AddBoots(853, 800, 1100, 1);          // Cinderclown Shoes

                    floor.AddSpecialFood(244, 1850, 2100, 1);   // Root Platter
                    floor.AddSpecialFood(253, 2100, 2350, 1);   // Triple Shot Espresso
                    floor.AddSpecialFood(231, 2100, 2600, 1);   // Eggplant Parmesan
                    floor.AddSpecialFood(204, 1850, 2100, 1);   // Lucky Lunch
                }
                else if (currentIndex == 6)                     // Floor 60 Shop
                {
                    floor = new(3, 2, 2, 2, 1);

                    floor.AddSword(4, 1100, 1225, 1);           // Galaxy Sword
                    floor.AddSword(54, 1150, 1275, 1);          // Dwarf Sword
                    floor.AddSword(57, 1350, 1475, 1);          // Dragontooth Cutlass
                    floor.AddSword(59, 1000, 1175, 1);           // Dragontooth Shiv
                    floor.AddSword(61, 1100, 1225, 1);          // Iridium Needle
                    floor.AddSword(28, 1275, 1350, 1);          // Slammer

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 700, 1);            // Emerald Ring

                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(863, 600, 700, 1);     // Phoenix Ring
                    floor.AddSpecialRing(524, 800, 900, 1);     // Ring of Yoba
                    floor.AddSpecialRing(839, 600, 700, 1);     // Thorns Ring
                    floor.AddSpecialRing(521, 600, 700, 1);     // Warrior Ring
                    floor.AddSpecialRing(887, 600, 700, 1);     // Immunity Band

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                    floor.AddBoots(854, 800, 1100, 1);          // Mermaid Boots
                    floor.AddBoots(853, 800, 1100, 1);          // Cinderclown Shoes

                    floor.AddSpecialFood(244, 1850, 2100, 1);   // Root Platter
                    floor.AddSpecialFood(253, 2100, 2350, 1);   // Triple Shot Espresso
                    floor.AddSpecialFood(231, 2100, 2600, 1);   // Eggplant Parmesan
                    floor.AddSpecialFood(204, 1850, 2100, 1);   // Lucky Lunch
                }
                else if (currentIndex == 7)                     // Floor 70 Shop
                {
                    floor = new(3, 2, 2, 2, 1);

                    floor.AddSword(4, 1100, 1225, 1);           // Galaxy Sword
                    floor.AddSword(54, 1150, 1275, 1);          // Dwarf Sword
                    floor.AddSword(57, 1350, 1475, 1);          // Dragontooth Cutlass
                    floor.AddSword(59, 1000, 1175, 1);           // Dragontooth Shiv
                    floor.AddSword(61, 1100, 1225, 1);          // Iridium Needle
                    floor.AddSword(29, 1975, 2100, 1);          // Galaxy Hammer

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(863, 600, 700, 1);     // Phoenix Ring
                    floor.AddSpecialRing(524, 800, 900, 1);     // Ring of Yoba
                    floor.AddSpecialRing(839, 600, 700, 1);     // Thorns Ring
                    floor.AddSpecialRing(521, 600, 700, 1);     // Warrior Ring
                    floor.AddSpecialRing(887, 600, 700, 1);     // Immunity Band

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                    floor.AddBoots(854, 800, 1100, 1);          // Mermaid Boots
                    floor.AddBoots(853, 800, 1100, 1);          // Cinderclown Shoes

                    floor.AddSpecialFood(244, 1850, 2100, 1);   // Root Platter
                    floor.AddSpecialFood(253, 2100, 2350, 1);   // Triple Shot Espresso
                    floor.AddSpecialFood(231, 2100, 2600, 1);   // Eggplant Parmesan
                    floor.AddSpecialFood(204, 1850, 2100, 1);   // Lucky Lunch
                }
                else if (currentIndex == 8)                     // Floor 80 Shop
                {
                    floor = new(3, 2, 2, 2, 1);

                    floor.AddSword(4, 1100, 1225, 1);           // Galaxy Sword
                    floor.AddSword(23, 1150, 1275, 1);          // Galaxy Dagger
                    floor.AddSword(57, 1350, 1475, 1);          // Dragontooth Cutlass
                    floor.AddSword(59, 1000, 1175, 1);           // Dragontooth Shiv
                    floor.AddSword(61, 1100, 1225, 1);          // Iridium Needle
                    floor.AddSword(29, 1975, 2100, 1);          // Galaxy Hammer
                    floor.AddSword(58, 2050, 2200, 1);          // Dragontooth Club

                    floor.AddRing(532, 500, 600, 1);            // Jade Ring
                    floor.AddRing(531, 500, 600, 1);            // Aquamarine Ring
                    floor.AddRing(534, 700, 800, 1);            // Ruby Ring
                    floor.AddRing(533, 500, 600, 1);            // Emerald Ring

                    floor.AddSpecialRing(523, 600, 700, 1);     // Savage Ring
                    floor.AddSpecialRing(861, 600, 700, 1);     // Protection Ring
                    floor.AddSpecialRing(811, 600, 700, 1);     // Napalm Ring
                    floor.AddSpecialRing(810, 600, 700, 1);     // Crabshell Ring
                    floor.AddSpecialRing(863, 600, 700, 1);     // Phoenix Ring
                    floor.AddSpecialRing(524, 800, 900, 1);     // Ring of Yoba
                    floor.AddSpecialRing(839, 600, 700, 1);     // Thorns Ring
                    floor.AddSpecialRing(521, 600, 700, 1);     // Warrior Ring
                    floor.AddSpecialRing(887, 600, 700, 1);     // Immunity Band

                    floor.AddBoots(514, 600, 700, 1);           // Space Boots
                    floor.AddBoots(855, 600, 700, 1);           // Dragon Scale Boots
                    floor.AddBoots(878, 600, 700, 1);           // Crystal Shoes
                    floor.AddBoots(854, 800, 1100, 1);          // Mermaid Boots
                    floor.AddBoots(853, 800, 1100, 1);          // Cinderclown Shoes

                    floor.AddSpecialFood(244, 1850, 2100, 1);   // Root Platter
                    floor.AddSpecialFood(253, 2100, 2350, 1);   // Triple Shot Espresso
                    floor.AddSpecialFood(231, 2100, 2600, 1);   // Eggplant Parmesan
                    floor.AddSpecialFood(204, 1850, 2100, 1);   // Lucky Lunch
                }
                else
                    break;

                result.Add(floor);
                currentIndex++;
            }

            return result;
        }
    }
}
