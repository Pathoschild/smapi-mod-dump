/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HaulinOats/StardewMods
**
*************************************************/



using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace WaterBalloon
{
    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
        void LoadAssets(string path);
    }
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private IJsonAssetsApi? JsonAssets;
        private int WaterBalloonID = -1;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets == null)
            {
                Monitor.Log("Can't load Json Assets API, which is needed for test mod to function", LogLevel.Debug);
            }
            else
            {
                JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
                Monitor.Log("JSON assets loaded", LogLevel.Debug);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            WaterBalloonID = JsonAssets.GetObjectId("Water Balloon");
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //if activating the item and Water Balloon item exists water 3 tiles out from the player
            if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA) && Game1.player.CurrentItem?.Name == "Water Balloon" && WaterBalloonID != -1)
            {
                const int radius = 2;
                GameLocation location = Game1.currentLocation;
                Point tile = Game1.player.getTileLocationPoint();
                for (int y = tile.Y - radius; y < tile.Y + radius + 1; y++)
                {
                    for (int x = tile.X - radius; x < tile.X + radius + 1; x++)
                    {
                        if (location.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature feature) && feature is HoeDirt dirt)
                            dirt.state.Value = HoeDirt.watered;
                    }
                }
                Game1.player.reduceActiveItemByOne();
                Game1.playSound("wateringCan");
            }
        }
    }
}
