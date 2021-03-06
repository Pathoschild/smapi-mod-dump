/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/GenericModConfigMenu
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericModConfigMenu.ModOption
{
    internal class PageLabelModOption : BaseModOption
    {
        public string NewPage { get; }

        public override void SyncToMod()
        {
        }

        public override void Save()
        {
        }

        public PageLabelModOption( string name, string desc, string newPage, IManifest mod )
        :   base( name, desc, name, mod )
        {
            NewPage = newPage;
        }
    }
}
