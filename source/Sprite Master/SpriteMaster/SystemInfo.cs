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

namespace SpriteMaster {
	internal static class SystemInfo {
		internal static void Dump (GraphicsDeviceManager gdm, GraphicsDevice device) {
			Debug.MessageLn("System Information:");

			try {
				Debug.MessageLn($"\tArchitecture: {(Environment.Is64BitProcess ? "x64" : "x86")}");
				Debug.MessageLn($"\tNumber of Cores: {Environment.ProcessorCount}");
			}
			catch { }

			try {
				if (!(device?.IsDisposed).GetValueOrDefault(false)) {
					var adapter = device?.Adapter;
					if (adapter != null) {
						Debug.MessageLn($"\tGraphics Adapter: {adapter}");
						Debug.MessageLn($"\tGraphics Adapter Description: {adapter.Description}");
					}
				}
			}
			catch { }
		}
	}
}
