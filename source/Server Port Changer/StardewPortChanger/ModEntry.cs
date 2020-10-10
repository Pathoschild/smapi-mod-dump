/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PhantomGamers/StardewPortChanger
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using System;

namespace StardewPortChanger
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public static int ServerPort;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            ServerPort = this.Config.ServerPort;
            var harmony = HarmonyInstance.Create("phantomgamers.serverportchanger");
            var original = Type.GetType("Lidgren.Network.NetPeerConfiguration, Lidgren.Network").GetProperty("Port").GetSetMethod();
            var prefix = typeof(NetPeerConfigPatch).GetMethod("Prefix");
            harmony.Patch(original, new HarmonyMethod(prefix));
        }
    }

    internal class ModConfig
    {
        public int ServerPort { get; set; } = 24642;
    }

    public class NetPeerConfigPatch
    {
        public static void Prefix(ref int value)
        {
            value = ModEntry.ServerPort;
        }
    }
}