using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using System.Collections.Generic;

namespace DeepWoodsMod
{
    class EasterEggFunctions
    {
        private enum ProcessMethod
        {
            Remove,
            Restore
        }

        private static void ProcessItemList(IList<Item> items, ProcessMethod method)
        {
            for (int index = items.Count - 1; index >= 0; --index)
            {
                items[index] = ProcessSingleItem(items[index], method);
            }
        }

        private static Item ProcessSingleItem(Item item, ProcessMethod method)
        {
            if (item is Chest chest)
            {
                ProcessItemList(chest.items, method);
            }
            else if (item is StardewValley.Object @object
                && @object.heldObject.Value is Chest chest2)
            {
                ProcessItemList(chest2.items, method);
            }

            if (method == ProcessMethod.Remove && item is EasterEggItem easterEggItem)
            {
                return new StardewValley.Object(EASTER_EGG_REPLACEMENT_ITEM, easterEggItem.Stack) { name = UNIQUE_NAME_FOR_EASTER_EGG_ITEMS };
            }
            else if (method == ProcessMethod.Restore
                && item is StardewValley.Object @object
                && @object.parentSheetIndex == EASTER_EGG_REPLACEMENT_ITEM
                && @object.name == UNIQUE_NAME_FOR_EASTER_EGG_ITEMS)
            {
                return new EasterEggItem() { Stack = @object.Stack };
            }

            return item;
        }

        private static void ProcessLocation(GameLocation location, ProcessMethod method)
        {
            if (location == null)
                return;

            if (location is BuildableGameLocation buildableGameLocation)
            {
                foreach (Building building in buildableGameLocation.buildings)
                {
                    ProcessLocation(building.indoors.Value, method);

                    if (building is Mill mill)
                    {
                        ProcessItemList(mill.output.Value.items, method);
                    }
                    else if (building is JunimoHut hut)
                    {
                        ProcessItemList(hut.output.Value.items, method);
                    }
                }
            }

            if (location is DecoratableLocation decoratableLocation)
            {
                foreach (Furniture furniture in decoratableLocation.furniture)
                {
                    furniture.heldObject.Value = (StardewValley.Object)ProcessSingleItem(furniture.heldObject.Value, method);
                }
            }

            if (location is FarmHouse farmHouse)
            {
                ProcessItemList(farmHouse.fridge.Value.items, method);
            }

            foreach (var pair in location.objects.Pairs)
            {
                location.objects[pair.Key] = (StardewValley.Object)ProcessSingleItem(pair.Value, method);
            }
        }

        public static void RemoveAllEasterEggsFromGame()
        {
            foreach (GameLocation location in Game1.locations)
            {
                ProcessLocation(location, ProcessMethod.Remove);
            }

            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                ProcessItemList(farmer.items, ProcessMethod.Remove);
            }
        }

        public static void RestoreAllEasterEggsInGame()
        {
            foreach (GameLocation location in Game1.locations)
            {
                ProcessLocation(location, ProcessMethod.Restore);
            }

            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                ProcessItemList(farmer.items, ProcessMethod.Restore);
            }
        }


        public static void InterceptIncubatorEggs()
        {
            if (!Game1.IsMasterGame)
                return;

            foreach (GameLocation location in Game1.locations)
            {
                if (location is BuildableGameLocation buildableGameLocation)
                {
                    foreach (Building building in buildableGameLocation.buildings)
                    {
                        if (building is Coop coop)
                        {
                            AnimalHouse animalHouse = coop.indoors.Value as AnimalHouse;

                            // Seems that this is dead legacy code, we still keep it to support either way of hatching, in case future game code changes back to this.
                            if (animalHouse.incubatingEgg.Y == EasterEggItem.PARENT_SHEET_INDEX && animalHouse.incubatingEgg.X == 1)
                            {
                                animalHouse.incubatingEgg.X = 0;
                                animalHouse.incubatingEgg.Y = -1;
                                animalHouse.map.GetLayer("Front").Tiles[1, 2].TileIndex = 45;
                                long newId = Game1MultiplayerAccessProvider.GetMultiplayer().getNewID();
                                animalHouse.animals.Add(newId, new FarmAnimal("Rabbit", newId, coop.owner));
                            }
                        }
                    }
                }
            }
        }

        public static void CheckEggHatched(Farmer who, AnimalHouse animalHouse)
        {
            if (who != Game1.player)
                return;

            foreach (StardewValley.Object @object in animalHouse.objects.Values)
            {
                if (@object.bigCraftable && @object.Name.Contains("Incubator") && @object.heldObject.Value != null && @object.heldObject.Value.ParentSheetIndex == EasterEggItem.PARENT_SHEET_INDEX && @object.minutesUntilReady <= 0 && !animalHouse.isFull())
                {
                    @object.heldObject.Value = null;
                    @object.ParentSheetIndex = 101;

                    Game1.exitActiveMenu();
                    if (animalHouse.currentEvent != null)
                    {
                        animalHouse.currentEvent.CurrentCommand = animalHouse.currentEvent.eventCommands.Length - 1;
                        animalHouse.currentEvent = new Event("none/-1000 -1000/farmer 2 9 0/pause 750/end");
                    }

                    Game1.drawDialogueNoTyping(I18N.EasterEggHatchedMessage);
                    Game1.afterDialogues = new Game1.afterFadeFunction(() => {
                        Game1.activeClickableMenu = new NamingMenu(new NamingMenu.doneNamingBehavior((string name) => {
                            AddNewHatchedRabbit(who, animalHouse, name);
                        }), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), null);
                    });
                }
            }
        }

        private static void AddNewHatchedRabbit(Farmer who, AnimalHouse animalHouse, string animalName)
        {
            long animalId = Game1MultiplayerAccessProvider.GetMultiplayer().getNewID();

            FarmAnimal farmAnimal = new FarmAnimal("Rabbit", animalId, who.uniqueMultiplayerID);
            farmAnimal.Name = animalName;
            farmAnimal.displayName = animalName;

            Building building = animalHouse.getBuilding();
            farmAnimal.home = building;
            farmAnimal.homeLocation.Value = new Vector2(building.tileX, building.tileY);
            farmAnimal.setRandomPosition(animalHouse);

            animalHouse.animals.Add(animalId, farmAnimal);
            animalHouse.animalsThatLiveHere.Add(animalId);

            Game1.exitActiveMenu();
        }
    }
}
