/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/CapitalistSplitMoney
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalistSplitMoney
{
	class BinListener
	{
		private static bool listening = false;

		private static long? farmerID = null;
		private static Farm farm = null;

		private static bool IsSaving => Game1.activeClickableMenu is StardewValley.Menus.SaveGameMenu || Game1.activeClickableMenu is StardewValley.Menus.ShippingMenu;

		public static void StartListen(long? _farmerID, Farm _farm)
		{
			if (IsSaving)
				return;

			Unregister();

			farmerID = _farmerID;
			farm = _farm;

			farm.shippingBin.OnValueAdded += OnAdd;
			farm.shippingBin.OnValueRemoved += OnRemove;

			listening = true;
		}

		public static void Unregister()
		{
			farmerID = null;
			listening = false;

			if (farm != null)
			{
				farm.shippingBin.OnValueAdded -= OnAdd;
				farm.shippingBin.OnValueRemoved -= OnRemove;
			}
		}

		private static void OnAdd(Item item)
		{
			var t = ModEntry.ModHelper.Reflection.GetField<Task>(typeof(Game1), "_newDayTask").GetValue();
			if (t != null)
			{
				return;
			}

			if (Game1.IsMasterGame && farmerID.HasValue && listening && !IsSaving)
			{
				ModEntry.OldItems.Add(item, farmerID.Value);
				Console.WriteLine($"Added {item.Name} x{item.Stack} by player {farmerID.Value}/{Game1.getFarmerMaybeOffline(farmerID.Value).Name}");

				Unregister();
			}
		}

		private static void OnRemove(Item item)
		{
			var t = ModEntry.ModHelper.Reflection.GetField<Task>(typeof(Game1), "_newDayTask").GetValue();
			if (t != null)
			{
				return;
			}

			if (Game1.IsMasterGame && farmerID.HasValue && listening && !IsSaving)
			{
				ModEntry.OldItems.Remove(item);
				Console.WriteLine($"Removed {item.Name} x{item.Stack}");

				Unregister();
			}
		}
	}
}
