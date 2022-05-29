/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceShared.APIs;
using BuildableGreenhouse.Compatibility;

namespace BuildableGreenhouse
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private GraphicsDevice graphicsDevice;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.graphicsDevice = Game1.graphics.GraphicsDevice;

            ModPatch.Initialize(helper, this.Monitor);
            ModCompatibility.Initialize(helper, this.Monitor, this.ModManifest);

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(GreenhouseBuilding), nameof(GreenhouseBuilding.drawInMenu)),
               prefix: new HarmonyMethod(typeof(ModPatch), nameof(ModPatch.drawInMenu_Prefix))
            );
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data\\Blueprints"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    String[] greenhouse = data["Greenhouse"].Split("/");
                    String buildableGreenhouseName = greenhouse[8];
                    String buildableGreenhouseDescription = greenhouse[9];
                    data.Add("BuildableGreenhouse", $"{BuildMaterialsToString()}/7/6/3/5/-1/-1/Greenhouse/{buildableGreenhouseName}/{buildableGreenhouseDescription}/Buildings/none/64/96/20/null/Farm/{this.Config.BuildPrice}/false");
                });
            }

            if(e.Name.IsEquivalentTo("Buildings\\BuildableGreenhouse"))
            {
                e.LoadFrom(() =>
                {
                    Texture2D greenhouseTexture = this.Helper.GameContent.Load<Texture2D>("Buildings\\Greenhouse");
                    Rectangle newBounds = greenhouseTexture.Bounds;
                    newBounds.Y += 160;
                    newBounds.Width -= 128;
                    newBounds.Height -= 160;

                    Texture2D greenhouse = new Texture2D(graphicsDevice, newBounds.Width, newBounds.Height);
                    Color[] data = new Color[newBounds.Width * newBounds.Height];
                    greenhouseTexture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
                    greenhouse.SetData(data);
                    return greenhouse;
                }, AssetLoadPriority.High);
            }

            if(e.Name.IsEquivalentTo("Maps\\BuildableGreenhouse"))
            {
                e.LoadFrom(() => this.Helper.GameContent.Load<Map>("Maps\\Greenhouse"), AssetLoadPriority.High);
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var spaceCore = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

            Type[] types =
            {
                typeof(BuildableGreenhouseBuilding),
                typeof(BuildableGreenhouseLocation)
            };

            foreach (Type type in types)
                spaceCore.RegisterSerializerType(type);

            ModCompatibility.applyGMCMCompatibility(sender, e);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            ModCompatibility.applyGreenhouseUpgradesCompatibility();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (var loc in Game1.locations)
            {
                if (loc is BuildableGameLocation buildable)
                {
                    foreach (var building in buildable.buildings)
                    {
                        if (building.indoors.Value == null)
                            continue;

                        building.indoors.Value.updateWarps();
                        building.updateInteriorWarps();
                    }
                }
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu menu)
            {
                if(this.Config.StartWithGreenhouse || Game1.getFarm().greenhouseUnlocked.Value)
                {
                    Monitor.Log("Adding Buildable Greenhouse to Carpenter Menu");
                    var blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints").GetValue();
                    blueprints.Add(this.GetBlueprint());
                }
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            BuildableGameLocation farm = e.NewLocation as BuildableGameLocation ?? e.OldLocation as BuildableGameLocation;
            if (farm != null)
            {
                for (int i = 0; i < farm.buildings.Count; ++i)
                {
                    var b = farm.buildings[i];
                    if(b.buildingType.Value == "BuildableGreenhouse")
                    {
                        if(b is not BuildableGreenhouseBuilding)
                        {
                            farm.buildings[i] = new BuildableGreenhouseBuilding();
                            farm.buildings[i].buildingType.Value = b.buildingType.Value;
                            farm.buildings[i].daysOfConstructionLeft.Value = b.daysOfConstructionLeft.Value;
                            farm.buildings[i].tileX.Value = b.tileX.Value;
                            farm.buildings[i].tileY.Value = b.tileY.Value;
                            farm.buildings[i].tilesWide.Value = b.tilesWide.Value;
                            farm.buildings[i].tilesHigh.Value = b.tilesHigh.Value;
                            farm.buildings[i].load();
                        }
                        else
                        {
                            if (!ModCompatibility.Greenhouses.ContainsKey(b.indoors.Value.uniqueName.Value))
                                ModCompatibility.Greenhouses.Add(b.indoors.Value.uniqueName.Value, b);
                        }
                    }
                }
            }
        }

        private BluePrint GetBlueprint()
        {
            return new BluePrint("BuildableGreenhouse");
        }

        private String BuildMaterialsToString()
        {
            string result = "";
            foreach (KeyValuePair<int, int> kvp in this.Config.BuildMaterals)
            {
                result += kvp.Key.ToString() + " " + kvp.Value.ToString() + " ";
            }
            return result.TrimEnd(' ');
        }
    }
}