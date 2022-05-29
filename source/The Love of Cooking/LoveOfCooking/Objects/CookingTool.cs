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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LoveOfCooking.Objects
{
	/// <summary>
	/// Simple extension of <see cref="StardewValley.Tools.GenericTool"/> to include some custom behaviours.
	/// <para/>
	/// While there's never an instance of <see cref="CookingTool"/> given to the player in the same way as a non-generic <see cref="StardewValley.Tool"/> instance,
	/// this is still instantiated for the persistent <see cref="StardewValley.Farmer.toolBeingUpgraded"/> value.
	/// <para/>
	/// The main reason for creating this class is to include an override for <see cref="StardewValley.Tool.drawInMenu"/>,
	/// so that we don't need to awkwardly patch our <see cref="AssetManager.CookingToolIconArea"/> slice from <see cref="ModEntry.SpriteSheet"/>
	/// into the base game <see cref="Game1.toolSpriteSheet"/> texture, which causes ugly conflicts when patched by other mods.
	/// <para/>
	/// A convenient side-effect of having this class is that we can replace our whole static class Tools with an instance
	/// containing all the same behaviours, only with less of all the jank.
	/// </summary>
	[XmlType("Mods_blueberry_cac_cookingtool")] // SpaceCore serialisation signature
	public class CookingTool : StardewValley.Tools.GenericTool
	{
		public const string InternalName = ModEntry.AssetPrefix + "cookingtool"; // DO NOT EDIT
		public const int DaysToUpgrade = 2;
		public const int MaxUpgradeLevel = 4;
		public const int MinIngredients = 1;
		public const int MaxIngredients = 6;
		public const int LegacyCookingToolSheetIndex = 17; // DEPRECATED


		/// <summary>
		/// Parameterless constructor for XML serialisation using <see cref="SpaceCore.Api.RegisterSerializerType"/>.
		/// </summary>
		public CookingTool()
			: this(upgradeLevel: CookingTool.GetEffectiveGlobalToolUpgradeLevel())
		{}

		/// <summary>
		/// please instantiate with <see cref="CookingTool.Create"/> method thanks bye
		/// </summary>
		/// <param name="upgradeLevel"></param>
		public CookingTool(int upgradeLevel)
			: base(
				  name: CookingTool.InternalName,
				  description: CookingTool.CookingToolDescription(upgradeLevel: upgradeLevel),
				  upgradeLevel: upgradeLevel,
				  parentSheetIndex: -1,
				  menuViewIndex: -1)
		{
			// hello this is a new class thanks
			// do not bully
		}

		/// <summary>
		/// Creates a *functional* <see cref="CookingTool"/> with the <paramref name="upgradeLevel"/> *correctly* applied to the result.
		/// </summary>
		public static CookingTool Create(int upgradeLevel)
		{
			CookingTool cookingTool = new CookingTool(upgradeLevel: upgradeLevel)
			{
				UpgradeLevel = upgradeLevel
			};
			return cookingTool;
		}

		/// <summary>
		/// Performs most behaviours from <see cref="StardewValley.Tool.actionWhenPurchased"/>, 
		/// </summary>
		public override bool actionWhenPurchased()
		{
			Game1.player.toolBeingUpgraded.Value = this;
			Game1.player.daysLeftForToolUpgrade.Value = ModEntry.Config.DebugMode ? 0 : CookingTool.DaysToUpgrade;
			Game1.playSound("parry");
			Game1.exitActiveMenu();
			Game1.drawDialogue(Game1.getCharacterFromName("Clint"),
			Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
			return true;
		}

		/// <summary>
		/// Adds custom behaviour for receiving an upgraded <see cref="CookingTool"/> from the Blacksmith.
		/// </summary>
		public override void actionWhenClaimed()
		{
			++ModEntry.Instance.States.Value.CookingToolLevel;
			base.actionWhenClaimed();
		}

		/// <summary>
		/// Checks whether this item will appear in shops it has been added to.
		/// <para/>
		/// See also: <seealso cref="AddToShopStock"/>
		/// </summary>
		public override bool CanBuyItem(Farmer who)
		{
			return this.UpgradeLevel < CookingTool.MaxUpgradeLevel || base.CanBuyItem(who);
		}

		/// <summary>
		/// Replace base behaviour by returning an instance of <see cref="CookingTool"/>.
		/// </summary>
		public override Item getOne()
		{
			CookingTool tool = new CookingTool(upgradeLevel: this.UpgradeLevel);
			tool._GetOneFrom(this);
			return tool;
		}

		/// <summary>
		/// Replace base behaviour by drawing from our custom <see cref="ModEntry.SpriteSheet"/> instead of <see cref="Game1.toolSpriteSheet"/>.
		/// </summary>
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize,
			float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			const int size = 16;
			spriteBatch.Draw(
				texture: ModEntry.SpriteSheet,
				position: location + new Vector2(size) / 2 * Game1.pixelZoom,
				sourceRectangle: CookingTool.CookingToolSourceRectangle(upgradeLevel: this.UpgradeLevel),
				color: color * transparency,
				rotation: 0f,
				origin: new Vector2(size) / 2,
				scale: Game1.pixelZoom * scaleSize,
				effects: SpriteEffects.None,
				layerDepth: layerDepth);
		}

		protected override string loadDisplayName()
		{
			return CookingTool.CookingToolDisplayName();
		}

		protected override string loadDescription()
		{
			return CookingToolDescription(upgradeLevel: this.UpgradeLevel);
		}

		public static bool CanBeUpgraded()
		{
			bool hasMail = Game1.player.mailReceived.Contains(ModEntry.MailCookbookUnlocked);
			bool isNotMaxLevel = ModEntry.Instance.States.Value.CookingToolLevel < CookingTool.MaxUpgradeLevel;
			return hasMail && isNotMaxLevel;
		}

		/// <summary>
		/// Checks whether any item is effectively considered a cooking tool, including legacy implementations.
		/// </summary>
		public static bool IsItemCookingTool(ISalable item)
		{
			bool isCookingTool = item is CookingTool;
			bool isLegacyTool = !isCookingTool
				&& item is not null
				&& item is StardewValley.Tools.GenericTool tool
				&& tool.IndexOfMenuItemView - CookingTool.LegacyCookingToolSheetIndex is >= 0 and < CookingTool.MaxUpgradeLevel;
			return isCookingTool || isLegacyTool;
		}

		/// <summary>
		/// Returns a valid upgrade level usable anywhere expecting a value representing a <see cref="StardewValley.Tool.UpgradeLevel"/>.
		/// </summary>
		public static int GetEffectiveGlobalToolUpgradeLevel()
		{
			// With tool progression disabled, the effective upgrade level will always be the maximum value
			int upgradeLevel = (ModEntry.Config.AddCookingToolProgression && ModEntry.Instance.States.Value.CookingToolLevel < CookingTool.MaxUpgradeLevel)
				? Math.Max(0, Math.Min(CookingTool.MaxUpgradeLevel, ModEntry.Instance.States.Value.CookingToolLevel))
				: CookingTool.MaxUpgradeLevel;
			return upgradeLevel;
		}

		/// <summary>
		/// Returns the effective usable ingredients count for the <see cref="CookingMenu"/> based on some <paramref name="upgradeLevel"/>.
		/// </summary>
		public static int GetIngredientsSlotsForToolUpgradeLevel(int upgradeLevel)
		{
			return upgradeLevel < CookingTool.MaxUpgradeLevel
				? CookingTool.MinIngredients + upgradeLevel
				: CookingTool.MaxIngredients;
		}

		public static string CookingToolDescription(int upgradeLevel)
		{
			return ModEntry.Instance.i18n.Get("menu.cooking_equipment.description", new { level = upgradeLevel + 1 }).ToString();
		}

		/// <summary>
		/// Returns the base display name of the tool, excluding any qualifiers.
		/// </summary>
		public static string CookingToolDisplayName()
		{
			return ModEntry.Instance.i18n.Get("menu.cooking_equipment.name");
		}

		/// <summary>
		/// Returns the full display name of the tool, including qualifiers.
		/// </summary>
		public static string CookingToolQualityDisplayName(int upgradeLevel)
		{
			string localisedName = CookingToolDisplayName();
			string displayName = string.Format(Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs." + (14299 + upgradeLevel - 1)),
				localisedName);
			return upgradeLevel is < 1 or > CookingTool.MaxUpgradeLevel ? localisedName : displayName;
		}

		/// <summary>
		/// Returns the source rectangle used when drawing the tool, respective of its <paramref name="upgradeLevel"/>.
		/// </summary>
		public static Rectangle CookingToolSourceRectangle(int upgradeLevel)
		{
			Rectangle source = AssetManager.CookingToolIconArea;
			source.X += upgradeLevel * source.Width;
			return source;
		}

		public static void AddToShopStock(Dictionary<ISalable, int[]> itemPriceAndStock, Farmer who)
		{
			// Upgrade cooking equipment at the blacksmith
			if (ModEntry.Config.AddCookingToolProgression && who == Game1.player && CookingTool.CanBeUpgraded())
			{
				int quantity = 1;
				int upgradeLevel = ModEntry.Instance.States.Value.CookingToolLevel + 1;
				int upgradePrice = ModEntry.Instance.Helper.Reflection.GetMethod(
					typeof(Utility), "priceForToolUpgradeLevel")
					.Invoke<int>(upgradeLevel);
				int extraMaterialIndex = ModEntry.Instance.Helper.Reflection.GetMethod(
					typeof(Utility), "indexOfExtraMaterialForToolUpgrade")
					.Invoke<int>(upgradeLevel);
				itemPriceAndStock.Add(
					CookingTool.Create(upgradeLevel: upgradeLevel),
					new int[] { upgradePrice / 2, quantity, extraMaterialIndex });
			}
		}
	}
}
