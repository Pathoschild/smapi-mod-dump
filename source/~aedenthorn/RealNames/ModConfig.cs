/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealNames
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public string LocaleString { get; set; } = "en-US";
        public string NeutralNameGender { get; set; } = "male/female";
        public bool RealNamesForAnimals { get; set; } = true;
    }
}
