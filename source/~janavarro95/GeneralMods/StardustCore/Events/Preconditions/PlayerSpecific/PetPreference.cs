/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class PetPreference:EventPrecondition
    {
        public bool wantsDog;

        public PetPreference()
        {

        }

        public PetPreference(bool WantsDog)
        {
            this.wantsDog = WantsDog;
        }

        public override string ToString()
        {
            if (this.wantsDog)
            {
                return this.precondition_playerWantsDog();
            }
            else
            {
                return this.precondition_playerWantsCat();
            }
        }


        /// <summary>
        /// Condition: The player has no pet and wants a cat.
        /// </summary>
        /// <returns></returns>
        public string precondition_playerWantsCat()
        {
            StringBuilder b = new StringBuilder();
            b.Append("h ");
            b.Append("cat");
            return b.ToString();
        }
        /// <summary>
        /// Condition: The player has no pet and wants a dog.
        /// </summary>
        /// <returns></returns>
        public string precondition_playerWantsDog()
        {
            StringBuilder b = new StringBuilder();
            b.Append("h ");
            b.Append("dog");
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            //Cat breeds
            if (Game1.player.whichPetBreed == 0 || Game1.player.whichPetBreed == 1 || Game1.player.whichPetBreed == 2)
            {
                if (this.wantsDog == false) return true;
                else return false;
            }
            else
            {
                //Dog breeds.
                if (this.wantsDog == true) return true;
                else return false;
            }
        }

    }
}
