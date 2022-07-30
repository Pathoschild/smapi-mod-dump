/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Characters;

namespace CarryYourPet
{
    public class CarriedCharacter
    {
        private NPC npc;
        private bool shouldDraw;
        // private bool isPet;

        // public bool IsPet
        // {
        //     get => isPet;
        //     set
        //     {
        //         isPet = value;
        //     }
        // }
        
        public bool ShouldDraw
        {
            get => shouldDraw;
            set
            {
                shouldDraw = value;
            }
        }
        
        public NPC Npc
        {
            get => npc;
            set
            {
                npc = value;
            }
        }
    }
}