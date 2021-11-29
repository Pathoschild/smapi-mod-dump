/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemPipes.Framework.Model
{
    class DataModel
    {
        public List<string> ModItems { get; set; }
        public List<string> NetworkItems { get; set; }
        public List<string> PipeNames { get; set; }
        public List<string> IOPipeNames { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Items { get; set; }
        public List<string> ExtraNames { get; set; }
        public List<string> Buildings { get; set; }

    }
}
