/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTrawler.Messages
{
    internal class DepartureMessage
    {
        public long MainDeckhand { get; set; }

        public DepartureMessage(long mainDeckhand)
        {
            MainDeckhand = mainDeckhand;
        }
    }
}
