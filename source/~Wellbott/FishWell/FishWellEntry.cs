/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace FishWellSpace
{
    public class FishWellEntry : Mod, IAssetLoader, IAssetEditor
    {

        private FishWellConfig Config;
        public FishWell dummyFishWell;
        public BluePrint wellBlueprint;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<FishWellConfig>();
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
            //helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
        }

        /// <summary>
        /// Add the FishWell asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Buildings/Fish Well");
        }
        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>("assets/Fish Well.xnb");
        }

        /// <summary>
        /// Add the blueprints; not directly used in building it, but still needed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Blueprints"))
            {
                return true;
            }

            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Blueprints"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                if (Config.FishWellsWorkSlower)
                {
                    data["Fish Well"] = "390 200 152 5 153 5/3/3/-1/-1/-2/-1/null/" + Helper.Translation.Get("fishWell.name") + "/" + Helper.Translation.Get("fishWell.descriptionSlow") + "/Buildings/none/48/48/10/null/Farm/5000/false/2";
                }
                else
                {
                    data["Fish Well"] = "390 200 152 5 153 5/3/3/-1/-1/-2/-1/null/" + Helper.Translation.Get("fishWell.name") + "/" + Helper.Translation.Get("fishWell.descriptionFast") + "/Buildings/none/48/48/10/null/Farm/5000/false/2";
                } 
            }
        }

        /// <summary>
        ///A little debug and development helper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.End)
            {
                if (Context.IsPlayerFree && Game1.activeClickableMenu == null)
                {
                    var carpenterMenu = new CarpenterMenu();
                    Game1.activeClickableMenu = (IClickableMenu)carpenterMenu;
                }
                else if (Game1.activeClickableMenu is CarpenterMenu)
                {
                    Game1.displayFarmer = true;
                    ((CarpenterMenu)Game1.activeClickableMenu).exitThisMenu();
                }
            }
            else if (e.Button == SButton.Home)
            {
                Game1.warpFarmer("Farm", Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
            }
        }

        /// <summary>
        /// Makes the FishWell buildable.
        /// Actually adds a modified "Fish Pond" blueprint to fool Buildable Locations.
        /// May cause temporary strangeness if the construction is Instant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.NewMenu is CarpenterMenu)
            {
                this.Monitor.Log($"{wellBlueprint.displayName}.", LogLevel.Debug);
                IList<BluePrint> blueprints = this.Helper.Reflection
                    .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                    .GetValue();
                blueprints.Add(fishWellBlueprint());
            }
            //on menu exit, scan for any new fish ponds to convert
            if (e.OldMenu is CarpenterMenu)
            {
                List<Vector2> tilesWithPonds = new List<Vector2>();
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value == "Fish Pond" && building.tilesWide.Value == 3)
                    {
                        tilesWithPonds.Add(new Vector2(building.tileX, building.tileY));
                    }
                }
                tilesWithPonds.ForEach(ConvertPondToWell);
            }
        }

        /// <summary>
        /// Makes the modifed Fish Pond blueprint
        /// </summary>
        /// <returns></returns>
        public BluePrint fishWellBlueprint()
        {
            BluePrint fishPondPrint = new BluePrint("Fish Pond");
            fishPondPrint.tilesHeight = 3;
            fishPondPrint.tilesWidth = 3;
            fishPondPrint.maxOccupants = Config.WellPopulationCap;
            fishPondPrint.sourceRectForMenuView = new Rectangle(0, 0, 48, 48);
            fishPondPrint.displayName = Helper.Translation.Get("fishWell.name");
            if (Config.FishWellsWorkSlower)
                fishPondPrint.description = Helper.Translation.Get("fishWell.descriptionSlow");
            else
                fishPondPrint.description = Helper.Translation.Get("fishWell.descriptionFast");
            if (Config.InstantConstruction)
                fishPondPrint.daysToConstruct = 0;
            return fishPondPrint;
        }

        /// <summary>
        /// Create dummy blueprint, building for the build menu.
        /// Find 3x3 Fish Ponds on the Farm and convert them into Fish Wells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            wellBlueprint = fishWellBlueprint();
            dummyFishWell = new FishWell(new BluePrint("Fish Well"), Vector2.Zero);
            List<Vector2> tilesWithPonds = new List<Vector2>();
            if (Context.IsMainPlayer)
            {
                
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value == "Fish Pond" && building.tilesWide.Value == 3)
                    {
                        tilesWithPonds.Add(new Vector2(building.tileX, building.tileY));
                    }
                }
                tilesWithPonds.ForEach(ConvertPondToWell);
            }
        }

        /// <summary>
        /// Converts all the Fish Wells on the Farm into 3x3 Fish Ponds.
        /// If mod is broken or removed, this prevents any serious damage or loss
        /// The Fish Ponds would look weird ofc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            List<Vector2> tilesWithWells = new List<Vector2>();
            if (Context.IsMainPlayer)
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value == "Fish Well")
                    {
                        tilesWithWells.Add(new Vector2(building.tileX, building.tileY));
                    }
                }
                tilesWithWells.ForEach(ConvertWellToPond);
            }
        }

        /// <summary>
        /// Renders the proper building in CarpenterMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu)
            {
                if (carpenterMenu.CurrentBlueprint.displayName == Helper.Translation.Get("fishWell.name"))
                {
                    Building building = this.Helper.Reflection.GetField<Building>(carpenterMenu, "currentBuilding").GetValue();
                    Building cachedBuilding = building;
                    Boolean cachedDrawBG = this.Helper.Reflection.GetField<Boolean>(carpenterMenu, "drawBG").GetValue();
                    this.Helper.Reflection.GetField<Boolean>(carpenterMenu, "drawBG").SetValue(false);
                    this.Helper.Reflection.GetField<Building>(carpenterMenu, "currentBuilding").SetValue(dummyFishWell);
                    carpenterMenu.draw(e.SpriteBatch);
                    this.Helper.Reflection.GetField<Boolean>(carpenterMenu, "drawBG").SetValue(cachedDrawBG);
                    this.Helper.Reflection.GetField<Building>(carpenterMenu, "currentBuilding").SetValue(cachedBuilding);
                }
            }
        }

        /// <summary>
        /// Smash Pond, make Well with the same contents and data
        /// </summary>
        /// <param name="pondTile"></param>
        private void ConvertPondToWell(Vector2 pondTile)
        {
            Farm farm = Game1.getLocationFromName("Farm") as Farm;
            FishPond oldPond = (FishPond)farm.getBuildingAt(pondTile);
            FishWell NewWell = new FishWell(new BluePrint("Fish Well"), Vector2.Zero);
            this.Monitor.Log($"Pond -> Well conversion at {pondTile}.", LogLevel.Trace);
            ReplacePondData(oldPond, NewWell);
            NewWell.Config = Config;
            bool destroyed = farm.destroyStructure(oldPond);
            bool built = farm.buildStructure(NewWell, pondTile, Game1.player, true);
            NewWell.performActionOnBuildingPlacement();
            NewWell.ApplyPopulationCap();
            NewWell.resetTexture();
        }

        /// <summary>
        /// Smash Well, make (3x3) Pond with the same contents and data
        /// </summary>
        /// <param name="pondTile"></param>
        private void ConvertWellToPond(Vector2 pondTile)
        {
            Farm farm = Game1.getLocationFromName("Farm") as Farm;
            FishPond oldWell = (FishPond)farm.getBuildingAt(pondTile);
            FishPond NewPond = new FishPond(new BluePrint("Fish Pond"), Vector2.Zero);
            this.Monitor.Log($"Well -> Pond conversion at {pondTile}.", LogLevel.Trace);
            ReplacePondData(oldWell, NewPond);
            NewPond.tilesWide.Value = 3;
            NewPond.tilesHigh.Value = 3;
            bool destroyed = farm.destroyStructure(oldWell);
            bool built = farm.buildStructure(NewPond, pondTile, Game1.player, true);
            NewPond.performActionOnBuildingPlacement();
            NewPond.UpdateMaximumOccupancy();
        }

        /// <summary>
        /// Moves the important bits from one Building to another
        /// </summary>
        /// <param name="fromPond"></param>
        /// <param name="toPond"></param>
        private void ReplacePondData(FishPond fromPond, FishPond toPond)
        {
            toPond.daysOfConstructionLeft.Value = fromPond.daysOfConstructionLeft.Value;
            toPond.daysUntilUpgrade.Value = fromPond.daysUntilUpgrade.Value;
            toPond.owner.Value = fromPond.owner.Value;
            toPond.fishType.Value = fromPond.fishType.Value;
            toPond.currentOccupants.Value = fromPond.currentOccupants.Value;
            toPond.lastUnlockedPopulationGate.Value = fromPond.lastUnlockedPopulationGate.Value;
            toPond.hasCompletedRequest.Value = fromPond.hasCompletedRequest.Value;
            toPond.sign.Value = fromPond.sign.Value;
            toPond.overrideWaterColor.Value = fromPond.overrideWaterColor.Value;
            toPond.output.Value = fromPond.output.Value;
            toPond.neededItemCount.Value = fromPond.neededItemCount.Value;
            toPond.neededItem.Value = fromPond.neededItem.Value;
            toPond.daysSinceSpawn.Value = fromPond.daysSinceSpawn.Value;
            toPond.nettingStyle.Value = fromPond.nettingStyle.Value;
            toPond.seedOffset.Value = fromPond.seedOffset.Value;
            toPond.hasSpawnedFish.Value = fromPond.hasSpawnedFish.Value;
            toPond.maxOccupants.Value = fromPond.maxOccupants;
        }
    }
}