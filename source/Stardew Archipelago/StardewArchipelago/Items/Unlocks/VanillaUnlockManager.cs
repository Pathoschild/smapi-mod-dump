/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewValley;

namespace StardewArchipelago.Items.Unlocks
{
    public class VanillaUnlockManager : IUnlockManager
    {
        public const string PROGRESSIVE_TOOL_AP_PREFIX = "Progressive ";
        public const string PROGRESSIVE_MINE_ELEVATOR = "Progressive Mine Elevator";
        public const string PROGRESSIVE_FISHING_ROD = "Progressive Fishing Rod";
        public const string RETURN_SCEPTER_AP_NAME = "Return Scepter";
        public const string GOLDEN_SCYTHE_AP_NAME = "Golden Scythe";
        public const string BEACH_BRIDGE_AP_NAME = "Beach Bridge";
        public const string FRUIT_BATS = "Fruit Bats";
        public const string MUSHROOM_BOXES = "Mushroom Boxes";
        public const string SPECIAL_ORDER_BOARD_AP_NAME = "Special Order Board";
        public const string QI_WALNUT_ROOM = "Qi Walnut Room";
        public const string PIERRE_STOCKLIST = "Pierre's Missing Stocklist";
        public const string ISLAND_FARMHOUSE = "Island Farmhouse";
        public const string ISLAND_MAILBOX = "Island Mailbox";
        public const string TREEHOUSE = "Treehouse";
        public const string PROGRESSIVE_WEAPON = "Progressive Weapon";
        public const string PROGRESSIVE_SWORD = "Progressive Sword";
        public const string PROGRESSIVE_CLUB = "Progressive Club";
        public const string PROGRESSIVE_DAGGER = "Progressive Dagger";
        public const string PROGRESSIVE_BOOTS = "Progressive Footwear";
        public const string PROGRESSIVE_SLINGSHOT = "Progressive Slingshot";

        private Dictionary<string, Func<ReceivedItem, LetterAttachment>> _unlockables;

        public VanillaUnlockManager()
        {
            _unlockables = new Dictionary<string, Func<ReceivedItem, LetterAttachment>>();
            RegisterCommunityCenterRepairs();
            RegisterPlayerSkills();
            RegisterPlayerImprovement();
            RegisterProgressiveTools();
            RegisterMineElevators();
            RegisterUniqueItems();
            RegisterIsolatedEventsItems();
            RegisterGingerIslandRepairs();
            RegisterSpecialItems();
            RegisterEquipment();
        }

        public bool IsUnlock(string unlockName)
        {
            return _unlockables.ContainsKey(unlockName);
        }

        public LetterAttachment PerformUnlockAsLetter(ReceivedItem unlock)
        {
            return _unlockables[unlock.ItemName](unlock);
        }

        private void RegisterCommunityCenterRepairs()
        {
            _unlockables.Add("Bridge Repair", RepairBridge);
            _unlockables.Add("Greenhouse", RepairGreenHouse);
            _unlockables.Add("Glittering Boulder Removed", RemoveGlitteringBoulder);
            _unlockables.Add("Minecarts Repair", RepairMinecarts);
            _unlockables.Add("Bus Repair", RepairBus);
        }

        private void RegisterPlayerImprovement()
        {
            _unlockables.Add("Progressive Backpack", SendProgressiveBackpackLetter);
            _unlockables.Add("Stardrop", SendStardropLetter);
            _unlockables.Add("Dwarvish Translation Guide", SendDwarvishTranslationGuideLetter);
            _unlockables.Add("Skull Key", SendSkullKeyLetter);
            _unlockables.Add("Rusty Key", SendRustyKeyLetter);
            
            _unlockables.Add("Club Card", SendClubCardLetter);
            _unlockables.Add("Magnifying Glass", SendMagnifyingGlassLetter);
            _unlockables.Add("Iridium Snake Milk", SendIridiumSnakeMilkLetter);
            _unlockables.Add("Dark Talisman", SendDarkTalismanLetter);
            _unlockables.Add("Key To The Town", SendKeyToTheTownLetter);
        }

        private void RegisterPlayerSkills()
        {
            _unlockables.Add($"{Skill.Farming} Level", SendProgressiveFarmingLevel);
            _unlockables.Add($"{Skill.Fishing} Level", SendProgressiveFishingLevel);
            _unlockables.Add($"{Skill.Foraging} Level", SendProgressiveForagingLevel);
            _unlockables.Add($"{Skill.Mining} Level", SendProgressiveMiningLevel);
            _unlockables.Add($"{Skill.Combat} Level", SendProgressiveCombatLevel);
        }

        private void RegisterProgressiveTools()
        {
            _unlockables.Add($"{PROGRESSIVE_TOOL_AP_PREFIX}Axe", SendProgressiveAxeLetter);
            _unlockables.Add($"{PROGRESSIVE_TOOL_AP_PREFIX}Pickaxe", SendProgressivePickaxeLetter);
            _unlockables.Add($"{PROGRESSIVE_TOOL_AP_PREFIX}Hoe", SendProgressiveHoeLetter);
            _unlockables.Add($"{PROGRESSIVE_TOOL_AP_PREFIX}Watering Can", SendProgressiveWateringCanLetter);
            _unlockables.Add($"{PROGRESSIVE_TOOL_AP_PREFIX}Trash Can", SendProgressiveTrashCanLetter);
            _unlockables.Add(PROGRESSIVE_FISHING_ROD, SendProgressiveFishingRodLetter);
            _unlockables.Add(RETURN_SCEPTER_AP_NAME, SendReturnScepterLetter);
        }

        private void RegisterUniqueItems()
        {
            _unlockables.Add(GOLDEN_SCYTHE_AP_NAME, SendGoldenScytheLetter);
            _unlockables.Add(PIERRE_STOCKLIST, SendPierreStocklistLetter);
        }

        private void RegisterIsolatedEventsItems()
        {
            _unlockables.Add(BEACH_BRIDGE_AP_NAME, SendBeachBridgeLetter);
            _unlockables.Add(FRUIT_BATS, SendFruitBatsLetter);
            _unlockables.Add(MUSHROOM_BOXES, SendMushroomBoxesLetter);
        }

        private void RegisterGingerIslandRepairs()
        {
            _unlockables.Add("Boat Repair", RepairBoat);
            _unlockables.Add("Island North Turtle", GetLeoTrustAndRemoveNorthernTurtle);
            _unlockables.Add("Island West Turtle", RemoveWesternTurtle);
            _unlockables.Add("Dig Site Bridge", RepairDigSiteBridge);
            _unlockables.Add("Island Trader", RestoreIslandTrader);
            _unlockables.Add("Island Resort", RepairResort);
            _unlockables.Add("Farm Obelisk", CreateFarmObelisk);
            _unlockables.Add(ISLAND_MAILBOX, RepairIslandMailbox);
            _unlockables.Add(ISLAND_FARMHOUSE, RepairIslandFarmhouse);
            _unlockables.Add("Parrot Express", RepairParrotExpress);
            _unlockables.Add("Volcano Bridge", ConstructVolcanoBridge);
            _unlockables.Add("Volcano Exit Shortcut", OpenVolcanoExitShortcut);
            _unlockables.Add("Open Professor Snail Cave", OpenProfessorSnailCave);
            _unlockables.Add(TREEHOUSE, ConstructTreeHouse);
        }

        private void RegisterSpecialItems()
        {
            _unlockables.Add("Ugly Baby", GetNewBabyLetter);
            _unlockables.Add("Cute Baby", GetNewBabyLetter);
        }

        private void RegisterMineElevators()
        {
            _unlockables.Add(PROGRESSIVE_MINE_ELEVATOR, SendProgressiveMineElevatorLetter);
        }

        private void RegisterEquipment()
        {
            _unlockables.Add(PROGRESSIVE_WEAPON, SendProgressiveWeaponLetter);
            _unlockables.Add(PROGRESSIVE_SWORD, SendProgressiveSwordLetter);
            _unlockables.Add(PROGRESSIVE_CLUB, SendProgressiveClubLetter);
            _unlockables.Add(PROGRESSIVE_DAGGER, SendProgressiveDaggerLetter);
            _unlockables.Add(PROGRESSIVE_BOOTS, SendProgressiveBootsLetter);
            _unlockables.Add(PROGRESSIVE_SLINGSHOT, SendProgressiveSlingshotLetter);
        }

        private LetterVanillaAttachment RepairBridge(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ccCraftsRoom", true);
        }

        private LetterVanillaAttachment RepairGreenHouse(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ccPantry", true);
        }

        private LetterVanillaAttachment RemoveGlitteringBoulder(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ccFishTank", true);
        }

        private LetterVanillaAttachment RepairMinecarts(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ccBoilerRoom", true);
        }

        private LetterVanillaAttachment RepairBus(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ccVault", true);
        }

        private LetterVanillaAttachment RepairBoat(ReceivedItem receivedItem)
        {
            var vanillaMails = new[] { "willyBoatFixed", "willyBackRoomInvitation" };
            return new LetterVanillaAttachment(receivedItem, vanillaMails, true);
        }

        private LetterActionAttachment GetLeoTrustAndRemoveNorthernTurtle(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Hut");
        }

        private LetterActionAttachment RemoveWesternTurtle(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Turtle");
        }

        private LetterActionAttachment RepairResort(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Resort");
        }

        private LetterActionAttachment RepairDigSiteBridge(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Bridge");
        }

        private LetterActionAttachment RestoreIslandTrader(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Trader");
        }

        private LetterActionAttachment CreateFarmObelisk(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "Obelisk");
        }

        private LetterActionAttachment RepairIslandMailbox(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "House_Mailbox");
        }

        private LetterActionAttachment RepairIslandFarmhouse(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "House");
        }

        private LetterActionAttachment RepairParrotExpress(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "ParrotPlatforms");
        }

        private LetterActionAttachment ConstructVolcanoBridge(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "VolcanoBridge");
        }

        private LetterActionAttachment OpenVolcanoExitShortcut(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "VolcanoShortcutOut");
        }

        private LetterActionAttachment OpenProfessorSnailCave(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, "ProfessorSnailCave");
        }

        private LetterActionAttachment ConstructTreeHouse(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IslandUnlock, TREEHOUSE);
        }

        private LetterActionAttachment GetNewBabyLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.SpawnBaby);
        }

        private LetterActionAttachment SendProgressiveBackpackLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.Backpack);
        }

        private LetterAttachment SendProgressiveMineElevatorLetter(ReceivedItem receivedItem)
        {
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterItemIdAttachment SendStardropLetter(ReceivedItem receivedItem)
        {
            return new LetterItemIdAttachment(receivedItem, 434);
        }

        private LetterActionAttachment SendDwarvishTranslationGuideLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.DwarvishTranslationGuide);
        }

        private LetterActionAttachment SendSkullKeyLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.SkullKey);
        }

        private LetterActionAttachment SendRustyKeyLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.RustyKey);
        }

        private LetterActionAttachment SendClubCardLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ClubCard);
        }

        private LetterActionAttachment SendMagnifyingGlassLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.MagnifyingGlass);
        }

        private LetterActionAttachment SendIridiumSnakeMilkLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.IridiumSnakeMilk);
        }

        private LetterActionAttachment SendDarkTalismanLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.DarkTalisman);
        }

        private LetterActionAttachment SendKeyToTheTownLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.KeyToTheTown);
        }

        private LetterActionAttachment SendGoldenScytheLetter(ReceivedItem receivedItem)
        {
            Game1.player.mailReceived.Add("gotGoldenScythe");
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GoldenScythe);
        }

        private LetterActionAttachment SendPierreStocklistLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.PierreStocklist);
        }

        private LetterActionAttachment SendBeachBridgeLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.BeachBridge);
        }

        private LetterActionAttachment SendFruitBatsLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.FruitBats);
        }

        private LetterActionAttachment SendMushroomBoxesLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.MushroomBoxes);
        }

        private LetterActionAttachment SendProgressiveAxeLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ProgressiveTool, "Axe");
        }

        private LetterActionAttachment SendProgressivePickaxeLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ProgressiveTool, "Pickaxe");
        }

        private LetterActionAttachment SendProgressiveHoeLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ProgressiveTool, "Hoe");
        }

        private LetterActionAttachment SendProgressiveWateringCanLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ProgressiveTool, "Watering Can");
        }

        private LetterActionAttachment SendProgressiveTrashCanLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ProgressiveTool, "Trash Can");
        }

        private LetterActionAttachment SendProgressiveFishingRodLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.FishingRod);
        }

        private LetterActionAttachment SendReturnScepterLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.ReturnScepter);
        }

        private LetterAttachment SendProgressiveFarmingLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                GiveExperienceToNextLevel(farmer, Skill.Farming);
                farmer.FarmingLevel = farmer.farmingLevel.Value + 1;
                farmer.newLevels.Add(new Point((int)Skill.Farming, farmer.farmingLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveFishingLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                GiveExperienceToNextLevel(farmer, Skill.Fishing);
                farmer.FishingLevel = farmer.fishingLevel.Value + 1;
                farmer.newLevels.Add(new Point((int)Skill.Fishing, farmer.fishingLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveForagingLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                GiveExperienceToNextLevel(farmer, Skill.Foraging);
                farmer.ForagingLevel = farmer.foragingLevel.Value + 1;
                farmer.newLevels.Add(new Point((int)Skill.Foraging, farmer.foragingLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveMiningLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                GiveExperienceToNextLevel(farmer, Skill.Mining);
                farmer.MiningLevel = farmer.miningLevel.Value + 1;
                farmer.newLevels.Add(new Point((int)Skill.Mining, farmer.miningLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveCombatLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                GiveExperienceToNextLevel(farmer, Skill.Combat);
                farmer.CombatLevel = farmer.combatLevel.Value + 1;
                farmer.newLevels.Add(new Point((int)Skill.Combat, farmer.combatLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        public void GiveExperienceToNextLevel(Farmer farmer, Skill skill)
        {
            var experienceForLevelUp = farmer.GetExperienceToNextLevel(skill);
            farmer.AddExperience(skill, experienceForLevelUp);
        }

        private LetterActionAttachment SendProgressiveWeaponLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveWeapon);
        }

        private LetterActionAttachment SendProgressiveSwordLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveSword);
        }

        private LetterActionAttachment SendProgressiveClubLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveClub);
        }

        private LetterActionAttachment SendProgressiveDaggerLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveDagger);
        }

        private LetterActionAttachment SendProgressiveBootsLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveProgressiveBoots);
        }

        private LetterActionAttachment SendProgressiveSlingshotLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveProgressiveSlingshot);
        }
    }
}
