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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace mouahrarasModuleCollection.MarniesShop.AnimalPurchase.Patches
{
	internal class PurchaseAnimalsMenuPatch
	{
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
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(DrawPostfix))
			);
		}

		private static bool SetUpForReturnAfterPurchasingAnimalPrefix(PurchaseAnimalsMenu __instance)
		{
			if (!ModEntry.Config.MarniesShopAnimalPurchase)
				return true;

			TextBox textBox = (TextBox)typeof(PurchaseAnimalsMenu).GetField("textBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			TextBoxEvent e = (TextBoxEvent)typeof(PurchaseAnimalsMenu).GetField("e", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			FieldInfo animalBeingPurchasedField = typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance);
			FarmAnimal animalBeingPurchased = (FarmAnimal)animalBeingPurchasedField.GetValue(__instance);
			Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

			typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
			typeof(TextBox).GetEvent("OnEnterPressed").RemoveEventHandler(textBox, e);
			typeof(TextBox).GetField("_selected", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(textBox, false);
			animalBeingPurchasedField.SetValue(__instance, new FarmAnimal(animalBeingPurchased.type.Value, multiplayer.getNewID(), animalBeingPurchased.ownerID.Value));
			return false;
		}

		private static bool ReceiveLeftClickPrefix(PurchaseAnimalsMenu __instance, int x, int y, bool playSound = true)
		{
			if (!ModEntry.Config.MarniesShopAnimalPurchase)
				return true;
			if ((bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return true;
			if (!__instance.overrideSnappyMenuCursorMovementBan())
				return true;
			if (Game1.IsFading())
				return true;

			bool namingAnimal = (bool)typeof(PurchaseAnimalsMenu).GetField("namingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			FarmAnimal animalBeingPurchased = (FarmAnimal)typeof(PurchaseAnimalsMenu).GetField("animalBeingPurchased", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			int priceOfAnimal = (int)typeof(PurchaseAnimalsMenu).GetField("priceOfAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			Vector2 tile = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64f));
			Building buildingAt = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(tile);
			if (buildingAt != null && !namingAnimal)
			{
				if (buildingAt.buildingType.Value.Contains(animalBeingPurchased.buildingTypeILiveIn.Value))
				{
					if (!(buildingAt.indoors.Value as AnimalHouse).isFull())
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
			return true;
		}

		private static void DrawPostfix(PurchaseAnimalsMenu __instance, SpriteBatch b)
		{
			if (!ModEntry.Config.MarniesShopAnimalPurchase)
				return;
			if ((bool)typeof(PurchaseAnimalsMenu).GetField("freeze", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
				return;
			if (!__instance.overrideSnappyMenuCursorMovementBan())
				return;
			if (Game1.IsFading())
				return;
			Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 0);
		}
	}
}
