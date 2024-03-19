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
using System.Linq;
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

			__instance.namingAnimal = false;
			__instance.textBox.Selected = false;
			__instance.textBox.OnEnterPressed -= __instance.textBoxEvent;
			__instance.animalBeingPurchased = new FarmAnimal(Randomize && AlternatePurchaseTypes.Any() ? Game1.random.ChooseFrom(AlternatePurchaseTypes) : __instance.animalBeingPurchased.type.Value, Game1.Multiplayer.getNewID(), __instance.animalBeingPurchased.ownerID.Value);
			return false;
		}

		private static bool ReceiveLeftClickPrefix(PurchaseAnimalsMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsBetterAnimalPurchase)
				return true;
			if (Game1.IsFading() || __instance.freeze)
				return true;

			Vector2 tile = new((int)((Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64f));
			Building buildingAt = __instance.TargetLocation.getBuildingAt(tile);

			if (__instance.onFarm)
			{
				if (!__instance.namingAnimal && buildingAt?.GetIndoors() is AnimalHouse animalHouse && !buildingAt.isUnderConstruction())
				{
					if (__instance.animalBeingPurchased.CanLiveIn(buildingAt))
					{
						if (!animalHouse.isFull())
						{
							if (Game1.player.Money < __instance.priceOfAnimal)
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
			if (Game1.IsFading() || __instance.freeze)
				return;

			if (__instance.onFarm)
			{
				if (!__instance.namingAnimal)
				{
					if (AlternatePurchaseTypes.Any())
					{
						if (new[]{ SButton.Left, SButton.Right, ModEntry.Config.ShopsBetterAnimalPurchasePreviousKey, ModEntry.Config.ShopsBetterAnimalPurchaseNextKey }.Any(button => ModEntry.Helper.Input.GetState(button) == SButtonState.Pressed))
						{
							int oldIndex = AlternatePurchaseTypes.FindIndex(type => type == __instance.animalBeingPurchased.type.Value);

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
								__instance.animalBeingPurchased = new FarmAnimal(AlternatePurchaseTypes[newIndex], __instance.animalBeingPurchased.myID.Value, __instance.animalBeingPurchased.ownerID.Value);
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
			if (Game1.IsFading() || __instance.freeze)
				return;

			if (__instance.onFarm)
			{
				Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 0);
			}
		}
	}
}
