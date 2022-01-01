/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;

namespace SullySDVcore
{
	public class Log
	{
        private readonly Mod mod;
		public Log(Mod modEntry)
		{
			mod = modEntry;
		}
		public void D(string str, bool debug)
		{
			if (debug) { mod.Monitor.Log(str, LogLevel.Debug); }
		}
		public void T(string str)
		{
			mod.Monitor.Log(str, LogLevel.Trace);
		}
		public void E(string str)
		{
			mod.Monitor.Log(str, LogLevel.Error);
			//ModEntry.Instance.Monitor.Log(str, LogLevel.Error);
		}
		public void E(string str, Exception e)
		{
			string errorMessage = e is null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";
			mod.Monitor.Log(str + errorMessage, LogLevel.Error);
		}
		public void I(string str)
		{
			mod.Monitor.Log(str, LogLevel.Info);
		}
		public void A(string str)
		{
			mod.Monitor.Log(str, LogLevel.Alert);
		}
		public void W(string str)
		{
			mod.Monitor.Log(str, LogLevel.Warn);
		}
	}
}