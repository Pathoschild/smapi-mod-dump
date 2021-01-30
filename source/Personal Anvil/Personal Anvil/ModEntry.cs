/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/Personal-Anvil
**
*************************************************/

using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PersonalAnvil
{
    public class ModEntry : Mod
    {
        private IJsonAssetsApi _jsonAssets;
        private int _anvilId;
        private int _leftClickXPos;
        private int _leftClickYPos;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (_jsonAssets == null) return;
            _anvilId = _jsonAssets.GetBigCraftableId("Anvil");
            if (_anvilId == -1) Monitor.Log("Could not get the ID for the Anvil item", LogLevel.Warn);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (e.Button == SButton.MouseLeft)
            {
                _leftClickXPos = (int) e.Cursor.ScreenPixels.X;
                _leftClickYPos = (int) e.Cursor.ScreenPixels.Y;
            }

            if (!e.Button.IsActionButton()) return;
            var tile = Helper.Input.GetCursorPosition().Tile;
            Game1.currentLocation.Objects.TryGetValue(tile, out var obj);
            if (obj == null || !obj.bigCraftable.Value) return;
            if (obj.ParentSheetIndex.Equals(_anvilId))
                Game1.activeClickableMenu = new WorkbenchGeodeMenu(Helper.Content);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            _jsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            _jsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Re-send a left click to the geode menu if one is already not being broken, the player has the room and money for it, and the click was on the geode spot.
            if (!e.IsMultipleOf(4) || !Helper.Input.IsDown(SButton.MouseLeft) ||
                !(Game1.activeClickableMenu is WorkbenchGeodeMenu menu)) return;
            var clintNotBusy = menu.heldItem != null && Utility.IsGeode(menu.heldItem) && menu.GeodeAnimationTimer <= 0;

            var playerHasRoom = Game1.player.freeSpotsInInventory() > 1 || Game1.player.freeSpotsInInventory() == 1 && menu.heldItem != null && menu.heldItem.Stack == 1;

            if (clintNotBusy && playerHasRoom && menu.GeodeSpot.containsPoint(_leftClickXPos, _leftClickYPos))
            {
                menu.receiveLeftClick(_leftClickXPos, _leftClickYPos, false);
            }
        }
    }

    public interface IJsonAssetsApi
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }
}