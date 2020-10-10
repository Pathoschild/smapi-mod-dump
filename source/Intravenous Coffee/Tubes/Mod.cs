/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Pathoschild.Stardew.Common;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;
using System.Linq;
using StardewValley.Menus;
using PyTK.Extensions;

namespace Tubes
{
    // Mod entry point.
    public class TubesMod : Mod
    {
        internal static IModHelper _helper;
        internal static IMonitor _monitor;

        private Dictionary<GameLocation, TubeNetwork[]> TubeNetworks = new Dictionary<GameLocation, TubeNetwork[]> ();
        private IEnumerable<TubeNetwork> AllTubeNetworks { get => TubeNetworks.SelectMany(kv => kv.Value); }
        private HashSet<GameLocation> ReloadQueue = new HashSet<GameLocation>();

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _monitor = Monitor;
            TubeObject.Init();
            TubeTerrain.Init();
            PortObject.Init();

            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            LocationEvents.LocationsChanged += this.LocationEvents_LocationsChanged;
            LocationEvents.LocationObjectsChanged += this.LocationEvents_LocationObjectsChanged;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            foreach (GameLocation location in this.ReloadQueue)
                this.TubeNetworks[location] = TubeNetwork.GetAllNetworksIn(location).ToArray();
            this.ReloadQueue.Clear();

            foreach (TubeNetwork network in this.AllTubeNetworks)
                network.Process();
        }

        private void LocationEvents_LocationsChanged(object sender, EventArgsGameLocationsChanged e)
        {
            this.TubeNetworks.Clear();
            foreach (GameLocation location in CommonHelper.GetLocations())
                this.ReloadQueue.Add(location);
        }

        private void LocationEvents_LocationObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
        {
            this.TubeNetworks.Remove(Game1.currentLocation);
            this.ReloadQueue.Add(Game1.currentLocation);

            // When the player places a TubeObject, it's placed as an object. Gather them and replace them with TubeTerrain
            // instead. This seems to be the only way to place terrain features.
            // We also remove any temporary "junk" objects, which are only used to trigger this event.
            List<Vector2> tubes = new List<Vector2>();
            List<Vector2> junk = new List<Vector2>();
            foreach (var obj in Game1.currentLocation.objects) {
                if (obj.Value.parentSheetIndex == TubeObject.ObjectData.sdvId)
                    tubes.Add(obj.Key);
                if (obj.Value.parentSheetIndex == JunkObject.objectData.sdvId)
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

            TubeTerrain.UpdateSpritesInLocation(Game1.currentLocation);
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            // Override the crafting menu so that our recipe has the proper icon and text.
            if (Game1.activeClickableMenu is GameMenu activeMenu && Helper.Reflection.GetField<List<IClickableMenu>>(activeMenu, "pages").GetValue().Find(p => p is CraftingPage) is CraftingPage craftingPage) {
                for (int i = 0; i < craftingPage.pagesOfCraftingRecipes.Count; i++) {
                    if (craftingPage.pagesOfCraftingRecipes[i].Find(k => k.Value.name == TubeObject.Blueprint.Fullid) is KeyValuePair<ClickableTextureComponent, CraftingRecipe> kv && kv.Value != null && kv.Key != null) {
                        kv.Key.texture = TubeObject.Icon;
                        kv.Key.sourceRect = TubeObject.ObjectData.sourceRectangle;
                        kv.Key.baseScale = 4.0f;
                        kv.Value.DisplayName = TubeObject.Blueprint.Name;
                        Helper.Reflection.GetField<string>(kv.Value, "description").SetValue(TubeObject.Blueprint.Description);
                    }
                    if (craftingPage.pagesOfCraftingRecipes[i].Find(k => k.Value.name == PortObject.Blueprint.Fullid) is KeyValuePair<ClickableTextureComponent, CraftingRecipe> kv2 && kv2.Value != null && kv2.Key != null) {
                        kv = kv2;
                        kv.Key.texture = PortObject.Icon;
                        kv.Key.sourceRect = PortObject.ObjectData.sourceRectangle;
                        kv.Key.baseScale = 4.0f;
                        kv.Value.DisplayName = PortObject.Blueprint.Name;
                        Helper.Reflection.GetField<string>(kv.Value, "description").SetValue(PortObject.Blueprint.Description);
                    }
                }
            }
        }
    }
}
