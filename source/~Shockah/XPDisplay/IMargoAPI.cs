/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.XPDisplay
{
	public interface IMargoAPI
	{
		IModConfig GetConfig();

		public interface IModConfig
		{
			bool EnableProfessions { get; }

			IProfessionsConfig Professions { get; }

			public interface IProfessionsConfig
			{
				bool EnablePrestige { get; }
			}
		}
	}
}
