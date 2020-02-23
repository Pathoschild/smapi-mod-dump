using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;


namespace PersonalAnvil
{
    public class ModEntry : Mod
    {
        private IJsonAssetsApi JsonAssets;
        private int AnvilID;
        private int leftClickXPos;
        private int leftClickYPos;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (JsonAssets != null)
            {
                AnvilID = JsonAssets.GetBigCraftableId("Anvil");

                if (AnvilID == -1)
                {
                    Monitor.Log("Could not get the ID for the Anvil item", LogLevel.Warn);
                }
            }
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.Button == SButton.MouseLeft)
            {
                leftClickXPos = (int)e.Cursor.ScreenPixels.X;
                leftClickYPos = (int)e.Cursor.ScreenPixels.Y;
            }
            if (!e.Button.IsActionButton())
                return;
            Vector2 tile = Helper.Input.GetCursorPosition().Tile;
            Game1.currentLocation.Objects.TryGetValue(tile, out Object obj);
            if (obj != null && obj.bigCraftable.Value)
            {
                if (obj.ParentSheetIndex.Equals(AnvilID))
                {
                    Game1.activeClickableMenu = new WorkbenchGeodeMenu(Helper.Content);
                }
            }
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(4) && Helper.Input.IsDown(SButton.MouseLeft) && Game1.activeClickableMenu is WorkbenchGeodeMenu menu)
            {
                bool clintNotBusy = menu.heldItem != null && (menu.heldItem.Name.Contains("Geode") || menu.heldItem.ParentSheetIndex == 275) && Game1.player.Money >= 0 && menu.geodeAnimationTimer <= 0;
                bool playerHasRoom = Game1.player.freeSpotsInInventory() > 1 || (Game1.player.freeSpotsInInventory() == 1 && menu.heldItem != null && menu.heldItem.Stack == 1);

                if (clintNotBusy && playerHasRoom && menu.geodeSpot.containsPoint(leftClickXPos, leftClickYPos))
                {
                    menu.receiveLeftClick(leftClickXPos, leftClickYPos, false);
                }
            }
        }
    }
    public interface IJsonAssetsApi
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }
}