using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using SVBundle = StardewValley.Menus.Bundle;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer
{
	public class BundleMenuAdjustments
	{
		private static JunimoNoteMenu _currentActiveBundleMenu { get; set; }

		/// <summary>
		/// Fixes the ability to highlight rings in the bundle menu
		/// </summary>
		public static void FixRingSelection(object sender, MenuChangedEventArgs e)
		{
			if (!Globals.Config.RandomizeBundles || !(e.NewMenu is JunimoNoteMenu))
			{
				_currentActiveBundleMenu = null;
				return;
			}

			_currentActiveBundleMenu = (JunimoNoteMenu)e.NewMenu;
			_currentActiveBundleMenu.inventory.highlightMethod = HighlightBundleCompatibleItems;
		}

		/// <summary>
		/// A copy of the Utlity.cs code for highlightSmallObjects, but with rings included
		/// </summary>
		/// <param name="item">The Stardew Valley item</param>
		/// <returns>True if the item should be draggable, false otherwise</returns>
		private static bool HighlightBundleCompatibleItems(SVItem item)
		{
			if (item is Ring)
			{
				return true;
			}
			else if (item is SVObject)
			{
				return !(bool)((NetFieldBase<bool, NetBool>)(item as SVObject).bigCraftable);
			}
			return false;
		}

		/// <summary>
		/// Fixes the ability to deposit rings into a bundle
		/// </summary>
		public static void FixRingDeposits()
		{
			if (_currentActiveBundleMenu == null) { return; }
			ReplaceTryToDepositMethod();
		}

		/// <summary>
		/// The new method that replaces Stardew Valley's Bundle.cs's tryToDepositThisItem with this file's NewTryToDepositItem
		/// </summary>
		/// <param name="item">The item you are trying to deposit</param>
		/// <param name="slot">The slot you're trying to deposit to</param>
		/// <param name="noteTextureName">Unsure what this is</param>
		/// <returns>What item the player should get back after trying to depositing</returns>
		public SVItem NewTryToDepositItem(SVItem item, ClickableTextureComponent slot, string noteTextureName)
		{
			SVBundle bundle =
				Globals.ModRef.Helper.Reflection
					.GetField<SVBundle>(_currentActiveBundleMenu, "currentPageBundle", true)
					.GetValue();

			if (!bundle.depositsAllowed)
			{
				if (Game1.player.hasCompletedCommunityCenter())
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtAJM"));
				else
					Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtCC"));
				return item;
			}

			bool isRing = item is Ring;
			if (!(item is SVObject || isRing) || item is Furniture)
				return item;
			SVObject @object = item as SVObject;
			bool ringDeposited = false;
			for (int index = 0; index < bundle.ingredients.Count; ++index)
			{
				if (!bundle.ingredients[index].completed &&
					bundle.ingredients[index].index == (int)((NetFieldBase<int, NetInt>)item.parentSheetIndex) &&
					(
						item.Stack >= bundle.ingredients[index].stack &&
						(isRing || (int)((NetFieldBase<int, NetInt>)@object.quality) >= bundle.ingredients[index].quality)
					) &&
					slot.item == null)
				{
					if (isRing)
					{
						ringDeposited = true;
					}
					item.Stack -= bundle.ingredients[index].stack;
					bundle.ingredients[index] = new BundleIngredientDescription(bundle.ingredients[index].index, bundle.ingredients[index].stack, bundle.ingredients[index].quality, true);
					bundle.ingredientDepositAnimation(slot, noteTextureName, false);
					slot.item = new SVObject(bundle.ingredients[index].index, bundle.ingredients[index].stack, false, -1, bundle.ingredients[index].quality);
					Game1.playSound("newArtifact");
					(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict[bundle.bundleIndex][index] = true;
					slot.sourceRect.X = 512;
					slot.sourceRect.Y = 244;

					Multiplayer multiplayer = Globals.ModRef.Helper.Reflection
						.GetField<Multiplayer>(typeof(Game1), "multiplayer", true)
						.GetValue();
					multiplayer.globalChatInfoMessage("BundleDonate", Game1.player.displayName, slot.item.DisplayName);
				}
			}
			if (!ringDeposited && item.Stack > 0)
				return item;
			return null;
		}

		/// <summary>
		/// Replaces the tryToDepositThisItem method in Stardew Valley's Bundle.cs with this file's NewTryToDepositItem method
		/// NOTE: THIS IS UNSAFE CODE, CHANGE WITH EXTREME CAUTION
		/// </summary>
		public static void ReplaceTryToDepositMethod()
		{
			SVBundle bundle =
				Globals.ModRef.Helper.Reflection
					.GetField<SVBundle>(_currentActiveBundleMenu, "currentPageBundle", true)
					.GetValue();

			if (bundle == null) { return; }

			MethodInfo methodToReplace = Globals.ModRef.Helper.Reflection.GetMethod(bundle, "tryToDepositThisItem", true).MethodInfo;
			MethodInfo methodToInject = typeof(BundleMenuAdjustments).GetMethod("NewTryToDepositItem");
			Globals.RepointMethod(methodToReplace, methodToInject);
		}
	}
}
