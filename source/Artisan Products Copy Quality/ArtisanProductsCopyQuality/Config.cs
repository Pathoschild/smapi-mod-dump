/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/justastranger/ArtisanProductsCopyQuality
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtisanProductsCopyQuality
{
    internal class Config
    {
        public HashSet<string> machinesToTarget { get; set; }

        public Config()
        {
            machinesToTarget = new() { "(BC)17", "(BC)12", "(BC)15", "(BC)16", "(BC)24", "(BC)19", "(BC)Dehydrator", "(BC)FishSmoker" };
        }
    }
}
