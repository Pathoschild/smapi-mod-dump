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

namespace CarryYourPet
{
    public class CarriedCharacter
    {
        // private bool isPet;

        // public bool IsPet
        // {
        //     get => isPet;
        //     set
        //     {
        //         isPet = value;
        //     }
        // }

        public bool ShouldDraw { get; set; }

        public Character? Npc { get; set; }
    }
}
