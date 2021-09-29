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
using StardewModdingAPI;

namespace GenericModConfigMenu.Framework
{
    internal class ModConfig
    {
        /// <summary>The name of the mod which registered the mod configuration.</summary>
        public string ModName => this.ModManifest.Name;

        /// <summary>The manifest for the mod which registered the mod configuration.</summary>
        public IManifest ModManifest { get; }
        public Action RevertToDefault { get; }
        public Action SaveToFile { get; }
        public Dictionary<string, ModConfigPage> Options { get; } = new();

        public bool DefaultOptedIngame { get; set; } = false;

        public ModConfigPage ActiveRegisteringPage;

        public ModConfigPage ActiveDisplayPage = null;

        public bool HasAnyInGame = false;

        public ModConfig(IManifest manifest, Action revertToDefault, Action saveToFile)
        {
            this.ModManifest = manifest;
            this.RevertToDefault = revertToDefault;
            this.SaveToFile = saveToFile;
            this.Options.Add("", this.ActiveRegisteringPage = new ModConfigPage(""));
        }
    }
}
