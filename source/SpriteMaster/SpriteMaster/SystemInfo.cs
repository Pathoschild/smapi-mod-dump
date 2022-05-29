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
using SpriteMaster.Extensions;
using System;
using System.Text;

namespace SpriteMaster;

internal static class SystemInfo {
	internal static void Dump(GraphicsDeviceManager gdm, GraphicsDevice device) {
		var dumpBuilder = new StringBuilder();

		dumpBuilder.AppendLine("System Information:");

		try {
			dumpBuilder.AppendLine($"\tArchitecture: {(Environment.Is64BitProcess ? "x64" : "x86")}");
			dumpBuilder.AppendLine($"\tNumber of Cores: {Environment.ProcessorCount}");
			dumpBuilder.AppendLine($"\tOS Version: {Environment.OSVersion}");
		}
		catch {
			// ignored
		}

		try {
			var memoryInfo = GC.GetGCMemoryInfo();
			dumpBuilder.AppendLine($"\tTotal Committed Memory: {memoryInfo.TotalCommittedBytes.AsDataSize()}");
			dumpBuilder.AppendLine($"\tTotal Available Memory: {memoryInfo.TotalAvailableMemoryBytes.AsDataSize()}");
		}
		catch {
			// ignored
		}

		try {
			if (!device.IsDisposed) {
				var adapter = device?.Adapter;
				if (adapter is not null) {
					dumpBuilder.AppendLine($"\tGraphics Adapter: {adapter}");
					dumpBuilder.AppendLine($"\tGraphics Adapter Description: {adapter.Description}");
				}
			}
		}
		catch {
			// ignored
		}

		Debug.Message(dumpBuilder.ToString());
	}
}
