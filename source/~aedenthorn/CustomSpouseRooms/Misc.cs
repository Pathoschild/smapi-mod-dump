/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace CustomSpouseRooms
{
    /// <summary>The mod entry point.</summary>
    public class Misc
    {
        private static Dictionary<string, int> topOfHeadOffsets = new Dictionary<string, int>();

        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public static Dictionary<string, NPC> GetSpouses(Farmer farmer, int all)
        {
            Dictionary<string, NPC> spouses = new Dictionary<string, NPC>();
            if (all < 0)
            {
                NPC ospouse = farmer.getSpouse();
                if (ospouse != null)
                {
                    spouses.Add(ospouse.Name, ospouse);
                }
            }
            foreach (string friend in farmer.friendshipData.Keys)
            {
                if (Game1.getCharacterFromName(friend, true) != null && farmer.friendshipData[friend].IsMarried() && (all > 0 || friend != farmer.spouse))
                {
                    spouses.Add(friend, Game1.getCharacterFromName(friend, true));
                }
            }

            return spouses;
        }

        public static void ExtendMap(GameLocation location, int w, int h)
        {
            List<Layer> layers = AccessTools.Field(typeof(Map), "m_layers").GetValue(location.map) as List<Layer>;
            for (int i = 0; i < layers.Count; i++)
            {
                Tile[,] tiles = AccessTools.Field(typeof(Layer), "m_tiles").GetValue(layers[i]) as Tile[,];
                Size size = (Size)AccessTools.Field(typeof(Layer), "m_layerSize").GetValue(layers[i]);
                if (tiles.GetLength(0) >= w && tiles.GetLength(1) >= h)
                    continue;

                w = Math.Max(w, tiles.GetLength(0));
                h = Math.Max(h, tiles.GetLength(1));

                ModEntry.PMonitor.Log($"Extending layer {layers[i].Id} from {size.Width},{size.Height} ({tiles.GetLength(0)},{tiles.GetLength(1)}) to {w},{h}");

                size = new Size(w, h);
                AccessTools.Field(typeof(Layer), "m_layerSize").SetValue(layers[i], size);
                AccessTools.Field(typeof(Map), "m_layers").SetValue(location.map, layers);

                Tile[,] newTiles = new Tile[w, h];

                for (int k = 0; k < tiles.GetLength(0); k++)
                {
                    for (int l = 0; l < tiles.GetLength(1); l++)
                    {
                        newTiles[k, l] = tiles[k, l];
                    }
                }
                AccessTools.Field(typeof(Layer), "m_tiles").SetValue(layers[i], newTiles);
                AccessTools.Field(typeof(Layer), "m_tileArray").SetValue(layers[i], new TileArray(layers[i], newTiles));

            }
            AccessTools.Field(typeof(Map), "m_layers").SetValue(location.map, layers);
        }
    }
}