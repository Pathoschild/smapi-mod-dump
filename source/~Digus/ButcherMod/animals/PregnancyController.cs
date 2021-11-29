/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
using HarmonyLib;
using StardewModdingAPI.Utilities;

namespace AnimalHusbandryMod.animals
{
    public class PregnancyController
    {
        private static IModEvents events => AnimalHusbandryModEntry.ModHelper.Events;
        static readonly PerScreen<Queue<FarmAnimal>> parentAnimals = new PerScreen<Queue<FarmAnimal>>(() => new Queue<FarmAnimal>());
        static readonly PerScreen<FarmAnimal> parentAnimal = new PerScreen<FarmAnimal>();

        static List<string> animalsWithBirthTomorrow = new List<string>();

        public static bool IsAnimalPregnant(FarmAnimal farmAnimal)
        {
            return farmAnimal?.GetDaysUntilBirth() != null;
        }

        public static void RemovePregnancy(FarmAnimal farmAnimal)
        {
            farmAnimal.ClearDaysUntilBirth();
            farmAnimal.ClearAllowReproductionBeforeInsemination();
        }

        public static void AddPregnancy(FarmAnimal farmAnimal, int daysUntilBirth)
        {
            farmAnimal.SetDaysUntilBirth(daysUntilBirth);
            farmAnimal.SetAllowReproductionBeforeInsemination(farmAnimal.allowReproduction.Value);
        }

        public static IEnumerable<FarmAnimal> AnimalsReadyForBirth()
        {
            return AnimalUtility.FindAnimals(
                a=> 
                {
                    int? daysUntilBirth = a.GetDaysUntilBirth();
                    return daysUntilBirth.HasValue && daysUntilBirth.Value <= 0;
                }
            );
        }

        public static IEnumerable<FarmAnimal> AnimalsReadyForBirthTomorrow()
        {
            return AnimalUtility.FindAnimals(
                a =>
                {
                    int? daysUntilBirth = a.GetDaysUntilBirth();
                    return daysUntilBirth.HasValue && daysUntilBirth.Value == 1;
                }
            );
        }

        public static bool CheckBuildingLimit(FarmAnimal animal)
        {
            AnimalHouse animalHouse = (AnimalHouse)animal.home.indoors.Value;

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
            return limit != null && animalHouse.animalsThatLiveHere.Count(a => IsAnimalPregnant(Utility.getAnimal(a))) >= limit;
        }

        public static void UpdatePregnancy()
        {
            AnimalUtility
                .FindAnimals(a => a.GetDaysUntilBirth().HasValue)
                .ToList()
                .ForEach(a=> a.SetDaysUntilBirth(a.GetDaysUntilBirth().Value - 1));
        }

        public static void CheckForBirth()
        {
            foreach (FarmAnimal animal in AnimalsReadyForBirth().ToList())
            {
                parentAnimals.Value.Enqueue(animal);
            }
            if (!DataLoader.ModConfig.DisableTomorrowBirthNotification)
            {
                animalsWithBirthTomorrow = CheckBirthTomorrow();
            }
            if (parentAnimals.Value.Count > 0 || animalsWithBirthTomorrow.Count > 0)
            {
                events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }

        public static List<string> CheckBirthTomorrow()
        {
            return AnimalsReadyForBirthTomorrow().ToList().Select(farmAnimal => farmAnimal.displayName).ToList();
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
            else if (parentAnimal.Value == null)
            {
                if (parentAnimals.Value.Count > 0)
                {
                    parentAnimal.Value = parentAnimals.Value.Dequeue();
                    if ((parentAnimal.Value.home.indoors.Value as AnimalHouse).isFull())
                    {
                        if (!DataLoader.ModConfig.DisableFullBuildingForBirthNotification)
                        {
                            Game1.drawObjectDialogue(DataLoader.i18n.Get("Tool.InseminationSyringe.FullBuilding", new { animalName = parentAnimal.Value.displayName, buildingType = parentAnimal.Value.displayHouse }));
                        }
                        parentAnimal.Value = null;
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", (object)parentAnimal.Value.displayName, (object)parentAnimal.Value.shortDisplayType()));
                    }
                }
                else
                {
                    Game1.messagePause = false;
                    if (parentAnimals.GetActiveValues().All(q => q.Value.Count == 0))
                    {
                        events.GameLoop.UpdateTicked -= OnUpdateTicked;
                    }
                }
            }
            else if (Game1.activeClickableMenu == null)
            {
                if (parentAnimal.Value.ownerID.Value == Game1.player.UniqueMultiplayerID || (AnimalHusbandryModEntry.ModHelper.Multiplayer.GetConnectedPlayer(parentAnimal.Value.ownerID.Value) == null && Context.IsMainPlayer))
                {
                    NamingMenu.doneNamingBehavior b = new NamingMenu.doneNamingBehavior(addNewHatchedAnimal);
                    string title = Game1.content.LoadString("Strings\\Events:AnimalNamingTitle", (object)parentAnimal.Value.displayType);
                    Game1.activeClickableMenu = (IClickableMenu)new NamingMenu(b, title, (string)null);
                }
                else
                {
                    parentAnimal.Value = null;
                }
            }
        }

        private static void addNewHatchedAnimal(string name)
        {
            try
            {
                Building building = parentAnimal.Value.home;
                FarmAnimal farmAnimal = new FarmAnimal(parentAnimal.Value.type.Value,
                    DataLoader.Helper.Multiplayer.GetNewID(),
                    parentAnimal.Value.ownerID.Value != 0
                        ? parentAnimal.Value.ownerID.Value
                        : (long) Game1.player.UniqueMultiplayerID)
                {
                    Name = name,
                    displayName = name,
                    home = building
                };
                farmAnimal.parentId.Value = parentAnimal.Value.myID.Value;
                farmAnimal.homeLocation.Value = new Vector2((float) building.tileX.Value, (float) building.tileY.Value);
                farmAnimal.setRandomPosition(farmAnimal.home.indoors.Value);
                AnimalHouse animalHouse = (building.indoors.Value as AnimalHouse);
                animalHouse?.animals.Add(farmAnimal.myID.Value, farmAnimal);
                animalHouse?.animalsThatLiveHere.Add(farmAnimal.myID.Value);

                bool? allowReproductionBeforeInsemination = parentAnimal.Value.GetAllowReproductionBeforeInsemination();
                parentAnimal.Value.allowReproduction.Value = allowReproductionBeforeInsemination ?? parentAnimal.Value.allowReproduction.Value;
                RemovePregnancy(parentAnimal.Value);
            }
            catch (Exception e)
            {
                AnimalHusbandryModEntry.monitor.Log($"Error while adding born baby '{name}'. The birth will be skipped.",LogLevel.Error);
                AnimalHusbandryModEntry.monitor.Log($"Message from birth error above: {e.Message}");
            }
            finally
            {
                parentAnimal.Value = null;
                Game1.exitActiveMenu();
            }

        }
    }
}
