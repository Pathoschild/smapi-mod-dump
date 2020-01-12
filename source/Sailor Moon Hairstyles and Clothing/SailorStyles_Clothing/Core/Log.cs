namespace SailorStyles_Clothing
{
	class Log
	{
		internal static void D(string str, bool isDebug=true)
		{
			ModEntry.Instance.Monitor.Log(str,
				isDebug ? StardewModdingAPI.LogLevel.Debug : StardewModdingAPI.LogLevel.Trace);
		}
		internal static void A(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Alert);
		}
		internal static void E(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Error);
		}
		internal static void I(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Info);
		}
		internal static void T(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Trace);
		}
		internal static void W(string str)
		{
			ModEntry.Instance.Monitor.Log(str, StardewModdingAPI.LogLevel.Warn);
		}
	}
}