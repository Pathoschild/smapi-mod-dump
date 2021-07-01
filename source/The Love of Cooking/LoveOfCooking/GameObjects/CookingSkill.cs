/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace LoveOfCooking.GameObjects
{
	public class CookingSkill : Skills.Skill
	{
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;
		public static readonly string InternalName = ModEntry.AssetPrefix + "CookingSkill";

		public static double GlobalExperienceRate;
		public static int MaxFoodStackPerDayForExperienceGains;
		public static int CraftNettleTeaLevel;

		public static int GiftBoostValue;
		public static float SalePriceModifier;
		public static float ExtraPortionChance;
		public static int RestorationValue;
		public static int RestorationAltValue;
		public static int BuffRateValue;
		public static int BuffDurationValue;

		public static float BurnChanceReduction;
		public static float BurnChanceModifier;

		public static readonly List<string> StartingRecipes = new List<string>();
		public static readonly Dictionary<int, List<string>> CookingSkillLevelUpRecipes = new Dictionary<int, List<string>>();
		public static readonly Dictionary<string, int> FoodsThatBuffCookingSkill = new Dictionary<string, int>();

		public class SkillProfession : Profession
		{
			public SkillProfession(Skills.Skill skill, string theId) : base(skill, theId) {}
	            
			internal string Name { get; set; }
			internal string Description { get; set; }
			public override string GetName() { return Name; }
			public override string GetDescription() { return Description; }
		}

		public CookingSkill() : base(InternalName)
		{
			Log.D($"Registering skill {InternalName}",
				ModEntry.Instance.Config.DebugMode);

			// Read class values from definitions data file
			var cookingSkillValues = Game1.content.Load<Dictionary<string, string>>(ModEntry.GameContentSkillValuesPath);
			foreach (var field in this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
			{
				System.Type type = field.GetValue(this).GetType();
				if (type == typeof(int))
					field.SetValue(this, int.Parse(cookingSkillValues[field.Name]));
				else if (type == typeof(float))
					field.SetValue(this, float.Parse(cookingSkillValues[field.Name]));
				else if (type == typeof(double))
					field.SetValue(this, double.Parse(cookingSkillValues[field.Name]));
			}

			// Read cooking skill level up recipes from data file
			var cookingSkillLevelUpTable = Game1.content.Load<Dictionary<string, List<string>>>(ModEntry.GameContentSkillRecipeTablePath);
			foreach (var pair in cookingSkillLevelUpTable)
			{
				CookingSkillLevelUpRecipes.Add(int.Parse(pair.Key), pair.Value);
			}

			// Read starting recipes from general data file
			foreach (var entry in ModEntry.ItemDefinitions["StartingRecipes"])
			{
				StartingRecipes.Add(entry);
			}

			// Set experience values
			var experienceBarColourSplit = cookingSkillValues["ExperienceBarColor"].Split(' ').ToList().ConvertAll(int.Parse);
			ExperienceBarColor = new Color(experienceBarColourSplit[0], experienceBarColourSplit[1], experienceBarColourSplit[2]);
			ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 }; // default
			// Set the skills page icon (cookpot)
			var size = 10;
			var texture = new Texture2D(Game1.graphics.GraphicsDevice, size, size);
			var pixels = new Color[size * size];
			ModEntry.SpriteSheet.GetData(0, new Rectangle(69, 220, size, size), pixels, 0, pixels.Length);
			texture.SetData(pixels);
			SkillsPageIcon = texture;

			// Set the skill level-up icon (pot on table)
			size = 16;
			texture = new Texture2D(Game1.graphics.GraphicsDevice, size, size);
			pixels = new Color[size * size];
			ModEntry.SpriteSheet.GetData(0, new Rectangle(0, 272, size, size), pixels, 0, pixels.Length);
			texture.SetData(pixels);
			Icon = texture;

			// Populate skill professions
			const string professionIdTemplate = "menu.cooking_skill.tier{0}_path{1}{2}";
			var textures = new Texture2D[6];
			for (var i = 0; i < textures.Length; ++i)
			{
				var x = 16 + (i * 16); // <-- Which profession icon to use is decided here
				ModEntry.SpriteSheet.GetData(0, new Rectangle(x, 272, size, size), pixels, 0, pixels.Length); // Pixel data copied from spritesheet
				textures[i] = new Texture2D(Game1.graphics.GraphicsDevice, size, size); // Unique texture created, no shared references
				textures[i].SetData(pixels); // Texture has pixel data applied

				// Set metadata for this profession
				var id = string.Format(professionIdTemplate,
					i < 2 ? 1 : 2, // Tier
					i / 2 == 0 ? i + 1 : i / 2, // Path
					i < 2 ? "" : i % 2 == 0 ? "a" : "b"); // Choice
				var extra = i == 1 && !ModEntry.Instance.Config.FoodHealingTakesTime ? "_alt" : "";
				var profession = new SkillProfession(this, id)
				{
					Icon = textures[i], // <-- Skill profession icon is applied here
					Name = i18n.Get($"{id}{extra}.name"),
					Description = i18n.Get($"{id}{extra}.description",
					new { // v-- Skill profession description values are tokenised here
						SaleValue = (SalePriceModifier - 1) * 100,
						RestorationAltValue = RestorationAltValue,
					})
				};
				// Skill professions are paired and applied
				Professions.Add(profession);
				if (i > 0 && i % 2 == 1)
					ProfessionsForLevels.Add(new ProfessionPair(ProfessionsForLevels.Count == 0 ? 5 : 10,
						Professions[i - 1], Professions[i]));
			}
		}

		public override string GetName()
		{
			return i18n.Get("menu.cooking_recipe.buff.12");
		}
		
		public override List<string> GetExtraLevelUpInfo(int level)
		{
			var list = new List<string>();
			if (ModEntry.Instance.Config.FoodCanBurn)
				list.Add(i18n.Get("menu.cooking_skill.levelup_burn", new { Number = level * BurnChanceModifier * BurnChanceReduction }));

			var extra = i18n.Get($"menu.cooking_skill.levelupbonus.{level}");
			if (extra.HasValue() && (level != CraftNettleTeaLevel || ModEntry.NettlesEnabled))
				list.Add(extra);

			return list;
		}

		public override string GetSkillPageHoverText(int level)
		{
			var str = "";

			if (ModEntry.Instance.Config.FoodCanBurn)
				str += "\n" + i18n.Get("menu.cooking_skill.levelup_burn", new { Number = level * BurnChanceModifier * BurnChanceReduction });

			return str;
		}
	}
}
