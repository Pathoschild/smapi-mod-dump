using AnimalHusbandryMod.farmer;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;

namespace AnimalHusbandryMod.animals
{
    public class PregnancyController
    {
        private static IModEvents events => AnimalHusbandryModEntry.ModHelper.Events;
        static Queue<FarmAnimal> parentAnimals = new Queue<FarmAnimal>();
        static FarmAnimal parentAnimal = null;

        static List<string> animalsWithBirthTomorrow = new List<string>();

        public static Boolean IsAnimalPregnant(long farmAnimalId)
        {
            return FarmerLoader.FarmerData.PregnancyData.Exists(f => f.Id == farmAnimalId);
        }

        public static PregnancyItem GetPregnancyItem(long farmAnimalId)
        {
            return FarmerLoader.FarmerData.PregnancyData.Find(f => f.Id == farmAnimalId);
        }

        public static void RemovePregnancyItem(long farmAnimalId)
        {
            FarmerLoader.FarmerData.PregnancyData.RemoveAll(f => f.Id == farmAnimalId);
        }

        public static void AddPregancy(PregnancyItem pregnancyItem)
        {
            FarmerLoader.FarmerData.PregnancyData.Add(pregnancyItem);
        }

        public static IEnumerable<PregnancyItem> AnimalsReadyForBirth()
        {
            return FarmerLoader.FarmerData.PregnancyData.Where(p => p.DaysUntilBirth <= 0);
        }

        public static IEnumerable<PregnancyItem> AnimalsReadyForBirthTomorrow()
        {
            return FarmerLoader.FarmerData.PregnancyData.Where(p => p.DaysUntilBirth == 1);
        }

        public static bool CheckBuildingLimit(FarmAnimal animal)
        {
            return CheckBuildingLimit((AnimalHouse)animal.home.indoors.Value);
        }

        public static bool CheckBuildingLimit(AnimalHouse animalHouse)
        {
            int? limit = null;
            switch (animalHouse.Name)
            {
                case "Deluxe Barn":
                    {
                        limit = DataLoader.AnimalBuildingData.DeluxeBarnPregnancyLimit;
                        break;
                    }
                case "Big Barn":
                    {
                        limit = DataLoader.AnimalBuildingData.BigBarnPregnancyLimit;
                        break;
                    }
                case "Barn":
                    {
                        limit = DataLoader.AnimalBuildingData.BarnPregnancyLimit;
                        break;
                    }
                case "Deluxe Coop":
                    {
                        limit = DataLoader.AnimalBuildingData.DeluxeCoopPregnancyLimit;
                        break;
                    }
                case "Big Coop":
                    {
                        limit = DataLoader.AnimalBuildingData.BigCoopPregnancyLimit;
                        break;
                    }
                case "Coop":
                    {
                        limit = DataLoader.AnimalBuildingData.CoopPregnancyLimit;
                        break;
                    }
                default:
                    {
                        limit = null;
                        break;
                    }
            }
            return limit != null && animalHouse.animalsThatLiveHere.Count(a => IsAnimalPregnant(a)) >= limit;
        }

        public static FarmAnimal GetAnimal(long id)
        {
            FarmAnimal animal = Utility.getAnimal(id);
            if (animal != null)
            {
                return animal;
            }
            else
            {
                AnimalHusbandryModEntry.monitor.Log($"The animal id '{id}' was not found in the game and its pregnancy data is being discarted.", LogLevel.Warn);
                PregnancyController.RemovePregnancyItem(id);
                return null;
            }
                
        }

        public static void UpdatePregnancy()
        {
            FarmerLoader.FarmerData.PregnancyData.ForEach(a => a.DaysUntilBirth--);

            FarmerLoader.SaveData();
        }

        public static void CheckForBirth()
        {
            foreach (PregnancyItem pregancyItem in AnimalsReadyForBirth().ToList())
            {
                FarmAnimal animal = GetAnimal(pregancyItem.Id);
                if (animal != null)
                {
                    parentAnimals.Enqueue(animal);
                }
            }
            if (!DataLoader.ModConfig.DisableTomorrowBirthNotification)
            {
                animalsWithBirthTomorrow = CheckBirthTomorrow();
            }
            if (parentAnimals.Count > 0 || animalsWithBirthTomorrow.Count > 0)
            {
                events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }

        public static List<string> CheckBirthTomorrow()
        {
            List<string> animals = new List<string>();
            foreach (PregnancyItem pregancyItem in AnimalsReadyForBirthTomorrow().ToList())
            {
                FarmAnimal farmAnimal = GetAnimal(pregancyItem.Id);
                if (farmAnimal != null)
                {
                    animals.Add(farmAnimal.displayName);
                }                
            }
            return animals;      
        }
        
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.dialogueUp ||  Game1.fadeToBlackAlpha > 0)
            {
                return;
            }                
            if (!Game1.messagePause)
            {
                Game1.messagePause = true;
            }                
            if (animalsWithBirthTomorrow.Count > 0)
            {
                if (animalsWithBirthTomorrow.Count() > 1)
                {
                    string animalsString = string.Join(", ", animalsWithBirthTomorrow.Take(animalsWithBirthTomorrow.Count() - 1)) + " and " + animalsWithBirthTomorrow.Last();
                    Game1.drawObjectDialogue(DataLoader.i18n.Get("Tool.InseminationSyringe.BirthsTomorrow", new { animalNames = animalsString }));
                }
                else if (animalsWithBirthTomorrow.Count() == 1)
                {
                    Game1.drawObjectDialogue(DataLoader.i18n.Get("Tool.InseminationSyringe.BirthTomorrow", new { animalName = animalsWithBirthTomorrow.FirstOrDefault() }));
                }
                animalsWithBirthTomorrow.Clear();
            }
            else if (parentAnimal == null)
            {
                if (parentAnimals.Count > 0)
                {
                    parentAnimal = parentAnimals.Dequeue();
                    if ((parentAnimal.home.indoors.Value as AnimalHouse).isFull())
                    {
                        if (!DataLoader.ModConfig.DisableFullBuildingForBirthNotification)
                        {
                            Game1.drawObjectDialogue(DataLoader.i18n.Get("Tool.InseminationSyringe.FullBuilding", new { animalName = parentAnimal.displayName, buildingType = parentAnimal.displayHouse }));
                        }
                        parentAnimal = null;
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", (object)parentAnimal.displayName, (object)parentAnimal.shortDisplayType()));
                    }
                }
                else
                {
                    Game1.messagePause = false;
                    events.GameLoop.UpdateTicked -= OnUpdateTicked;
                }
            }
            else if (Game1.activeClickableMenu == null)
            {
                NamingMenu.doneNamingBehavior b = new NamingMenu.doneNamingBehavior(addNewHatchedAnimal);
                string title = Game1.content.LoadString("Strings\\Events:AnimalNamingTitle", (object)parentAnimal.displayType);
                Game1.activeClickableMenu = (IClickableMenu)new NamingMenu(b, title, (string)null);
            }
        }

        private static void addNewHatchedAnimal(string name)
        {
            Building building = parentAnimal.home; 
             FarmAnimal farmAnimal = new FarmAnimal(parentAnimal.type.Value, DataLoader.Helper.Multiplayer.GetNewID(), (long)Game1.player.UniqueMultiplayerID)
            {
                Name = name,
                displayName = name,
                
                home = building,
                
            };
            farmAnimal.parentId.Value = parentAnimal.myID.Value;
            farmAnimal.homeLocation.Value = new Vector2((float) building.tileX.Value, (float) building.tileY.Value);
            farmAnimal.setRandomPosition(farmAnimal.home.indoors.Value);
            (building.indoors.Value as AnimalHouse).animals.Add(farmAnimal.myID.Value, farmAnimal);
            (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(farmAnimal.myID.Value);

            PregnancyItem pregnacyItem = PregnancyController.GetPregnancyItem(parentAnimal.myID.Value);
            parentAnimal.allowReproduction.Value = pregnacyItem.AllowReproductionBeforeInsemination;
            PregnancyController.RemovePregnancyItem(pregnacyItem.Id);
            parentAnimal = null;

            Game1.exitActiveMenu();
        }
    }
}
