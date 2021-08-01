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
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Multiplayer"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class MultiplayerPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Multiplayer>(nameof(Multiplayer.processIncomingMessage)),
                prefix: this.GetHarmonyMethod(nameof(Before_ProcessIncomingMessage))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Multiplayer.processIncomingMessage"/>.</summary>
        private static bool Before_ProcessIncomingMessage(Multiplayer __instance, IncomingMessage msg)
        {
            // MTN uses packets 30, 31, and 50, PyTK uses 99

            if (msg.MessageType == 234)
            {
                string msgType = msg.Reader.ReadString();
                if (Networking.MessageHandlers.TryGetValue(msgType, out var handler))
                    handler.Invoke(msg);

                if (Game1.IsServer)
                {
                    foreach (long key in Game1.otherFarmers.Keys)
                    {
                        if (key != msg.FarmerID)
                            Game1.server.sendMessage(key, 234, Game1.otherFarmers[msg.FarmerID], msg.Data);
                    }
                }
            }

            return true;
        }
    }
}
