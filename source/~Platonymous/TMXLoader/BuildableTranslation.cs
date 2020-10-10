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

namespace TMXLoader
{
    public class BuildableTranslation
    {
        public string name { get; set; }

        public string set { get; set; } = "Others";

        internal string getSetName()
        {
            if (set != "Others")
                return set;
            else
                return TMXLoaderMod._instance.i18n.Get("Others");
        }

        public static BuildableTranslation FromEdit(BuildableEdit edit)
        {
            var t = new BuildableTranslation();
            t.name = edit.name;
            t.set = edit.set;

            return t;
        }

    }
}
