/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mizzion.Stardew.Common.Integrations;
using Mizzion.Stardew.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;

namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class GMCMIntegrationForCMO
    {
        private readonly GenericModConfigMenuIntegration<CmoConfig> ConfigMenu;

        public GMCMIntegrationForCMO(IModRegistry modRegistry, IMonitor monitor, IManifest manifest,
            Func<CmoConfig> getConfig, Action reset, Action saveAndApply)
        {
            ConfigMenu = new GenericModConfigMenuIntegration<CmoConfig>(modRegistry, monitor, manifest, getConfig, reset, saveAndApply);
        }

        public void Register()
        {

        }
    }
}
