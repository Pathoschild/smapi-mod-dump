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
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IContextfulFluentFunctionProvider
	{
		IEnumerable<(string name, ContextfulFluentFunction function)> GetFluentFunctionsForMod(IManifest mod);
	}

	internal delegate IFluentFunctionValue ContextfulFluentFunction(
		IGameLocale locale,
		IReadOnlyList<IFluentFunctionValue> positionalArguments,
		IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
	);

	internal class ContextfulFluentFunctionProvider: IContextfulFluentFunctionProvider
	{
		private IManifest ProjectFluentMod { get; set; }
		private IFluentFunctionProvider FluentFunctionProvider { get; set; }

		public ContextfulFluentFunctionProvider(IManifest projectFluentMod, IFluentFunctionProvider fluentFunctionProvider)
		{
			this.ProjectFluentMod = projectFluentMod;
			this.FluentFunctionProvider = fluentFunctionProvider;
		}

		public IEnumerable<(string name, ContextfulFluentFunction function)> GetFluentFunctionsForMod(IManifest mod)
		{
			var remainingFunctions = FluentFunctionProvider.GetFluentFunctions().ToList();

			var projectFluentFunctions = remainingFunctions.Where(f => f.mod.UniqueID == ProjectFluentMod.UniqueID).ToList();
			foreach (var function in projectFluentFunctions)
				remainingFunctions.Remove(function);

			var modFunctions = remainingFunctions.Where(f => f.mod.UniqueID == mod.UniqueID).ToList();
			foreach (var function in modFunctions)
				remainingFunctions.Remove(function);

			IEnumerable<(string name, ContextfulFluentFunction function)> EnumerableFunctions(IEnumerable<(IManifest mod, string name, FluentFunction function)> input)
			{
				foreach (var function in input)
				{
					IFluentFunctionValue ContextfulFunction(IGameLocale locale, IReadOnlyList<IFluentFunctionValue> positionalArguments, IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments)
						=> function.function(locale, mod, positionalArguments, namedArguments);

					yield return (function.name, ContextfulFunction);
					yield return ($"{function.mod.UniqueID.Replace(".", "_").ToUpper()}_{function.name}", ContextfulFunction);
				}
			}

			foreach (var function in EnumerableFunctions(projectFluentFunctions))
				yield return function;
			foreach (var function in EnumerableFunctions(modFunctions))
				yield return function;
			foreach (var function in EnumerableFunctions(remainingFunctions))
				yield return function;
		}
	}
}