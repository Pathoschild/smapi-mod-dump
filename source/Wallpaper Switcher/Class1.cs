/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JulianoSFA/WallpaperSwitcher
**
*************************************************/

using System;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace YourProjectName
{
    internal sealed class ModEntry : Mod
    {
        Dictionary<string, NetString> floors = new Dictionary<string, NetString>();
        Dictionary<string, NetString> wallpapers = new Dictionary<string, NetString>();

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.SaveLoaded += this.OnLocationChanged;
            Helper.Events.Player.Warped += this.OnLocationChanged;
            Helper.Events.Player.InventoryChanged += this.GetUsedDecorationAndRetrieve;
        }

        private void GetUsedDecorationAndRetrieve(object? sender, InventoryChangedEventArgs eventArgs)
        {
            if (!eventArgs.IsLocalPlayer) return;

            List<Item> usedDecorations = new List<Item>();
            foreach (Item item in eventArgs.Removed)
            {
                if (item.Name == "Wallpaper" || item.Name == "Flooring") usedDecorations.Add(item);
            }
            if (usedDecorations.Count < 1) return;

            var locationOnInventoryChange = Game1.player.currentLocation;
            if (!locationOnInventoryChange.GetType().IsSubclassOf(typeof(DecoratableLocation))) return;
            DecoratableLocation currentDecoratableLocation = (DecoratableLocation)locationOnInventoryChange;

            foreach(Item item in usedDecorations)
            {
                if (item.Name == "Wallpaper")
                {
                    var differences = this.wallpapers.Except(currentDecoratableLocation.appliedWallpaper.FieldDict);
                    foreach (var difference in differences)
                    {
                        string[] id_info = difference.Value.Value.Split(':');
                        if (id_info.Length == 2)
                        {
                            Game1.player.addItemToInventory(new Wallpaper(id_info[0], Int32.Parse(id_info[1])));
                        }
                        else
                        {
                            Game1.player.addItemToInventory(new Wallpaper(Int32.Parse(id_info[0]), false));
                        }
                    }
                    this.SaveWallpapers(currentDecoratableLocation);

                }

                if (item.Name == "Flooring")
                {
                    var differences = this.floors.Except(currentDecoratableLocation.appliedFloor.FieldDict);
                    foreach (var difference in differences)
                    {
                        string[] id_info = difference.Value.Value.Split(':');
                        if (id_info.Length == 2)
                        {
                            Game1.player.addItemToInventory(new Wallpaper(id_info[0], Int32.Parse(id_info[1])));
                        }
                        else
                        {
                            Game1.player.addItemToInventory(new Wallpaper(Int32.Parse(id_info[0]), true));
                        }
                    }
                    this.SaveFloors(currentDecoratableLocation);
                }
            }
        }

        private void OnLocationChanged(object? sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            var currentLocation = Game1.player.currentLocation;
            var isDecoratable = currentLocation.GetType().IsSubclassOf(typeof(DecoratableLocation));
            if (isDecoratable) {
                this.SaveFloors((DecoratableLocation)currentLocation);
                this.SaveWallpapers((DecoratableLocation)currentLocation);
            }
        }

        private void SaveFloors(DecoratableLocation farmHouse)
        {
            this.floors = farmHouse.appliedFloor.FieldDict.DeepClone();
        }

        private void SaveWallpapers(DecoratableLocation farmHouse)
        {
            this.wallpapers = farmHouse.appliedWallpaper.FieldDict.DeepClone();
        }
    }
}