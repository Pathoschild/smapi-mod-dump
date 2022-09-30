/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    internal static class IanShop
    {
        const int waterPlantsPriceSmall = 1000;
        const int waterPlantsPriceMedium = 2500;
        const int waterPlantsPriceLarge = 5000;
        const int wpsmall = 120;
        const int wpmedium = 480;
        const int wplarge = 960;
        const int daysWillWater = 3;
        const int daysWillPet = 7;
        const int perFencePrice = 6;
        const int perAnimalPrice = 60;

        const string willWaterPlants = "IanShop.WaterPlants";
        const string willFixFences = "IanShop.fixFences";
        const string willPetAnimals = "IanShop.PetAnimals";

        private static bool canRenovate = false;

        static IModHelper Helper;
        static IMonitor Monitor;
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            TileActionHandler.RegisterTileAction("IanCounter", OpenIanMenu);
        }

        [EventPriority(EventPriority.High)]
        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            canRenovate = Game1.MasterPlayer.eventsSeen.Contains(RSVConstants.E_SUMMITUNLOCK);

            if (Game1.IsMasterGame)
            {
                // Farming services
                var FarmModData = Game1.getFarm().modData;

                if (FarmModData.ContainsKey(willFixFences))
                {
                    FixTheFences();
                    Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("IanShop.HasFixedFences"), HUDMessage.newQuest_type));
                    FarmModData.Remove(willFixFences);
                }

                if (FarmModData.ContainsKey(willPetAnimals))
                {
                    if (!FarmModData.TryGetValue(willPetAnimals, out string value) || !int.TryParse(value, out int daysLeft) || daysLeft <= 0)
                    {
                        FarmModData.Remove(willPetAnimals);
                        return;
                    }
                    else if (daysLeft == 1)
                    {
                        FarmModData.Remove(willPetAnimals);
                    }
                    else
                    {
                        FarmModData[willPetAnimals] = (daysLeft - 1).ToString();
                    }
                    petAnimals();
                    Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("IanShop.HasPetAnimals"), HUDMessage.newQuest_type));
                }
                Helper.Events.GameLoop.OneSecondUpdateTicked += waterPlantsIfNeeded;

                // Construction services
                if (Game1.player.activeDialogueEvents.TryGetValue(RSVConstants.CT_HOUSEUPGRADE, out int housect) && housect == 0)
                {
                    Game1.player.mailReceived.Add(RSVConstants.M_HOUSEUPGRADED);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_HOUSEUPGRADE);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_ACTIVECONSTRUCTION);
                }
                if (Game1.player.activeDialogueEvents.TryGetValue(RSVConstants.CT_CLIMATE, out int climatect) && climatect == 0)
                {
                    Game1.getLocationFromName(RSVConstants.L_SUMMITFARM).isGreenhouse.Value = true;
                    Game1.player.mailReceived.Add(RSVConstants.M_CLIMATECONTROLLED);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_CLIMATE);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_ACTIVECONSTRUCTION);
                }
                if (Game1.player.activeDialogueEvents.TryGetValue(RSVConstants.CT_SPRINKLER, out int sprinklerct) && sprinklerct == 0)
                {
                    Game1.player.mailReceived.Add(RSVConstants.M_GOTSPRINKLERS);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_SPRINKLER);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_ACTIVECONSTRUCTION);
                }
                if (Game1.player.activeDialogueEvents.TryGetValue(RSVConstants.CT_OREAREA, out int orect) && orect == 0)
                {
                    Game1.player.mailReceived.Add(RSVConstants.M_OREAREAOPENED);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_OREAREA);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_ACTIVECONSTRUCTION);
                }
                if (Game1.player.activeDialogueEvents.TryGetValue(RSVConstants.CT_SHED, out int shedct) && shedct == 0)
                {
                    Game1.player.mailReceived.Add(RSVConstants.M_SHEDADDED);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_SHED);
                    Game1.player.activeDialogueEvents.Remove(RSVConstants.CT_ACTIVECONSTRUCTION);
                }

                if (Game1.player.mailReceived.Contains(RSVConstants.M_GOTSPRINKLERS))
                {
                    WaterThePlants(Game1.getLocationFromName(RSVConstants.L_SUMMITFARM), 9999);
                }
            }
        }

        private static void petAnimals()
        {
            var FarmAnimals = Game1.getFarm().getAllFarmAnimals();
            foreach(var farmAnimal in FarmAnimals)
            {
                farmAnimal.pet(Game1.player, is_auto_pet: false);
            }
        }

        //Will water plants if player has flag
        //format is daysLeft/Size
        private static void waterPlantsIfNeeded(object sender, OneSecondUpdateTickedEventArgs e)
        {

            Helper.Events.GameLoop.OneSecondUpdateTicked -= waterPlantsIfNeeded;
            var farmModData = Game1.getFarm().modData;
            if (Game1.IsRainingHere(Game1.getFarm()))
            {
                return;
            }
            if (farmModData.TryGetValue(willWaterPlants, out string value))
            {
                var valueSplit = value.Split('/');
                if (valueSplit.Length != 2)
                {
                    return;
                }

                int wateringDaysLeft = int.Parse(valueSplit[0]);
                int numberOfTiles = int.Parse(valueSplit[1]);

                if(wateringDaysLeft <= 0)
                {
                    //shouldnt happen
                    farmModData.Remove(willWaterPlants);
                    return;
                }
                else if(wateringDaysLeft == 0)
                {
                    //last day. remove flag
                    Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("IanShop.Deadline"), HUDMessage.newQuest_type));
                    farmModData.Remove(willWaterPlants);
                }
                else
                {
                    wateringDaysLeft--;
                    farmModData[willWaterPlants] = $"{wateringDaysLeft}/{numberOfTiles}";

                }
                WaterThePlants(Game1.getFarm(), numberOfTiles);
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("IanShop.HasWatered"), HUDMessage.newQuest_type));
            }
        }

        private static void OpenIanMenu(string tileActionString, Vector2 position)
        {
            OpenIanMenu(tileActionString);
        }
        private static void OpenIanMenu(string tileActionString = "")
        {
            bool isSomeoneHere = UtilFunctions.IsSomeoneHere(8, 13, 2, 2);
            if (isSomeoneHere)
            {
                IanCounterMenu();
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.Closed"));
            }
            
            
        }
        private static void IanCounterMenu()
        {
            var responses = new List<Response>
            {
                new Response("waterPlants", Helper.Translation.Get("IanShop.WaterPlants")),
                new Response("fixFences", Helper.Translation.Get("IanShop.fixFences")),
                new Response("willPet", Helper.Translation.Get("IanShop.PetAnimals")),
                new Response("cancel", Helper.Translation.Get("IanShop.Cancel"))
            };
            var responseActions = new List<Action>
            {
                delegate
                {
                    WaterPlantsMenu();
                },
                delegate
                {
                    FixFencesMenu();
                },
                delegate
                {
                    PetAnimalsMenu();
                },
                delegate
                {
                    Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.Bye"));
                }
            };

            if (canRenovate)
            {
                responses.Insert(3, new Response("renovate", Helper.Translation.Get("IanShop.SummitFarm")));
                responseActions.Insert(3, delegate { RenovateOptions(); });
            }
            Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("IanShop.Open"), responses, responseActions);
        }

        private static void PetAnimalsMenu()
        {
            int n = Game1.getFarm().getAllFarmAnimals().Count();

            if (!Game1.getFarm().modData.ContainsKey(willPetAnimals) && n > 0)
            {
                var responses = new List<Response>
                {
                    new Response("petAnimals", Helper.Translation.Get("IanShop.PetAnimalsSelection") + (n * perAnimalPrice) + "$"),
                    new Response("cancel", Helper.Translation.Get("IanShop.Cancel"))
                };
                var responseActions = new List<Action>
                {
                    delegate
                    {
                        if(Game1.player.Money >= n * perAnimalPrice)
                        {
                            Game1.player.Money -= (n * perAnimalPrice);
                            Game1.getFarm().modData.Add(willPetAnimals, daysWillPet.ToString());
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.PetThanks"));
                        }
                        else
                        {
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
                        }
                    },
                    delegate
                    {
                        IanCounterMenu();
                    }
                };
                Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("IanShop.PetMenu"), responses, responseActions);
            }
            else if (n <= 0)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.YouHaveNoAnimals"));
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.AlreadyWillPet"));
            }
        }

        private static void WaterPlantsMenu()
        {
            if (!Game1.getFarm().modData.ContainsKey(willWaterPlants))
            {
                var responses = new List<Response>
                {
                    new Response("small", wpsmall + Helper.Translation.Get("IanShop.WaterInfo") + waterPlantsPriceSmall + "$"),
                    new Response("medium", wpmedium + Helper.Translation.Get("IanShop.WaterInfo") + waterPlantsPriceMedium + "$"),
                    new Response("large", wplarge + Helper.Translation.Get("IanShop.WaterInfo") + waterPlantsPriceLarge + "$"),
                    new Response("cancel", Helper.Translation.Get("IanShop.Cancel"))
                };
                var responseActions = new List<Action>
                {
                    delegate
                    {
                        if(Game1.player.Money < waterPlantsPriceSmall)
                        {
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
                        }
                        else
                        {
                            Game1.player.Money -= waterPlantsPriceSmall;
                            Game1.getFarm().modData.Add(willWaterPlants, $"{daysWillWater}/{wpsmall}");
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.Thankyou"));
                        };
                    },
                    delegate
                    {
                        if(Game1.player.Money < waterPlantsPriceMedium)
                        {
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
                        }
                        else
                        {
                            Game1.player.Money -= waterPlantsPriceMedium;
                            Game1.getFarm().modData.Add(willWaterPlants, $"{daysWillWater}/{wpmedium}");
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.Thankyou"));
                        };
                    },
                    delegate
                    {
                        if(Game1.player.Money < waterPlantsPriceLarge)
                        {
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
                        }
                        else
                        {
                            Game1.player.Money -= waterPlantsPriceLarge;
                            Game1.getFarm().modData.Add(willWaterPlants, $"{daysWillWater}/{wplarge}");
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.Thankyou"));
                        };
                    },
                    delegate
                    {
                        IanCounterMenu();
                    }
                };

                Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("IanShop.WaterPlantsMenu"), responses, responseActions);
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.AlreadyWatering"));
            }
        }

        internal static void WaterThePlants(GameLocation location, int maxNumberToWater)
        {
            int n = 0;
            int farm_size = location.terrainFeatures.Pairs.Count();
            foreach (var pair in location.terrainFeatures.Pairs)
            {
                if(n >= maxNumberToWater || n >= farm_size)
                {
                    break;
                }

                if (pair.Value is HoeDirt dirt && dirt.state.Value == 0 && dirt.crop != null)
                {
                    dirt.state.Value = 1;
                    n++;
                }
            }
            
        }
      
        private static void FixFencesMenu()
        {
            int n = Game1.getFarm().Objects.Values.OfType<Fence>().Count();

            if (!Game1.getFarm().modData.ContainsKey(willFixFences) && n > 0)
            {
                var responses = new List<Response>
                {
                    new Response("fixFence", n + Helper.Translation.Get("IanShop.Fences") + (n * perFencePrice) + "$"),
                    new Response("cancel", Helper.Translation.Get("IanShop.Cancel"))
                };
                var responseActions = new List<Action>
                {
                    delegate
                    {
                        if(Game1.player.Money >= n * perFencePrice)
                        {
                            Game1.player.Money -= (n * perFencePrice);
                            Game1.getFarm().modData.Add(willFixFences, "");
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.FenceThanks"));
                        }
                        else
                        {
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
                        }
                    },
                    delegate
                    {
                        IanCounterMenu();
                    }
                };
                Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("IanShop.FenceMenu"), responses, responseActions);
            }
            else if (n <= 0)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.YouHaveNoFences"));
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("IanShop.AlreadyWillFix"));
            }
        }

        private static void FixTheFences()
        {
            foreach (Fence fence in Game1.getFarm().Objects.Values.OfType<Fence>())
            {
                fence.repair();
                fence.health.Value *= 2f;
                fence.maxHealth.Value = fence.health.Value;
                if (fence.isGate.Value)
                    fence.health.Value *= 2f;
            }
        }

        private static void RenovateOptions()
        {
            NPC worker = Game1.isRaining ? Game1.getCharacterFromName("Ian") : Game1.getCharacterFromName("Sean");
            if (!Game1.MasterPlayer.mailReceived.Contains(RSVConstants.M_MINECARTSFIXED))
            {
                worker.CurrentDialogue.Clear();
                worker.CurrentDialogue.Push(new Dialogue(Helper.Translation.Get("IanShop.BrokenCarts"), worker));
                Game1.drawDialogue(worker);
            }
            else if (Game1.MasterPlayer.activeDialogueEvents.TryGetValue(RSVConstants.CT_ACTIVECONSTRUCTION, out int value) && value > 0)
            {
                worker.CurrentDialogue.Clear();
                worker.CurrentDialogue.Push(new Dialogue(Helper.Translation.Get("IanShop.AlreadyBuilding"), worker));
                Game1.drawDialogue(worker);
            }
            else
            {
                Game1.activeClickableMenu = new SummitRenovateMenu();
            }
        }

    }
  
}
