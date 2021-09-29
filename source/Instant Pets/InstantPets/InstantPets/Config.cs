/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/InstantPets
**
*************************************************/

using System.Collections.Generic;

namespace InstantPets
{
	public class Config
	{
		internal static readonly Dictionary<string, string> ConfigPropertiesAndDescriptions = new Dictionary<string, string>
		{
			{
				nameof(Config.InstantPets),
				$"Sets pet preferences immediately and opens the new pet menu; provided you don't already have a pet."
			},
			{
				nameof(Config.NoPets),
				$"Marnie will never appear to choose your pet."
			},
			{
				nameof(Config.CatPerson),
				$"Sets pet preference to cats; or dogs if not enabled."
			},
			{
				nameof(Config.InstantCave),
				$"Sets farm cave preferences immediately; regardless of your current cave choice."
			},
			{
				nameof(Config.EmptyCave),
				$"Demetrius will never appear to choose your cave."
			},
			{
				nameof(Config.MushroomPerson),
				$"Sets cave preference to mushrooms; or fruit bats if not enabled."
			},
		};

		public bool InstantPets { get; set; } = false;
		public bool NoPets { get; set; } = false;
		public bool CatPerson { get; set; } = true;
		public bool InstantCave { get; set; } = false;
		public bool EmptyCave { get; set; } = false;
		public bool MushroomPerson { get; set; } = true;
	}
}
