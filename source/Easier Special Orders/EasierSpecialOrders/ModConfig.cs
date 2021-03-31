/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/EasierSpecialOrders
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasierSpecialOrders
{
    internal class ModConfig
    {
        public bool RemoveCollectionObjective { get; set; } = true;
        public QiChallenges EasierQiChallenges { get; set; } = new QiChallenges();       
        internal class QiChallenges
        {
            public bool ShipLessQiFruit { get; set; } = false;
            public bool DonateLessPrismaticShards { get; set; } = false;
            public bool LowerJunimoKartScore { get; set; } = false;
            public bool ShipLessCookedItems { get; set; } = false;
            public bool GiveLessGifts { get; set; } = false;
            public bool MoreTimeForExtendedFamily { get; set; } = false;
            public bool LessItemsForQisPrismaticGrange { get; set; } = false;
        }
    }
}
