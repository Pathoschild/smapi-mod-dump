/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCC.Utilities
{
    public class Requests
    {
        public static List<Request> RequestList { get; set; }
    }

    public class Request
    {
        public int itemIndex { get; set; }

        public int itemCount { get; set; }

        public int totalPrice { get; set; }

        public int CreationDate { get; set; }

        public Request(int index, int count, int price, int date)
        {
            itemIndex = index;
            itemCount = count;
            totalPrice = price;
            CreationDate = date;
        }
    }
}
