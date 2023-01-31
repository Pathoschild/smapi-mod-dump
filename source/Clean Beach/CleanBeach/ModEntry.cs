/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Artinity/clean-beach
**
*************************************************/

using StardewModdingAPI;
using xTile.Layers;
using xTile;

namespace CleanBeach
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        private void Content_AssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/Forest"))
            {
                e.Edit(asset =>
                {
                    Map map = asset.AsMap().Data;

                    Layer layer = map.GetLayer("Buildings");

                    // For the beach south and west of the Hat Mouse:
                    layer.Tiles[2, 85] = null;
                    layer.Tiles[4, 85] = null;
                    layer.Tiles[13, 105] = null;
                    layer.Tiles[17, 106] = null;
                    layer.Tiles[43, 106] = null;

                    // For the beach below the sewer entrance:
                    layer.Tiles[83, 108] = null;
                    layer.Tiles[91, 104] = null;
                    layer.Tiles[98, 105] = null;
                    layer.Tiles[100, 105] = null;
                    layer.Tiles[101, 105] = null;
                    layer.Tiles[103, 106] = null;
                    layer.Tiles[106, 106] = null;
                });
            }
        }
    }
}