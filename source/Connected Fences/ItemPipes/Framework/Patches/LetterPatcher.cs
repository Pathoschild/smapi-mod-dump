/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Recipes;
using ItemPipes.Framework.Util;
using Netcode;
using StardewValley.Menus;
using ItemPipes.Framework.Nodes.ObjectNodes;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using System.Data;

namespace ItemPipes.Framework.Patches
{
    public class LetterPatcher
    {
		public static bool WrenchCrafted { get; set; }
		public static void Apply(Harmony harmony)
		{
			WrenchCrafted = false;
			try
			{
				harmony.Patch(
					original: typeof(LetterViewerMenu).GetMethod(nameof(LetterViewerMenu.update), new Type[] { typeof(GameTime) }),
					prefix: new HarmonyMethod(typeof(LetterPatcher), nameof(LetterPatcher.LetterViewerMenu_update_Prefix))
				);
			}
			catch (Exception ex)
			{
				Printer.Error($"Failed to add crafting patches: {ex}");
			}
		}

		private static bool LetterViewerMenu_update_Prefix(LetterViewerMenu __instance)
		{
			if (__instance.mailTitle.Equals("ItemPipes_SendWrench"))
			{
				Item wrench = Factories.ItemFactory.CreateTool("Wrench");
				if (!__instance.itemsToGrab.Any(c => c != null && c.item != null && c.item.Name.Equals("Wrench")) && !WrenchCrafted)
				{
					__instance.itemsToGrab.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width / 2 - 48, __instance.yPositionOnScreen + __instance.height - 32 - 96, 96, 96), Factories.ItemFactory.CreateTool("Wrench")));
					WrenchCrafted = true;
				}
			}
			else if (__instance.mailTitle.Equals("ItemPipes_ItemsLost"))
			{
				foreach (Item lostItem in DataAccess.GetDataAccess().LostItems.ToList())
				{
					if (!__instance.itemsToGrab.Any(c => c.item != null && c.item.Name.Equals(lostItem.Name)))
					{
						__instance.itemsToGrab.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width / 2 - 48, __instance.yPositionOnScreen + __instance.height - 32 - 96, 96, 96), lostItem));
						DataAccess.GetDataAccess().LostItems.Remove(lostItem);
					}
				}
			}
			return true;
		}
	}
}
