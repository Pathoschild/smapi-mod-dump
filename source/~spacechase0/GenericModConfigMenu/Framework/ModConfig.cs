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
using StardewModdingAPI;

namespace GenericModConfigMenu.Framework
{
    internal class ModConfig
    {
        public class ModPage
        {
            public string Name { get; }
            public string DisplayName { get; set; }
            public List<Action<string, object>> ChangeHandler { get; } = new();
            public List<BaseModOption> Options { get; set; } = new();

            public ModPage(string name)
            {
                this.Name = name;
                this.DisplayName = this.Name;
            }
        }

        public IManifest ModManifest { get; }
        public Action RevertToDefault { get; }
        public Action SaveToFile { get; }
        public Dictionary<string, ModPage> Options { get; } = new();

        public bool DefaultOptedIngame { get; set; } = false;

        public ModPage ActiveRegisteringPage;

        public ModPage ActiveDisplayPage = null;

        public bool HasAnyInGame = false;

        public ModConfig(IManifest manifest, Action revertToDefault, Action saveToFile)
        {
            this.ModManifest = manifest;
            this.RevertToDefault = revertToDefault;
            this.SaveToFile = saveToFile;
            this.Options.Add("", this.ActiveRegisteringPage = new ModPage(""));
        }
    }
}
