using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Tools;

namespace JoysOfEfficiency.Automation
{
    internal class AnimalAutomation
    {
        private static IMonitor Monitor => InstanceHolder.Monitor;
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;
        private static Config Config => InstanceHolder.Config;

        public static void LetAnimalsInHome()
        {
            Farm farm = Game1.getFarm();
            foreach (KeyValuePair<long, FarmAnimal> kv in farm.animals.Pairs.ToArray())
            {
                FarmAnimal animal = kv.Value;
                Monitor.Log($"Warped {animal.displayName}({animal.shortDisplayType()}) to {animal.displayHouse}@[{animal.homeLocation.X}, {animal.homeLocation.Y}]");
                animal.warpHome(farm, animal);
            }
        }

        public static void AutoOpenAnimalDoor()
        {
            if (Game1.isRaining || Game1.isSnowing)
            {
                Monitor.Log("Don't open the animal door because of rainy/snowy weather.");
                return;
            }
            if (Game1.IsWinter)
            {
                Monitor.Log("Don't open the animal door because it's winter");
                return;
            }
            Farm farm = Game1.getFarm();
            foreach (Building building in farm.buildings)
            {
                switch (building)
                {
                    case Coop coop:
                    {
                        if (coop.indoors.Value is AnimalHouse house)
                        {
                            if (house.animals.Any() && !coop.animalDoorOpen.Value)
                            {
                                Monitor.Log($"Opening coop door @[{coop.animalDoor.X},{coop.animalDoor.Y}]");
                                coop.animalDoorOpen.Value = true;
                                Reflection.GetField<NetInt>(coop, "animalDoorMotion").SetValue(new NetInt(-2));
                            }
                        }
                        break;
                    }
                    case Barn barn:
                    {
                        if (barn.indoors.Value is AnimalHouse house)
                        {
                            if (house.animals.Any() && !barn.animalDoorOpen.Value)
                            {
                                Monitor.Log($"Opening barn door @[{barn.animalDoor.X},{barn.animalDoor.Y}]");
                                barn.animalDoorOpen.Value = true;
                                Reflection.GetField<NetInt>(barn, "animalDoorMotion").SetValue(new NetInt(-3));
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static void AutoCloseAnimalDoor()
        {
            Farm farm = Game1.getFarm();
            foreach (Building building in farm.buildings)
            {
                switch (building)
                {
                    case Coop coop:
                    {
                        if (coop.indoors.Value is AnimalHouse house)
                        {
                            if (house.animals.Any() && coop.animalDoorOpen.Value)
                            {
                                coop.animalDoorOpen.Value = false;
                                Reflection.GetField<NetInt>(coop, "animalDoorMotion").SetValue(new NetInt(2));
                            }
                        }
                        break;
                    }
                    case Barn barn:
                    {
                        if (barn.indoors.Value is AnimalHouse house)
                        {
                            if (house.animals.Any() && barn.animalDoorOpen.Value)
                            {
                                barn.animalDoorOpen.Value = false;
                                Reflection.GetField<NetInt>(barn, "animalDoorMotion").SetValue(new NetInt(2));
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static void PetNearbyPets()
        {
            GameLocation location = Game1.currentLocation;
            Farmer player = Game1.player;

            Rectangle bb = Util.Expand(player.GetBoundingBox(), Config.AutoPetRadius * Game1.tileSize);

            foreach (Pet pet in location.characters.OfType<Pet>().Where(pet => pet.GetBoundingBox().Intersects(bb)))
            {
                bool wasPet = Reflection.GetField<bool>(pet, "wasPetToday").GetValue();
                if (!wasPet)
                {
                    pet.checkAction(player, location); // Pet pet... lol
                }
            }
        }

        public static void PetNearbyAnimals()
        {
            int radius = Config.AutoPetRadius * Game1.tileSize;
            Rectangle bb = Util.Expand(Game1.player.GetBoundingBox(), radius);
            foreach (FarmAnimal animal in Util.GetAnimalsList(Game1.player))
            {
                if (!bb.Contains((int) animal.Position.X, (int) animal.Position.Y) || animal.wasPet.Value)
                {
                    continue;
                }

                if (Game1.timeOfDay >= 1900 && !animal.isMoving())
                {
                    continue;
                }
                animal.pet(Game1.player);
            }
        }

        public static void ShearingAndMilking(Farmer player)
        {
            int radius = InstanceHolder.Config.AnimalHarvestRadius * Game1.tileSize;
            Rectangle bb = Util.Expand(player.GetBoundingBox(), radius);
            foreach (FarmAnimal animal in Util.GetAnimalsList(player))
            {
                string lowerType = animal.type.Value.ToLower();
                if (animal.currentProduce.Value < 0 || animal.age.Value < animal.ageWhenMature.Value || player.CurrentTool == null || !animal.GetBoundingBox().Intersects(bb))
                {
                    continue;
                }

                if ((!lowerType.Contains("sheep") || !(player.CurrentTool is Shears) || !(player.Stamina >= 4f)) &&
                    (!lowerType.Contains("cow") || !(player.CurrentTool is MilkPail) || !(player.Stamina >= 4f)) &&
                    (!lowerType.Contains("goat") || !(player.CurrentTool is MilkPail) || !(player.Stamina >= 4f)))
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
                Game1.playSound("coin");
                animal.currentProduce.Value = -1;
                if (animal.showDifferentTextureWhenReadyForHarvest.Value)
                {
                    animal.Sprite.LoadTexture("Animals\\Sheared" + animal.type.Value);
                }
                player.gainExperience(0, 5);
            }
        }
        
    }
}
