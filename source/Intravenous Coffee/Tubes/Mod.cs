using System;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Pathoschild.Stardew.Common;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using StardewValley.Locations;
using StardewValley.Buildings;
using Pathoschild.Stardew.Automate.Framework;
using System.Linq;
using System.Collections;
using StardewValley.Menus;
using PyTK.Extensions;

namespace Tubes
{
    // A do-nothing machine that only serves as a placeholder for Automate. It's used by Tubes to link machine groups together,
    // and for buildings to link buildings to their ContainerBridge.
    public class DummyMachine : IMachine
    {
        public ITrackedStack GetOutput()
        {
            return null;
        }
        public MachineState GetState()
        {
            return MachineState.Empty;
        }
        public bool SetInput(IStorage input)
        {
            return false;
        }
    }

    // A container that links the MachineGroup at a building's interior (adjacent to the exit) to the MachineGroup at the
    // building exterior's entrance. Any machines in the source group will pull recipes from and push output to containers
    // in the target group.
    public class ContainerBridge : IContainer
    {
        // The building interior's exit.
        public Warp warp { get; }
        // The building exterior.
        public GameLocation targetLocation { get; }
        // The MachineGroup at the building's entrance.
        private MachineGroup targetMachineGroup;

        public ContainerBridge(Warp warp)
        {
            this.warp = warp;
            this.targetLocation = CommonHelper.GetLocations().FirstOrDefault((loc) => loc.Name == warp.TargetName);
        }

        public void UpdateLink(MachineGroup[] machineGroups)
        {
            foreach (MachineGroup group in machineGroups) {
                if (group.Tiles.Contains(new Vector2(warp.TargetX, warp.TargetY))) {
                    targetMachineGroup = group;
                    break;
                }
            }
        }

        public string Name { get => "ContainerBridge"; }

        public ITrackedStack Get(Func<Item, bool> predicate, int count)
        {
            return null;
        }

        public void Store(ITrackedStack stack)
        {
            if (targetMachineGroup == null)
                return;
            targetMachineGroup?.StorageManager.TryPush(stack);
        }

        public IEnumerator<ITrackedStack> GetEnumerator()
        {
            if (targetMachineGroup == null)
                return Enumerable.Empty<ITrackedStack>().GetEnumerator();
            return targetMachineGroup.StorageManager.GetItems().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    // Mod entry point.
    public class TubesMod : Mod
    {
        internal static IModHelper _helper;
        internal static IMonitor _monitor;
        internal static IAutomateAPI automateApi;
        internal Dictionary<GameLocation, HashSet<ContainerBridge>> bridges = new Dictionary<GameLocation, HashSet<ContainerBridge>>();

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _monitor = Monitor;

            TubeInfo.init();

            GameEvents.FirstUpdateTick += this.GameEvents_FirstUpdateTick;
            LocationEvents.LocationsChanged += this.LocationEvents_LocationsChanged;
            LocationEvents.LocationObjectsChanged += this.LocationEvents_LocationObjectsChanged;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            automateApi = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            if (automateApi == null) {
                this.Monitor.Log("Could not find Automate mod API. This mod will not work correctly.");
                this.Monitor.Log("Please ensure you have the latest version of Automate installed.");
                return;
            }
            automateApi.RegisterGetMachineHook(this.getMachineHook);
            automateApi.RegisterGetContainerHook(this.getContainerHook);
            automateApi.LocationMachinesChanged += AutomateEvents_LocationMachinesChanged;
        }

        private void LocationEvents_LocationsChanged(object sender, EventArgsGameLocationsChanged e)
        {
            this.bridges.Clear();
        }

        private void LocationEvents_LocationObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
        {
            this.bridges.Remove(Game1.currentLocation);

            // When the player places a TubeObject, it's placed as an object. Gather them and replace them with TubeTerrain
            // instead. This seems to be the only way to place terrain features.
            // We also remove any temporary "junk" objects, which are only used to trigger this event.
            List<Vector2> tubes = new List<Vector2>();
            List<Vector2> junk = new List<Vector2>();
            foreach (var obj in Game1.currentLocation.objects) {
                if (obj.Value.parentSheetIndex == TubeInfo.objectData.sdvId)
                    tubes.Add(obj.Key);
                if (obj.Value.parentSheetIndex == TubeInfo.junkObjectData.sdvId)
                    junk.Add(obj.Key);
            }
            foreach (var pos in tubes) {
                SObject obj = Game1.currentLocation.objects[pos];
                Game1.currentLocation.objects.Remove(pos);

                if (!Game1.currentLocation.terrainFeatures.ContainsKey(pos)) {
                    Game1.currentLocation.terrainFeatures.Add(pos, new TubeTerrain());
                } else {
                    Game1.player.addItemToInventory(obj);
                }
            }
            foreach (var pos in junk) {
                Game1.currentLocation.objects.Remove(pos);
            }

            TubeTerrain.updateSpritesInLocation(Game1.currentLocation);
        }

        private void AutomateEvents_LocationMachinesChanged(object sender, EventArgsLocationMachinesChanged e)
        {
            // Automate is done adding MachineGroups. Time to connect our ContainerBridges to their proper groups.
            foreach (var bridges in this.bridges.Values) {
                foreach (ContainerBridge container in bridges) {
                    if (container.targetLocation == e.Location)
                        container.UpdateLink(e.MachineGroups);
                }
            }
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            // Override the crafting menu so that our recipe has the proper icon and text.
            if (Game1.activeClickableMenu is GameMenu activeMenu && Helper.Reflection.GetField<List<IClickableMenu>>(activeMenu, "pages").GetValue().Find(p => p is CraftingPage) is CraftingPage craftingPage) {
                for (int i = 0; i < craftingPage.pagesOfCraftingRecipes.Count; i++) {
                    if (craftingPage.pagesOfCraftingRecipes[i].Find(k => k.Value.name == TubeInfo.fullid) is KeyValuePair<ClickableTextureComponent, CraftingRecipe> kv && kv.Value != null && kv.Key != null) {
                        kv.Key.texture = TubeInfo.icon;
                        kv.Key.sourceRect = TubeInfo.objectData.sourceRectangle;
                        kv.Key.baseScale = 4.0f;
                        kv.Value.DisplayName = TubeInfo.name;
                        Helper.Reflection.GetField<string>(kv.Value, "description").SetValue(TubeInfo.description);
                    }
                }
            }
        }

        // Called by Automate mod to check if there's a machine at the given tile.
        public IMachine getMachineHook(GameLocation location, Vector2 tile, out Vector2 size)
        {
            size = Vector2.Zero;
            if (!(location is Farm))
                return null;

            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) && feature is TubeTerrain) {
                size = Vector2.One;
                return new DummyMachine();
            }

            if (location is BuildableGameLocation buildableLocation) {
                foreach (Building building in buildableLocation.buildings) {
                    Vector2 doorTile = new Vector2(building.tileX + building.humanDoor.X, building.tileY + building.humanDoor.Y);
                    if (building.indoors != null && tile == doorTile) {
                        size = Vector2.One;
                        return new DummyMachine();
                    }
                }
            }
            return null;
        }

        // Called by Automate mod to check if there's a container at the given tile.
        public IContainer getContainerHook(GameLocation location, Vector2 tile, out Vector2 size)
        {
            foreach (var warp in location.warps) {
                // Manhattan distance.
                if (Math.Abs(tile.X - warp.X) + Math.Abs(tile.Y - warp.Y) <= 1 && location.GetTiles().Contains(tile)) {
                    size = Vector2.One;
                    ContainerBridge bridge = new ContainerBridge(warp);
                    this.Monitor.Log($"Adding container from {location.Name} at {tile} to {warp.TargetName} at {warp.TargetX}, {warp.TargetY}");
                    if (!this.bridges.ContainsKey(location))
                        this.bridges.Add(location, new HashSet<ContainerBridge>());
                    this.bridges[location].Add(bridge);
                    return bridge;
                }
            }

            size = Vector2.Zero;
            return null;
        }
    }
}
