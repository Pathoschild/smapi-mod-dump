/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>
    /// Json Assets API.
    /// </summary>
    public interface IJsonAssetApi
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }

    public class DresserHandlerJson
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of Json Assets API
        private IJsonAssetApi JsonAssets;

        // Instance of Glam Menu
        private GlamMenu Menu;

        // Dresser Json ID
        private int DresserAssetId = -1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        public DresserHandlerJson(ModEntry entry)
        {
            Entry = entry;
            LoadJsonAssetsApi();
            LoadDresserAsset();
        }

        /// <summary>
        /// Loads the Json Assets API.
        /// </summary>
        private void LoadJsonAssetsApi()
        {
            JsonAssets = Entry.Helper.ModRegistry.GetApi<IJsonAssetApi>("spacechase0.JsonAssets");
        }

        /// <summary>
        /// Uses Json Assets API To Load The Dresser.
        /// </summary>
        public void LoadDresserAsset()
        {
            JsonAssets.LoadAssets(Path.Combine(Entry.Helper.DirectoryPath, "assets", "json"));
        }

        /// <summary>
        /// Sets The Glam Menu.
        /// </summary>
        /// <param name="menu"></param>
        public void SetMenu(GlamMenu menu)
        {
            Menu = menu;
        }

        /// <summary>
        /// Gets The Dressers BigCraftable Id.
        /// </summary>
        public void GetCraftableId()
        {
            if (JsonAssets != null)
                DresserAssetId = JsonAssets.GetBigCraftableId("Glam Dresser");
        }

        /// <summary>
        /// Check If the Glam Menu Can Be Opened.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Button Pressed Event</param>
        public void CheckJsonInput(object sender, ButtonPressedEventArgs e) 
        {
            if (Context.IsWorldReady && Game1.currentLocation != null && Game1.activeClickableMenu == null && IsActionButton(e.Button))
            {
                GameLocation location = Game1.currentLocation;

                Vector2 currentTile = Entry.Helper.Input.GetCursorPosition().GrabTile;
                location.Objects.TryGetValue(currentTile, out StardewValley.Object craftable);
                if (craftable != null && craftable.bigCraftable.Value)
                {
                    if (craftable.ParentSheetIndex.Equals(DresserAssetId))
                    {
                        Menu.TakeSnapshot();
                        Entry.ChangePlayerDirection();
                        Entry.OpenGlamMenu();
                    }           
                }
            }
        }

        /// <summary>
        /// Checks if an Action button was pressed.
        /// </summary>
        /// <param name="button">The button in question</param>
        /// <returns>Wether an Action Button was pressed</returns>
        private bool IsActionButton(SButton button)
        {
            //Check the different buttons
            if (button.Equals(SButton.MouseRight) || button.Equals(SButton.ControllerA) || button.IsActionButton())
                return true;

            return false;
        }
    }
}
