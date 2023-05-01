/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using xTile.ObjectModel;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;

namespace BNWCore
{
    public class Shop_Mechanics
    {
        public static Vector2 _playerPos = Vector2.Zero;
        public void GameLoop_UpdateTicking(UpdateTickingEventArgs e)
        {
            if (ModEntry.SourceLocation != null && (Game1.locationRequest?.Name == "AnimalShop" || Game1.locationRequest?.Name == "WizardHouse" || Game1.locationRequest?.Name == "ScienceHouse"))
            {
                Game1.locationRequest.Location = ModEntry.SourceLocation;
                Game1.locationRequest.IsStructure = ModEntry.SourceLocation.isStructure.Value;
            }
        }
        public void Display_MenuChanged(MenuChangedEventArgs e)
        {
            _playerPos = Vector2.Zero;
            if (e.OldMenu is CarpenterMenu && e.NewMenu is DialogueBox)
            {
                var RobinMessage = ((DialogueBox)e.NewMenu).getCurrentString();
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new DialogueBox(RobinMessage);
            }
            if (e.NewMenu == null && _playerPos != Vector2.Zero)
            {
                Game1.player.position.Set(_playerPos);
            }
        }
        public void Input_ButtonPressed(ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;
            Vector2 clickedTile = ModEntry.ModHelper.Input.GetCursorPosition().GrabTile;
            IPropertyCollection tileProperty = Tile_Utility.GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);
            if (tileProperty == null)
                return;
            CheckForShopToOpen(tileProperty, e);
        }
        public void CheckForShopToOpen(IPropertyCollection tileProperty, ButtonPressedEventArgs e)
        {
            tileProperty.TryGetValue("Shop", out PropertyValue shopProperty);
            if (shopProperty != null)
            {
                IClickableMenu menu = Tile_Utility.CheckVanillaShop(shopProperty, out bool warpingShop);
                if (menu != null)
                {
                    if (warpingShop)
                    {
                        ModEntry.SourceLocation = Game1.currentLocation;
                        _playerPos = Game1.player.position.Get();
                    }
                    ModEntry.ModHelper.Input.Suppress(e.Button);
                    Game1.activeClickableMenu = menu;

                }
            }
        }
        public void OnUpdateTicked(UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(8))
                return;
            Farmer farmer = Game1.player;
            Item item;
            try
            {
                item = farmer.Items[farmer.CurrentToolIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                if (Game1.player.toolBeingUpgraded.Value is GenericTool genericTool)
                {
                    genericTool.actionWhenClaimed();
                }
                else
                {
                    Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);
                }
                Game1.player.toolBeingUpgraded.Value = null;
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }
    }
}
