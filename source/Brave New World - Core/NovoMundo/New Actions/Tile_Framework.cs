/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.ObjectModel;
using xTile.Tiles;
using xTile.Dimensions;

namespace NovoMundo
{
    public class Tile_Framework:Question_Dialogues
    {
        private Vector2 _playerPos = new();        
        internal static GameLocation SourceLocation;


        public static IPropertyCollection GetTileProperty(GameLocation map, string layer, Vector2 tile)
        {
            if (map == null)
                return null;
            Tile checkTile = map.Map.GetLayer(layer).Tiles[(int)tile.X, (int)tile.Y];
            return checkTile?.Properties;
        }
        public void CheckForTileProperty(IPropertyCollection tileProperty, ButtonPressedEventArgs e)
        {
            tileProperty.TryGetValue("nm_Action", out PropertyValue property);
            if (property != null)
            {
                ActionCheck(property);
            }
        }
        public void ActionCheck(string property)
        {
            
            switch (property)
            {
                case null:
                    {
                        break;
                    }
                case "carpenter":
                    {
                        CarpenterMenu();
                        break;
                    }
            }

        }
        public void OnGameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (SourceLocation != null && (Game1.locationRequest?.Name == "AnimalShop" || Game1.locationRequest?.Name == "WizardHouse" || Game1.locationRequest?.Name == "ScienceHouse"))
            {
                Game1.locationRequest.Location = SourceLocation;
                Game1.locationRequest.IsStructure = SourceLocation.isStructure.Value;
            }
        }
        public void OnDisplayMenuChanged(object sender, MenuChangedEventArgs e)
        {
            _playerPos = Vector2.Zero;
            if (e.OldMenu is CarpenterMenu && e.NewMenu is DialogueBox)
            {
                var message = ((DialogueBox)e.NewMenu).getCurrentString();
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new DialogueBox(message);
            }
            if (e.NewMenu == null && _playerPos != Vector2.Zero)
            {
                Game1.player.position.Set(_playerPos);
            }
            if (e.OldMenu is PurchaseAnimalsMenu && e.NewMenu is DialogueBox)
            {
                Game1.exitActiveMenu();
                Game1.activeClickableMenu = new DialogueBox(ModEntry.ModHelper.Translation.Get("Tile_Action_Framework.AnimalShop.PurchaseAnimals.Completed"));
            }
        }
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
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
            IPropertyCollection tileProperty = GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);
            if (tileProperty == null)
                return;
            CheckForTileProperty(tileProperty, e);
        }

    }
}
