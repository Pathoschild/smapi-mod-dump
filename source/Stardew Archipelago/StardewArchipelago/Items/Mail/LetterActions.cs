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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewArchipelago.Items.Traps;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using Object = StardewValley.Object;
using StardewArchipelago.Items.Unlocks;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Items.Mail
{
    public class LetterActions
    {
        private readonly IModHelper _modHelper;
        private readonly Mailman _mail;
        private ArchipelagoClient _archipelago;
        private WeaponsManager _weaponsManager;
        private readonly TrapManager _trapManager;
        private readonly BabyBirther _babyBirther;
        private Dictionary<string, Action<string>> _letterActions;

        public LetterActions(IModHelper modHelper, Mailman mail, ArchipelagoClient archipelago, WeaponsManager weaponsManager, TrapManager trapManager)
        {
            _modHelper = modHelper;
            _mail = mail;
            _archipelago = archipelago;
            _weaponsManager = weaponsManager;
            _trapManager = trapManager;
            _babyBirther = new BabyBirther();
            _letterActions = new Dictionary<string, Action<string>>();
            _letterActions.Add(LetterActionsKeys.Friendship, IncreaseFriendshipWithEveryone);
            _letterActions.Add(LetterActionsKeys.Backpack, (_) => IncreaseBackpackLevel());
            _letterActions.Add(LetterActionsKeys.DwarvishTranslationGuide, (_) => ReceiveDwarvishTranslationGuide());
            _letterActions.Add(LetterActionsKeys.SkullKey, (_) => ReceiveSkullKey());
            _letterActions.Add(LetterActionsKeys.RustyKey, (_) => ReceiveRustyKey());
            _letterActions.Add(LetterActionsKeys.AdventurerGuild, (_) => ReceiveAdventurerGuild());
            _letterActions.Add(LetterActionsKeys.ClubCard, (_) => ReceiveClubCard());
            _letterActions.Add(LetterActionsKeys.MagnifyingGlass, (_) => ReceiveMagnifyingGlass());
            _letterActions.Add(LetterActionsKeys.IridiumSnakeMilk, (_) => ReceiveIridiumSnakeMilk());
            _letterActions.Add(LetterActionsKeys.DarkTalisman, (_) => ReceiveDarkTalisman());
            _letterActions.Add(LetterActionsKeys.KeyToTheTown, (_) => ReceiveKeyToTheTown());
            _letterActions.Add(LetterActionsKeys.GoldenScythe, (_) => ReceiveGoldenScythe());
            _letterActions.Add(LetterActionsKeys.PierreStocklist, (_) => ReceivePierreStocklist());
            _letterActions.Add(LetterActionsKeys.BeachBridge, (_) => RepairBeachBridge());
            _letterActions.Add(LetterActionsKeys.ProgressiveTool, ReceiveProgressiveTool);
            _letterActions.Add(LetterActionsKeys.FishingRod, (_) => GetFishingRodOfNextLevel());
            _letterActions.Add(LetterActionsKeys.ReturnScepter, (_) => GetReturnScepter());
            _letterActions.Add(LetterActionsKeys.GiveBigCraftable, ReceiveBigCraftable);
            _letterActions.Add(LetterActionsKeys.GiveRing, ReceiveRing);
            _letterActions.Add(LetterActionsKeys.GiveBoots, ReceiveBoots);
            _letterActions.Add(LetterActionsKeys.GiveMeleeWeapon, ReceiveMeleeWeapon);
            _letterActions.Add(LetterActionsKeys.GiveWeapon, (_) => GetWeaponOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveSword, (_) => GetSwordOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveClub, (_) => GetClubOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveDagger, (_) => GetDaggerOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveSlingshot, ReceiveSlingshot);
            _letterActions.Add(LetterActionsKeys.GiveBed, ReceiveBed);
            _letterActions.Add(LetterActionsKeys.GiveFishTank, ReceiveFishTank);
            _letterActions.Add(LetterActionsKeys.GiveTV, ReceiveTV);
            _letterActions.Add(LetterActionsKeys.GiveFurniture, ReceiveFurniture);
            _letterActions.Add(LetterActionsKeys.GiveHat, ReceiveHat);
            _letterActions.Add(LetterActionsKeys.IslandUnlock, PerformParrotUpgrade);
            _letterActions.Add(LetterActionsKeys.SpawnBaby, (_) => _babyBirther.SpawnNewBaby());
            _letterActions.Add(LetterActionsKeys.Trap, ExecuteTrap);
            _letterActions.Add(LetterActionsKeys.LearnCookingRecipe, LearnCookingRecipe);
        }

        public void ExecuteLetterAction(string key, string parameter)
        {
            _letterActions[key](parameter);
        }

        private void IncreaseFriendshipWithEveryone(string friendshipPoints)
        {
            var farmer = Game1.player;
            var numberOfPoints = int.Parse(friendshipPoints);
            foreach (var npc in farmer.friendshipData.Keys)
            {
                farmer.changeFriendship((int)(numberOfPoints / _archipelago.SlotData.FriendshipMultiplier), Game1.getCharacterFromName(npc));
            }
        }

        private void IncreaseBackpackLevel()
        {
            var previousMaxItems = Game1.player.MaxItems;
            var backpackName = "";
            switch (Game1.player.MaxItems)
            {
                case < 12:
                    Game1.player.MaxItems = 12;
                    break;
                case < 24:
                    Game1.player.MaxItems = 24;
                    backpackName = Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708");
                    break;
                case < 36:
                    Game1.player.MaxItems = 36;
                    backpackName = Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709");
                    break;
                case >= 36:
                    if (_archipelago.SlotData.Mods.HasMod(ModNames.BIGGER_BACKPACK) & (Game1.player.MaxItems >= 36))
                    {
                        Game1.player.MaxItems = 48;
                        backpackName = "Premium Pack";
                    }
                    break;
            }
            
            if (previousMaxItems >= Game1.player.MaxItems)
            {
                return;
            }

            while (Game1.player.Items.Count < Game1.player.MaxItems)
            {
                Game1.player.Items.Add(null);
            }
            Game1.player.holdUpItemThenMessage(new SpecialItem(99, backpackName));
        }

        private void ReceiveDwarvishTranslationGuide()
        {
            Game1.player.canUnderstandDwarves = true;
            Game1.playSound("fireball");
            Game1.player.holdUpItemThenMessage(new StardewValley.Object(326, 1));
        }

        private void ReceiveSkullKey()
        {
            Game1.player.hasSkullKey = true;
            Game1.player.addQuest(19);
            Game1.player.holdUpItemThenMessage(new SpecialItem(4));
        }

        private void ReceiveRustyKey()
        {
            Game1.player.hasRustyKey = true;
            // We could play the event or something here
        }

        private void ReceiveClubCard()
        {
            Game1.player.hasClubCard = true;
            Game1.player.holdUpItemThenMessage(new SpecialItem(2));
        }

        private void ReceiveMagnifyingGlass()
        {
            Game1.player.hasMagnifyingGlass = true;
            Game1.player.holdUpItemThenMessage(new SpecialItem(5));
        }

        private void ReceiveIridiumSnakeMilk()
        {
            Game1.player.maxHealth += 25;
        }

        public void ReceiveDarkTalisman()
        {
            Game1.player.hasDarkTalisman = true;
        }

        public void ReceiveKeyToTheTown()
        {
            Game1.player.HasTownKey = true;
        }

        private void ReceiveAdventurerGuild()
        {
            Game1.player.mailReceived.Add("guildMember");
        }

        private void ReceiveGoldenScythe()
        {
            Game1.playSound("parry");
            var goldenScythe = new MeleeWeapon(53);
            Game1.player.holdUpItemThenMessage(goldenScythe);
            Game1.player.addItemByMenuIfNecessary(goldenScythe);
        }

        private void ReceivePierreStocklist()
        {
            Game1.addMailForTomorrow("gotMissingStocklist", true, true);
            var stocklist = new Object(897, 1);
            stocklist.questItem.Value = true;
            Game1.player.holdUpItemThenMessage(stocklist);
            Game1.player.addItemByMenuIfNecessary(stocklist);
        }

        private void RepairBeachBridge()
        {
            var beach = Game1.getLocationFromName("Beach") as Beach;
            beach.bridgeFixed.Value = true;
            Beach.fixBridge(beach);
        }

        private void ReceiveProgressiveTool(string toolGenericName)
        {
            if (toolGenericName.Contains("Trash_Can"))
            {
                ReceiveTrashCanUpgrade();
                return;
            }

            var upgradedTool = UpgradeToolInEntireWorld(toolGenericName);

            if (upgradedTool == null)
            {
                throw new Exception($"Could not find a upgradedTool of type {toolGenericName} in this entire world");
            }

            Game1.player.holdUpItemThenMessage(upgradedTool);
        }

        private static Tool UpgradeToolInEntireWorld(string toolGenericName)
        {
            var player = Game1.player;
            var toolName = toolGenericName.Replace(" ", "_");
            if (TryUpgradeToolInInventory(player, toolName, out var upgradedTool))
            {
                return upgradedTool;
            }

            if (TryUpgradeToolInChests(toolName, out upgradedTool))
            {
                return upgradedTool;
            }

            if (TryUpgradeToolInLostAndFoundBox(player, toolName, out upgradedTool))
            {
                return upgradedTool;
            }

            return null;
        }

        private static bool TryUpgradeToolInInventory(Farmer player, string toolName, out Tool upgradedTool)
        {
            foreach (var playerItem in player.Items)
            {
                if (TryUpgradeCorrectTool(toolName, playerItem, out upgradedTool))
                {
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        private static bool TryUpgradeToolInChests(string toolName, out Tool upgradedTool)
        {
            var locations = Game1.locations.ToList();

            foreach (var building in Game1.getFarm().buildings)
            {
                if (building?.indoors.Value == null)
                {
                    continue;
                }
                locations.Add(building.indoors.Value);
            }

            foreach (var gameLocation in locations)
            {
                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not Chest chest)
                    {
                        continue;
                    }

                    foreach (var chestItem in chest.items)
                    {
                        if (TryUpgradeCorrectTool(toolName, chestItem, out upgradedTool))
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var junimoChestItem in Game1.player.team.junimoChest)
            {
                if (TryUpgradeCorrectTool(toolName, junimoChestItem, out upgradedTool))
                {
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        private static bool TryUpgradeToolInLostAndFoundBox(Farmer player, string toolName, out Tool upgradedTool)
        {
            foreach (var lostAndFoundItem in player.team.returnedDonations)
            {
                if (TryUpgradeCorrectTool(toolName, lostAndFoundItem, out upgradedTool))
                {
                    return true;
                }
            }

            upgradedTool = null;
            return false;
        }

        private static bool TryUpgradeCorrectTool(string toolName, Item item, out Tool upgradedTool)
        {
            if (item is not Tool toolToUpgrade || !toolToUpgrade.Name.Replace(" ", "_").Contains(toolName))
            {
                upgradedTool = null;
                return false;
            }

            if (toolToUpgrade.UpgradeLevel < 4)
            {
                toolToUpgrade.UpgradeLevel++;
            }

            {
                upgradedTool = toolToUpgrade;
                return true;
            }
        }

        private static void ReceiveTrashCanUpgrade()
        {
            Game1.player.trashCanLevel++;
            Game1.player.trashCanLevel = Math.Max(1, Math.Min(4, Game1.player.trashCanLevel));
            var trashCanToHoldUp = new GenericTool("Trash Can",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:TrashCan_Description",
                    ((Game1.player.trashCanLevel * 15).ToString() ?? "")), Game1.player.trashCanLevel,
                12 + Game1.player.trashCanLevel, 12 + Game1.player.trashCanLevel);
            trashCanToHoldUp.upgradeLevel.Value = Game1.player.trashCanLevel;
            Game1.player.holdUpItemThenMessage(trashCanToHoldUp);
        }

        private void GetFishingRodOfNextLevel()
        {
            // This includes the current letter due to the timing of this patch
            var numberOfPreviousFishingRodLetters = _mail.OpenedMailsContainingKey(VanillaUnlockManager.PROGRESSIVE_FISHING_ROD_AP_NAME);

            // received 1 -> training rod [1]
            // received 2 -> bamboo [0]
            // received 3 -> fiberglass [2]
            // received 4 -> iridium [3]

            numberOfPreviousFishingRodLetters = Math.Max(1, Math.Min(4, numberOfPreviousFishingRodLetters));
            var upgradeLevel = numberOfPreviousFishingRodLetters - 1;
            if (upgradeLevel < 2)
            {
                upgradeLevel = 1 - upgradeLevel;
            }

            var itemToAdd = new FishingRod(upgradeLevel);

            Game1.player.holdUpItemThenMessage(itemToAdd);
            Game1.player.addItemByMenuIfNecessary(itemToAdd);
        }

        private void GetReturnScepter()
        {
            Game1.player.mailReceived.Add("ReturnScepter");
            var itemToAdd = new Wand();
            Game1.player.holdUpItemThenMessage(itemToAdd);
            Game1.player.addItemByMenuIfNecessary(itemToAdd);
        }

        private void ReceiveBigCraftable(string bigCraftableId)
        {
            var id = int.Parse(bigCraftableId);
            var bigCraftable = new Object(Vector2.Zero, id);
            bigCraftable.Stack = 1;
            ReceiveItem(bigCraftable);
        }

        private void ReceiveRing(string ringId)
        {
            var id = int.Parse(ringId);
            var ring = new Ring(id);
            ReceiveItem(ring);
        }

        private void ReceiveBoots(string bootsId)
        {
            var id = int.Parse(bootsId);
            var boots = new Boots(id);
            ReceiveItem(boots);
        }

        private void ReceiveMeleeWeapon(string weaponId)
        {
            var id = int.Parse(weaponId);
            var weapon = new MeleeWeapon(id);
            ReceiveItem(weapon);
        }

        private void ReceiveSlingshot(string slingshotId)
        {
            var id = int.Parse(slingshotId);
            var slingshot = new Slingshot(id);
            ReceiveItem(slingshot);
        }

        private void ReceiveBed(string furnitureId)
        {
            var id = int.Parse(furnitureId);
            var furniture = new BedFurniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveFishTank(string furnitureId)
        {
            var id = int.Parse(furnitureId);
            var furniture = new FishTankFurniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveTV(string furnitureId)
        {
            var id = int.Parse(furnitureId);
            var furniture = new TV(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveFurniture(string furnitureId)
        {
            var id = int.Parse(furnitureId);
            var furniture = new Furniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveItem(Item item)
        {
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(item);
        }

        private void ReceiveHat(string hatId)
        {
            var id = int.Parse(hatId);
            var hat = new Hat(id);
            ReceiveItem(hat);
        }

        private void PerformParrotUpgrade(string whichUpgrade)
        {
            switch (whichUpgrade)
            {
                case "Turtle":
                    MoveTurtleToIslandWest();
                    return;
                case "Resort":
                    RestoreIslandResort();
                    return;
                case "Hut":
                    GainLeoTrustAndRemoveNorthernTurtle();
                    return;
                case "Bridge":
                    RepairDigSiteBridge();
                    return;
                case "Trader":
                    RestoreIslandTrader();
                    return;
                case "Obelisk":
                    CreateFarmObelisk();
                    return;
                case "House_Mailbox":
                    RepairIslandMailbox();
                    return;
                case "House":
                    RepairIslandFarmhouse();
                    return;
                case "ParrotPlatforms":
                    RepairParrotExpress();
                    return;
                case "VolcanoBridge":
                    ConstructVolcanoBridge();
                    return;
                case "VolcanoShortcutOut":
                    OpenVolcanoExitShortcut();
                    return;
                case "ProfessorSnailCave":
                    OpenProfessorSnailCave();
                    return;
                case VanillaUnlockManager.TREEHOUSE:
                    ConstructTreeHouse();
                    return;
            }
        }

        private const string _islandHut = "IslandHut";
        private const string _islandSouth = "IslandSouth";
        private const string _islandNorth = "IslandNorth";
        private const string _islandWest = "IslandWest";
        private const string _volcanoDungeon = "VolcanoDungeon0";
        private const string _mountain = "Mountain";

        private static T FindLocation<T>(string locationName)
        {
            var location = Game1.getLocationFromName(locationName);
            if (!(location is T locationOfDesiredType))
            {
                throw new Exception($"Could not find location: {locationName}");
            }

            return locationOfDesiredType;
        }

        private static void GainLeoTrustAndRemoveNorthernTurtle()
        {
            var islandHut = FindLocation<IslandHut>(_islandHut);
            const string vanillaLetter = "Island_FirstParrot";
            if (!Game1.player.mailReceived.Contains(vanillaLetter))
            {
                Game1.player.mailReceived.Add(vanillaLetter);
            }

            islandHut.firstParrotDone.Value = true;
            islandHut.parrotBoyEvent.Fire();
        }

        private static void MoveTurtleToIslandWest()
        {
            var islandSouth = FindLocation<IslandSouth>(_islandSouth);

            Game1.addMailForTomorrow("Island_Turtle", true, true);
            islandSouth.westernTurtleMoved.Value = true;
            islandSouth.moveTurtleEvent.Fire();
        }

        private static void RestoreIslandResort()
        {
            var islandSouth = FindLocation<IslandSouth>(_islandSouth);

            Game1.addMailForTomorrow("Island_Resort", true, true);
            islandSouth.resortRestored.Value = true;
        }

        private static void RepairDigSiteBridge()
        {
            var islandNorth = FindLocation<IslandNorth>(_islandNorth);

            Game1.addMailForTomorrow("Island_UpgradeBridge", true, true);
            islandNorth.bridgeFixed.Value = true;
        }

        private static void RestoreIslandTrader()
        {
            var islandNorth = FindLocation<IslandNorth>(_islandNorth);

            Game1.addMailForTomorrow("Island_UpgradeTrader", true, true);
            islandNorth.traderActivated.Value = true;
        }

        private static void CreateFarmObelisk()
        {
            var islandWest = FindLocation<IslandWest>(_islandWest);

            Game1.addMailForTomorrow("Island_W_Obelisk", true, true);
            islandWest.farmObelisk.Value = true;
        }

        private static void RepairIslandMailbox()
        {
            var islandWest = FindLocation<IslandWest>(_islandWest);

            Game1.addMailForTomorrow("Island_UpgradeHouse_Mailbox", true, true);
            islandWest.farmhouseMailbox.Value = true;
        }

        private static void RepairIslandFarmhouse()
        {
            var islandWest = FindLocation<IslandWest>(_islandWest);

            Game1.addMailForTomorrow("Island_UpgradeHouse", true, true);
            islandWest.farmhouseRestored.Value = true;
        }

        private static void RepairParrotExpress()
        {
            var islandWest = FindLocation<IslandWest>(_islandWest);

            Game1.addMailForTomorrow("Island_UpgradeParrotPlatform", true, true);
            Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value = true;
        }

        private void ConstructVolcanoBridge()
        {
            var volcanoDungeon = FindLocation<VolcanoDungeon>(_volcanoDungeon);

            Game1.addMailForTomorrow("Island_VolcanoBridge", true, true);
            var bridgeUnlockedField = _modHelper.Reflection.GetField<NetBool>(volcanoDungeon, "bridgeUnlocked");
            bridgeUnlockedField.GetValue().Value = true;
        }

        private void OpenVolcanoExitShortcut()
        {
            var volcanoDungeon = FindLocation<VolcanoDungeon>(_volcanoDungeon);

            Game1.addMailForTomorrow("Island_VolcanoShortcutOut", true, true);
            var shortcutOutUnlockedField = _modHelper.Reflection.GetField<NetBool>(volcanoDungeon, "shortcutOutUnlocked");
            shortcutOutUnlockedField.GetValue().Value = true;
        }

        private void OpenProfessorSnailCave()
        {
            var islandNorth = FindLocation<IslandNorth>(_islandNorth);

            islandNorth.caveOpened.Value = true;
            Game1.addMailForTomorrow("islandNorthCaveOpened", true, true);
        }

        private void ConstructTreeHouse()
        {
            var mountain = FindLocation<Mountain>(_mountain);
            mountain.ApplyTreehouseIfNecessary();
        }

        private void ExecuteTrap(string trapName)
        {
            if (!_trapManager.IsTrap(trapName))
            {
                throw new ArgumentException(trapName);
            }

            _trapManager.TryExecuteTrapImmediately(trapName);
        }

        private void GetWeaponOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(VanillaUnlockManager.PROGRESSIVE_WEAPON, _weaponsManager.WeaponsByTier);
        }

        private void GetSwordOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(VanillaUnlockManager.PROGRESSIVE_SWORD, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_SWORD]);
        }

        private void GetClubOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(VanillaUnlockManager.PROGRESSIVE_CLUB, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_CLUB]);
        }

        private void GetDaggerOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(VanillaUnlockManager.PROGRESSIVE_DAGGER, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_DAGGER]);
        }

        private void GetBootsOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(VanillaUnlockManager.PROGRESSIVE_BOOTS, _weaponsManager.BootsByTier);
        }

        private void GetProgressiveEquipmentOfNextTier(string apUnlock, Dictionary<int, List<StardewItem>> equipmentsByTier)
        {
            // This includes the current letter due to the timing of this patch
            var tier = _mail.OpenedMailsContainingKey(apUnlock);
            tier = Math.Max(1, Math.Min(5, tier));

            var equipmentsOfTier = equipmentsByTier[tier];
            if (!equipmentsOfTier.Any())
            {
                while (tier > 1 && !equipmentsOfTier.Any())
                {
                    tier--;
                    equipmentsOfTier = equipmentsByTier[tier];
                }

                if (!equipmentsOfTier.Any())
                {
                    return;
                }
            }

            var chosenEquipmentIndex = Game1.random.Next(0, equipmentsOfTier.Count);
            var chosenEquipment = equipmentsOfTier[chosenEquipmentIndex];

            var equipmentToGive = chosenEquipment.PrepareForGivingToFarmer();

            Game1.player.holdUpItemThenMessage(equipmentToGive);
            Game1.player.addItemByMenuIfNecessary(equipmentToGive);
        }

        private void LearnCookingRecipe(string recipeItemName)
        {
            Game1.player.cookingRecipes.Add(recipeItemName, 0);
        }
    }
}
