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
using Omegasis.StardustCore.Events.Preconditions;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events.EventPreconditions
{
    public class FarmHouseLevelPrecondition:EventPrecondition
    {

        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.EventPreconditions.FarmHouseLevelPrecondition";

        public int farmHouseLevel;

        public FarmHouseLevelPrecondition()
        {

        }

        public FarmHouseLevelPrecondition(int Level)
        {
            this.farmHouseLevel = Level;
        }


        public override string ToString()
        {
            return EventPreconditionId + " " + this.farmHouseLevel.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.HouseUpgradeLevel == this.farmHouseLevel;
        }
    }
}
