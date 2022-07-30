/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace IndustrialFurnace.Data
{
	/// <summary>
	/// The blueprint data class.
	/// </summary>
	public class Blueprint
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? BlueprintType { get; set; }
		public string? NameOfBuildingToUpgrade { get; set; }
		public string? MaxOccupants { get; set; }
		public string? Width { get; set; }
		public string? Height { get; set; }
		public string? HumanDoorX { get; set; }
		public string? HumanDoorY { get; set; }
		public string? AnimalDoorX { get; set; }
		public string? AnimalDoorY { get; set; }
		public string? MapToWarpTo { get; set; }
		public string? SourceRectForMenuViewX { get; set; }
		public string? SourceRectForMenuViewY { get; set; }
		public string? ActionBehaviour { get; set; }
		public string? NamesOfBuildingLocations { get; set; }
		public string? Magical { get; set; }
		public string? DaysToBuild { get; set; }
		public string? MoneyRequired { get; set; }
		public List<RequiredItem> ItemsRequired { get; set; }


		public Blueprint()
		{
			ItemsRequired = new List<RequiredItem>();
		}


		/// <summary>Convert the blueprint data to a string stardew valley understands, and replace the name and description with the current language variants, if present.</summary>
		/// <param name="i18n"></param>
		/// <returns>The blueprint in the format Stardew Valley understands</returns>
		public string ToBlueprintString(ITranslationHelper i18n)
		{
			string s;

			string items = string.Join(" ", ItemsRequired);

#pragma warning disable CS8601 // Possible null reference assignment.
			s = string.Join("/", new string[] {items, Width, Height, HumanDoorX, HumanDoorY, AnimalDoorX, AnimalDoorY, MapToWarpTo, i18n.Get("industrial-furnace.name"), i18n.Get("industrial-furnace.description"),
				BlueprintType, NameOfBuildingToUpgrade, SourceRectForMenuViewX, SourceRectForMenuViewY, MaxOccupants, ActionBehaviour, NamesOfBuildingLocations, MoneyRequired, Magical, DaysToBuild});
#pragma warning restore CS8601 // Possible null reference assignment.

			return s;
		}
	}
}
