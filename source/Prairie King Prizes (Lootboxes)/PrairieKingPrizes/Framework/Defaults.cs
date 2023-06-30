/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialRenny/PrairieKingPrizes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrairieKingPrizes.Framework
{
    internal class Defaults
    {
        public static Prize[] DefaultCommonItems { get; set; } = new Prize[]
        {
            new Prize { ItemId = 495, Quantity = 5 },
            new Prize { ItemId = 496, Quantity = 5 },
            new Prize { ItemId = 497, Quantity = 5 },
            new Prize { ItemId = 498, Quantity = 5 },
            new Prize { ItemId = 390, Quantity = 30 },
            new Prize { ItemId = 388, Quantity = 30 },
            new Prize { ItemId = 441, Quantity = 5 },
            new Prize { ItemId = 463, Quantity = 5 },
            new Prize { ItemId = 464, Quantity = 5 },
            new Prize { ItemId = 465, Quantity = 5 },
            new Prize { ItemId = 535, Quantity = 5 },
            new Prize { ItemId = 709, Quantity = 15 }
        };

        public static Prize[] DefaultUncommonItems { get; set; } = new Prize[]
        {
            new Prize { ItemId = 88, Quantity = 3 },
            new Prize { ItemId = 301, Quantity = 3 },
            new Prize { ItemId = 302, Quantity = 3 },
            new Prize { ItemId = 431, Quantity = 3 },
            new Prize { ItemId = 453, Quantity = 3 },
            new Prize { ItemId = 472, Quantity = 3 },
            new Prize { ItemId = 473, Quantity = 3 },
            new Prize { ItemId = 475, Quantity = 3 },
            new Prize { ItemId = 477, Quantity = 3 },
            new Prize { ItemId = 478, Quantity = 3 },
            new Prize { ItemId = 479, Quantity = 3 },
            new Prize { ItemId = 480, Quantity = 3 },
            new Prize { ItemId = 481, Quantity = 3 },
            new Prize { ItemId = 482, Quantity = 3 },
            new Prize { ItemId = 483, Quantity = 3 },
            new Prize { ItemId = 484, Quantity = 3 },
            new Prize { ItemId = 485, Quantity = 3 },
            new Prize { ItemId = 487, Quantity = 3 },
            new Prize { ItemId = 488, Quantity = 3 },
            new Prize { ItemId = 489, Quantity = 3 },
            new Prize { ItemId = 490, Quantity = 3 },
            new Prize { ItemId = 491, Quantity = 3 },
            new Prize { ItemId = 492, Quantity = 3 },
            new Prize { ItemId = 493, Quantity = 3 },
            new Prize { ItemId = 494, Quantity = 3 },
            new Prize { ItemId = 466, Quantity = 3 },
            new Prize { ItemId = 340, Quantity = 3 },
            new Prize { ItemId = 724, Quantity = 3 },
            new Prize { ItemId = 725, Quantity = 3 },
            new Prize { ItemId = 726, Quantity = 3 },
            new Prize { ItemId = 536, Quantity = 3 },
            new Prize { ItemId = 537, Quantity = 3 },
            new Prize { ItemId = 335, Quantity = 3 }
        };

        public static Prize[] DefaultRareItems { get; set; } = new Prize[]
        {
            new Prize { ItemId = 72, Quantity = 2 },
            new Prize { ItemId = 337, Quantity = 2 },
            new Prize { ItemId = 417, Quantity = 2 },
            new Prize { ItemId = 305, Quantity = 2 },
            new Prize { ItemId = 308, Quantity = 2 },
            new Prize { ItemId = 336, Quantity = 2 },
            new Prize { ItemId = 787, Quantity = 2 },
            new Prize { ItemId = 710, Quantity = 2 },
            new Prize { ItemId = 413, Quantity = 2 },
            new Prize { ItemId = 430, Quantity = 2 },
            new Prize { ItemId = 433, Quantity = 2 },
            new Prize { ItemId = 437, Quantity = 2 },
            new Prize { ItemId = 444, Quantity = 2 },
            new Prize { ItemId = 446, Quantity = 2 },
            new Prize { ItemId = 439, Quantity = 2 },
            new Prize { ItemId = 680, Quantity = 2 },
            new Prize { ItemId = 749, Quantity = 2 },
            new Prize { ItemId = 797, Quantity = 2 },
            new Prize { ItemId = 486, Quantity = 2 },
            new Prize { ItemId = 681, Quantity = 2 },
            new Prize { ItemId = 690, Quantity = 2 },
            new Prize { ItemId = 688, Quantity = 2 },
            new Prize { ItemId = 689, Quantity = 2 },
        };

        public static Prize[] DefaultCovetedItems { get; set; } = new Prize[]
        {
            new Prize { ItemId = 499, Quantity = 1 },
            new Prize { ItemId = 347, Quantity = 1 },
            new Prize { ItemId = 417, Quantity = 1 },
            new Prize { ItemId = 163, Quantity = 1 },
            new Prize { ItemId = 166, Quantity = 1 },
            new Prize { ItemId = 107, Quantity = 1 },
            new Prize { ItemId = 341, Quantity = 1 },
            new Prize { ItemId = 645, Quantity = 1 },
            new Prize { ItemId = 789, Quantity = 1 },
            new Prize { ItemId = 520, Quantity = 1 },
            new Prize { ItemId = 682, Quantity = 1 },
            new Prize { ItemId = 585, Quantity = 1 },
            new Prize { ItemId = 586, Quantity = 1 },
            new Prize { ItemId = 587, Quantity = 1 },
            new Prize { ItemId = 373, Quantity = 1 },
        };

        public static Prize[] DefaultLegendaryItems { get; set; } = new Prize[]
        {
            new Prize { ItemId = 74, Quantity = 1 },
        };
    }
}
