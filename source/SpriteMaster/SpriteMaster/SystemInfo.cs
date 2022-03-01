/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace SpriteMaster;

static class SystemInfo {
	internal static void Dump(GraphicsDeviceManager gdm, GraphicsDevice device) {
		var dumpBuilder = new StringBuilder();

		dumpBuilder.AppendLine("System Information:");

		try {
			dumpBuilder.AppendLine($"\tArchitecture: {(Environment.Is64BitProcess ? "x64" : "x86")}");
			dumpBuilder.AppendLine($"\tNumber of Cores: {Environment.ProcessorCount}");
			dumpBuilder.AppendLine($"\tOS Version: {Environment.OSVersion}");
		}
		catch { }

		try {
			if (!(device?.IsDisposed).GetValueOrDefault(false)) {
				var adapter = device?.Adapter;
				if (adapter != null) {
					dumpBuilder.AppendLine($"\tGraphics Adapter: {adapter}");
					dumpBuilder.AppendLine($"\tGraphics Adapter Description: {adapter.Description}");
				}
			}
		}
		catch { }

		Debug.Message(dumpBuilder.ToString());
	}
}
