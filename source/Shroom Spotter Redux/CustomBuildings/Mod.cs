/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using CustomBuildings.Overrides;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

namespace CustomBuildings
{
    public class Mod : StardewModdingAPI.Mod, IAssetEditor, IAssetLoader
    {
        public static Mod instance;

        internal Dictionary<string, BuildingData> buildings = new Dictionary<string, BuildingData>();
        
        internal static int ResolveObjectId(object data)
        {
            if (data.GetType() == typeof(long))
                return (int)(long)data;
            else
            {
                foreach (var obj in Game1.objectInformation)
                {
                    if (obj.Value.Split('/')[0] == (string)data)
                        return obj.Key;
                }

                Log.warn($"No idea what '{data}' is!");
                return 0;
            }
        }

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.Events.Display.MenuChanged += onMenuChanged;
            helper.Events.Player.Warped += onWarped;

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(Coop), "getIndoors"), prefix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.getIndoors_Prefix))));
            harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.performActionOnConstruction)), postfix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.performActionOnConstruction_Postfix))));
            harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.performActionOnUpgrade)), prefix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.performActionOnUpgrade_Prefix))));
            harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.dayUpdate)), postfix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.dayUpdate_Postfix))));
            harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.upgrade)), prefix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.upgrade_Prefix))));
            //harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.getUpgradeSignLocation)), prefix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.getUpgradeSignLocation_Postfix))));
            harmony.Patch(AccessTools.Method(typeof(Coop), nameof(Coop.draw)), prefix: new HarmonyMethod(AccessTools.Method(typeof(CoopPatches), nameof(CoopPatches.draw_Prefix))));
            
            Log.debug("Loading content packs...");
            foreach ( var cp in helper.ContentPacks.GetOwned() )
            {
                DirectoryInfo buildingsDir = new DirectoryInfo(Path.Combine(cp.DirectoryPath, "Buildings"));
                foreach ( var dir in buildingsDir.EnumerateDirectories() )
                {
                    string relDir = $"Buildings/{dir.Name}";
                    BuildingData binfo = cp.ReadJsonFile<BuildingData>(Path.Combine(relDir, "building.json"));
                    if (binfo == null)
                        continue;
                    binfo.texture = cp.LoadAsset<Texture2D>(Path.Combine(relDir, "building.png"));
                    binfo.mapLoader = () => cp.LoadAsset<xTile.Map>(Path.Combine(relDir, "building.tbin"));
                    buildings.Add(binfo.Id, binfo);
                }
            }
        }

        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carp)
            {
                var blueprints = Helper.Reflection.GetField<List<BluePrint>>(carp, "blueprints").GetValue();
                foreach ( var building in buildings )
                {
                    if (building.Value.PreviousTier == null || Game1.getFarm().isBuildingConstructed(building.Value.PreviousTier))
                        blueprints.Add(new BluePrint(building.Value.Id));
                }
            }
        }

        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;
            
            BuildableGameLocation farm = e.NewLocation as BuildableGameLocation;
            if (farm == null)
                farm = e.OldLocation as BuildableGameLocation;
            if (farm != null)
            {
                for (int i = 0; i < farm.buildings.Count; ++i)
                {
                    var b = farm.buildings[i];

                    // This is probably a new building if it hasn't been converted yet.
                    if (buildings.ContainsKey(b.buildingType.Value) && !(b is Coop))
                    {
                        farm.buildings[i] = new Coop(new BluePrint(b.buildingType), new Vector2(b.tileX, b.tileY));
                        farm.buildings[i].indoors.Value = b.indoors.Value;
                        farm.buildings[i].load();
                        (farm.buildings[i].indoors.Value as AnimalHouse).animalLimit.Value = buildings[b.buildingType.Value].MaxOccupants;
                    }
                }
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            foreach ( var building in buildings )
            {
                if (asset.AssetNameEquals("Buildings\\" + building.Key) || asset.AssetNameEquals("Maps\\" + building.Key))
                    return true;
            }
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            foreach (var building in buildings)
            {
                if (asset.AssetNameEquals("Buildings\\" + building.Key))
                    return (T) (object) building.Value.texture;
                else if (asset.AssetNameEquals("Maps\\" + building.Key))
                    return (T)(object)building.Value.mapLoader();
            }
            return default(T);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Blueprints");
        }

        public void Edit<T>(IAssetData asset)
        {
            var dict = asset.AsDictionary<string, string>();
            foreach ( var building in buildings)
            {
                dict.Data.Add(building.Value.Id, building.Value.BlueprintString());
            }
        }
    }
}
