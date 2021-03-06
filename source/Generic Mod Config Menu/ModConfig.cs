/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/GenericModConfigMenu
**
*************************************************/

using GenericModConfigMenu.ModOption;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericModConfigMenu
{
    internal class ModConfig
    {
        public class ModPage
        {
            public string Name { get; }
            public string DisplayName { get; set; }
            public List<Action<string, object>> ChangeHandler { get; } = new List<Action<string, object>>();
            public List<BaseModOption> Options{ get; set; } = new List<BaseModOption>();

            public ModPage(string name)
            {
                Name = name;
                DisplayName = Name;
            }
        }

        public IManifest ModManifest { get; }
        public Action RevertToDefault { get; }
        public Action SaveToFile { get; }
        public Dictionary<string, ModPage> Options { get; } = new Dictionary<string, ModPage>();

        public ModPage ActiveRegisteringPage = null;

        public ModPage ActiveDisplayPage = null;

        public ModConfig(IManifest manifest, Action revertToDefault, Action saveToFile )
        {
            ModManifest = manifest;
            RevertToDefault = revertToDefault;
            SaveToFile = saveToFile;
            Options.Add( "", ActiveRegisteringPage = new ModPage( "" ) );
        }
    }
}
