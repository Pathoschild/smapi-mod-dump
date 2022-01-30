/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

using System;
using StardewModdingAPI;

namespace Farmtronics {
	public static class Debug {
		public static void Log(string s, object context=null) {
			ModEntry.instance.Monitor.Log(DateTime.Now.ToString("'['HH':'mm':'ss'] '") + s, LogLevel.Debug);
		}
	}
}
