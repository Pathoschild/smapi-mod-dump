/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Spacechase.Shared.Harmony;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Network;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="GameServer"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class GameServerPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<GameServer>(nameof(GameServer.sendServerIntroduction)),
                postfix: this.GetHarmonyMethod(nameof(After_SendServerIntroduction))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="GameServer.sendServerIntroduction"/>.</summary>
        private static void After_SendServerIntroduction(GameServer __instance, long peer)
        {
            SpaceEvents.InvokeServerGotClient(__instance, peer);
        }
    }
}
