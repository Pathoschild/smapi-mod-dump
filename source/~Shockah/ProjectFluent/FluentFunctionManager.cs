/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using Shockah.Kokoro;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shockah.ProjectFluent
{
	internal interface IFluentFunctionManager
	{
		void RegisterFunction(IManifest mod, string name, FluentFunction function);
		void UnregisterFunction(IManifest mod, string name);
	}

	internal class FluentFunctionManager : IFluentFunctionManager, IFluentFunctionProvider
	{
		private IList<(IManifest mod, string name, FluentFunction function)> Functions { get; set; } = new List<(IManifest mod, string name, FluentFunction function)>();

		private Regex ValidationRegex { get; set; } = new("^[A-Z][A-Z0-9_\\-]*$");

		public void RegisterFunction(IManifest mod, string name, FluentFunction function)
		{
			if (!ValidationRegex.IsMatch(name))
				throw new ArgumentException("Fluent function names can only contain uppercase letters, digits, the _, or the - character. They must also start with an uppercase letter.");
			Functions.Add((mod, name, function));
		}

		public void UnregisterFunction(IManifest mod, string name)
		{
			var index = Functions.FirstIndex(f => f.mod.UniqueID == mod.UniqueID && f.name == name);
			if (index is not null)
				Functions.RemoveAt(index.Value);
		}

		public IEnumerable<(IManifest mod, string name, FluentFunction function)> GetFluentFunctions()
			=> Functions;
	}
}