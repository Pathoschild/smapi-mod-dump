/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;

namespace BNWCore
{
    public class Magic_Bootle
    {
        public void OnButtonPressed(ButtonPressedEventArgs e)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("water_fishing_blessing"))
            {
                if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA) && Game1.player.CurrentItem.ParentSheetIndex == 738)
                {
                    const int radius = 2;
                    GameLocation location = Game1.currentLocation;
                    Point tile = Game1.player.getTileLocationPoint();
                    for (int y = tile.Y - radius; y < tile.Y + radius + 1; y++)
                    {
                        for (int x = tile.X - radius; x < tile.X + radius + 1; x++)
                        {
                            if (location.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature feature) && feature is HoeDirt dirt)
                            {
                                dirt.state.Value = HoeDirt.watered;
                            }
                        }
                    }
                    Game1.player.reduceActiveItemByOne();
                }
            }
        }
    }
}
