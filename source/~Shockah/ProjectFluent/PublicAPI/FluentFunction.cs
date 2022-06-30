/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace Shockah.ProjectFluent
{
	/// <summary>A delegate implementing a custom function, available to Project Fluent translations.</summary>
	/// <param name="locale">The locale the translation is being provided for.</param>
	/// <param name="mod">The mod the translation is being provided for.</param>
	/// <param name="positionalArguments">A list of positional arguments passed to the function.</param>
	/// <param name="namedArguments">A list of named arguments passed to the function.</param>
	/// <returns>The resulting value of the function.</returns>
	public delegate IFluentFunctionValue FluentFunction(
		IGameLocale locale,
		IManifest mod,
		IReadOnlyList<IFluentFunctionValue> positionalArguments,
		IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
	);
}