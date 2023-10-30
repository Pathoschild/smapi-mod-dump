/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewRoguelike.Patches;
using StardewRoguelike.UI;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Tiles;

namespace StardewRoguelike
{
    public static class Merchant
    {
        public static readonly List<MerchantFloor> MerchantFloors = PopulateMerchantFloors();

        public static readonly List<Vector2> GardenPotTiles = new()
        {
            new(3, 5),
            new(3, 6),
            new(3, 7),
            new(3, 8),
            new(3, 9)
        };

        public static ShopMenu? CurrentShop { get; set; } = null;

        public static ShopMenu? CurrentSeedShop { get; set; } = null;

        public static HatBoard? CurrentHatBoard { get; set; } = null;

        internal static CurseType? CurseToAdd { get; set; } = null;

        public static string GetMapPath(MineShaft mine)
        {
            string result;
            int level = Roguelike.GetLevelFromMineshaft(mine);
            if (DebugCommands.ForcedFortuneTeller)
                result = "custom-merchant-curses";
            else if (level == 1 || level == Constants.ScalingOrder[0] || level == Constants.ScalingOrder[2])
                result = "custom-merchant";
            else if (level == Constants.ScalingOrder[1])
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

        public static void Initialize(MineShaft mine)
        {
            SpawnMarlon(mine);

            int level = Roguelike.GetLevelFromMineshaft(mine);

            if (level == 1 && !Roguelike.HardMode)
            {
                Vector2 signTile = new(17, 16);
                Sign sign = new(signTile, 38);
                sign.displayItem.Value = new SObject(773, 1);
                sign.displayType.Value = 1;
                mine.Objects.Add(signTile, sign);
            }

            foreach (Vector2 potTile in GardenPotTiles)
            {
                var gardenPot = new IndoorPot(potTile);
                gardenPot.hoeDirt.Value.state.Value = HoeDirt.watered;
                gardenPot.showNextIndex.Value = true;
                mine.Objects.Add(potTile, gardenPot);
                Vector2 sprinklerTile = new(potTile.X - 1, potTile.Y);
                var sprinkler = new SObject(599, 1)
                {
                    TileLocation = sprinklerTile
                };
                mine.Objects.Add(sprinklerTile, sprinkler);
            }

            GrowCrops(mine);
        }

        public static void GrowCrop(Crop crop)
        {
            int plantedAgo = crop.get_CropMerchantsPlantedAgo();

            if (plantedAgo == 0)
                throw new Exception("Crop tried to grow but it was just planted!");

            switch (crop.netSeedIndex.Value)
            {
                case 472:  // Parsnips
                    crop.currentPhase.Value = crop.phaseDays.Count - 1;
                    break;
                case 479:  // Melons
                    if (plantedAgo == 1)
                        crop.currentPhase.Value = 2;
                    else
                        crop.currentPhase.Value = crop.phaseDays.Count - 1;
                    break;
                case 490:  // Pumpkins
                    if (plantedAgo == 1)
                        crop.currentPhase.Value = 1;
                    else if (plantedAgo == 2)
                        crop.currentPhase.Value = 3;
                    else
                        crop.currentPhase.Value = crop.phaseDays.Count - 1;
                    break;
                case 486:  // Starfruit
                    if (plantedAgo == 1)
                        crop.currentPhase.Value = 1;
                    else if (plantedAgo == 2)
                        crop.currentPhase.Value = 3;
                    else if (plantedAgo == 3)
                        crop.currentPhase.Value = 4;
                    else
                        crop.currentPhase.Value = crop.phaseDays.Count - 1;
                    break;
                case 347:  // Sweet Gem Berry
                    if (plantedAgo == 1)
                        crop.currentPhase.Value = 1;
                    else if (plantedAgo == 2)
                        crop.currentPhase.Value = 1;
                    else if (plantedAgo == 3)
                        crop.currentPhase.Value = 3;
                    else if (plantedAgo == 4)
                        crop.currentPhase.Value = 4;
                    else if (plantedAgo == 5)
                        crop.currentPhase.Value = 5;
                    else
                        crop.currentPhase.Value = crop.phaseDays.Count - 1;
                    break;
            }
        }

        public static void GrowCrops(MineShaft mine)
        {
            int level = Roguelike.GetLevelFromMineshaft(mine);

            int previousMerchantLevel = level > 6 ? level - 6 : 1;
            MineShaft? previousMerchant = ChallengeFloor.GetMineFromLevel(previousMerchantLevel);
            if (previousMerchant is null)
                return;

            foreach (Vector2 tile in GardenPotTiles)
            {
                IndoorPot currentPot = (IndoorPot)mine.Objects[tile];
                IndoorPot previousPot = (IndoorPot)previousMerchant.Objects[tile];
                Crop previousCrop = previousPot.hoeDirt.Value.crop;
                if (previousCrop is null)
                    continue;

                Crop currentCrop = new(previousCrop.netSeedIndex.Value, (int)tile.X, (int)tile.Y);
                currentCrop.set_CropMerchantsPlantedAgo(previousCrop.get_CropMerchantsPlantedAgo() + 1);

                GrowCrop(currentCrop);
                currentPot.hoeDirt.Value.crop = currentCrop;
            }
        }

        public static void PlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft mine && IsMerchantFloor(mine))
            {
                Perks.CurrentMenu = new();
                CurseToAdd = Curse.GetRandomUniqueCurse(Roguelike.FloorRng);

                ShopMenu menu;
                if (Perks.HasPerk(Perks.PerkType.Indecisive))
                    menu = new RefreshableShopMenu(GetMerchantStock(random: Roguelike.FloorRng), false, context: "Blacksmith", on_purchase: OpenShopPatch.OnPurchase);
                else
                    menu = new(GetMerchantStock(random: Roguelike.FloorRng), context: "Blacksmith", on_purchase: OpenShopPatch.OnPurchase);
                menu.setUpStoreForContext();
                CurrentShop = menu;
                CurrentSeedShop = new(GetSeedStock(), context: "SeedShop");
                CurrentHatBoard = new();

                foreach (Vector2 potTile in GardenPotTiles)
                {
                    Vector2 sprinklerTile = new(potTile.X - 1, potTile.Y);
                    mine.objects[sprinklerTile].ApplySprinklerAnimation(mine);
                }

                if (Perks.HasPerk(Perks.PerkType.Deconstructor))
                {
                    Vector2 deconstructorTile = new(26, 7);
                    mine.Objects[deconstructorTile] = new SObject(deconstructorTile, 265);
                }

                int level = Roguelike.GetLevelFromMineshaft(mine);
                Vector2 dialogueLocation;

                if (level == 1)
                {
                    // Do Marlon introduction
                    dialogueLocation = new Vector2(23, 3) * 64f;
                    dialogueLocation.X += 32;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, I18n.Merchant_MarlonIntroduction(), 400);
                }
                else if (level == Constants.ScalingOrder[0])
                {
                    // Do Gil introduction
                    dialogueLocation = new Vector2(18, 3) * 64f;
                    dialogueLocation.X -= 16;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, I18n.Merchant_GilIntroduction(), 400);

                }
                else if (level == Constants.ScalingOrder[1])
                {
                    // Do fortune teller introduction
                    dialogueLocation = new Vector2(13, 3) * 64f;
                    dialogueLocation.X -= 16;
                    dialogueLocation.Y -= 16;
                    mine.DrawSpeechBubble(dialogueLocation, I18n.Merchant_FortuneIntroduction(), 400);
                }
                else if (level == Constants.ScalingOrder[2])
                {
                    // Spawn hat board
                    TileSheet town_tilesheet = mine.map.GetTileSheet("z_Town");
                    int tilesheet_index = mine.map.TileSheets.IndexOf(town_tilesheet);

                    mine.setMapTileIndex(11, 5, 2045, "Buildings", tilesheet_index);
                    mine.setMapTileIndex(12, 5, 2046, "Buildings", tilesheet_index);
                    mine.setMapTileIndex(13, 5, 2047, "Buildings", tilesheet_index);
                    mine.setTileProperty(11, 5, "Buildings", "Action", "HatBoard");
                    mine.setTileProperty(12, 5, "Buildings", "Action", "HatBoard");
                    mine.setTileProperty(13, 5, "Buildings", "Action", "HatBoard");
                    mine.setMapTileIndex(11, 4, 2013, "Front", tilesheet_index);
                    mine.setMapTileIndex(12, 4, 2014, "Front", tilesheet_index);
                    mine.setMapTileIndex(13, 4, 2015, "Front", tilesheet_index);
                }

                else if (level == Constants.ScalingOrder[^1])
                {
                    // Spawn Qi when victory
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

            qi.setNewDialogue(I18n.Merchant_QiVictory());
        }

        public static void SpawnQi(MineShaft mine)
        {
            NPC qi = mine.getCharacterFromName("Mister Qi");
            if (qi is null)
            {
                qi = Game1.getCharacterFromName("Mister Qi");
                qi.setTileLocation(new(17, 11));
                qi.faceDirection(3);
                mine.addCharacter(qi);
            }
        }

        public static void SpawnMarlon(MineShaft mine)
        {
            NPC marlon = Game1.getCharacterFromName("Marlon");
            marlon.setTileLocation(new(23, 5));
            mine.addCharacter(marlon);
        }

        public static void DespawnGil(MineShaft mine)
        {
            mine.removeTileProperty(17, 4, "Buildings", "Action");
            mine.removeTileProperty(18, 4, "Buildings", "Action");
            mine.removeTileProperty(17, 5, "Buildings", "Action");
            mine.removeTileProperty(18, 5, "Buildings", "Action");

            mine.removeTile(17, 4, "Front");
            mine.removeTile(18, 4, "Front");
            mine.removeTile(17, 5, "Buildings");
            mine.removeTile(18, 5, "Buildings");
        }

        public static void SpawnBackpack(MineShaft mine)
        {
            mine.setTileProperty(19, 5, "Buildings", "Action", "RoguelikeBackpack");
            mine.setTileProperty(19, 6, "Buildings", "Action", "RoguelikeBackpack");
        }

        public static bool ShouldSpawnBackpack()
        {
            return Game1.player.MaxItems == 12;
        }

        public static bool ShouldSpawnGil(int level)
        {
            int perkCount = Perks.GetActivePerks().Count;
            return (level > 1 && perkCount < Constants.MaximumPerkCount) || DebugCommands.ForcedGil;
        }

        public static void SetupForLocalPlayer(MineShaft mine)
        {
            if (!ShouldSpawnGil(Roguelike.GetLevelFromMineshaft(mine)))
                DespawnGil(mine);

            if (ShouldSpawnBackpack())
                SpawnBackpack(mine);
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
            if (questionAndAnswer.StartsWith("cursePurchase"))
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
                        Game1.drawObjectDialogue(I18n.Merchant_NotEnoughHP());
                }
                else if (questionAndAnswer == "cursePurchase_YesGold")
                {
                    if (Game1.player.Money >= goldNeeded)
                    {
                        Game1.player.Money -= goldNeeded;
                        paid = true;
                    }
                    else
                        Game1.drawObjectDialogue(I18n.Merchant_NotEnoughGold());
                }

                if (paid && CurseToAdd is not null)
                {
                    Curse.AddCurse(CurseToAdd.Value);
                    Game1.playSound("debuffSpell");
                    mine.set_MineShaftUsedFortune(true);
                }
            }
            else if (questionAndAnswer == "roguelikeBackpackPurchase_Yes")
            {
                int goldNeeded = 2000;

                if (Game1.player.Money < goldNeeded)
                    Game1.drawObjectDialogue(I18n.Merchant_NotEnoughGold());
                else if (Game1.player.MaxItems > 12)
                    Game1.drawObjectDialogue(I18n.Merchant_AlreadyHaveBackpack());
                else
                {
                    Game1.player.Money -= goldNeeded;
                    Game1.player.maxItems.Value += 12;
                    for (int i = 0; i < Game1.player.MaxItems; i++)
                    {
                        if (Game1.player.Items.Count <= i)
                            Game1.player.Items.Add(null);
                    }

                    Game1.player.holdUpItemThenMessage(new SpecialItem(99, Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708")));
                    mine.removeTileProperty(19, 5, "Buildings", "Action");
                    mine.removeTileProperty(19, 6, "Buildings", "Action");
                }
            }

            return false;
        }

        public static bool PerformAction(MineShaft mine, string action, Farmer who, Location tileLocation)
        {
            if (action == "Curses" && !mine.get_MineShaftUsedFortune())
            {
                if (Curse.HasAllCurses() || !CurseToAdd.HasValue)
                {
                    Game1.drawObjectDialogue(I18n.Merchant_HasAllCurses());
                    return true;
                }

                int hpNeeded = 15;
                int goldNeeded = 500;

                var responses = new Response[3]
                {
                    new Response("YesGold", I18n.Merchant_YesGold(goldNeeded: goldNeeded)),
                    new Response("YesHP", I18n.Merchant_YesHP(hpNeeded: hpNeeded)),
                    new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
                };

                mine.createQuestionDialogue(I18n.Merchant_DoCurse(), responses, "cursePurchase");
                return true;
            }
            else if (action == "RoguelikeBackpack")
            {
                var responses = mine.createYesNoResponses();
                mine.createQuestionDialogue(I18n.Merchant_DoBackpackUpgrade(), responses, "roguelikeBackpackPurchase");
                return true;
            }
            else if (action == "Arcade_Minecart")
            {
                Game1.currentMinigame = new MineCart(0, 2);
                return true;
            }
            else if (action == "HatBoard")
            {
                Game1.activeClickableMenu = CurrentHatBoard;
                Game1.playSound("bigSelect");

                return true;
            }

            return false;
        }

        public static void Draw(MineShaft mine, SpriteBatch b)
        {
            if (ShouldSpawnBackpack())
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1224, 320)), new Microsoft.Xna.Framework.Rectangle(255, 1436, 12, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01232f);
        }

        public static Dictionary<ISalable, int[]> GetSeedStock()
        {
            Dictionary<ISalable, int[]> stock = new();

            Utility.AddStock(stock, new SObject(472, 5), 50, 5);   // Parsnip Seeds
            Utility.AddStock(stock, new SObject(479, 5), 100, 5);  // Melon Seeds
            Utility.AddStock(stock, new SObject(490, 5), 150, 5);  // Pumpkin Seeds
            Utility.AddStock(stock, new SObject(486, 5), 200, 5);  // Starfruit Seeds
            Utility.AddStock(stock, new SObject(347, 5), 250, 5);  // Rare Seeds

            return stock;
        }

        public static Dictionary<ISalable, int[]> GetMerchantStock(float priceAdjustment = 1f, Random? random = null)
        {
            random ??= Game1.random;

            Dictionary<ISalable, int[]> stock = new();

            if (Perks.HasPerk(Perks.PerkType.Discount))
                priceAdjustment *= 0.9f;

            if (Roguelike.CurrentLevel < Constants.ScalingOrder[0])         // Floor 1 Shop
                MerchantFloors[0].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[1])    // Floor 10 Shop
                MerchantFloors[1].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[2])    // Floor 20 Shop
                MerchantFloors[2].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[3])    // Floor 30 Shop
                MerchantFloors[3].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[4])    // Floor 40 Shop
                MerchantFloors[4].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[5])    // Floor 50 Shop
                MerchantFloors[5].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[6])    // Floor 60 Shop
                MerchantFloors[6].AddToStock(stock, priceAdjustment, random);
            else if (Roguelike.CurrentLevel < Constants.ScalingOrder[7])    // Floor 70 Shop
                MerchantFloors[7].AddToStock(stock, priceAdjustment, random);
            else                                                            // Floor 80+ Shop
                MerchantFloors[8].AddToStock(stock, priceAdjustment, random);

            int foodQuantity = Curse.HasCurse(CurseType.CheaperMerchant) ? 1 : 3;
            int foodPriceOffset = Curse.HasCurse(CurseType.CheaperMerchant) ? -100 : 0;

            Utility.AddStock(stock, new SObject(194, foodQuantity), buyPrice: (int)((200 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Fried Egg
            Utility.AddStock(stock, new SObject(196, foodQuantity), buyPrice: (int)((300 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Salad
            Utility.AddStock(stock, new SObject(773, foodQuantity), buyPrice: (int)((500 + foodPriceOffset) * priceAdjustment), limitedQuantity: foodQuantity);  // Life Elixir

            int totalMilksInGame = (Roguelike.MaxHP - Roguelike.StartingHP) / 25;
            int milkQuantity = totalMilksInGame - Roguelike.MilksBought;
            if (milkQuantity > 0)
                Utility.AddStock(stock, new SObject(803, milkQuantity), buyPrice: (int)(500 * priceAdjustment), limitedQuantity: milkQuantity);  // Iridium Milk

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
