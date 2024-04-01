/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;

namespace MusicMaster;

internal static class GameConstants {
	internal static class FrameTime {
		internal const int Nanoseconds = 16_666_667; // default 60hz
		internal const int Ticks = Nanoseconds / 100;
		internal static readonly TimeSpan TimeSpan = new(Ticks);
	}
}
