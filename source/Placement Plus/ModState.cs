/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace PlacementPlus
{
    /// <summary> Static class holding and maintaining data used primarily in Harmony patches. </summary>
    public static class ModState
    {
        internal static Vector2 CursorTile;
        internal static Item    HeldItem;
        internal static bool    HoldingToolButton;
        internal static Object  TileObject;
        internal static NetVector2Dictionary<TerrainFeature,NetRef<TerrainFeature>> TerrainFeatures;
        internal static readonly IMonitor Monitor = PlacementPlus.Instance.Monitor;
        
        private static bool initialized;
        
        
        /// <summary> Initializes ModState to begin tracking values. </summary>
        /// <param name="helper">An <see cref="IModHelper">IModHelper</see> reference for hooking into events.</param>
        public static void Initialize(ref IModHelper helper)
        {
            if (initialized) return;
            
            helper.Events.Input.ButtonsChanged  += (_, e) => {
                if (!Context.IsWorldReady) return;
                
                // Updating mod state fields.
                TerrainFeatures = Game1.player.currentLocation.terrainFeatures;
                CursorTile = e.Cursor.Tile;
                HeldItem = Game1.player.CurrentItem;

                HoldingToolButton = e.Held.Any(button => button.IsUseToolButton()); // (i.e. left click)
                
                TileObject = Game1.player.currentLocation.getObjectAtTile((int) e.Cursor.Tile.X, (int) e.Cursor.Tile.Y);
            };
            initialized = true;
        }
    }
}