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

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class ThreadRun {
	private static readonly Func<List<Action>> ThreadingActionsGet = 
		typeof(Microsoft.Xna.Framework.Threading).
		GetFieldGetter<List<Action>>("actions") ??
			throw new NullReferenceException(nameof(ThreadingActionsGet));
	
	// TODO : find a nice, generic way to do this without [ModuleInitializer]
	private static readonly bool Functional = true;

	[Harmonize(
		typeof(Microsoft.Xna.Framework.Threading),
		"Run",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool Run() {
		if (!Functional || !Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeEngineTaskRunner) {
			return true;
		}

		var actions = ThreadingActionsGet();
		var localActions = actions.ExchangeClearLocked();

		foreach (var action in localActions) {
			action();
		}

		return false;
	}
}
