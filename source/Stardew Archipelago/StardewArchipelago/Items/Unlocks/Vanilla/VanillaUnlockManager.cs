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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Locations;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Items.Unlocks.Vanilla
{
    public class VanillaUnlockManager : IUnlockManager
    {
        public const string PROGRESSIVE_MINE_ELEVATOR = "Progressive Mine Elevator";
        public const string BEACH_BRIDGE = "Beach Bridge";
        public const string FRUIT_BATS = "Fruit Bats";
        public const string MUSHROOM_BOXES = "Mushroom Boxes";
        public const string SPECIAL_ORDER_BOARD_AP_NAME = "Special Order Board";
        public const string QI_WALNUT_ROOM = "Qi Walnut Room";
        public const string PIERRE_STOCKLIST = "Pierre's Missing Stocklist";
        public const string FREE_CACTIS = "Free Cactis";
        public const string ISLAND_FARMHOUSE = "Island Farmhouse";
        public const string ISLAND_MAILBOX = "Island Mailbox";
        public const string TREEHOUSE = "Treehouse";

        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;
        private List<IUnlockManager> _childUnlockManagers;

        public VanillaUnlockManager(ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _childUnlockManagers = new List<IUnlockManager>()
            {
                new SkillUnlockManager(archipelago),
                new ToolUnlockManager(),
                new EquipmentUnlockManager(),
                new BookUnlockManager(),
            };
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            RegisterCommunityCenterRepairs(unlocks);
            RegisterRaccoons(unlocks);
            RegisterPlayerImprovement(unlocks);
            RegisterMineElevators(unlocks);
            RegisterUniqueItems(unlocks);
            RegisterIsolatedEventsItems(unlocks);
            RegisterGingerIslandRepairs(unlocks);
            RegisterSpecialItems(unlocks);
            foreach (var childUnlockManager in _childUnlockManagers)
            {
                childUnlockManager.RegisterUnlocks(unlocks);
            }
        }

        private void RegisterCommunityCenterRepairs(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add("Bridge Repair", RepairBridge);
            unlocks.Add("Greenhouse", RepairGreenHouse);
            unlocks.Add("Glittering Boulder Removed", RemoveGlitteringBoulder);
            unlocks.Add("Minecarts Repair", RepairMinecarts);
            unlocks.Add("Bus Repair", RepairBus);
        }

        private void RegisterRaccoons(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(APItem.PROGRESSIVE_RACCOON, SendProgressiveRaccoon);
        }

        private void RegisterPlayerImprovement(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add("Progressive Backpack", SendProgressiveBackpackLetter);
            unlocks.Add("Stardrop", SendStardropLetter);
            unlocks.Add("Dwarvish Translation Guide", SendDwarvishTranslationGuideLetter);
            unlocks.Add("Skull Key", SendSkullKeyLetter);
            unlocks.Add("Rusty Key", SendRustyKeyLetter);

            unlocks.Add("Club Card", SendClubCardLetter);
            unlocks.Add("Magnifying Glass", SendMagnifyingGlassLetter);
            unlocks.Add("Iridium Snake Milk", SendIridiumSnakeMilkLetter);
            unlocks.Add("Dark Talisman", SendDarkTalismanLetter);
            unlocks.Add("Key To The Town", SendKeyToTheTownLetter);

            unlocks.Add(APItem.HEALTH_BONUS, SendHealthBonusLetter);
        }

        private void RegisterUniqueItems(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(PIERRE_STOCKLIST, SendPierreStocklistLetter);
            unlocks.Add(FREE_CACTIS, SendFreeCactisLetter);
        }

        private void RegisterIsolatedEventsItems(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(BEACH_BRIDGE, SendBeachBridgeLetter);
            unlocks.Add(FRUIT_BATS, SendFruitBatsLetter);
            unlocks.Add(MUSHROOM_BOXES, SendMushroomBoxesLetter);
        }

        private void RegisterGingerIslandRepairs(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add("Boat Repair", RepairBoat);
            unlocks.Add("Island North Turtle", GetLeoTrustAndRemoveNorthernTurtle);
            unlocks.Add("Island West Turtle", RemoveWesternTurtle);
            unlocks.Add("Dig Site Bridge", RepairDigSiteBridge);
            unlocks.Add("Island Trader", RestoreIslandTrader);
            unlocks.Add("Island Resort", RepairResort);
            unlocks.Add("Farm Obelisk", CreateFarmObelisk);
            unlocks.Add(ISLAND_MAILBOX, RepairIslandMailbox);
            unlocks.Add(ISLAND_FARMHOUSE, RepairIslandFarmhouse);
            unlocks.Add("Parrot Express", RepairParrotExpress);
            unlocks.Add("Volcano Bridge", ConstructVolcanoBridge);
            unlocks.Add("Volcano Exit Shortcut", OpenVolcanoExitShortcut);
            unlocks.Add("Open Professor Snail Cave", OpenProfessorSnailCave);
            unlocks.Add(TREEHOUSE, ConstructTreeHouse);
        }

        private void RegisterSpecialItems(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add("Ugly Baby", GetNewBabyLetter);
            unlocks.Add("Cute Baby", GetNewBabyLetter);
        }

        private void RegisterMineElevators(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(PROGRESSIVE_MINE_ELEVATOR, SendProgressiveMineElevatorLetter);
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

        private LetterAttachment SendProgressiveRaccoon(ReceivedItem receivedItem)
        {
            if (!Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen"))
            {
                Game1.MasterPlayer.mailReceived.Add("raccoonTreeFallen");
                return new LetterInformationAttachment(receivedItem);
            }

            if (_archipelago.SlotData.QuestLocations.StoryQuestsEnabled && !Game1.MasterPlayer.mailReceived.Contains("raccoonMovedIn"))
            {
                Game1.MasterPlayer.mailReceived.Add("raccoonMovedIn");
                if (_locationChecker.IsLocationMissing("The Giant Stump") && !Game1.player.hasQuest("134"))
                {
                    Game1.player.addQuest("134");
                    Game1.player.mailReceived.Add("checkedRaccoonStump");
                }
                return new LetterInformationAttachment(receivedItem);
            }

            ++Game1.netWorldState.Value.TimesFedRaccoons;
            return new LetterInformationAttachment(receivedItem);
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
            return new LetterItemIdAttachment(receivedItem, ObjectIds.STARDROP);
        }

        private LetterInformationAttachment SendHealthBonusLetter(ReceivedItem receivedItem)
        {
            Game1.player.maxHealth += 10;
            return new LetterInformationAttachment(receivedItem);
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

        private LetterActionAttachment SendPierreStocklistLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.PierreStocklist);
        }

        private LetterActionAttachment SendFreeCactisLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.FreeCactis);
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
    }
}
