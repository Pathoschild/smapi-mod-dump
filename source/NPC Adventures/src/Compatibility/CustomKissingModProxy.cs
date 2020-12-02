/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Compatibility
{
    /// <summary>
    /// Proxy to Custom Kissing Mod by Digus for keep compatibility with this mod.
    /// </summary>
    internal class CustomKissingModProxy : ICustomKissingModApi
    {
        private const string MINIMUM_VERSION = "1.2.0";
        private readonly ICustomKissingModApi api;

        public CustomKissingModProxy(IModRegistry registry, IMonitor monitor)
        {
            IModInfo modInfo = registry.Get(ModUids.KISSINGMOD_UID);

            if (modInfo != null && modInfo.Manifest.Version.IsOlderThan(MINIMUM_VERSION))
            {
                monitor.Log($"Couldn't work correctly with Custom Kissing Mod version {modInfo.Manifest.Version} (requires >= {MINIMUM_VERSION}). Don't worry, this issue doesn't affect stability, but update is recommended :)", LogLevel.Warn);
            }

            this.api = registry.GetApi<ICustomKissingModApi>("Digus.CustomKissingMod");
        }
        public bool CanKissNpc(Farmer who, NPC npc)
        {
            bool married = Helper.IsSpouseMarriedToFarmer(npc, who);

            // Check for friendship exists here because bug in Custom Kissing Mod. 
            // Custom Kissing Mod don't check if farmer already met that NPC. 
            if (this.api != null && who.friendshipData.ContainsKey(npc.Name))
            {
                return married || this.api.CanKissNpc(who, npc);
            }

            return married;
        }

        public bool HasRequiredFriendshipToKiss(Farmer who, NPC npc)
        {
            if (this.api != null && !Helper.IsSpouseMarriedToFarmer(npc, who))
            {
                return this.api.HasRequiredFriendshipToKiss(who, npc);
            }

            return who.getFriendshipHeartLevelForNPC(npc.Name) > 9;
        }
    }

    public interface ICustomKissingModApi
    {
        bool CanKissNpc(Farmer who, NPC npc);
        bool HasRequiredFriendshipToKiss(Farmer who, NPC npc);
    }
}
