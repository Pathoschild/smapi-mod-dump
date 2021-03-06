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

namespace LoveOfCooking.GameObjects
{
	public class CookingSkill : Skills.Skill
	{
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;
		public static readonly string InternalName = ModEntry.AssetPrefix + "CookingSkill";
		protected static readonly string ProfessionI18nId = "menu.cooking_skill.tier{0}_path{1}{2}";
		internal const float DebugExperienceRate = 1f;

		public static readonly int GiftBoostValue = 10;
		public static readonly int SaleValue = 30;
		public static readonly int ExtraPortionChance = 4;
		public static readonly int RestorationValue = 35;
		public static readonly int RestorationAltValue = 5;
		public static readonly int BuffRateValue = 3;
		public static readonly int BuffDurationValue = 36;

		public static readonly float BurnChanceReduction = 0.015f;
		public static readonly float BurnChanceModifier = 1.5f;

		public int AddedLevel;

		public static readonly List<string> StartingRecipes = new List<string>
		{
			"Fried Egg",
			ModEntry.ObjectPrefix + "bakedpotato"
		};
		public static readonly Dictionary<int, List<string>> CookingSkillLevelUpRecipes = new Dictionary<int, List<string>>
		{
			{ 0, new List<string>() },
			{ 1, new List<string> { "burrito", "fritters" } },
			{ 2, new List<string> { "porridge", "breakfast" } },
			{ 3, new List<string> { "lobster", "loadedpotato", "stuffedpotato" } },
			{ 4, new List<string> { "cake", "hotcocoa", "waffles", "mornay" } },
			{ 5, new List<string>() },
			{ 6, new List<string> { "burger", "unagi", "cabbagepot", "stew" } }, // "redberrypie", 
			{ 7, new List<string> { "applepie", "eggsando", "skewers", "tropicalsalad" } }, // "burger", 
			{ 8, new List<string> { "admiralpie", "saladsando", "dwarfstew", "roast", "hunters" } },
			{ 9, new List<string> { "gardenpie", "seafoodsando", "curry", "kebab", "oceanplatter" } },
			{ 10, new List<string>() },
		};

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

			// Set experience values
			ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 }; // default
			ExperienceBarColor = new Color(57, 135, 214);

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
			var textures = new Texture2D[6];
			for (var i = 0; i < textures.Length; ++i)
			{
				var x = 16 + (i * 16); // <-- Which profession icon to use is decided here
				ModEntry.SpriteSheet.GetData(0, new Rectangle(x, 272, size, size), pixels, 0, pixels.Length); // Pixel data copied from spritesheet
				textures[i] = new Texture2D(Game1.graphics.GraphicsDevice, size, size); // Unique texture created, no shared references
				textures[i].SetData(pixels); // Texture has pixel data applied

				// Set metadata for this profession
				var id = string.Format(ProfessionI18nId,
					i < 2 ? 1 : 2, // Tier
					i / 2 == 0 ? i + 1 : i / 2, // Path
					i < 2 ? "" : i % 2 == 0 ? "a" : "b"); // Choice
				var extra = i == 1 && !ModEntry.Instance.Config.FoodHealingTakesTime ? "_alt" : "";
				var profession = new SkillProfession(this, id)
				{
					Icon = textures[i], // <-- Skill profession icon is applied here
					Name = i18n.Get($"{id}{extra}.name"),
					Description = i18n.Get($"{id}{extra}.description", new {SaleValue, RestorationAltValue})
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
			if (extra.HasValue() && (level != ModEntry.CraftNettleTeaLevel || ModEntry.NettlesEnabled))
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
