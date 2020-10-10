/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedBag
{
    public class Config
    {
        public string Shop { get; set; } = "Pierre";
        public int Price { get; set; } = 30000;

        public SButton AndroidKey { get; set; } = SButton.B;
    }
}
