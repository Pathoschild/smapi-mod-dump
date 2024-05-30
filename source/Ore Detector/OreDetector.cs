/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Ore-Detector
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;

namespace OreDetector
{
    public class OreDetector
    {
        public Dictionary<string, List<StardewValley.Object>> Ores = new Dictionary<string, List<StardewValley.Object>>();

        public Dictionary<string, List<StardewValley.Object>> MinedOres = new Dictionary<string, List<StardewValley.Object>>();

        public List<Vector2> ladderPositions = new List<Vector2>();

        public List<Vector2> HolePositions = new List<Vector2>();

        public Dictionary<string, string> itemIds = new Dictionary<string, string>();

        public MineShaft currentShaft;

        public static List<string> foundTileNames = new List<string>();

        private static OreDetector instance;

        public bool LadderRevealed { get => currentShaft.ladderHasSpawned || ladderPositions.Count > 0; }

        public bool HoleRevealed { get => HolePositions.Count > 0; }

        public bool isDesertMine { get => currentShaft.mineLevel > 120; }

        private OreDetector() { }

        public static OreDetector GetOreDetector()
        {
            return instance != null ? instance : new OreDetector();    
        }
        public void LookForSpawnedLadders()
        {
            if (currentShaft == null)
                return;

            Layer layer = currentShaft.Map.GetLayer("Buildings");
            for (int y = 0; y < layer.LayerHeight; y++)
            {
                for (int x = 0; x < layer.LayerWidth; x++)
                {
                    var tile = layer.Tiles[x, y];
                    if (tile?.TileIndex == 173)
                    {
                        Vector2 ladderPostion = new Vector2(x, y);
                        if (!ladderPositions.Contains(ladderPostion))
                        {
                            ladderPositions.Add(ladderPostion);
                        }
                    }
                }
            }
        }
        public void LookForSpawnedHoles()
        {
            if (currentShaft == null || !isDesertMine) // >= 121 means its desert
                return;

            Layer layer = currentShaft.Map.GetLayer("Buildings");
            for (int y = 0; y < layer.LayerHeight; y++)
            {
                for (int x = 0; x < layer.LayerWidth; x++)
                {
                    var tile = layer.Tiles[x, y];
                    if (tile?.TileIndex == 174)
                    {
                        Vector2 holePostion = new Vector2(x, y);
                        if (!HolePositions.Contains(holePostion))
                        {
                            HolePositions.Add(holePostion);
                        }
                    }
                }
            }

        }
        public void GetOreInCurrentShaft()
        {
            Ores.Clear();
            MinedOres.Clear();
            itemIds.Clear();
            ladderPositions.Clear();
            HolePositions.Clear();

            currentShaft = (MineShaft)Game1.player.currentLocation;
            OverlaidDictionary current_ores = currentShaft.Objects;
            foreach (var ore in current_ores.Values)
            {
                //Debug.WriteLine($"{ore.Name} {ore.Category} {ore.ParentSheetIndex}");
                if (ore.Category == -999 || ore.Category == -2 || (ore.Category == -9 && ore.Name == "Barrel"))
                {
                    if (!ModEntry.saveModel.discoveredmaterialsQualifiedIds.Contains(ore.QualifiedItemId) && !ModEntry.saveModel.discoveredMaterials.Contains(ore.DisplayName))
                    {
                        ModEntry.saveModel.discoveredMaterials.Add(ore.DisplayName);
                        ModEntry.saveModel.discoveredmaterialsQualifiedIds.Add(ore.QualifiedItemId);
                    }
                    if (!Ores.ContainsKey(ore.DisplayName))
                    {
                        Ores.Add(ore.DisplayName, new List<StardewValley.Object>());
                        itemIds.Add(ore.DisplayName, ore.QualifiedItemId);
                        MinedOres.Add(ore.DisplayName, new List<StardewValley.Object>());
                    }
                    Ores[ore.DisplayName].Add(ore);
                }
            }
        }
    }
}
