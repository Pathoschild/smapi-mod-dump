/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Extensions;
using System.Linq;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Shops.BetterAnimalPurchase.Patches
{
	internal class PurchaseAnimalsMenuPatch
	{
		private static readonly PerScreen<bool>			randomize = new(() => true);
		private static readonly PerScreen<List<string>>	alternatePurchaseTypes = new(() => new());

		private static bool Randomize
		{
			get => randomize.Value;
			set => randomize.Value = value;
		}

		private static List<string> AlternatePurchaseTypes
		{
			get => alternatePurchaseTypes.Value;
			set => alternatePurchaseTypes.Value = value;
		}

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal)),
				prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(SetUpForReturnAfterPurchasingAnimalPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(ReceiveLeftClickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
				postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(ReceiveKeyPressPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(DrawPostfix))
			);
		}

		private static bool SetUpForReturnAfterPurchasingAnimalPrefix(PurchaseAnimalsMenu __instance)
		{
			if (!ModEntry.Config.ShopsBetterAnimalPurchase)
				return true;

			TextBox textBox = (TextBox)typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			TextBoxEvent e = (TextBoxEvent)typeof(PurchaseAnimalsMenu).GetField("e", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			FieldInfo animalBeingPurchasedField = typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance);
			FarmAnimal animalBeingPurchased = (FarmAnimal)animalBeingPurchasedField.GetValue(__instance);

			typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
			typeof(TextBox).GetEvent("OnEnterPressed").RemoveEventHandler(textBox, e);
			typeof(TextBox).GetField("_selected", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(textBox, false);
			animalBeingPurchasedField.SetValue(__instance, new FarmAnimal(Randomize && AlternatePurchaseTypes.Any() ? Game1.random.ChooseFrom(AlternatePurchaseTypes) : animalBeingPurchased.type.Value, Game1.Multiplayer.getNewID(), animalBeingPurchased.ownerID.Value));
			return false;
		}

		private static bool ReceiveLeftClickPrefix(PurchaseAnimalsMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsBetterAnimalPurchase)
				return true;
			if (Game1.IsFading() || (bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return true;

			bool onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			bool namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			FarmAnimal animalBeingPurchased = (FarmAnimal)typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			int priceOfAnimal = (int)typeof(PurchaseAnimalsMenu).GetField("priceOfAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			Vector2 tile = new((int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f));
			Building buildingAt = __instance.TargetLocation.getBuildingAt(tile);

			if (onFarm)
			{
				if (!namingAnimal && buildingAt?.GetIndoors() is AnimalHouse animalHouse && !buildingAt.isUnderConstruction())
				{
					if (animalBeingPurchased.CanLiveIn(buildingAt))
					{
						if (!animalHouse.isFull())
						{
							if (Game1.player.Money < priceOfAnimal)
							{
								Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
								Game1.playSound("cancel");
								return false;
							}
						}
					}
				}
			}
			else
			{
				foreach (ClickableTextureComponent item in __instance.animalsToPurchase)
				{
					if (__instance.readOnly || !item.containsPoint(x, y) || (item.item as StardewValley.Object).Type != null)
					{
						continue;
					}
					if (Game1.player.Money >= item.item.salePrice())
					{
						Randomize = true;
						AlternatePurchaseTypes.Clear();
						if (Game1.farmAnimalData.TryGetValue(item.hoverText, out FarmAnimalData value) && value.AlternatePurchaseTypes != null)
						{
							foreach (AlternatePurchaseAnimals alternatePurchaseType in value.AlternatePurchaseTypes)
							{
								if (GameStateQuery.CheckConditions(alternatePurchaseType.Condition, null, null, null, null, null, new HashSet<string> { "RANDOM" }))
								{
									AlternatePurchaseTypes.AddRange(alternatePurchaseType.AnimalIds);
								}
							}
						}
					}
				}
			}
			return true;
		}

		private static void ReceiveKeyPressPostfix(PurchaseAnimalsMenu __instance)
		{
			if (Game1.IsFading() || (bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return;

			bool onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			bool namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			FieldInfo animalBeingPurchasedField = typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance);
			FarmAnimal animalBeingPurchased = (FarmAnimal)animalBeingPurchasedField.GetValue(__instance);

			if (onFarm)
			{
				if (!namingAnimal)
				{
					if (AlternatePurchaseTypes.Any())
					{
						if (new[]{ SButton.Left, SButton.Right, ModEntry.Config.ShopsBetterAnimalPurchasePreviousKey, ModEntry.Config.ShopsBetterAnimalPurchaseNextKey }.Any(button => ModEntry.Helper.Input.GetState(button) == SButtonState.Pressed))
						{
							int oldIndex = AlternatePurchaseTypes.FindIndex(type => type == animalBeingPurchased.type.Value);

							if (oldIndex != -1)
							{
								int newIndex = oldIndex;

								if (new[]{ SButton.Left, ModEntry.Config.ShopsBetterAnimalPurchasePreviousKey }.Any(button => ModEntry.Helper.Input.GetState(button) == SButtonState.Pressed))
								{
									newIndex--;
									if (newIndex < 0)
									{
										newIndex += AlternatePurchaseTypes.Count;
									}
								}
								else if (new[]{ SButton.Right, ModEntry.Config.ShopsBetterAnimalPurchaseNextKey }.Any(button => ModEntry.Helper.Input.GetState(button) == SButtonState.Pressed))
								{
									newIndex++;
									if (newIndex >= AlternatePurchaseTypes.Count)
									{
										newIndex -= AlternatePurchaseTypes.Count;
									}
								}
								animalBeingPurchasedField.SetValue(__instance, new FarmAnimal(AlternatePurchaseTypes[newIndex], animalBeingPurchased.myID.Value, animalBeingPurchased.ownerID.Value));
								Game1.playSound("shwip");
								Randomize = false;
							}
						}
					}
				}
			}
		}

		private static void DrawPostfix(PurchaseAnimalsMenu __instance, SpriteBatch b)
		{
			if (!ModEntry.Config.ShopsBetterAnimalPurchase)
				return;
			if (Game1.IsFading() || (bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return;

			bool onFarm = (bool)typeof(PurchaseAnimalsMenu).GetField("onFarm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			if (onFarm)
			{
				Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 0);
			}
		}
	}
}
