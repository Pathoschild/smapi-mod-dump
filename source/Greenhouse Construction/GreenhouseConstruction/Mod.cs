/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/derslayr10/GreenhouseConstruction
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using GreenhouseConstruction.Custom_Buildings.Greenhouse;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using SpaceShared;
using SpaceShared.APIs;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace GreenhouseConstruction
{
    internal class Mod : StardewModdingAPI.Mod, IAssetEditor, IAssetLoader
    {

        public static StardewModdingAPI.Mod Instance;
        private Texture2D GreenhouseExterior;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;

            this.GreenhouseExterior = this.Helper.Content.Load<Texture2D>("assets/GreenhouseConstruction_Greenhouse.png");

            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Player.Warped += this.onWarped;
            helper.Events.GameLoop.SaveLoaded += this.FixWarps;

        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var sc = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(CustomGreenhouseBuilding));
            sc.RegisterSerializerType(typeof(CustomGreenhouseLocation));
        }

        private void FixWarps(object sender, EventArgs e)
        {

            foreach (var loc in Game1.locations) {

                if (loc is BuildableGameLocation buildable) {

                    foreach (var building in buildable.buildings) {

                        if (building.indoors.Value == null) {

                            continue;

                        }

                        building.indoors.Value.updateWarps();
                        building.updateInteriorWarps();
                    
                    }
                
                }
            
            }

        }

        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer) {

                return;
            
            }

            BuildableGameLocation farm = e.NewLocation as BuildableGameLocation ?? e.OldLocation as BuildableGameLocation;
            if (farm != null) {

                for (int i = 0; i < farm.buildings.Count; ++i) {

                    var b = farm.buildings[i];

                    if (b.buildingType.Value == "GreenhouseConstruction_SpecialGreenhouse" && !(b is CustomGreenhouseBuilding)) {

                        farm.buildings[i] = new CustomGreenhouseBuilding();
                        farm.buildings[i].buildingType.Value = b.buildingType.Value;
                        farm.buildings[i].daysOfConstructionLeft.Value = b.daysOfConstructionLeft.Value;
                        farm.buildings[i].indoors.Value = b.indoors.Value;
                        farm.buildings[i].tileX.Value = b.tileX.Value;
                        farm.buildings[i].tileY.Value = b.tileY.Value;
                        farm.buildings[i].tilesWide.Value = b.tilesWide.Value;
                        farm.buildings[i].tilesHigh.Value = b.tilesHigh.Value;
                        farm.buildings[i].load();
                    
                    }
                
                }
            
            }

        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carp) {

                var blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(carp, "blueprints").GetValue();

                if (Game1.MasterPlayer.mailReceived.Contains("ccPantry")){

                    blueprints.Add(new BluePrint("GreenhouseConstruction_SpecialGreenhouse"));
                
                }
            
            }

        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Blueprints");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Data.Add("GreenhouseConstruction_SpecialGreenhouse", "335 50 709 100 390 300/7/3/3/2/-1/-1/GreenhouseConstruction_SpecialGreenhouse/Special Greenhouse/A place to grow crops year-round./Buildings/none/96/96/20/null/Farm/100000/false");
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\GreenhouseConstruction_SpecialGreenhouse") || asset.AssetNameEquals("Maps\\GreenhouseConstruction_SpecialGreenhouse")) {

                return true;
            
            }

            return false;

        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\GreenhouseConstruction_SpecialGreenhouse"))
            {

                return (T)(object)this.GreenhouseExterior;

            }

            else if (asset.AssetNameEquals("Maps\\GreenhouseConstruction_SpecialGreenhouse"))
            {

                return (T)(object)Game1.content.Load<xTile.Map>("Maps\\Greenhouse");

            }

            return (T)(object)null;

        }

    }
}
