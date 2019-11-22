using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Bookcase.Utils {
    public class NPCUtils {
        /// <summary>
        /// Gets all NPCs currently on the tiles under the mouse cursor
        /// </summary>
        /// <returns>A List of all NPCs under the mouse cursor</returns>
        public static List<NPC> GetNPCsUnderCursor() {
            List<NPC> npcs = new List<NPC>();
            foreach (NPC n in Game1.player.currentLocation.getCharacters()) {
                if (IsNPCOnMouseTile(n.getTileLocation(), true)) {
                    npcs.Add(n);
                }
            }
            return npcs;
        }
        /// <summary>
        /// Gets the NPC currently on the tiles under the mouse cursor
        /// </summary>
        /// <returns>The first NPC below the mouse, or null</returns>
        public static NPC GetNPCUnderCursor() {
            return GetNPCsUnderCursor().FirstOrDefault();
        }
        /// <summary>
        /// Checks if an NPC is on the same tile as the mouse, with the option of checking the tile above aswell. (Most NPCs fill 2 tiles, visually)
        /// </summary>
        /// <param name="checkOneTileAbove">If it is desirable for the tile above the NPC's feet to be checked aswell</param>
        /// <param name="npcTileLoc">The NPC's tile location</param>
        /// <returns></returns>
        public static bool IsNPCOnMouseTile(Vector2 npcTileLoc, bool checkOneTileAbove = false) {
            return Game1.currentCursorTile == npcTileLoc || (checkOneTileAbove && Game1.currentCursorTile == (npcTileLoc - Vector2.UnitY));
        }
        /// <summary>
        /// Tries to get an NPC which is on the tile under the mouse cursor.
        /// </summary>
        /// <param name="npc">The NPC under the cursor, or null if none is found.</param>
        /// <returns>True if there is an NPC present, False if there is not.</returns>
        public static bool TryGetNPCUnderCursor(out NPC npc) {
            npc = GetNPCUnderCursor();
            return npc != null;
        }
        /// <summary>
        /// Tries to get an NPC (filtered by some specified filter) on the tile under the mouse cursor.
        /// </summary>
        /// <param name="npc">The NPC present under the mouse cursor or null</param>
        /// <param name="filter">The filter to be applied when searching for a </param>
        /// <returns>True if there is a filtered NPC present, False if there is not, or the filter removes the NPC present.</returns>
        public static bool TryGetNPCUnderCursor(out NPC npc, Func<NPC, bool> filter) {
            bool b = TryGetNPCsUnderCursor(out List<NPC> toFilter, filter);
            npc = toFilter.FirstOrDefault();
            return b;
        }
        /// <summary>
        /// Tries to get a list of NPCs on the tile under the mouse cursor.
        /// </summary>
        /// <param name="npcs">A List of the NPCs present under the mouse.</param>
        /// <returns>True if there is an NPC present, False if there is not.</returns>
        public static bool TryGetNPCsUnderCursor(out List<NPC> npcs) {
            npcs = GetNPCsUnderCursor();
            return npcs.Count > 0;
        }
        /// <summary>
        /// Tries to get a List of NPCs (filtered by some specified filter) on the tile under the mouse cursor.
        /// </summary>
        /// <param name="npc">A List of the NPCs present under the mouse, filtered by the given filter.</param>
        /// <param name="filter">The filter to be applied when searching for a </param>
        /// <returns>True if there is a filtered NPC present, False if there is not, or the filter removes the NPC present</returns>
        public static bool TryGetNPCsUnderCursor(out List<NPC> npc, Func<NPC, bool> filter) {
            npc = GetNPCsUnderCursor().Where(filter).ToList();
            return npc.Count > 0;
        }
    }
}