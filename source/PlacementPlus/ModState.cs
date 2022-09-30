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
        internal static Vector2 cursorTile;
        internal static Item    heldItem;
        internal static bool    holdingToolButton;
        internal static Object  tileObject;
        internal static NetVector2Dictionary<TerrainFeature,NetRef<TerrainFeature>> terrainFeatures;
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
                terrainFeatures = Game1.player.currentLocation.terrainFeatures;
                cursorTile = e.Cursor.Tile;
                heldItem = Game1.player.CurrentItem;

                holdingToolButton = e.Held.Any(button => button.IsUseToolButton()); // (i.e. left click)
                
                tileObject = Game1.player.currentLocation.getObjectAtTile((int) e.Cursor.Tile.X, (int) e.Cursor.Tile.Y);
            };
            initialized = true;
        }
    }
}