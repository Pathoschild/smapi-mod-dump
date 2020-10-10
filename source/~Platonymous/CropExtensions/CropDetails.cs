/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CropExtensions
{
    public class CropDetails
    {
        public string[] Seasons { get; set; } = new string[] { "default", "default" };
        public int[] Days { get; set; } = new int[] { 0, 0 };
    }
}
