using System;
using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;

namespace JoysOfEfficiency.EventHandler
{
    internal class UpdateEvents
    {
        public static bool Paused => EventHolder.Update._paused;

        public static bool DayEnded { get; set; }

        public static int LastTimeOfDay { get; set; }

        private bool _paused;

        private int _ticks;

        private double _timeoutCounter;

        private static Config Conf => InstanceHolder.Config;
        private static IMonitor Monitor => InstanceHolder.Monitor;

        public void OnGameUpdateEvent(object sender, UpdateTickedEventArgs args)
        {
            OnEveryUpdate();
            if (args.IsMultipleOf(8))
            {
                OnGameEighthUpdate();
            }
        }

        public void OnEveryUpdate()
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            if (Conf.PauseWhenIdle)
            {
                if (Util.IsPlayerIdle())
                {
                    _timeoutCounter += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_timeoutCounter > Conf.IdleTimeout * 1000)
                    {
                        if (!_paused)
                        {
                            Monitor.Log("Paused game");
                            _paused = true;
                        }

                        Game1.timeOfDay = LastTimeOfDay;
                    }
                }
                else
                {
                    if (_paused)
                    {
                        _paused = false;
                        Monitor.Log("Resumed game");
                    }

                    _timeoutCounter = 0;
                    LastTimeOfDay = Game1.timeOfDay;
                }
            }
            else
            {
                _paused = false;
            }

            Farmer player = Game1.player;
            if (Conf.AutoGate)
            {
                FenceGateAutomation.TryToggleGate(player);
            }

            if (player.CurrentTool is FishingRod rod)
            {
                FishingProbabilitiesBox.UpdateProbabilities(rod);
            }

            GiftInformationTooltip.UpdateTooltip();
        }

        private void OnGameEighthUpdate()
        {
            if (Game1.currentGameTime == null)
            {
                return;
            }

            if (Conf.CloseTreasureWhenAllLooted && Game1.activeClickableMenu is ItemGrabMenu menu)
            {
                InventoryAutomation.TryCloseItemGrabMenu(menu);
            }

            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }

            Farmer player = Game1.player;
            GameLocation location = Game1.currentLocation;
            try
            {
                if (Conf.AutoReelRod)
                {
                    AutoFisher.AutoReelRod();
                }
                if (Game1.currentLocation is MineShaft shaft)
                {
                    bool isFallingDownShaft = InstanceHolder.Reflection.GetField<bool>(shaft, "isFallingDownShaft").GetValue();
                    if (isFallingDownShaft)
                    {
                        return;
                    }
                }
                if (!Context.CanPlayerMove)
                {
                    return;
                }
                if (Conf.UnifyFlowerColors)
                {
                    FlowerColorUnifier.UnifyFlowerColors();
                }

                _ticks = (_ticks + 1) % 8;
                if (Conf.BalancedMode && _ticks != 0)
                {
                    return;
                }

                if (Conf.AutoEat)
                {
                    FoodAutomation.TryToEatIfNeeded(player);
                }
                if (Conf.AutoPickUpTrash)
                {
                    TrashCanScavenger.ScavengeTrashCan();
                }
                if (Conf.AutoWaterNearbyCrops)
                {
                    HarvestAutomation.WaterNearbyCrops();
                }
                if (Conf.AutoPetNearbyAnimals)
                {
                    AnimalAutomation.PetNearbyAnimals();
                }

                if (Conf.AutoShearingAndMilking)
                {
                    AnimalAutomation.ShearingAndMilking(player);
                }
                if (Conf.AutoPullMachineResult)
                {
                    MachineOperator.PullMachineResult();
                }
                if (Conf.AutoDepositIngredient)
                {
                    MachineOperator.DepositIngredientsToMachines();
                }
                if (Conf.AutoHarvest)
                {
                    HarvestAutomation.HarvestNearbyCrops(player);
                }
                if (Conf.AutoDestroyDeadCrops)
                {
                    HarvestAutomation.DestroyNearDeadCrops(player);
                }
                if (Conf.AutoRefillWateringCan)
                {
                    WateringCanRefiller.RefillWateringCan();
                }
                if (Conf.AutoCollectCollectibles)
                {
                    CollectibleCollector.CollectNearbyCollectibles(location);
                }
                if (Conf.AutoDigArtifactSpot)
                {
                    ArtifactSpotDigger.DigNearbyArtifactSpots();
                }
                if (Conf.AutoShakeFruitedPlants)
                {
                    HarvestAutomation.ShakeNearbyFruitedTree();
                    HarvestAutomation.ShakeNearbyFruitedBush();
                }
                if (Conf.AutoAnimalDoor && !DayEnded && Game1.timeOfDay >= 1900)
                {
                    DayEnded = true;
                    EventHolder.Save.OnBeforeSave(null, null);
                }
                if (Conf.AutoPetNearbyPets)
                {
                    AnimalAutomation.PetNearbyPets();
                }
            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Source);
                Monitor.Log(ex.ToString());
            }
        }
    }
}
