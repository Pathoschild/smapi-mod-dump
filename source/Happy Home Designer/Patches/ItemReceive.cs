/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HarmonyLib;
using StardewValley;

namespace HappyHomeDesigner.Patches
{
	internal class ItemReceive
	{
		public static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Farmer).GetMethod(nameof(Farmer.GetItemReceiveBehavior)),
				postfix: new(typeof(ItemReceive), nameof(ChangeItemReceiveBehavior))
			);

			harmony.TryPatch(
				typeof(Farmer).GetMethod(nameof(Farmer.OnItemReceived)),
				postfix: new(typeof(ItemReceive), nameof(ReceiveItem))
			);

			harmony.TryPatch(
				typeof(Item).GetMethod(nameof(Item.checkForSpecialItemHoldUpMeessage)),
				postfix: new(typeof(ItemReceive), nameof(AddHoldUpMessage))
			);
		}

		private static void ReceiveItem(Farmer __instance, Item item)
		{
			switch(item.QualifiedItemId)
			{
				case "(O)" + AssetManager.CARD_ID:
					if (__instance.hasOrWillReceiveMail(AssetManager.CARD_FLAG))
						return;

					__instance.mailReceived.Add(AssetManager.CARD_FLAG);
					Game1.PerformActionWhenPlayerFree(
						() => __instance.holdUpItemThenMessage(item, true)
					);
					break;
				case "(O)" + AssetManager.PORTABLE_ID:
					__instance.removeItemFromInventory(item);
					__instance.addItemToInventory(ItemRegistry.Create("(T)" + AssetManager.PORTABLE_ID));
					break;
			}
		}

		private static string AddHoldUpMessage(string original, Item __instance)
		{
			if (__instance.QualifiedItemId is "(O)" + AssetManager.CARD_ID)
				return ModEntry.i18n.Get("item.card.receive");
			return original;
		}

		private static void ChangeItemReceiveBehavior(Item item, ref bool needsInventorySpace, ref bool showNotification)
		{
			switch (item.QualifiedItemId)
			{
				case "(O)" + AssetManager.CARD_ID:
					needsInventorySpace = false;
					showNotification = false;
					break;

				case "(O)" + AssetManager.PORTABLE_ID:
					needsInventorySpace = true;
					showNotification = false;
					break;
			}
		}
	}
}
