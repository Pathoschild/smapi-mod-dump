/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.Game;

static class ThreadRun {
	private static readonly Func<List<Action>> ThreadingActionsGet = typeof(XNA.Color).Assembly.
	GetType("Microsoft.Xna.Framework.Threading")?.
	GetFieldGetter<List<Action>>("actions") ??
	throw new NullReferenceException("ThreadingActionsGet");

	[Harmonize(
		typeof(XNA.Color),
		"Microsoft.Xna.Framework.Threading",
		"Run",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool Run() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeEngineTaskRunner) {
			return true;
		}

		var actions = ThreadingActionsGet();
		Action[] localList;
		lock (actions) {
			localList = actions.ToArray();
			actions.Clear();
		}

		foreach (var action in localList) {
			action();
		}

		return false;
	}
}
