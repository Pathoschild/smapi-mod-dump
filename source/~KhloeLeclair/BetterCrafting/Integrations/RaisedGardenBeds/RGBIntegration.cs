/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Integrations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Integrations.RaisedGardenBeds {
	public class RGBIntegration : BaseIntegration<ModEntry>, IRecipeProvider {

		private readonly IReflectionHelper Helper;

		// ModEntry
		public readonly Type Entry;
		private readonly IReflectedField<IDictionary> _ItemDefinitions;
		private readonly IReflectedField<Dictionary<string, Texture2D>> _Sprites;

		// ItemDefinition
		public readonly Type ItemDefinition;

		// GardenPot
		public readonly Type OutdoorPot;
		public readonly string GenericName;

		private readonly ConstructorInfo _Constructor;
		private readonly IReflectedMethod _GetVariantKeyFromName;
		private readonly IReflectedMethod _GetDisplayNameFromVariantKey;
		private readonly IReflectedMethod _GetRawDescription;
		private readonly MethodInfo _GetSpriteSourceRectangle;

		public RGBIntegration(ModEntry mod)
		: base(mod, "blueberry.RaisedGardenBeds", "1.0.2") {
			if (!IsLoaded)
				return;

			Helper = mod.Helper.Reflection;

			try {
				OutdoorPot = Type.GetType("RaisedGardenBeds.OutdoorPot, RaisedGardenBeds");
				if (OutdoorPot == null)
					throw new ArgumentNullException("cannot get OutdoorPot");

				Entry = Type.GetType("RaisedGardenBeds.ModEntry, RaisedGardenBeds");
				if (Entry == null)
					throw new ArgumentNullException("cannot get ModEntry");

				ItemDefinition = Type.GetType("RaisedGardenBeds.ItemDefinition, RaisedGardenBeds");
				if (ItemDefinition == null)
					throw new ArgumentNullException("cannot get ItemDefinition");

				// Sanity check
				_ItemDefinitions = Helper.GetField<IDictionary>(Entry, "ItemDefinitions", required: true);
				_Sprites = Helper.GetField<Dictionary<string, Texture2D>>(Entry, "Sprites", required: true);

				// Get Generic Name
				GenericName = Helper.GetField<string>(OutdoorPot, "GenericName", required: true).GetValue();

				// Methods
				_Constructor = OutdoorPot.GetConstructor(new[] { typeof(string), typeof(Vector2) });
				if (_Constructor == null)
					throw new ArgumentNullException("cannot get constructor");

				_GetVariantKeyFromName = Helper.GetMethod(OutdoorPot, "GetVariantKeyFromName", required: true);
				_GetDisplayNameFromVariantKey = Helper.GetMethod(OutdoorPot, "GetDisplayNameFromVariantKey", required: true);
				_GetRawDescription = Helper.GetMethod(OutdoorPot, "GetRawDescription", required: true);
				//_GetSpriteSourceRectangle = Helper.GetMethod(OutdoorPot, "GetSpriteSourceRectangle", required: true);
				_GetSpriteSourceRectangle = OutdoorPot.GetMethod("GetSpriteSourceRectangle", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
				if (_GetSpriteSourceRectangle == null)
					throw new ArgumentNullException("cannot get GetSpriteSourceRectangle");

			} catch (Exception ex) {
				Log($"Unable to find GardenPot class. Disabling integration.", LogLevel.Info, ex, LogLevel.Debug);
				IsLoaded = false;
			}

			if (IsLoaded)
				mod.Recipes.AddProvider(this);
		}

		public string GetVariantKeyFromName(string name) {
			AssertLoaded();
			return _GetVariantKeyFromName.Invoke<string>(name);
		}

		public string GetDisplayNameFromVariantKey(string name) {
			AssertLoaded();
			return _GetDisplayNameFromVariantKey.Invoke<string>(name);
		}

		public string GetRawDescription() {
			AssertLoaded();
			return _GetRawDescription.Invoke<string>();
		}

		public Rectangle GetSpriteSourceRectangle(int spriteIndex) {
			AssertLoaded();
			return (Rectangle) _GetSpriteSourceRectangle.Invoke(null, new object[] { spriteIndex, false });
			//return _GetSpriteSourceRectangle.Invoke<Rectangle>(spriteIndex);
		}

		public object GetItemDefinition(string variant) {
			AssertLoaded();
			return _ItemDefinitions.GetValue()[variant];
		}

		public Texture2D GetSprite(string spriteKey) {
			AssertLoaded();
			return _Sprites.GetValue()[spriteKey];
		}

		public string GetSpriteKey(object definition) {
			AssertLoaded();
			return Helper.GetProperty<string>(definition, "SpriteKey", true).GetValue();
		}

		public int GetSpriteIndex(object definition) {
			AssertLoaded();
			return Helper.GetProperty<int>(definition, "SpriteIndex", true).GetValue();
		}

		public Item MakeOutdoorPot(string variant) {
			AssertLoaded();
			return (Item) Activator.CreateInstance(OutdoorPot, new object[] { variant, Vector2.Zero });
		}

		#region IRecipeProvider

		public int RecipePriority => 10;

		public IRecipe GetRecipe(CraftingRecipe recipe) {
			if (!IsLoaded || recipe == null || recipe.isCookingRecipe)
				return null;

			if (string.IsNullOrEmpty(recipe.name) || string.IsNullOrEmpty(GenericName) || !recipe.name.StartsWith(GenericName))
				return null;

			return new GardenPotRecipe(this, recipe);
		}

		public IEnumerable<IRecipe> GetAdditionalRecipes(bool cooking) {
			return null;
		}

		#endregion

	}
}
