/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace RaisedGardenBeds
{
	public class ItemDefinition
	{
		/***************
		Required entries
		***************/

		/// <summary>
		/// Number of pixels to be added to the Y-offset of the garden bed dirt for it to appear 'raised'.
		/// </summary>
		public int SoilHeightAboveGround { get; set; }
		/// <summary>
		/// List of ingredients, expecting keys "Object" and "Count", where "Object" is given as a name or ID.
		/// </summary>
		public List<Dictionary<string, string>> RecipeIngredients { get; set; }

		/***************
		Optional entries
		***************/

		/// <summary>
		/// Quantity of this object produced when crafting.
		/// </summary>
		public int RecipeCraftedCount { get; set; } = 1;
		/// <summary>
		/// Whether this object is available for crafting at all times.
		/// </summary>
		public bool RecipeIsDefault { get; set; } = false;
		/// <summary>
		/// List of conditions before a non-default object may become available for crafting.
		/// Format is given as standard Stardew Valley event preconditions.
		/// Multiple conditions may be included if slash-separated ('/').
		/// </summary>
		public string RecipeConditions { get; set; } = null;
		/// <summary>
		/// Number of days before the object will be considered for breakage at the end-of-season.
		/// Value is not given as number of seasons the object will last to afford lenience for late placement.
		/// </summary>
		public int DaysToBreak { get; set; } = 0;
		/// <summary>
		/// Whether this object will build up with others to form large arrangements.
		/// </summary>
		public bool CanBeArranged { get; set; } = true;

		/********************
		Code-generated values
		********************/

		/// <summary>
		/// Pointer to the content pack this object variety is sourced from.
		/// </summary>
		[JsonIgnore]
		public IContentPack ContentPack { get; set; }
		/// <summary>
		/// Name of this object variety within its own content pack.
		/// </summary>
		[JsonIgnore]
		public string LocalName { get; set; }
		/// <summary>
		/// Key for the spritesheet this object should draw from
		/// </summary>
		[JsonIgnore]
		public string SpriteKey { get; set; }
		/// <summary>
		/// Index of this object in the common <see cref="OutdoorPot.Sprites"/> spritesheet.
		/// Has no entry in the content data file.
		/// </summary>
		[JsonIgnore]
		public int SpriteIndex { get; set; }
		/// <summary>
		/// The <see cref="ItemDefinition.RecipeIngredients"/> dictionary aggregated to a string
		/// with all "Object" fields parsed to their equivalent <see cref="StardewValley.Object.ParentSheetIndex"/>.
		/// Has no entry in the content data file.
		/// </summary>
		[JsonIgnore]
		public string ParsedRecipeIngredients { get; set; }

		/**************
		Internal values
		**************/

		internal const string DefinitionsFile = "content.json";
		internal const string SpritesFile = "sprites.png";

		/***************
		Internal methods
		***************/

		internal static string ParseRecipeIngredients(ItemDefinition data)
		{
			List<string> ingredients = new List<string>();
			foreach (Dictionary<string, string> entry in data.RecipeIngredients)
			{
				string strId = entry["Object"];
				int id = int.TryParse(strId, out int intId)
						// Base game objects may be referenced by ID
						? intId
						// Base and Json Assets objects may be referenced by name
						: Utility.fuzzyItemSearch(query: strId)?.ParentSheetIndex ?? -1;
				int quantity = int.Parse(entry["Count"]);
				ingredients.Add($"{id} {quantity}");
			}
			return string.Join(" ", ingredients);
		}
	}
}
