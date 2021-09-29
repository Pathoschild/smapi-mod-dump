/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using GenericModConfigMenu.ModOption;

namespace GenericModConfigMenu.Framework
{
    internal class ModConfigPage
    {
        public string Name { get; }
        public string DisplayName { get; set; }
        public List<Action<string, object>> ChangeHandler { get; } = new();
        public List<BaseModOption> Options { get; set; } = new();

        public ModConfigPage(string name)
        {
            this.Name = name;
            this.DisplayName = this.Name;
        }
    }
}
