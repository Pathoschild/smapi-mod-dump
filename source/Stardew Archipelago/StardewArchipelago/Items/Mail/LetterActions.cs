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
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items.Traps;
using StardewArchipelago.Items.Unlocks.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.NameMapping;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

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
        private readonly ToolUpgrader _toolUpgrader;
        private Dictionary<string, Action<string>> _letterActions;

        public LetterActions(IModHelper modHelper, Mailman mail, ArchipelagoClient archipelago, WeaponsManager weaponsManager, TrapManager trapManager, BabyBirther babyBirther, StardewItemManager _stardewItemManager)
        {
            _modHelper = modHelper;
            _mail = mail;
            _archipelago = archipelago;
            _weaponsManager = weaponsManager;
            _trapManager = trapManager;
            _babyBirther = babyBirther;
            _toolUpgrader = new ToolUpgrader();
            var modLetterActions = new ModLetterActions(_stardewItemManager);
            _letterActions = new Dictionary<string, Action<string>>();
            _letterActions.Add(LetterActionsKeys.Friendship, IncreaseFriendshipWithEveryone);
            _letterActions.Add(LetterActionsKeys.Backpack, (_) => IncreaseBackpackLevel());
            _letterActions.Add(LetterActionsKeys.DwarvishTranslationGuide, (_) => ReceiveDwarvishTranslationGuide());
            _letterActions.Add(LetterActionsKeys.SkullKey, (_) => ReceiveSkullKey());
            _letterActions.Add(LetterActionsKeys.RustyKey, (_) => ReceiveRustyKey());
            _letterActions.Add(LetterActionsKeys.ClubCard, (_) => ReceiveClubCard());
            _letterActions.Add(LetterActionsKeys.MagnifyingGlass, (_) => ReceiveMagnifyingGlass());
            _letterActions.Add(LetterActionsKeys.IridiumSnakeMilk, (_) => ReceiveIridiumSnakeMilk());
            _letterActions.Add(LetterActionsKeys.DarkTalisman, (_) => ReceiveDarkTalisman());
            _letterActions.Add(LetterActionsKeys.KeyToTheTown, (_) => ReceiveKeyToTheTown());
            _letterActions.Add(LetterActionsKeys.GoldenScythe, (_) => ReceiveGoldenScythe());
            _letterActions.Add(LetterActionsKeys.ProgressiveScythe, (_) => ReceiveProgressiveScythe());
            _letterActions.Add(LetterActionsKeys.PierreStocklist, (_) => ReceivePierreStocklist());
            _letterActions.Add(LetterActionsKeys.FreeCactis, (_) => ReceiveFreeCactis());
            _letterActions.Add(LetterActionsKeys.BeachBridge, (_) => RepairBeachBridge());
            _letterActions.Add(LetterActionsKeys.FruitBats, (_) => SetupFruitBats());
            _letterActions.Add(LetterActionsKeys.MushroomBoxes, (_) => SetupMushroomBoxes());
            _letterActions.Add(LetterActionsKeys.ProgressiveTool, ReceiveProgressiveTool);
            _letterActions.Add(LetterActionsKeys.FishingRod, (_) => GetFishingRodOfNextLevel());
            _letterActions.Add(LetterActionsKeys.ReturnScepter, (_) => GetReturnScepter());
            _letterActions.Add(LetterActionsKeys.GiveBigCraftable, ReceiveBigCraftable);
            _letterActions.Add(LetterActionsKeys.GiveRing, ReceiveRing);
            _letterActions.Add(LetterActionsKeys.GiveSpecificBoots, ReceiveBoots);
            _letterActions.Add(LetterActionsKeys.GiveMeleeWeapon, ReceiveMeleeWeapon);
            _letterActions.Add(LetterActionsKeys.GiveWeapon, (_) => GetWeaponOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveSword, (_) => GetSwordOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveClub, (_) => GetClubOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveDagger, (_) => GetDaggerOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveProgressiveBoots, (_) => GetBootsOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveSlingshot, ReceiveSlingshot);
            _letterActions.Add(LetterActionsKeys.GiveProgressiveSlingshot, (_) => GetSlingshotOfNextTier());
            _letterActions.Add(LetterActionsKeys.GiveBed, ReceiveBed);
            _letterActions.Add(LetterActionsKeys.GiveFishTank, ReceiveFishTank);
            _letterActions.Add(LetterActionsKeys.GiveTV, ReceiveTV);
            _letterActions.Add(LetterActionsKeys.GiveFurniture, ReceiveFurniture);
            _letterActions.Add(LetterActionsKeys.GiveHat, ReceiveHat);
            _letterActions.Add(LetterActionsKeys.IslandUnlock, PerformParrotUpgrade);
            _letterActions.Add(LetterActionsKeys.SpawnBaby, (_) => _babyBirther.SpawnNewBaby());
            _letterActions.Add(LetterActionsKeys.Trap, ExecuteTrap);
            _letterActions.Add(LetterActionsKeys.LearnCookingRecipe, LearnCookingRecipe);
            _letterActions.Add(LetterActionsKeys.LearnSpecialCraftingRecipe, LearnSpecialCraftingRecipe);
            modLetterActions.AddModLetterActions(_letterActions);
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
            Game1.player.holdUpItemThenMessage(new Object("326", 1));
        }

        private void ReceiveSkullKey()
        {
            Game1.player.hasSkullKey = true;
            Game1.player.addQuest("19");
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

        private void ReceiveGoldenScythe()
        {
            Game1.playSound("parry");
            var goldenScythe = ItemRegistry.Create("(W)53");
            Game1.player.holdUpItemThenMessage(goldenScythe);
            Game1.player.addItemByMenuIfNecessary(goldenScythe);
        }

        private void ReceiveProgressiveScythe()
        {
            Game1.playSound("parry");

            // This includes the current letter due to the timing of this patch
            var scytheNumber = _mail.OpenedMailsContainingKey(ToolUnlockManager.PROGRESSIVE_SCYTHE);
            scytheNumber = Math.Max(1, Math.Min(2, scytheNumber));
            var scytheId = "(W)53"; // Golden Scythe
            if (scytheNumber > 1)
            {
                scytheId = "(W)66"; // Iridium Scythe
            }

            var itemToAdd = ItemRegistry.Create(scytheId);
            Game1.player.holdUpItemThenMessage(itemToAdd);
            Game1.player.addItemByMenuIfNecessary(itemToAdd);
        }

        private void ReceivePierreStocklist()
        {
            Game1.addMailForTomorrow("gotMissingStocklist", true, true);
            var stocklist = new Object("897", 1);
            stocklist.questItem.Value = true;
            Game1.player.holdUpItemThenMessage(stocklist);
            Game1.player.addItemByMenuIfNecessary(stocklist);
        }

        private void ReceiveFreeCactis()
        {
            var seed = (int)(Game1.player.UniqueMultiplayerID + Game1.stats.DaysPlayed);
            Game1.player.addItemToInventoryBool(new RandomizedPlantFurniture("FreeCactus", Vector2.Zero, seed));
        }

        private void RepairBeachBridge()
        {
            var beach = Game1.getLocationFromName("Beach") as Beach;
            beach.bridgeFixed.Value = true;
            Beach.fixBridge(beach);
        }

        private void SetupFruitBats()
        {
            Game1.MasterPlayer.caveChoice.Value = 1;
        }

        private void SetupMushroomBoxes()
        {
            var farmCave = Game1.getLocationFromName("FarmCave") as FarmCave;
            farmCave.setUpMushroomHouse();
        }

        private void ReceiveProgressiveTool(string toolGenericName)
        {
            if (toolGenericName.Contains("Trash_Can"))
            {
                ReceiveTrashCanUpgrade();
                return;
            }

            // This includes the current letter due to the timing of this patch
            var numberOfPreviousToolLetters = _mail.OpenedMailsContainingKey($"{ToolUnlockManager.PROGRESSIVE_TOOL_AP_PREFIX}{toolGenericName}");
            if (numberOfPreviousToolLetters <= 1 && toolGenericName == "Pan")
            {
                var newTool = _toolUpgrader.CreateTool(toolGenericName);
                Game1.player.holdUpItemThenMessage(newTool);
                Game1.player.addItemByMenuIfNecessary(newTool);
                return;
            }

            var upgradedTool = _toolUpgrader.UpgradeToolInEntireWorld(toolGenericName);

            if (upgradedTool == null)
            {
                throw new Exception($"Could not find a upgradedTool of type {toolGenericName} in this entire world");
            }

            Game1.player.holdUpItemThenMessage(upgradedTool);
        }

        private static void ReceiveTrashCanUpgrade()
        {
            var currentTrashCanLevel = Game1.player.trashCanLevel;
            foreach (var (toolKey, toolData) in Game1.toolData)
            {
                if (!toolKey.Contains(Tools.TRASH_CAN.Replace(" ", "")) || !toolData.UpgradeFrom.Any())
                {
                    continue;
                }

                var upgradeFrom = toolData.UpgradeFrom.First();
                var condition = upgradeFrom.Condition;
                var fields = condition.Split(" ");
                var level = int.Parse(fields.Last());
                if (level != currentTrashCanLevel)
                {
                    continue;
                }

                Game1.player.trashCanLevel = Math.Max(1, Math.Min(4, currentTrashCanLevel + 1));
                var trashCanToHoldUp = ItemRegistry.Create("(T)" + toolKey);
                Game1.player.holdUpItemThenMessage(trashCanToHoldUp);
                return;
            }
        }

        private void GetFishingRodOfNextLevel()
        {
            // This includes the current letter due to the timing of this patch
            var numberOfPreviousFishingRodLetters = _mail.OpenedMailsContainingKey(ToolUnlockManager.PROGRESSIVE_FISHING_ROD);

            // received 1 -> training rod [1]
            // received 2 -> bamboo [0]
            // received 3 -> fiberglass [2]
            // received 4 -> iridium [3]
            // received 5 -> advanced iridium [4]

            numberOfPreviousFishingRodLetters = Math.Max(1, Math.Min(5, numberOfPreviousFishingRodLetters));
            var upgradeLevel = numberOfPreviousFishingRodLetters - 1;
            if (upgradeLevel < 2)
            {
                upgradeLevel = 1 - upgradeLevel;
            }

            foreach (var (toolKey, toolData) in Game1.toolData)
            {
                if (!toolData.ClassName.Equals(Tools.FISHING_ROD))
                {
                    continue;
                }

                if (toolData.UpgradeLevel != upgradeLevel)
                {
                    continue;
                }

                var itemToAdd = ItemRegistry.Create("(T)" + toolKey);
                Game1.player.holdUpItemThenMessage(itemToAdd);
                Game1.player.addItemByMenuIfNecessary(itemToAdd);
                return;
            }
        }

        private void GetReturnScepter()
        {
            Game1.player.mailReceived.Add("ReturnScepter");
            var itemToAdd = new Wand();
            Game1.player.holdUpItemThenMessage(itemToAdd);
            Game1.player.addItemByMenuIfNecessary(itemToAdd);
        }

        private void ReceiveBigCraftable(string bigCraftableIdAndAmount)
        {
            var parts = bigCraftableIdAndAmount.Split(BigCraftable.BIG_CRAFTABLE_SEPARATOR);
            var id = parts[0];
            var amount = parts.Length > 1 ? int.Parse(parts[1]) : 1;
            var bigCraftable = new Object(Vector2.Zero, id);
            bigCraftable.Stack = amount;
            ReceiveItem(bigCraftable);
        }

        private void ReceiveRing(string ringId)
        {
            var id = ringId;
            var ring = new Ring(id);
            ReceiveItem(ring);
        }

        private void ReceiveBoots(string bootsId)
        {
            var id = bootsId;
            var boots = new Boots(id);
            ReceiveItem(boots);
        }

        private void ReceiveMeleeWeapon(string weaponId)
        {
            var id = weaponId;
            var weapon = new MeleeWeapon(id);
            ReceiveItem(weapon);
        }

        private void ReceiveSlingshot(string slingshotId)
        {
            var id = slingshotId;
            var slingshot = new Slingshot(id);
            ReceiveItem(slingshot);
        }

        private void ReceiveBed(string furnitureId)
        {
            var id = furnitureId;
            var furniture = new BedFurniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveFishTank(string furnitureId)
        {
            var id = furnitureId;
            var furniture = new FishTankFurniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveTV(string furnitureId)
        {
            var id = furnitureId;
            var furniture = new TV(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveFurniture(string furnitureId)
        {
            var id = furnitureId;
            var furniture = new Furniture(id, Vector2.Zero);
            ReceiveItem(furniture);
        }

        private void ReceiveItem(Item item)
        {
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(item);
        }

        private void ReceiveHat(string hatId)
        {
            var id = hatId;
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
            Game1.addMailForTomorrow("Island_UpgradeParrotPlatform", true, true);
            Game1.netWorldState.Value.ParrotPlatformsUnlocked = true;
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
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_WEAPON, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_WEAPON]);
        }

        private void GetSwordOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_SWORD, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_SWORD]);
        }

        private void GetClubOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_CLUB, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_CLUB]);
        }

        private void GetDaggerOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_DAGGER, _weaponsManager.WeaponsByCategoryByTier[WeaponsManager.TYPE_DAGGER]);
        }

        private void GetBootsOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_BOOTS, _weaponsManager.BootsByTier);
        }

        private void GetSlingshotOfNextTier()
        {
            GetProgressiveEquipmentOfNextTier(EquipmentUnlockManager.PROGRESSIVE_SLINGSHOT, _weaponsManager.SlingshotsByTier);
        }

        private void GetProgressiveEquipmentOfNextTier(string apUnlock, Dictionary<int, List<StardewItem>> equipmentsByTier)
        {
            // This includes the current letter due to the timing of this patch
            var tier = _mail.OpenedMailsContainingKey(apUnlock);
            tier = Math.Max(1, tier);

            while (!equipmentsByTier.ContainsKey(tier) || !equipmentsByTier[tier].Any())
            {
                tier--;
            }

            var equipmentsOfTier = equipmentsByTier[tier];
            var chosenEquipmentIndex = Game1.random.Next(0, equipmentsOfTier.Count);
            var chosenEquipment = equipmentsOfTier[chosenEquipmentIndex];
            var equipmentToGive = chosenEquipment.PrepareForGivingToFarmer();

            Game1.player.holdUpItemThenMessage(equipmentToGive);
            Game1.player.addItemByMenuIfNecessary(equipmentToGive);
        }

        private void LearnCookingRecipe(string recipeItemName)
        {
            var realRecipeName = recipeItemName.Replace("_", " ");
            if (Game1.player.cookingRecipes.ContainsKey(realRecipeName))
            {
                Game1.player.cookingRecipes[realRecipeName] = 0;
                return;
            }
            Game1.player.cookingRecipes.Add(realRecipeName, 0);
        }

        private void LearnSpecialCraftingRecipe(string recipeItemName)
        {
            // When more mods start to need name mapping, we can make a generic version of this
            var nameMapper = new CraftingRecipeNameMapper();
            var internalName = nameMapper.GetRecipeName(recipeItemName.Replace("_", " "));
            if (Game1.player.craftingRecipes.ContainsKey(internalName))
            {
                Game1.player.craftingRecipes[internalName] = 0;
                return;
            }
            Game1.player.craftingRecipes.Add(internalName, 0);
        }
    }
}
