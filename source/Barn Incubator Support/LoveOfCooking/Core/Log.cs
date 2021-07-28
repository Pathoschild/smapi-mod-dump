/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace LoveOfCooking
{
	public static class Log
	{
		public static void D(string str, bool isDebug=true)
		{
			ModEntry.Instance.Monitor.Log(str,
				isDebug ? StardewModdingAPI.LogLevel.Debug : StardewModdingAPI.LogLevel.Trace);
		}
		public static void A(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Alert);
		}
		public static void E(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Error);
		}
		public static void I(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Info);
		}
		public static void T(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Trace);
		}
		public static void W(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Warn);
		}
	}
}
