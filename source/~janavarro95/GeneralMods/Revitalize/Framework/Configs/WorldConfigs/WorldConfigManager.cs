/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace Omegasis.Revitalize.Framework.Configs.WorldConfigs
{
    /// <summary>
    /// Deals with configurations that affect the game world.
    /// </summary>
    public class WorldConfigManager
    {

        public DarkerNightConfig darkerNightConfig;

        public WorldConfigManager()
        {
            this.darkerNightConfig = ConfigManager.initializeConfig<DarkerNightConfig>("Configs", "WorldConfigs", "DarkerNightConfig.json");
        }
    }
}
