using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.GameLocationPatches
{
    /// <summary>
    /// REASON FOR PATCHING: In game events use hardcoded values for placing/warping/etc. If
    /// objects like the farm house are relocated, adjustments are needed.
    /// 
    /// Patches the GameLocation.startEvent method to allocate for the reloation of the Farmhouse.
    /// </summary>
    public class startEventPatch
    {
        private static CustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public startEventPatch(CustomManager customManager) {
            startEventPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Intercepts the eventCommands before they are executed to adjust for relocations of various
        /// objects such as the Farm House.
        /// </summary>
        /// <param name="evt">The instance of <see cref="Event"/> that called the startEvent method.</param>
        public static void Prefix(ref Event evt) {
            // TO-DO: There are way more events then this...
            if (!customManager.Canon) {
                Point p = customManager.FarmHousePorch;
                if (evt.id == 1590166) {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 cat " + (p.X - 1).ToString() + " " + (p.Y + 2).ToString() + " 2";
                } else if (evt.id == 897405) {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 dog " + (p.X - 1).ToString() + " " + (p.Y + 2).ToString() + " 2";
                }
            }
        }
    }
}
