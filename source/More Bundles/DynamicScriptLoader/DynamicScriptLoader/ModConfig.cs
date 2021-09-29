/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace DynamicScriptLoader
{
    public class ModConfig
    {
        public bool OverwriteKeyBinds { get; }

        public List<Macro> SavedMacros { get; } = new List<Macro>();

        public ModConfig() { }
    }
}
