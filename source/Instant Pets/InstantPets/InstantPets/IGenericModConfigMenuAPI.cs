/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/InstantPets
**
*************************************************/

using StardewModdingAPI;
using System;

namespace InstantPets
{
	public interface IGenericModConfigMenuAPI
	{
		void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
		void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
	}
}
