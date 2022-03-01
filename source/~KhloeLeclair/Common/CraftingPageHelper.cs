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
using System.Reflection;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common {
	public static class CraftingPageHelper {

		private static FieldInfo heldItemField;
		private static FieldInfo cookingField;

		private static void InitFields(Type page) {
			if (heldItemField == null) {
				FieldInfo field = page.GetField("heldItem", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field?.FieldType == typeof(Item))
					heldItemField = field;
			}

			if (cookingField == null) {
				FieldInfo field = page.GetField("cooking", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field?.FieldType == typeof(bool))
					cookingField = field;
			}
		}

		public static bool IsCooking(this CraftingPage menu) {
			InitFields(menu.GetType());
			if (cookingField == null)
				return false;

			return (bool) cookingField.GetValue(menu);
		}

		public static bool GetHeldItem(this CraftingPage menu, out Item item) {
			InitFields(menu.GetType());
			if (heldItemField == null) {
				item = null;
				return false;
			}

			item = (Item) heldItemField.GetValue(menu);
			return true;
		}

		public static bool SetHeldItem(this CraftingPage menu, Item item) {
			InitFields(menu.GetType());
			if (heldItemField == null)
				return false;

			heldItemField.SetValue(menu, item);
			return true;
		}
	}
}
