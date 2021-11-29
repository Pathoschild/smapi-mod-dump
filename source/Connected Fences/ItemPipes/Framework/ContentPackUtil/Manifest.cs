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
using StardewModdingAPI;


namespace ItemPipes.Framework.ContentPackUtil
{
    class Manifest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public ISemanticVersion Version { get; set; }

        public ISemanticVersion MinimumApiVersion { get; set; }

        public string UniqueID { get; set; }

        public string EntryDll { get; set; }

        public IManifestContentPackFor ContentPackFor { get; set; }

        public CPManifestExtra[] Dependencies { get; set; }

        public string[] UpdateKeys { get; set; }

        public IDictionary<string, object> ExtraFields { get; set; }


        public Manifest()
        {

        }
    }
}
