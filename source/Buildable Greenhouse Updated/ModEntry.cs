/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley.Buildings;
using SpaceShared.APIs;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace BuildableGreenhouse
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        private ModConfig Config;
        private GraphicsDevice graphicsDevice;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            this.Config = helper.ReadConfig<ModConfig>();
            ModPatch.Initialize(this.Helper, this.Monitor);

            this.graphicsDevice = Game1.graphics.GraphicsDevice;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Player.Warped += this.OnWarped;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(GreenhouseBuilding), nameof(GreenhouseBuilding.drawInMenu)),
               prefix: new HarmonyMethod(typeof(ModPatch), nameof(ModPatch.drawInMenu_Prefix))
            );
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
            if(e.NewMenu is CarpenterMenu menu && Game1.getFarm().greenhouseUnlocked.Value)
            {
                var blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints").GetValue();
                blueprints.Add(this.GetBlueprint());
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
                    if (b.buildingType.Value == "BuildableGreenhouse" && b is not BuildableGreenhouseBuilding)
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
                }
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Blueprints");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data.Add("BuildableGreenhouse", $"{BuildMaterialsToString()}/7/6/3/5/-1/-1/BuildableGreenhouse/{I18n.BuildableGreenhouse_Name()}/{I18n.BuildableGreenhouse_Description()}/Buildings/none/64/96/20/null/Farm/{Config.BuildPrice}/false");
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\BuildableGreenhouse") || asset.AssetNameEquals("Maps\\BuildableGreenhouse"))
                return true;
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\BuildableGreenhouse"))
            {
                Texture2D greenhouseTexture = Game1.content.Load<Texture2D>("Buildings\\Greenhouse");
                Rectangle newBounds = greenhouseTexture.Bounds;
                newBounds.Y += 160;
                newBounds.Width -= 128;
                newBounds.Height -= 160;

                Texture2D greenhouse = new Texture2D(graphicsDevice, newBounds.Width, newBounds.Height);
                Color[] data = new Color[newBounds.Width * newBounds.Height];
                greenhouseTexture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
                greenhouse.SetData(data);
                return (T)(object)greenhouse;
            }
            else if (asset.AssetNameEquals("Maps\\BuildableGreenhouse"))
                return (T)(object)Game1.content.Load<T>("Maps\\Greenhouse");
            return (T)(object)null;
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