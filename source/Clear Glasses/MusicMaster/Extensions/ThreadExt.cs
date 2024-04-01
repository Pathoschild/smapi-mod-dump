/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Threading;

namespace MusicMaster.Extensions;

internal static class ThreadExt {
	internal static Thread Run(ThreadStart start, bool background = false, string? name = null) {
		var thread = new Thread(start) {
			IsBackground = background,
			Name = name
		};
		thread.Start();
		return thread;
	}

	internal static Thread Run(ParameterizedThreadStart start, object obj, bool background = false, string? name = null) {
		var thread = new Thread(start) {
			IsBackground = background,
			Name = name
		};
		thread.Start(obj);
		return thread;
	}
}
