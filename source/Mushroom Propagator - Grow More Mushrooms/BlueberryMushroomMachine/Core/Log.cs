/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/razikh-git/BlueberryMushroomMachine
**
*************************************************/

namespace BlueberryMushroomMachine
{
	internal class Log
	{
		internal static void A(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Alert);
		}
		/// <summary>
		/// Print to log file in visible channel, or to hidden channel if debug flag is false.
		/// </summary>
		internal static void D(string str, bool isDebug = true)
		{
			ModEntry.Instance.Monitor.Log(str,
				isDebug ? StardewModdingAPI.LogLevel.Debug : StardewModdingAPI.LogLevel.Trace);
		}
		internal static void E(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Error);
		}
		internal static void I(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Info);
		}
		/// <summary>
		/// Print to log file, ignoring if debug flag is false.
		/// </summary>
		internal static void T(string str, bool isDebug = true)
		{
			if (isDebug)
				ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Trace);
		}
		internal static void W(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Warn);
		}
	}
}
