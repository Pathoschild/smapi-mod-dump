/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

extern alias slnet;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Logging;
using StardewValley.Network;
using StardewValley.Network.Compress;
using System.Net.Sockets;

namespace IPv6.Patch;

internal static partial class MyPatch
{
    private static IModHelper helper = null!;
    public static IModHelper Helper { get { return helper; } }
    private static Harmony harmony = null!;
    public static Harmony Harmony { get { return harmony; } }

    private static IReflectedField<INetCompression> _netCompression = null!;
    public static INetCompression netCompression { get { return _netCompression.GetValue(); } }

    private static IReflectedField<IGameLogger> _log = null!;
    public static IGameLogger log { get { return _log.GetValue(); } }

    private static IReflectedField<Multiplayer> _multiplayer = null!;
    public static Multiplayer multiplayer { get { return _multiplayer.GetValue(); } }

    public static List<Server> GetServers(object gameserver) => helper.Reflection.
        GetField<List<Server>>(gameserver, "servers").GetValue();

    public enum IPMode { IPv4, IPv6, IPv4IPv6 };

    public static IPMode ChooseIPMode
    {
        get
        {
            var d4 = helper.Input.IsDown(SButton.D4);
            var d6 = helper.Input.IsDown(SButton.D6);

            if (d4 && d6)
                return IPMode.IPv4IPv6;
            else if (d4)
                return IPMode.IPv4;
            else if (d6)
                return IPMode.IPv6;
            else if (!Socket.OSSupportsIPv6 && Socket.OSSupportsIPv4)
                return IPMode.IPv4;
            else if (Socket.OSSupportsIPv6 && !Socket.OSSupportsIPv4)
                return IPMode.IPv6;
            else
                return IPMode.IPv4IPv6;
        }
    }

    private static void InitRefValues(IModHelper helper, string harmonyID)
    {
        _netCompression = helper.Reflection.GetField<INetCompression>(typeof(Program), "netCompression");
        _multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
        _log = helper.Reflection.GetField<IGameLogger>(typeof(Game1), "log");
        MyPatch.helper = helper;
        MyPatch.harmony = new(harmonyID);
    }

}
