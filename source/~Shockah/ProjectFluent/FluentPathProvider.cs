/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using System.IO;

namespace Shockah.ProjectFluent
{
	internal interface IFluentPathProvider
	{
		IEnumerable<string> GetFilePathCandidates(IGameLocale locale, string directory, string? file);
	}

	internal class FluentPathProvider : IFluentPathProvider
	{
		public IEnumerable<string> GetFilePathCandidates(IGameLocale locale, string directory, string? file)
		{
			foreach (var relevantLocale in locale.GetRelevantLocaleCodes())
			{
				string fileNameWithoutExtension = $"{(string.IsNullOrEmpty(file) ? "" : $"{file}.")}{relevantLocale}";
				yield return Path.Combine(directory, $"{fileNameWithoutExtension}.ftl");
			}
		}
	}
}