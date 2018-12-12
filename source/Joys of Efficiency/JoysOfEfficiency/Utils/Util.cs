using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using xTile.Layers;
using static System.String;
using static StardewValley.Game1;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace JoysOfEfficiency.Utils
{
    using Player = Farmer;
    using SVObject = Object;
    internal class Util
    {
        public static IModHelper Helper;
        public static IMonitor Monitor;
        internal static ModEntry ModInstance;
        private static bool _catchingTreasure;

        private static readonly MineIcons Icons = new MineIcons();
        private static List<Monster> _lastMonsters = new List<Monster>();
        private static readonly List<Vector2> FlowerLocationProducingNectar = new List<Vector2>();

        public static string LastKilledMonster { get; private set; }

        private static int LastItemIndex;

        private static int AutoFishingCounter;

        #region Public EntryPoint

        public static void LootAllAcceptableItems(ItemGrabMenu menu, bool skipCheck = false)
        {
            if (!skipCheck)
            {
                if (menu.shippingBin || IsCAShippingBinMenu(menu))
                {
                    Monitor.Log("Don't do anything with shipping bin", LogLevel.Trace);
                    return;
                }

                if (menu.reverseGrab)
                {
                    Monitor.Log("You can't get item from this menu.", LogLevel.Trace);
                    return;
                }

                if (menu.source == ItemGrabMenu.source_chest)
                {
                    Monitor.Log("Don't do anything with chest player placed", LogLevel.Trace);
                    return;
                }

                if (menu.showReceivingMenu && menu.source == ItemGrabMenu.source_none)
                {
                    Monitor.Log("showReceivingMenu true but is not gift or fishing chest.", LogLevel.Trace);
                    return;
                }
            }

            for (int i = menu.ItemsToGrabMenu.actualInventory.Count - 1; i >= 0; i--)
            {
                if (i >= menu.ItemsToGrabMenu.actualInventory.Count)
                {
                    continue;
                }
                Item item = menu.ItemsToGrabMenu.actualInventory[i];
                int oldStack = item.Stack;
                int remain = AddItemIntoInventory(item);
                int taken = oldStack - remain;
                if (taken > 0)
                {
                    Monitor.Log($"You looted {item.DisplayName}{(taken == 1 ? "" : " x" + taken)}.");
                }

                if (remain == 0)
                {
                    menu.ItemsToGrabMenu.actualInventory.Remove(item);
                    continue;
                }

                menu.ItemsToGrabMenu.actualInventory[i].Stack = remain;
            }
        }
        
        public static void LetAnimalsInHome()
        {
            Farm farm = getFarm();
            foreach (KeyValuePair<long, FarmAnimal> kv in farm.animals.Pairs.ToArray())
            {
                FarmAnimal animal = kv.Value;
                Monitor.Log($"Warped {animal.displayName}({animal.shortDisplayType()}) to {animal.displayHouse}@[{animal.homeLocation.X}, {animal.homeLocation.Y}]");
                animal.warpHome(farm, animal);
            }
        }

        public static void TryToEatIfNeeded(Player player)
        {
            if (player.isEating || activeClickableMenu != null)
            {
                return;
            }
            if (player.CurrentTool != null && player.CurrentTool is FishingRod rod)
            {
                if (rod.inUse() && !player.UsingTool)
                {
                    return;
                }
            }
            if (player.Stamina <= player.MaxStamina * ModEntry.Conf.StaminaToEatRatio || player.health <= player.maxHealth * ModEntry.Conf.HealthToEatRatio)
            {
                SVObject itemToEat = null;
                foreach (SVObject item in player.Items.OfType<SVObject>())
                {
                    if (item.Edibility > 0)
                    {
                        //It's a edible item
                        if (itemToEat == null || itemToEat.Edibility / itemToEat.salePrice() < item.Edibility / item.salePrice())
                        {
                            //Found good edibility per price or just first food
                            itemToEat = item;
                        }
                    }
                }
                if (itemToEat != null)
                {
                    player.eatObject(itemToEat);
                    itemToEat.Stack--;
                    if (itemToEat.Stack == 0)
                    {
                        player.removeItemFromInventory(itemToEat);
                    }
                }
            }
        }

        public static void AutoFishing(BobberBar bar)
        {
            AutoFishingCounter = (AutoFishingCounter + 1) % 3;
            if (AutoFishingCounter > 0)
            {
                return;
            }

            IReflectionHelper reflection = Helper.Reflection;

            IReflectedField<float> bobberSpeed = reflection.GetField<float>(bar, "bobberBarSpeed");

            float barPos = reflection.GetField<float>(bar, "bobberBarPos").GetValue();
            int barHeight = reflection.GetField<int>(bar, "bobberBarHeight").GetValue();
            float fishPos = reflection.GetField<float>(bar, "bobberPosition").GetValue();
            float treasurePos = reflection.GetField<float>(bar, "treasurePosition").GetValue();
            float distanceFromCatching = reflection.GetField<float>(bar, "distanceFromCatching").GetValue();
            bool treasureCaught = reflection.GetField<bool>(bar, "treasureCaught").GetValue();
            bool treasure = reflection.GetField<bool>(bar, "treasure").GetValue();
            float treasureApeearTimer = reflection.GetField<float>(bar, "treasureAppearTimer").GetValue();
            float bobberBarSpeed = bobberSpeed.GetValue();

            float top = barPos;

            if (treasure && treasureApeearTimer <= 0 && !treasureCaught)
            {
                if (!_catchingTreasure && distanceFromCatching > 0.7f)
                {
                    _catchingTreasure = true;
                }
                if (_catchingTreasure && distanceFromCatching < 0.3f)
                {
                    _catchingTreasure = false;
                }
                if (_catchingTreasure)
                {
                    fishPos = treasurePos;
                }
            }

            if (fishPos > barPos + barHeight / 2)
            {
                return;
            }

            float strength = (fishPos - (barPos + barHeight / 2)) / 16f;
            float distance = fishPos - top;

            float threshold = Cap(ModEntry.Conf.CpuThresholdFishing, 0, 0.5f);
            if (distance < threshold * barHeight || distance > (1 - threshold) * barHeight)
            {
                bobberBarSpeed = strength;
            }

            bobberSpeed.SetValue(bobberBarSpeed);
        }
        public static void ShearingAndMilking(Player player)
        {
            int radius = ModEntry.Conf.AnimalHarvestRadius * tileSize;
            Rectangle bb = Expand(player.GetBoundingBox(), radius);
            foreach (FarmAnimal animal in GetAnimalsList(player))
            {
                string lowerType = animal.type.Value.ToLower();
                if (animal.currentProduce.Value < 0 || animal.age.Value < animal.ageWhenMature.Value || player.CurrentTool == null || !animal.GetBoundingBox().Intersects(bb))
                {
                    continue;
                }
                if (lowerType.Contains("sheep") && player.CurrentTool is Shears && player.Stamina >= 4f ||
                    lowerType.Contains("cow") && player.CurrentTool is MilkPail && player.Stamina >= 4f ||
                    lowerType.Contains("goat") && player.CurrentTool is MilkPail && player.Stamina >= 4f
                    )
                {
                }
                else
                    continue;

                if (!player.addItemToInventoryBool(new Object(Vector2.Zero, animal.currentProduce.Value, null, false, true, false, false)
                {
                    Quality = animal.produceQuality.Value
                }))
                {
                    continue;
                }

                switch (player.CurrentTool)
                {
                    case Shears _: Shears.playSnip(player); break;
                    case MilkPail _:
                        player.currentLocation.localSound("Milking");
                        DelayedAction.playSoundAfterDelay("fishingRodBend", 300);
                        DelayedAction.playSoundAfterDelay("fishingRodBend", 1200);
                        break;
                    default: continue;
                }
                animal.doEmote(20);
                playSound("coin");
                animal.currentProduce.Value = -1;
                if (animal.showDifferentTextureWhenReadyForHarvest.Value)
                {
                    animal.Sprite.LoadTexture("Animals\\Sheared" + animal.type.Value);
                }
                player.gainExperience(0, 5);
            }
        }

        public static void TryCloseItemGrabMenu(ItemGrabMenu menu)
        {
            if (!menu.areAllItemsTaken() || menu.heldItem != null)
            {
                return;
            }

            if (menu.shippingBin || IsCAShippingBinMenu(menu))
            {
                //It's a shipping bin.
                return;
            }

            if (menu.context is Event && GetEssential(menu))
            {
                // You should not emergency close in events (it may stop the dialogue).
                return;
            }
            switch (menu.source)
            {
                case ItemGrabMenu.source_chest:
                case ItemGrabMenu.source_none when menu.context == null:
                    return;// It's a chest.
            }

            menu.exitThisMenu();
        }

        public static void ScavengeTrashCan()
        {
            if (!(currentLocation is Town))
            {
                return;
            }

            int radius = ModEntry.Conf.BalancedMode ? 1 : ModEntry.Conf.ScavengingRadius;
            Layer layer = currentLocation.Map.GetLayer("Buildings");
            int ox = player.getTileX(), oy = player.getTileY();
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int x = ox + dx, y = oy + dy;

                    if (layer.Tiles[x, y]?.TileIndex == 78)
                    {
                        CollectTrashCan(x, y);
                    }
                }
            }
        }

        public static void CollectMailAttachmentsAndQuests(LetterViewerMenu menu)
        {
            IReflectedField<int> questIDField = Helper.Reflection.GetField<int>(menu, "questID");
            int questID = questIDField.GetValue();

            if (menu.itemsLeftToGrab())
            {
                foreach (ClickableComponent component in menu.itemsToGrab.ToArray())
                {
                    if (component.item != null && CanPlayerAcceptsItemPartially(component.item))
                    {
                        int stack = component.item.Stack;
                        playSound("coin");
                        int remain = AddItemIntoInventory(component.item);

                        Monitor.Log($"You collected {component.item.DisplayName}{(stack - remain > 1 ? " x" + (stack - remain) : "")}.");
                        if (remain == 0)
                        {
                            component.item = null;
                        }
                        else
                        {
                            component.item.Stack = remain;
                        }
                    }
                }
            }

            if (questID != -1)
            {
                Monitor.Log($"You started Quest: {Quest.getQuestFromId(questID).questTitle}''.");
                player.addQuest(questID);
                playSound("newArtifact");
                questIDField.SetValue(-1);
            }
        }

        public static void UnifyFlowerColors()
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> featurePair in currentLocation.terrainFeatures.Pairs.Where(kv => kv.Value is HoeDirt))
            {
                Vector2 loc = featurePair.Key;
                HoeDirt dirt = (HoeDirt)featurePair.Value;
                Crop crop = dirt.crop;
                if (crop == null || dirt.crop.dead.Value || !dirt.crop.programColored.Value)
                {
                    continue;
                }
                Color oldColor = crop.tintColor.Value;
                switch (crop.indexOfHarvest.Value)
                {
                    case 376:
                        //Poppy
                        crop.tintColor.Value = ModEntry.Conf.PoppyColor;
                        break;
                    case 591:
                        //Tulip
                        crop.tintColor.Value = ModEntry.Conf.TulipColor;
                        break;
                    case 597:
                        //Blue Jazz
                        crop.tintColor.Value = ModEntry.Conf.JazzColor;
                        break;
                    case 593:
                        //Summer Spangle
                        crop.tintColor.Value = ModEntry.Conf.SummerSpangleColor;
                        break;
                    case 595:
                        //Fairy Rose
                        crop.tintColor.Value = ModEntry.Conf.FairyRoseColor;
                        break;
                    default:
                        continue;
                }

                if (oldColor.PackedValue != crop.tintColor.Value.PackedValue)
                {
                    SVObject obj = new SVObject(crop.indexOfHarvest.Value, 1);
                    Monitor.Log($"changed {obj.DisplayName} @[{loc.X},{loc.Y}] to color(R:{crop.tintColor.R},G:{crop.tintColor.G},B:{crop.tintColor.B},A:{crop.tintColor.A})", LogLevel.Trace);
                }
            }
        }

        public static void UpdateNectarInfo()
        {
            FlowerLocationProducingNectar.Clear();
            foreach (KeyValuePair<Vector2, SVObject> kv in currentLocation.Objects.Pairs.Where(pair => pair.Value.Name == "Bee House"))
            {
                Vector2 houseLoc = kv.Key;
                foreach (Vector2 flowerLoc in GetAreaOfCollectingNector(houseLoc))
                {
                    if ((int)flowerLoc.X < 0 && (int)flowerLoc.Y < 0 && !FlowerLocationProducingNectar.Contains(flowerLoc))
                    {
                        FlowerLocationProducingNectar.Add(flowerLoc);
                    }
                }
            }
        }

        public static void PetNearbyPets()
        {
            GameLocation location = currentLocation;
            Player player = Game1.player;

            Rectangle bb = Expand(player.GetBoundingBox(), ModEntry.Conf.AutoPetRadius * tileSize);

            foreach (Pet pet in location.characters.OfType<Pet>().Where(pet => pet.GetBoundingBox().Intersects(bb)))
            {
                bool wasPet = Helper.Reflection.GetField<bool>(pet, "wasPetToday").GetValue();
                if (!wasPet)
                {
                    pet.checkAction(player, location); // Pet pet... lol
                }
            }
        }

        public static void DepositIngredientsToMachines()
        {
            Player player = Game1.player;
            if (player.CurrentItem == null || !(Game1.player.CurrentItem is SVObject item))
            {
                return;
            }
            foreach (SVObject obj in GetObjectsWithin<SVObject>(ModEntry.Conf.MachineRadius).Where(IsObjectMachine))
            {
                Vector2 loc = GetLocationOf(currentLocation, obj);
                if (obj.heldObject.Value != null)
                    continue;

                bool flag = false;
                bool accepted = obj.Name == "Furnace" ? CanFurnaceAcceptItem(item, player) : Utility.isThereAnObjectHereWhichAcceptsThisItem(currentLocation, item, (int)loc.X * tileSize, (int)loc.Y * tileSize);
                if (obj is Cask)
                {
                    if (ModEntry.IsCoGOn || ModEntry.IsCAOn)
                    {
                        if (obj.performObjectDropInAction(item, true, player))
                        {
                            obj.heldObject.Value = null;
                            flag = true;
                        }
                    }
                    else if (currentLocation is Cellar && accepted)
                    {
                        flag = true;
                    }
                }
                else if (accepted)
                {
                    flag = true;
                }
                if (!flag)
                    continue;

                obj.performObjectDropInAction(item, false, player);
                if (obj.Name != "Furnace" || item.getStack() == 0)
                {
                    player.reduceActiveItemByOne();
                }

                return;
            }
        }

        public static void PullMachineResult()
        {
            Player player = Game1.player;
            foreach (SVObject obj in GetObjectsWithin<SVObject>(ModEntry.Conf.MachineRadius).Where(IsObjectMachine))
            {
                if (!obj.readyForHarvest.Value || obj.heldObject.Value == null)
                    continue;

                Item item = obj.heldObject.Value;
                if (player.couldInventoryAcceptThisItem(item))
                    obj.checkForAction(player);
            }
        }

        public static void ShakeNearbyFruitedBush()
        {
            int radius = ModEntry.Conf.AutoShakeRadius;
            foreach (Bush bush in currentLocation.largeTerrainFeatures.OfType<Bush>())
            {
                Vector2 loc = bush.tilePosition.Value;
                Vector2 diff = loc - player.getTileLocation();
                if (Math.Abs(diff.X) > radius || Math.Abs(diff.Y) > radius)
                    continue;

                if (!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(currentSeason, dayOfMonth))
                    bush.performUseAction(loc, currentLocation);
            }
        }

        public static void ShakeNearbyFruitedTree()
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> kv in GetFeaturesWithin<TerrainFeature>(ModEntry.Conf.AutoShakeRadius))
            {
                Vector2 loc = kv.Key;
                TerrainFeature feature = kv.Value;
                switch (feature)
                {
                    case Tree tree:
                        if (tree.hasSeed.Value && !tree.stump.Value)
                        {
                            if (!IsMultiplayer && player.ForagingLevel < 1)
                            {
                                break;
                            }
                            int num2;
                            switch (tree.treeType.Value)
                            {
                                case 3:
                                    num2 = 311;
                                    break;
                                case 1:
                                    num2 = 309;
                                    break;
                                case 2:
                                    num2 = 310;
                                    break;
                                case 6:
                                    num2 = 88;
                                    break;
                                default:
                                    num2 = -1;
                                    break;
                            }
                            if (currentSeason.Equals("fall") && tree.treeType.Value == 2 && dayOfMonth >= 14)
                            {
                                num2 = 408;
                            }
                            if (num2 != -1)
                                Helper.Reflection.GetMethod(tree, "shake").Invoke(loc, false);
                        }
                        break;
                    case FruitTree ftree:
                        if (ftree.growthStage.Value >= 4 && ftree.fruitsOnTree.Value > 0 && !ftree.stump.Value)
                        {
                            ftree.shake(loc, false);
                        }
                        break;
                }
            }
        }

        public static void DigNearbyArtifactSpots()
        {
            int radius = ModEntry.Conf.AutoDigRadius;
            Hoe hoe = FindToolFromInventory<Hoe>(player, ModEntry.Conf.FindHoeFromInventory);
            GameLocation location = player.currentLocation;
            if (hoe != null)
            {
                bool flag = false;
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        int x = player.getTileX() + i;
                        int y = player.getTileY() + j;
                        Vector2 loc = new Vector2(x, y);
                        if (location.Objects.ContainsKey(loc) && location.Objects[loc].ParentSheetIndex == 590 && !location.isTileHoeDirt(loc))
                        {
                            location.digUpArtifactSpot(x, y, player);
                            location.Objects.Remove(loc);
                            location.terrainFeatures.Add(loc, new HoeDirt());
                            flag = true;
                        }
                    }
                }
                if (flag)
                    playSound("hoeHit");
            }
        }

        public static void CollectNearbyCollectibles(GameLocation location)
        {
            foreach (SVObject obj in GetObjectsWithin<SVObject>(ModEntry.Conf.AutoCollectRadius))
                if (obj.IsSpawnedObject || obj.isAnimalProduct())
                    CollectObj(location, obj);
        }

        public static void DestroyNearDeadCrops(Player player)
        {
            GameLocation location = player.currentLocation;
            foreach (KeyValuePair<Vector2, HoeDirt> kv in GetFeaturesWithin<HoeDirt>(1))
            {
                Vector2 loc = kv.Key;
                HoeDirt dirt = kv.Value;
                if (dirt.crop != null && dirt.crop.dead.Value)
                {
                    dirt.destroyCrop(loc, true, location);
                }
            }
            foreach (IndoorPot pot in GetObjectsWithin<IndoorPot>(1))
            {
                Vector2 loc = GetLocationOf(location, pot);
                HoeDirt dirt = pot.hoeDirt.Value;
                if (dirt?.crop != null && dirt.crop.dead.Value)
                {
                    dirt.destroyCrop(loc, true, location);
                }
            }
        }

        public static void HarvestNearCrops(Player player)
        {
            GameLocation location = player.currentLocation;
            int radius = ModEntry.Conf.AutoHarvestRadius;

            if (ModEntry.Conf.ProtectNectarProducingFlower)
            {
                UpdateNectarInfo();
            }

            foreach (KeyValuePair<Vector2, HoeDirt> kv in GetFeaturesWithin<HoeDirt>(radius))
            {
                Vector2 loc = kv.Key;
                HoeDirt dirt = kv.Value;
                if (dirt.crop == null)
                    continue;

                if (dirt.readyForHarvest())
                {
                    if (IsBlackListed(dirt.crop) || (ModEntry.Conf.ProtectNectarProducingFlower && IsProducingNectar(loc)))
                        continue;
                    if (Harvest((int)loc.X, (int)loc.Y, dirt))
                    {
                        if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                        {
                            //destroy crop if it does not regrow.
                            dirt.destroyCrop(loc, true, location);
                        }
                    }
                }
            }
            foreach (IndoorPot pot in GetObjectsWithin<IndoorPot>(radius))
            {
                HoeDirt dirt = pot.hoeDirt.Value;
                if (dirt?.crop == null || !dirt.readyForHarvest())
                    continue;
                Vector2 tileLoc = GetLocationOf(location, pot);
                if (dirt.crop.harvest((int)tileLoc.X, (int)tileLoc.Y, dirt))
                {
                    if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                    {
                        //destroy crop if it does not regrow.
                        dirt.destroyCrop(tileLoc, true, location);
                    }
                }
            }
        }

        public static void WaterNearbyCrops()
        {
            WateringCan can = FindToolFromInventory<WateringCan>(player, ModEntry.Conf.FindCanFromInventory);
            if (can != null)
            {
                GetMaxCan(can);
                bool watered = false;
                foreach (KeyValuePair<Vector2, HoeDirt> kv in GetFeaturesWithin<HoeDirt>(ModEntry.Conf.AutoWaterRadius))
                {
                    HoeDirt dirt = kv.Value;
                    float consume = 2 * (1.0f / (can.UpgradeLevel / 2.0f + 1));
                    if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state.Value == 0 && player.Stamina >= consume && can.WaterLeft > 0)
                    {
                        dirt.state.Value = 1;
                        player.Stamina -= consume;
                        can.WaterLeft--;
                        watered = true;
                    }
                }
                foreach (IndoorPot pot in GetObjectsWithin<IndoorPot>(ModEntry.Conf.AutoWaterRadius))
                {
                    if (pot.hoeDirt.Value != null)
                    {
                        HoeDirt dirt = pot.hoeDirt.Value;
                        float consume = 2 * (1.0f / (can.UpgradeLevel / 2.0f + 1));
                        if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state.Value != 1 && player.Stamina >= consume && can.WaterLeft > 0)
                        {
                            dirt.state.Value = 1;
                            pot.showNextIndex.Value = true;
                            player.Stamina -= consume;
                            can.WaterLeft--;
                            watered = true;
                        }
                    }
                }
                if (watered)
                {
                    playSound("slosh");
                }
            }
        }

        public static void ToggleBlacklistUnderCursor()
        {
            GameLocation location = currentLocation;
            Vector2 tile = currentCursorTile;
            if (!location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrain))
                return;
            if (!(terrain is HoeDirt dirt))
                return;

            if (dirt.crop == null)
            {
                ShowHudMessage("There is no crop under the cursor");
            }
            else
            {
                string name = dirt.crop.forageCrop.Value ? new SVObject(dirt.crop.whichForageCrop.Value, 1).Name : new SVObject(dirt.crop.indexOfHarvest.Value, 1).Name;
                if (name == "")
                {
                    return;
                }

                string text = ToggleBlackList(dirt.crop)
                    ? $"{name} has been added to AutoHarvest exception"
                    : $"{name} has been removed from AutoHarvest exception";
                ShowHudMessage(text, 1000);
                Monitor.Log(text);
            }
        }
        
        public static void DrawMineGui(SpriteBatch batch, SpriteFont font, Player player, MineShaft shaft)
        {
            ITranslationHelper translation = Helper.Translation;
            int stonesLeft = CountActualStones(shaft);
            Vector2 ladderPos = FindLadder(shaft);
            bool ladder = ladderPos != Vector2.Zero;

            List<Monster> currentMonsters = shaft.characters.OfType<Monster>().ToList();
            foreach (Monster mon in _lastMonsters)
            {
                if (!currentMonsters.Contains(mon) && mon.Name != "ignoreMe")
                {
                    LastKilledMonster = mon.Name;
                }
            }
            _lastMonsters = currentMonsters.ToList();
            string tallyStr = null;
            string ladderStr = null;
            if (LastKilledMonster != null)
            {
                int kills = stats.getMonstersKilled(LastKilledMonster);
                tallyStr = Format(translation.Get("monsters.tally"), LastKilledMonster, kills);
            }

            string stonesStr;
            if (stonesLeft == 0)
            {
                stonesStr = translation.Get("stones.none");
            }
            else
            {
                bool single = stonesLeft == 1;
                stonesStr = single ? translation.Get("stones.one") : Format(translation.Get("stones.many"), stonesLeft);
            }
            if (ladder)
            {
                ladderStr = translation.Get("ladder");
            }
            Icons.Draw(stonesStr, tallyStr, ladderStr);

        }

        public static void DrawFishingInfoBox(SpriteBatch batch, BobberBar bar, SpriteFont font)
        {
            IReflectionHelper reflection = Helper.Reflection;
            ITranslationHelper translation = Helper.Translation;

            int width = 0, height = 120;


            float scale = 1.0f;


            int whitchFish = reflection.GetField<int>(bar, "whichFish").GetValue();
            int fishSize = reflection.GetField<int>(bar, "fishSize").GetValue();
            int fishQuality = reflection.GetField<int>(bar, "fishQuality").GetValue();
            bool treasure = reflection.GetField<bool>(bar, "treasure").GetValue();
            bool treasureCaught = reflection.GetField<bool>(bar, "treasureCaught").GetValue();
            float treasureAppearTimer = reflection.GetField<float>(bar, "treasureAppearTimer").GetValue() / 1000;

            SVObject fish = new SVObject(whitchFish, 1);

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                scale = 0.7f;
            }

            string speciesText = TryFormat(translation.Get("fishinfo.species").ToString(), fish.DisplayName);
            string sizeText = TryFormat(translation.Get("fishinfo.size").ToString(), GetFinalSize(fishSize));
            string qualityText1 = translation.Get("fishinfo.quality").ToString();
            string qualityText2 = translation.Get(GetKeyForQuality(fishQuality)).ToString();
            string incomingText = TryFormat(translation.Get("fishinfo.treasure.incoming").ToString(), treasureAppearTimer);
            string appearedText = translation.Get("fishinfo.treasure.appear").ToString();
            string caughtText = translation.Get("fishinfo.treasure.caught").ToString();

            {
                Vector2 size = font.MeasureString(speciesText) * scale;
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;
                size = font.MeasureString(sizeText) * scale;
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;
                Vector2 temp = font.MeasureString(qualityText1);
                Vector2 temp2 = font.MeasureString(qualityText2);
                size = new Vector2(temp.X + temp2.X, Math.Max(temp.Y, temp2.Y));
                if (size.X > width)
                {
                    width = (int)size.X;
                }
                height += (int)size.Y;
            }

            if (treasure)
            {
                if (treasureAppearTimer > 0)
                {
                    Vector2 size = font.MeasureString(incomingText) * scale;
                    if (size.X > width)
                    {
                        width = (int)size.X;
                    }
                    height += (int)size.Y;
                }
                else
                {
                    if (!treasureCaught)
                    {
                        Vector2 size = font.MeasureString(appearedText) * scale;
                        if (size.X > width)
                        {
                            width = (int)size.X;
                        }
                        height += (int)size.Y;
                    }
                    else
                    {
                        Vector2 size = font.MeasureString(caughtText) * scale;
                        if (size.X > width)
                        {
                            width = (int)size.X;
                        }
                        height += (int)size.Y;
                    }
                }
            }

            width += 64;

            int x = bar.xPositionOnScreen + bar.width + 96;
            if (x + width > viewport.Width)
            {
                x = bar.xPositionOnScreen - width - 96;
            }
            int y = (int)Cap(bar.yPositionOnScreen, 0, viewport.Height - height);

            IClickableMenu.drawTextureBox(batch, menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            fish.drawInMenu(batch, new Vector2(x + width / 2 - 32, y + 16), 1.0f, 1.0f, 0.9f, false);

            Vector2 vec2 = new Vector2(x + 32, y + 96);
            DrawString(batch, font, ref vec2, speciesText, Color.Black, scale);
            DrawString(batch, font, ref vec2, sizeText, Color.Black, scale);
            DrawString(batch, font, ref vec2, qualityText1, Color.Black, scale, true);
            DrawString(batch, font, ref vec2, qualityText2, GetColorForQuality(fishQuality), scale);
            vec2.X = x + 32;
            if (treasure)
            {
                if (!treasureCaught)
                {
                    if (treasureAppearTimer > 0f)
                    {
                        DrawString(batch, font, ref vec2, incomingText, Color.Red, scale);
                    }
                    else
                    {
                        DrawString(batch, font, ref vec2, appearedText, Color.LightGoldenrodYellow, scale);
                    }
                }
                else
                {
                    DrawString(batch, font, ref vec2, caughtText, Color.ForestGreen, scale);
                }
            }
        }

        public static void TryToggleGate(Player player)
        {
            foreach (Fence fence in GetObjectsWithin<Fence>(2).Where(f => f.isGate.Value))
            {
                Vector2 loc = fence.TileLocation;

                bool? isUpDown = IsUpsideDown(fence);
                if (isUpDown == null)
                {
                    if (!fence.getBoundingBox(loc).Intersects(player.GetBoundingBox()))
                    {
                        fence.gatePosition.Value = 0;
                    }
                    continue;
                }

                int gatePosition = fence.gatePosition.Value;
                bool flag = IsPlayerInClose(player, fence, fence.TileLocation, isUpDown);

                if (flag && gatePosition == 0)
                {
                    fence.gatePosition.Value = 88;
                    playSound("doorClose");
                }
                else if (!flag && gatePosition >= 88)
                {
                    fence.gatePosition.Value = 0;
                    playSound("doorClose");
                }
            }
        }

        #endregion

        #region Public Utility

        public static T FindToolFromInventory<T>(bool fromEntireInventory) where T : Tool
        {
            Player player = Game1.player;
            if (player.CurrentTool is T t)
            {
                return t;
            }
            if (!fromEntireInventory)
                return null;

            foreach (Item item in player.Items)
            {
                if (item is T t2)
                {
                    return t2;
                }
            }
            return null;
        }

        public static float Cap(float f, float min, float max)
        {
            return f < min ? min : (f > max ? max : f);
        }

        public static List<FarmAnimal> GetAnimalsList(Player player)
        {
            List<FarmAnimal> list = new List<FarmAnimal>();
            if (player.currentLocation is Farm farm)
            {
                foreach (SerializableDictionary<long, FarmAnimal> animal in farm.animals)
                {
                    foreach (KeyValuePair<long, FarmAnimal> kv in animal)
                    {
                        list.Add(kv.Value);
                    }
                }
            }
            else if (player.currentLocation is AnimalHouse house)
            {
                foreach (SerializableDictionary<long, FarmAnimal> animal in house.animals)
                {
                    foreach (KeyValuePair<long, FarmAnimal> kv in animal)
                    {
                        list.Add(kv.Value);
                    }
                }
            }
            return list;
        }

        public static void ShowHudMessage(string message, int duration = 3500)
        {
            HUDMessage hudMessage = new HUDMessage(message, 3)
            {
                noIcon = true,
                timeLeft = duration
            };
            addHUDMessage(hudMessage);
        }


        public static void ShowHudMessageWithIcon(string message, Item icon, int number = -1, int duration = 3500)
        {
            if (number == -1)
            {
                number = icon.Stack;
            }
            HUDMessage hudMessage = new HUDMessage(message, number, true, Color.Black, icon)
            {
                timeLeft = duration
            };
            addHUDMessage(hudMessage);
        }

        public static Rectangle Expand(Rectangle rect, int radius)
        {
            return new Rectangle(rect.Left - radius, rect.Top - radius, 2 * radius, 2 * radius);
        }

        public static void DrawSimpleTextbox(SpriteBatch batch, string text, int x, int y, SpriteFont font, Item item = null)
        {
            Vector2 stringSize = text == null ? Vector2.Zero : font.MeasureString(text);
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
            int rightX = (int)stringSize.X + tileSize / 2 + 8;
            if (item != null)
            {
                rightX += tileSize;
            }
            if (x + rightX > viewport.Width)
            {
                x = viewport.Width - rightX;
            }
            int bottomY = (int)stringSize.Y + 32;
            if (item != null)
            {
                bottomY = (int)(tileSize * 1.7);
            }
            if (bottomY + y > viewport.Height)
            {
                y = viewport.Height - bottomY;
            }
            IClickableMenu.drawTextureBox(batch, menuTexture, new Rectangle(0, 256, 60, 60), x, y, rightX, bottomY, Color.White);
            if (!IsNullOrEmpty(text))
            {
                Vector2 vector2 = new Vector2(x + tileSize / 4, y + (bottomY - stringSize.Y) / 2 + 8f);
                Utility.drawTextWithShadow(batch, text, font, vector2, Color.Black);
            }
            item?.drawInMenu(batch, new Vector2(x + (int)stringSize.X + 24, y + 16), 1.0f, 1.0f, 0.9f, false);
        }

        public static void DrawSimpleTextbox(SpriteBatch batch, string text, SpriteFont font, bool isIcon = false, Item item = null)
        {
            DrawSimpleTextbox(batch, text, getMouseX() + tileSize / 2, getMouseY() + (isIcon ? 24 : tileSize) + 24, font, item);
        }

        public static bool IsThereAnyWaterNear(GameLocation location, Vector2 tileLocation)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2 toCheck = tileLocation + new Vector2(i, j);
                    int x = (int)toCheck.X, y = (int)toCheck.Y;
                    if (location.doesTileHaveProperty(x, y, "Water", "Back") != null || location.doesTileHaveProperty(x, y, "WaterSource", "Back") != null || location is BuildableGameLocation loc2 && loc2.buildings.Where(b => b.occupiesTile(toCheck)).Any(building => building.buildingType.Value == "Well"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static T FindToolFromInventory<T>(Player player, bool findFromInventory) where T : Tool
        {
            if (player.CurrentTool is T t)
            {
                return t;
            }
            return findFromInventory ? player.Items.OfType<T>().FirstOrDefault() : null;
        }

        public static void PrintFishingInfo(FishingRod rod)
        {
            GameLocation location = currentLocation;
            bool flag = false;
            Rectangle rectangle = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64);
            Rectangle value = new Rectangle((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64);
            flag = rectangle.Intersects(value);
            int clearWaterDistance = Helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            Dictionary<int, double> dict = GetFishes(currentLocation, rod.attachments[0]?.ParentSheetIndex ?? -1, clearWaterDistance + (flag ? 1 : 0), player);
            DrawProbBox(dict);
        }

        public static List<T> GetObjectsWithin<T>(int radius) where T : SVObject
        {
            if (!Context.IsWorldReady || currentLocation?.Objects == null)
            {
                return new List<T>();
            }
            if (ModEntry.Conf.BalancedMode)
            {
                radius = 1;
            }

            GameLocation location = player.currentLocation;
            Vector2 ov = player.getTileLocation();
            List<T> list = new List<T>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 loc = ov + new Vector2(dx, dy);
                    if (location.Objects.ContainsKey(loc) && location.Objects[loc] is T t)
                    {
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public static void DrawPausedHud()
        {
            SpriteFont font = dialogueFont;
            string text = Helper.Translation.Get("hud.paused");
            Vector2 stringSize = font.MeasureString(text);
            int x = ModEntry.Conf.PauseNotificationX;
            int y = ModEntry.Conf.PauseNotificationY;
            int width = 16 + (int)stringSize.X + 16;
            int height = 16 + (int)stringSize.Y + 16;

            IClickableMenu.drawTextureBox(spriteBatch, x, y, width, height, Color.White);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2(x + 16, y + 16 + 8), Color.Black);
        }

        public static bool IsPlayerIdle()
        {
            if (paused || !shouldTimePass())
            {
                //When game is paused or time is stopped already. it's not idle.
                return false;
            }

            if (player.CurrentToolIndex != LastItemIndex)
            {
                //When tool index changed, it's not idle.
                LastItemIndex = player.CurrentToolIndex;
                return false;
            }

            if (player.isMoving() || player.UsingTool)
            {
                //When player is moving or is using tools, it's not idle of cause.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a item into player inventory.
        /// </summary>-0
        /// <param name="item">The item to push</param>
        /// <returns>Remaining stack number that couldn't be added</returns>
        public static int AddItemIntoInventory(Item item)
        {
            int oldStack = item.Stack;
            int remaining = oldStack;
            for (int i = 0; i < player.MaxItems; i++)
            {
                if (player.Items[i] == null || i >= player.Items.Count)
                {
                    remaining = 0;
                    break;
                }

                Item stack = player.Items[i];

                if (!stack.canStackWith(item))
                {
                    continue;
                }

                int toPut = Math.Min(remaining, stack.maximumStackSize() - stack.Stack);
                if (toPut > 0)
                {
                    remaining -= toPut;
                }

                if (remaining == 0)
                {
                    break;
                }
            }

            player.addItemToInventoryBool(item);
            if (activeClickableMenu is ItemGrabMenu && oldStack - remaining > 0)
            {
                // Draw item pickup hud because addItemToInventoryBool doesn't if ItemGrabMenu is opened.
                Item toShow = item.getOne();
                toShow.Stack = oldStack - remaining;
                DrawItemPickupHud(toShow);
            }

            return remaining;
        }

        public static Chest GetFridge()
        {
            if (!ModEntry.Conf.CraftingFromChests)
            {
                return null;
            }
            int radius = ModEntry.Conf.RadiusCraftingFromChests;
            if (ModEntry.Conf.BalancedMode)
            {
                radius = 1;
            }
            if (currentLocation is FarmHouse house && house.upgradeLevel >= 1)
            {
                Layer layer = house.Map.GetLayer("Buildings");
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int x = player.getTileX() + dx;
                        int y = player.getTileY() + dy;
                        if (x >= 0 && y >= 0 && x < layer.TileWidth && y < layer.TileHeight && layer.Tiles[x, y]?.TileIndex == 173)
                        {
                            //It's the fridge sprite
                            return house.fridge.Value;
                        }
                    }
                }
            }
            return null;
        }

        public static List<Chest> GetNearbyChests(Player player)
        {
            int radius = ModEntry.Conf.BalancedMode ? 1 : ModEntry.Conf.RadiusCraftingFromChests;
            List<Chest> chests = new List<Chest>();
            if (ModEntry.Conf.CraftingFromChests)
            {
                foreach (Chest chest in GetObjectsWithin<Chest>(radius))
                {
                    chests.Add(chest);
                }

                Chest fridge = GetFridge();
                if (fridge != null)
                {
                    chests.Add(fridge);
                }
            }
            return chests;
        }

        public static List<Item> GetNearbyItems(Player player)
        {
            List<Item> items = new List<Item>(player.Items);
            foreach (Chest chest in GetNearbyChests(player))
            {
                items.AddRange(chest.items);
            }
            return items;
        }


        public static void DrawCursor()
        {
            if (!options.hardwareCursor)
                spriteBatch.Draw(mouseCursors, new Vector2(getOldMouseX(), getOldMouseY()),
                    getSourceRectForStandardTileSheet(mouseCursors, options.gamepadControls ? 44 : 0, 16, 16),
                    Color.White, 0f, Vector2.Zero, pixelZoom + dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        public static int GetMaxCan(WateringCan can)
        {
            if (can == null)
                return -1;
            switch (can.UpgradeLevel)
            {
                case 0:
                    can.waterCanMax = 40;
                    break;
                case 1:
                    can.waterCanMax = 55;
                    break;
                case 2:
                    can.waterCanMax = 70;
                    break;
                case 3:
                    can.waterCanMax = 85;
                    break;
                case 4:
                    can.waterCanMax = 100;
                    break;
                default:
                    return -1;
            }

            return can.waterCanMax;

        }

        public static void DrawShippingPrice(IClickableMenu menu, SpriteFont font)
        {
            if (!(menu is ItemGrabMenu grabMenu) || !(grabMenu.shippingBin || IsCAShippingBinMenu(grabMenu)))
            {
                return;
            }
            int shippingPrice = getFarm().shippingBin.Sum(item => GetTruePrice(item) / 2 * item.Stack);
            string title = Helper.Translation.Get("estimatedprice.title");
            string text = $" {shippingPrice}G";
            Vector2 sizeTitle = font.MeasureString(title) * 1.2f;
            Vector2 sizeText = font.MeasureString(text) * 1.2f;
            int width = Math.Max((int)sizeTitle.X, (int)sizeText.X) + 32;
            int height = 16 + (int)sizeTitle.Y + 8 + (int)sizeText.Y + 16;
            Vector2 basePos = new Vector2(menu.xPositionOnScreen - width, menu.yPositionOnScreen + menu.height / 4 - height);

            IClickableMenu.drawTextureBox(spriteBatch, (int)basePos.X, (int)basePos.Y, width, height, Color.White);
            Utility.drawTextWithShadow(spriteBatch, title, font, basePos + new Vector2(16, 16), Color.Black, 1.2f);
            Utility.drawTextWithShadow(spriteBatch, text, font, basePos + new Vector2(16, 16 + (int)sizeTitle.Y + 8), Color.Black, 1.2f);
        }

        #endregion

        #region Private Utility

        private static Dictionary<Vector2, T> GetFeaturesWithin<T>(int radius) where T : TerrainFeature
        {
            if (!Context.IsWorldReady)
            {
                return new Dictionary<Vector2, T>();
            }
            GameLocation location = player.currentLocation;
            Vector2 ov = player.getTileLocation();
            Dictionary<Vector2, T> list = new Dictionary<Vector2, T>();

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 loc = ov + new Vector2(dx, dy);
                    if (location.terrainFeatures.ContainsKey(loc) && location.terrainFeatures[loc] is T t)
                    {
                        list.Add(loc, t);
                    }
                }
            }
            return list;
        }

        private static Vector2 FindLadder(GameLocation shaft)
        {
            for (int i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; i++)
            {
                for (int j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; j++)
                {
                    int index = shaft.getTileIndexAt(new Point(i, j), "Buildings");
                    Vector2 loc = new Vector2(i, j);
                    if (!shaft.Objects.ContainsKey(loc) && !shaft.terrainFeatures.ContainsKey(loc))
                    {
                        if (index == 171 || index == 173 || index == 174)
                            return loc;
                    }
                }
            }
            return Vector2.Zero;
        }

        private static Vector2 GetLocationOf(GameLocation location, SVObject obj)
        {
            return location.Objects.Pairs.Any(kv => kv.Value == obj) ? location.Objects.Pairs.First(kv => kv.Value == obj).Key : new Vector2(-1, -1);
        }

        private static void CollectObj(GameLocation loc, SVObject obj)
        {
            Player who = player;

            Vector2 vector = GetLocationOf(loc, obj);

            if ((int)vector.X == -1 && (int)vector.Y == -1) return;
            if (obj.questItem.Value) return;

            int quality = obj.Quality;
            Random random = new Random((int)uniqueIDForThisGame / 2 + (int)stats.DaysPlayed + (int)vector.X + (int)vector.Y * 777);

            if (who.professions.Contains(16) && obj.isForage(loc))
                obj.Quality = 4;

            else if (obj.isForage(loc))
            {
                if (random.NextDouble() < who.ForagingLevel / 30f)
                    obj.Quality = 2;
                else if (random.NextDouble() < who.ForagingLevel / 15f)
                    obj.Quality = 1;
            }

            if (who.couldInventoryAcceptThisItem(obj))
            {
                Monitor.Log($"picked up {obj.DisplayName} at [{vector.X},{vector.Y}]");
                if (who.IsLocalPlayer)
                {
                    loc.localSound("pickUpItem");
                    DelayedAction.playSoundAfterDelay("coin", 300);
                }
                if (!who.isRidingHorse() && !who.ridingMineElevator)
                    who.animateOnce(279 + who.FacingDirection);

                if (!loc.isFarmBuildingInterior())
                {
                    if (obj.isForage(loc))
                        who.gainExperience(2, 7);
                }
                else
                    who.gainExperience(0, 5);

                who.addItemToInventoryBool(obj.getOne());
                stats.ItemsForaged++;
                if (who.professions.Contains(13) && random.NextDouble() < 0.2 && !obj.questItem.Value && who.couldInventoryAcceptThisItem(obj) && !loc.isFarmBuildingInterior())
                {
                    who.addItemToInventoryBool(obj.getOne());
                    who.gainExperience(2, 7);
                }
                loc.Objects.Remove(vector);
                return;
            }
            obj.Quality = quality;
        }
        private static bool GetEssential(ItemGrabMenu menu)
        {
            return Helper.Reflection.GetField<bool>(menu, "essential").GetValue();
        }

        private static void CollectTrashCan(int x, int y)
        {
            if (!(currentLocation is Town town))
            {
                return;
            }

            NetArray<bool, NetBool> garbageChecked =
                Helper.Reflection.GetField<NetArray<bool, NetBool>>(town, "garbageChecked").GetValue();

            string text = currentLocation.doesTileHaveProperty(x, y, "Action", "Buildings");
            int num = text != null ? Convert.ToInt32(text.Split(' ')[1]) : -1;
            if (num >= 0 && num < garbageChecked.Length && !garbageChecked[num])
            {
                garbageChecked[num] = true;
                currentLocation.playSound("trashcan");
                Random random = new Random((int)uniqueIDForThisGame / 2 + (int)stats.DaysPlayed + 777 + num);
                if (random.NextDouble() < 0.2 + dailyLuck)
                {
                    int parentSheetIndex = 168;
                    switch (random.Next(10))
                    {
                        case 0:
                            parentSheetIndex = 168;
                            break;
                        case 1:
                            parentSheetIndex = 167;
                            break;
                        case 2:
                            parentSheetIndex = 170;
                            break;
                        case 3:
                            parentSheetIndex = 171;
                            break;
                        case 4:
                            parentSheetIndex = 172;
                            break;
                        case 5:
                            parentSheetIndex = 216;
                            break;
                        case 6:
                            parentSheetIndex = Utility.getRandomItemFromSeason(currentSeason, x * 653 + y * 777, false);
                            break;
                        case 7:
                            parentSheetIndex = 403;
                            break;
                        case 8:
                            parentSheetIndex = 309 + random.Next(3);
                            break;
                        case 9:
                            parentSheetIndex = 153;
                            break;
                    }
                    switch (num)
                    {
                        case 3 when random.NextDouble() < 0.2 + dailyLuck:
                            parentSheetIndex = 535;
                            if (random.NextDouble() < 0.05)
                            {
                                parentSheetIndex = 749;
                            }

                            break;
                        case 4 when random.NextDouble() < 0.2 + dailyLuck:
                            parentSheetIndex = 378 + random.Next(3) * 2;
                            break;
                        case 5 when random.NextDouble() < 0.2 + dailyLuck && dishOfTheDay != null:
                            parentSheetIndex = dishOfTheDay.ParentSheetIndex != 217 ? dishOfTheDay.ParentSheetIndex : 216;
                            break;
                        case 6 when random.NextDouble() < 0.2 + dailyLuck:
                            parentSheetIndex = 223;
                            break;
                    }

                    Monitor.Log($"You picked up trash @ [{x},{y}]");
                    player.addItemByMenuIfNecessary(new Object(parentSheetIndex, 1));
                }
            }
        }
        
        private static bool CanPlayerAcceptsItemPartially(Item item)
        {
            if (player.Items.Contains(null) || player.Items.Count < player.MaxItems)
            {
                // Inventory includes at least one free space.
                return true;
            }

            return player.Items.Any(stack => stack.canStackWith(item) && stack.Stack < stack.maximumStackSize());
        }

        private static bool IsCAShippingBinMenu(ItemGrabMenu menu)
        {
            return !menu.reverseGrab && menu.showReceivingMenu && menu.context is Farm;
        }

        private static void DrawItemPickupHud(Item item)
        {
            Color color = Color.WhiteSmoke;
            string text = item.DisplayName;

            if (item is Object obj2)
            {
                switch (obj2.Type)
                {
                    case "Arch":
                        color = Color.Tan;
                        text += content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1954");
                        break;
                    case "Fish":
                        color = Color.SkyBlue;
                        break;
                    case "Mineral":
                        color = Color.PaleVioletRed;
                        break;
                    case "Vegetable":
                        color = Color.PaleGreen;
                        break;
                    case "Fruit":
                        color = Color.Pink;
                        break;
                }
            }

            addHUDMessage(new HUDMessage(text, Math.Max(1, item.Stack), true, color, item));
        }

        private static int GetTruePrice(Item item)
        {
            int truePrice = 0;

            if (item is SVObject objectItem)
            {
                truePrice = objectItem.sellToStorePrice() * 2;
            }
            else
            {
                truePrice = item.salePrice();
            }

            return truePrice;
        }

        private static List<Vector2> GetAreaOfCollectingNector(Vector2 homePoint)
        {
            List<Vector2> cropLocations = new List<Vector2>();
            Queue<Vector2> vector2Queue = new Queue<Vector2>();
            HashSet<Vector2> vector2Set = new HashSet<Vector2>();
            vector2Queue.Enqueue(homePoint);
            for (int index1 = 0; index1 <= 150 && vector2Queue.Count > 0; ++index1)
            {
                Vector2 index2 = vector2Queue.Dequeue();
                if (currentLocation.terrainFeatures.ContainsKey(index2) &&
                    currentLocation.terrainFeatures[index2] is HoeDirt dirt && dirt.crop != null &&
                    dirt.crop.programColored.Value && !dirt.crop.dead.Value && dirt.crop.currentPhase.Value >= dirt.crop.phaseDays.Count - 1)
                {
                    cropLocations.Add(index2);
                }
                foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(index2))
                {
                    if (!vector2Set.Contains(adjacentTileLocation))
                        vector2Queue.Enqueue(adjacentTileLocation);
                }
                vector2Set.Add(index2);
            }

            return cropLocations;
        }

        private static int CountActualStones(MineShaft shaft)
        {
            return shaft.Objects.Pairs.Count(kv => kv.Value.Name.Contains("Stone"));
        }

        private static bool CanFurnaceAcceptItem(Item item, Player player)
        {
            if (player.getTallyOfObject(382, false) <= 0)
                return false;
            if (item.Stack < 5 && item.ParentSheetIndex != 80 && item.ParentSheetIndex != 82 && item.ParentSheetIndex != 330)
                return false;
            switch (item.ParentSheetIndex)
            {
                case 378:
                case 380:
                case 384:
                case 386:
                case 80:
                case 82:
                    break;
                default:
                    return false;
            }
            return true;
        }
        private static void DrawProbBox(Dictionary<int, double> probs)
        {
            SpriteBatch b = spriteBatch;
            Size size = GetProbBoxSize(probs);
            IClickableMenu.drawTextureBox(spriteBatch, ModEntry.Conf.ProbBoxX, ModEntry.Conf.ProbBoxY, size.Width, size.Height, Color.White);
            const int square = (int)(tileSize / 1.5);
            int x = ModEntry.Conf.ProbBoxX + 8;
            int y = ModEntry.Conf.ProbBoxY + 16;
            SpriteFont font = dialogueFont;
            {
                foreach (KeyValuePair<int, double> kv in probs)
                {
                    string text = $"{kv.Value * 100:f1}%";
                    SVObject fish = new SVObject(kv.Key, 1);

                    fish.drawInMenu(b, new Vector2(x + 8, y), 1.0f);
                    Utility.drawTextWithShadow(b, text, font, new Vector2(x + 32 + square, y + 16), Color.Black);

                    y += square + 16;
                }
            }
        }

        private static Size GetProbBoxSize(Dictionary<int, double> probs)
        {
            int width = 16, height = 48;
            int square = (int)(tileSize / 1.5);
            SpriteFont font = dialogueFont;
            {
                foreach (KeyValuePair<int, double> kv in probs)
                {
                    string text = $"{kv.Value * 100:f1}%";
                    Vector2 textSize = font.MeasureString(text);
                    int w = square + (int)textSize.X + 64;
                    if (w > width)
                    {
                        width = w;
                    }
                    height += square + 16;
                }
            }
            return new Size(width, height);
        }

        private static Dictionary<int, double> GetFinalProbabilities(Dictionary<int, double> dict)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            double ratio = 1.0;
            foreach (KeyValuePair<int, double> kv in dict)
            {
                double d = kv.Value * ratio;
                result.Add(kv.Key, d);
                ratio = ratio * (1 - kv.Value);
            }

            return result;
        }

        private static Dictionary<int, double> MagnifyProbabilities(Dictionary<int, double> dict, double ratio)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (KeyValuePair<int, double> kv in dict)
                result.Add(kv.Key, kv.Value * ratio);

            return result;
        }

        private static Dictionary<K, V> ConcatDictionary<K, V>(Dictionary<K, V> a, Dictionary<K, V> b)
        {
            return a.Concat(b).ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<int, double> GetFishes(GameLocation location, int bait, int waterDepth, Player who)
        {
            double sum = 0;
            Dictionary<int, double> dict;
            switch (location)
            {
                case Farm _:
                    dict = GetFishesFarm(waterDepth, who);
                    break;
                case MineShaft shaft:
                    dict = GetFishesMine(shaft, bait, waterDepth, who);
                    break;
                case Submarine _:
                    dict = GetFishesSubmarine();
                    break;
                default:
                    dict = GetFishes(waterDepth, who);
                    break;
            }


            Dictionary<int, double> dict2 =
                GetFinalProbabilities(dict).OrderByDescending(x => x.Value)
                    .Where(kv => !IsGarbage(kv.Key)).ToDictionary(x => x.Key, x => x.Value);
            sum = dict2.Sum(kv => kv.Value);
            if (1 - sum >= 0.001f)
            {
                dict2.Add(168, 1 - sum);
            }
            return dict2;
        }

        private static bool IsGarbage(int index)
        {
            if (index >= 167 && index <= 172)
            {
                return true;
            }
            switch (index)
            {
                case 152:
                case 153:
                case 157: return true;
            }
            return false;
        }

        private static Dictionary<int, double> GetFishes(int waterDepth, Player who, string locationName = null)
        {
            Dictionary<int, double> dict = new Dictionary<int, double>();

            Dictionary<string, string> dictionary = content.Load<Dictionary<string, string>>("Data\\Locations");
            string key = locationName ?? currentLocation.Name;
            if (key.Equals("WitchSwamp") && !MasterPlayer.mailReceived.Contains("henchmanGone") && !player.hasItemInInventory(308, 1))
            {
                return new Dictionary<int, double>
                {
                    {308,0.25}
                };
            }

            try
            {
                if (dictionary.ContainsKey(key))
                {
                    string[] array = dictionary[key].Split('/')[4 + Utility.getSeasonNumber(currentSeason)].Split(' ');
                    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                    if (array.Length > 1)
                    {
                        for (int i = 0; i < array.Length; i += 2)
                        {
                            dictionary2.Add(array[i], array[i + 1]);
                        }
                    }

                    string[] array2 = dictionary2.Keys.ToArray();
                    Dictionary<int, string> dictionary3 = content.Load<Dictionary<int, string>>("Data\\Fish");
                    //Utility.Shuffle(random, array2);
                    foreach (string t in array2)
                    {
                        bool flag2 = true;
                        string[] array3 = dictionary3[Convert.ToInt32(t)].Split('/');
                        string[] array4 = array3[5].Split(' ');
                        int num2 = Convert.ToInt32(dictionary2[t]);
                        if (num2 == -1 || currentLocation.getFishingLocation(who.getTileLocation()) == num2)
                        {
                            int num3 = 0;
                            while (num3 < array4.Length)
                            {
                                if (timeOfDay < Convert.ToInt32(array4[num3]) ||
                                    timeOfDay >= Convert.ToInt32(array4[num3 + 1]))
                                {
                                    num3 += 2;
                                    continue;
                                }

                                flag2 = false;
                                break;
                            }
                        }

                        if (!array3[7].Equals("both"))
                        {
                            if (array3[7].Equals("rainy") && !isRaining)
                            {
                                flag2 = true;
                            }
                            else if (array3[7].Equals("sunny") && isRaining)
                            {
                                flag2 = true;
                            }
                        }

                        if (who.FishingLevel < Convert.ToInt32(array3[12]))
                        {
                            flag2 = true;
                        }

                        if (flag2)
                            continue;

                        double num4 = Convert.ToDouble(array3[10]);
                        double num5 = Convert.ToDouble(array3[11]) * num4;
                        num4 -= Math.Max(0, Convert.ToInt32(array3[9]) - waterDepth) * num5;
                        num4 += who.FishingLevel / 50f;
                        num4 = Math.Min(num4, 0.89999997615814209);
                        int num = Convert.ToInt32(t);

                        dict.Add(num, num4);
                    }
                }
            }
            catch (KeyNotFoundException knf)
            {
                Monitor.Log("KeyNotFound Exception occured. Ignoring...");
                Monitor.Log(knf.ToString());
            }

            return dict;
        }

        private static Dictionary<int, double> GetFishesSubmarine()
        {
            return new Dictionary<int, double>
            {
                { 800, 0.1 },
                { 799, 0.18 },
                { 798, 0.28 },
                { 154, 0.1 },
                { 155, 0.08 },
                { 149, 0.05 },
                { 797, 0.01 }
            };
        }

        private static Dictionary<int, double> GetFishesMine(MineShaft shaft, int bait, int waterDepth, Player who)
        {
            Dictionary<int, double> dict = new Dictionary<int, double>();
            double num2 = 1.0;
            num2 += 0.4 * who.FishingLevel;
            num2 += waterDepth * 0.1;
            double p = 0;
            int level = shaft.getMineArea();
            switch (level)
            {
                case 0:
                case 10:
                    num2 += bait == 689 ? 3 : 0;
                    p = 0.02 + 0.01 * num2;
                    dict.Add(158, p);
                    break;
                case 40:
                    num2 += bait == 682 ? 3 : 0;
                    p = 0.015 + 0.009 * num2;
                    dict.Add(161, p);
                    break;
                case 80:
                    num2 += bait == 684 ? 3 : 0;
                    p = 0.01 + 0.008 * num2;
                    dict.Add(162, p);
                    break;
            }

            if (level == 10 || level == 40)
            {
                return ConcatDictionary(dict,
                    MagnifyProbabilities(
                        GetFishes(waterDepth, who, "UndergroundMine")
                            .Where(kv => !IsGarbage(kv.Key)).ToDictionary(x => x.Key, x => x.Value),
                        1 - p));
            }

            return dict;
        }

        private static Dictionary<int, double> GetFishesFarm(int waterDepth, Player who)
        {
            switch (whichFarm)
            {
                case 1:
                    return ConcatDictionary(MagnifyProbabilities(GetFishes(waterDepth, who, "Forest"), 0.3), MagnifyProbabilities(GetFishes(waterDepth, who, "Town"), 0.7));
                case 3:
                    return MagnifyProbabilities(GetFishes(waterDepth, who, "Forest"), 0.5);
                case 2:
                    {
                        double p = 0.05 + dailyLuck;
                        return ConcatDictionary(
                            new Dictionary<int, double> { { 734, p } },
                            MagnifyProbabilities(
                                GetFishes(waterDepth, who, "Forest"),
                                (1 - p) * 0.45)
                            );
                    }
                case 4:
                    {
                        return MagnifyProbabilities(
                            GetFishes(waterDepth, who, "Mountain"),
                            0.35);
                    }
                default:
                    return GetFishes(waterDepth, who);
            }
        }

        private static bool Harvest(int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester = null)
        {
            IReflectionHelper reflection = Helper.Reflection;

            Multiplayer multiplayer = reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            Crop crop = soil.crop;
            if (crop.dead.Value)
            {
                return false;
            }
            if (crop.forageCrop.Value)
            {
                SVObject o = null;
                const int experience2 = 3;
                int num = crop.whichForageCrop.Value;
                if (num == 1)
                {
                    o = new SVObject(399, 1);
                }
                if (player.professions.Contains(16))
                {
                    if (o != null) o.Quality = 4;
                }
                else if (random.NextDouble() < player.ForagingLevel / 30f)
                {
                    if (o != null) o.Quality = 2;
                }
                else if (random.NextDouble() < player.ForagingLevel / 15f)
                {
                    if (o != null) o.Quality = 1;
                }

                if (o == null)
                    return false;

                stats.ItemsForaged += (uint)o.Stack;
                if (junimoHarvester != null)
                {
                    junimoHarvester.tryToAddItemToHut(o);
                    return true;
                }

                if (player.addItemToInventoryBool(o))
                {
                    Vector2 initialTile2 = new Vector2(xTile, yTile);
                    player.animateOnce(279 + player.FacingDirection);
                    player.canMove = false;
                    player.currentLocation.playSound("harvest");
                    DelayedAction.playSoundAfterDelay("coin", 260);
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        multiplayer.broadcastSprites(currentLocation,
                            new TemporaryAnimatedSprite(17, new Vector2(initialTile2.X * 64f, initialTile2.Y * 64f),
                                Color.White, 7, random.NextDouble() < 0.5, 125f));
                        multiplayer.broadcastSprites(currentLocation,
                            new TemporaryAnimatedSprite(14, new Vector2(initialTile2.X * 64f, initialTile2.Y * 64f),
                                Color.White, 7, random.NextDouble() < 0.5, 50f));
                    }

                    player.gainExperience(2, experience2);
                    return true;
                }
            }
            else if (crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0))
            {
                int numToHarvest = 1;
                int cropQuality = 0;
                int fertilizerQualityLevel = 0;
                if (crop.indexOfHarvest.Value == 0)
                {
                    return true;
                }
                Random r = new Random(xTile * 7 + yTile * 11 + (int)stats.DaysPlayed + (int)uniqueIDForThisGame);

                switch (soil.fertilizer.Value)
                {
                    case 368:
                        fertilizerQualityLevel = 1;
                        break;
                    case 369:
                        fertilizerQualityLevel = 2;
                        break;
                }

                double chanceForGoldQuality = 0.2 * (player.FarmingLevel / 10.0) + 0.2 * fertilizerQualityLevel * ((player.FarmingLevel + 2.0) / 12.0) + 0.01;
                double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);
                if (r.NextDouble() < chanceForGoldQuality)
                {
                    cropQuality = 2;
                }
                else if (r.NextDouble() < chanceForSilverQuality)
                {
                    cropQuality = 1;
                }
                if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1)
                {
                    numToHarvest = r.Next(crop.minHarvest.Value, Math.Min(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value));
                }
                if (crop.chanceForExtraCrops.Value > 0.0)
                {
                    while (r.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value))
                    {
                        numToHarvest++;
                    }
                }
                if (crop.harvestMethod.Value == 1)
                {
                    for (int j = 0; j < numToHarvest; j++)
                    {
                        createObjectDebris(crop.indexOfHarvest.Value, xTile, yTile, -1, cropQuality);
                    }
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        return true;
                    }
                    crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                    crop.fullyGrown.Value = true;
                }
                else if (player.addItemToInventoryBool(crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                {
                    Quality = cropQuality
                }
                : new SVObject(crop.indexOfHarvest.Value, 1, false, -1, cropQuality)))
                {
                    Vector2 initialTile = new Vector2(xTile, yTile);
                    if (junimoHarvester == null)
                    {
                        player.animateOnce(279 + player.FacingDirection);
                        player.canMove = false;
                    }
                    else
                    {
                        junimoHarvester.tryToAddItemToHut(crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                        {
                            Quality = cropQuality
                        } : new SVObject(crop.indexOfHarvest.Value, 1, false, -1, cropQuality));
                    }
                    if (r.NextDouble() < player.LuckLevel / 1500f + dailyLuck / 1200.0 + 9.9999997473787516E-05)
                    {
                        numToHarvest *= 2;
                        if (junimoHarvester == null)
                        {
                            player.currentLocation.playSound("dwoop");
                        }
                        else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
                        {
                            junimoHarvester.currentLocation.playSound("dwoop");
                        }
                    }
                    else if (crop.harvestMethod.Value == 0)
                    {
                        if (junimoHarvester == null)
                        {
                            player.currentLocation.playSound("harvest");
                        }
                        if (junimoHarvester == null)
                        {
                            DelayedAction.playSoundAfterDelay("coin", 260, player.currentLocation);
                        }
                        else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
                        {
                            DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
                        }
                        if (crop.regrowAfterHarvest.Value == -1)
                        {
                            multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(17, new Vector2(initialTile.X * 64f, initialTile.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 125f));
                            multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(14, new Vector2(initialTile.X * 64f, initialTile.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 50f));
                        }
                    }
                    if (crop.indexOfHarvest.Value == 421)
                    {
                        crop.indexOfHarvest.Value = 431;
                        numToHarvest = r.Next(1, 4);
                    }
                    for (int i = 0; i < numToHarvest - 1; i++)
                    {
                        createObjectDebris(crop.indexOfHarvest.Value, xTile, yTile);
                    }
                    int price = Convert.ToInt32(objectInformation[crop.indexOfHarvest.Value].Split('/')[1]);
                    float experience = (float)(16.0 * Math.Log(0.018 * price + 1.0, 2.7182818284590451));
                    if (junimoHarvester == null)
                    {
                        player.gainExperience(0, (int)Math.Round(experience));
                    }
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        return true;
                    }
                    crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                    crop.fullyGrown.Value = true;
                }
            }
            return false;
        }

        private static Vector2 GetCropLocation(Crop crop)
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> kv in currentLocation.terrainFeatures.Pairs)
            {
                if (!(kv.Value is HoeDirt dirt))
                    continue;

                if (dirt.crop != null && !dirt.crop.dead.Value && dirt.crop == crop)
                {
                    return kv.Key;
                }
            }
            return new Vector2(-1, -1);
        }

        /// <summary>
        /// Is the dirt's crop is a flower and producing nectar
        /// </summary>
        /// <param name="location">HoeDirt location to evaluate</param>
        /// <returns>Result</returns>
        private static bool IsProducingNectar(Vector2 location) => FlowerLocationProducingNectar.Contains(location);

        private static bool IsBlackListed(Crop crop)
        {
            int index = crop.forageCrop.Value ? crop.whichForageCrop.Value : crop.indexOfHarvest.Value;
            return ModEntry.Conf.HarvestException.Contains(index);
        }

        private static bool ToggleBlackList(Crop crop)
        {
            int index = crop.forageCrop.Value ? crop.whichForageCrop.Value : crop.indexOfHarvest.Value;
            if (IsBlackListed(crop))
                ModEntry.Conf.HarvestException.Remove(index);
            else
                ModEntry.Conf.HarvestException.Add(index);

            ModInstance.WriteConfig();
            return IsBlackListed(crop);
        }

        private static bool IsObjectMachine(SVObject obj)
        {
            if (obj is CrabPot)
                return true;

            if (!obj.bigCraftable.Value)
                return false;

            switch (obj.Name)
            {
                case "Incubator":
                case "Slime Incubator":
                case "Keg":
                case "Preserves Jar":
                case "Cheese Press":
                case "Mayonnaise Machine":
                case "Loom":
                case "Oil Maker":
                case "Seed Maker":
                case "Crystalarium":
                case "Recycling Machine":
                case "Furnace":
                case "Charcoal Kiln":
                case "Slime Egg-Press":
                case "Cask":
                case "Bee House":
                case "Mushroom Box":
                case "Statue Of Endless Fortune":
                case "Statue Of Perfection":
                case "Tapper":
                    return true;
                default: return false;
            }
        }

        private static bool? IsUpsideDown(Fence fence)
        {
            int num2 = 0;
            Vector2 tileLocation = fence.TileLocation;
            int whichType = fence.whichType.Value;
            tileLocation.X += 1f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 100;
            }
            tileLocation.X -= 2f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 10;
            }
            tileLocation.X += 1f;
            tileLocation.Y += 1f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 500;
            }
            tileLocation.Y -= 2f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 1000;
            }

            if (fence.isGate.Value)
            {
                switch (num2)
                {
                    case 110:
                        return true;
                    case 1500:
                        return false;
                    default:
                        return null;
                }
            }
            return null;
        }

        private static bool IsPlayerInClose(Player player, Fence fence, Vector2 fenceLocation, bool? isUpDown)
        {
            if (isUpDown == null)
            {
                return fence.getBoundingBox(fence.TileLocation).Intersects(player.GetBoundingBox());
            }
            Vector2 playerTileLocation = player.getTileLocation();
            if (playerTileLocation == fenceLocation)
            {
                return true;
            }
            if (!IsPlayerFaceOrBackToFence(isUpDown == true, player))
            {
                return false;
            }
            return isUpDown.Value ? ExpandSpecific(fence.getBoundingBox(fenceLocation), 0, 16).Intersects(player.GetBoundingBox()) : ExpandSpecific(fence.getBoundingBox(fenceLocation), 16, 0).Intersects(player.GetBoundingBox());
        }

        private static Rectangle ExpandSpecific(Rectangle rect, int deltaX, int deltaY)
        {
            return new Rectangle(rect.X - deltaX, rect.Y - deltaY, rect.Width + deltaX * 2, rect.Height + deltaY * 2);
        }

        private static bool IsPlayerFaceOrBackToFence(bool isUpDown, Player player)
        {
            return isUpDown ? player.FacingDirection % 2 == 0 : player.FacingDirection % 2 == 1;
        }

        private static string GetKeyForQuality(int fishQuality)
        {
            switch (fishQuality)
            {
                case 1: return "quality.silver";
                case 2: return "quality.gold";
                case 3: return "quality.iridium";
                default: return "quality.normal";
            }
        }

        private static Color GetColorForQuality(int fishQuality)
        {
            switch (fishQuality)
            {
                case 1: return Color.AliceBlue;
                case 2: return Color.Tomato;
                case 3: return Color.Purple;
            }
            return Color.WhiteSmoke;
        }

        private static void DrawString(SpriteBatch batch, SpriteFont font, ref Vector2 location, string text, Color color, float scale, bool next = false)
        {
            Vector2 stringSize = font.MeasureString(text) * scale;
            batch.DrawString(font, text, location, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (next)
            {
                location += new Vector2(stringSize.X, 0);
            }
            else
            {
                location += new Vector2(0, stringSize.Y + 4);
            }
        }

        private static int GetFinalSize(int inch)
        {
            return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? inch : (int)Math.Round(inch * 2.54);
        }

        private static string TryFormat(string str, params object[] args)
        {
            try
            {
                string ret = Format(str, args);
                return ret;
            }
            catch
            {
                // ignored
            }

            return "";
        }

        #endregion
        
    }
}
