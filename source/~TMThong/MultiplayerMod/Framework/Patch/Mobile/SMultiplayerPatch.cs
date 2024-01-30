/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using MultiplayerMod.Framework.Network;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.SDKs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    internal class SMultiplayerPatch : IPatch
    {
        private readonly Type PATCH_TYPE = typeof(IMultiplayerPeer).Assembly.GetType("StardewModdingAPI.Framework.SMultiplayer");
        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "InitClient"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_InitClient)));
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "InitServer"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_InitServer)));
        }
        public static bool prefix_InitClient(Client client, ref Client __result, Multiplayer __instance)
        {
            switch (client)
            {
                case Network.LidgrenClient:
                    {
                        string address = ModUtilities.Helper.Reflection.GetField<string?>(client, "address").GetValue() ?? throw new InvalidOperationException("Can't initialize base networking client: no valid address found.");
                        var OnClientProcessingMessageMethod = __instance.GetType().GetMethod("OnClientProcessingMessage");
                        var OnClientSendingMessageMethod = __instance.GetType().GetMethod("OnClientSendingMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        __result = new SLidgrenClient(address, onProcessingMessage: (message, sendMessage, resume) =>
                        {
                            OnClientProcessingMessageMethod.Invoke(__instance, new object[] { message, sendMessage, resume });
                        }, (message, sendMessage, resume) =>
                        {
                            OnClientSendingMessageMethod.Invoke(__instance, new object[] { message, sendMessage, resume });

                        });
                        return true;
                    }
            }
            return false;
        }

        /// <summary>Initialize a server before the game connects to an incoming player.</summary>
        /// <param name="server">The server to initialize.</param>
        public static bool prefix_InitServer(Server server, ref Server __result, Multiplayer __instance)
        {
            switch (server)
            {
                case Network.LidgrenServer:
                    {
                        IGameServer gameServer = ModUtilities.Helper.Reflection.GetField<IGameServer?>(server, "gameServer").GetValue() ?? throw new InvalidOperationException("Can't initialize base networking client: the required 'gameServer' field wasn't found.");
                        var OnServerProcessingMessageMethod = __instance.GetType().GetMethod("OnServerProcessingMessage");
                        __result = new SLidgrenServer(gameServer, __instance, (message, sendMessage, resume) =>
                        {
                            OnServerProcessingMessageMethod.Invoke(__instance, new object[] { message, sendMessage, resume });

                        });
                        return true;
                    }
            }
            return false;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
