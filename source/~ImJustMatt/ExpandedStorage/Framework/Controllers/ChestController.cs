/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImJustMatt.ExpandedStorage.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ImJustMatt.ExpandedStorage.Framework.Controllers
{
    internal class ChestController
    {
        private readonly ExpandedStorage _mod;
        private readonly PerScreen<Chest> _heldChest = new();
        private readonly bool _carryChest;

        public ChestController(ExpandedStorage mod)
        {
            _mod = mod;
            _carryChest = _mod.Helper.ModRegistry.IsLoaded("spacechase0.CarryChest");
            if (_carryChest)
            {
                _mod.Monitor.Log("Do not run Expanded Storage with Carry Chest!", LogLevel.Warn);
            }
            else
            {
                _mod.Helper.Events.Input.ButtonPressed += OnButtonPressed;
                _mod.Helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            }

            if (Context.IsMainPlayer)
            {
                _mod.Helper.Events.World.ObjectListChanged += OnObjectListChanged;
            }

            _mod.Helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
        }

        public Chest HeldChest => _heldChest.Value;

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            foreach (var chest in Game1.player.Items.Take(12).OfType<Chest>())
            {
                chest.updateWhenCurrentLocation(Game1.currentGameTime, Game1.player.currentLocation);
            }

            if (_carryChest || ReferenceEquals(_heldChest.Value, Game1.player.CurrentItem)) return;
            StorageController storage = null;
            if (Game1.player.CurrentItem is Chest activeChest && _mod.AssetController.TryGetStorage(activeChest, out storage))
            {
                if (ReferenceEquals(_heldChest.Value, activeChest)) return;
                activeChest.owner.Value = Game1.player.UniqueMultiplayerID;
                activeChest.fixLidFrame();
                _heldChest.Value = activeChest;
            }
            else if (storage == null && _heldChest.Value != null)
            {
                _heldChest.Value.owner.Value = -1;
                _heldChest.Value = null;
            }
        }

        /// <summary>Raised after objects are added/removed in any location (including machines, furniture, fences, etc).</summary>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            _mod.Helper.Events.World.ObjectListChanged -= OnObjectListChanged;
            var removed = e.Removed.FirstOrDefault(r => _mod.AssetController.TryGetStorage(r.Value, out _));
            var added = e.Added.FirstOrDefault(a => _mod.AssetController.TryGetStorage(a.Value, out _));

            if (removed.Value != null && _mod.AssetController.TryGetStorage(removed.Value, out var storage))
            {
                var x = removed.Value.modData.TryGetValue("furyx639.ExpandedStorage/X", out var xStr) ? int.Parse(xStr) : 0;
                var y = removed.Value.modData.TryGetValue("furyx639.ExpandedStorage/Y", out var yStr) ? int.Parse(yStr) : 0;
                if ((x != 0 || y != 0)
                    && storage.StorageSprite is { } spriteSheet
                    && (spriteSheet.TileWidth > 1 || spriteSheet.TileHeight > 1))
                {
                    spriteSheet.ForEachPos(x, y, pos =>
                    {
                        if (!pos.Equals(removed.Key) && e.Location.Objects.ContainsKey(pos)) e.Location.Objects.Remove(pos);
                    });
                }
            }
            else if (added.Value is Chest chest && _mod.AssetController.TryGetStorage(chest, out storage))
            {
                chest.modData["furyx639.ExpandedStorage/X"] = added.Key.X.ToString(CultureInfo.InvariantCulture);
                chest.modData["furyx639.ExpandedStorage/Y"] = added.Key.Y.ToString(CultureInfo.InvariantCulture);

                // Add objects for extra Tile spaces
                if (storage.StorageSprite is { } spriteSheet && (spriteSheet.TileWidth > 1 || spriteSheet.TileHeight > 1))
                {
                    spriteSheet.ForEachPos((int) added.Key.X, (int) added.Key.Y, pos =>
                    {
                        if (!pos.Equals(added.Key) && !e.Location.Objects.ContainsKey(pos)) e.Location.Objects.Add(pos, chest.ToObject(storage));
                    });
                }
            }

            _mod.Helper.Events.World.ObjectListChanged += OnObjectListChanged;
        }

        /// <summary>Raised after the player pressed a keyboard, mouse, or controller button.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;
            var pos = _mod.Config.Controller ? Game1.player.GetToolLocation() / 64f : e.Cursor.Tile;
            pos.X = (int) pos.X;
            pos.Y = (int) pos.Y;
            Game1.currentLocation.objects.TryGetValue(pos, out var obj);

            // Carry Chest
            if (obj != null && e.Button.IsUseToolButton() && Utility.withinRadiusOfPlayer((int) (64 * pos.X), (int) (64 * pos.Y), 1, Game1.player))
            {
                if (CarryChest(obj, Game1.currentLocation, pos)) _mod.Helper.Input.Suppress(e.Button);
                return;
            }

            // Access Carried Chest
            if (obj == null && _heldChest.Value != null && e.Button.IsActionButton())
            {
                if (AccessCarriedChest(_heldChest.Value)) _mod.Helper.Input.Suppress(e.Button);
            }
        }

        /// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller.</summary>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;
            var pos = _mod.Config.Controller ? Game1.player.GetToolLocation() / 64f : e.Cursor.Tile;
            pos.X = (int) pos.X;
            pos.Y = (int) pos.Y;
            Game1.currentLocation.objects.TryGetValue(pos, out var obj);
            if (_mod.Config.Controls.OpenCrafting.JustPressed())
            {
                if (OpenCrafting()) _mod.Helper.Input.SuppressActiveKeybinds(_mod.Config.Controls.OpenCrafting);
            }
        }

        private bool CarryChest(Object obj, GameLocation location, Vector2 pos)
        {
            if (!_mod.AssetController.TryGetStorage(obj, out var storage) || storage.Config.Option("CanCarry", true) != StorageConfigController.Choice.Enable) return false;
            var x = obj.modData.TryGetValue("furyx639.ExpandedStorage/X", out var xStr) ? int.Parse(xStr) : 0;
            var y = obj.modData.TryGetValue("furyx639.ExpandedStorage/Y", out var yStr) ? int.Parse(yStr) : 0;
            if (!location.Objects.TryGetValue(new Vector2(x, y), out obj)) return false;
            var chest = obj.ToChest(storage);
            chest.TileLocation = Vector2.Zero;
            chest.modData.Remove("furyx639.ExpandedStorage/X");
            chest.modData.Remove("furyx639.ExpandedStorage/Y");
            if (!Game1.player.addItemToInventoryBool(chest, true)) return false;
            if (!string.IsNullOrWhiteSpace(storage.CarrySound)) location.playSound(storage.CarrySound);
            location.objects.Remove(pos);
            return true;
        }

        private bool AccessCarriedChest(Chest chest)
        {
            if (!_mod.AssetController.TryGetStorage(chest, out var storage) || storage.Config.Option("AccessCarried", true) != StorageConfigController.Choice.Enable) return false;
            return chest.CheckForAction(storage, Game1.player, true);
        }

        private bool OpenCrafting()
        {
            if (_heldChest.Value == null || Game1.activeClickableMenu != null)
                return false;
            if (!_mod.AssetController.TryGetStorage(_heldChest.Value, out var storage) || storage.Config.Option("AccessCarried", true) != StorageConfigController.Choice.Enable)
                return false;
            _heldChest.Value.GetMutex().RequestLock(delegate
            {
                var pos = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
                Game1.activeClickableMenu = new CraftingPage(
                    (int) pos.X,
                    (int) pos.Y,
                    800 + IClickableMenu.borderWidth * 2,
                    600 + IClickableMenu.borderWidth * 2,
                    false,
                    true,
                    new List<Chest> {_heldChest.Value})
                {
                    exitFunction = delegate { _heldChest.Value.GetMutex().ReleaseLock(); }
                };
            });
            return true;
        }
    }
}