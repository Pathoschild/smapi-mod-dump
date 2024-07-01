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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HarmonyLib;
using StardewValley.Menus;
using mouahrarasModuleCollection.UserInterface.FastScrolling.Utilities;

namespace mouahrarasModuleCollection.UserInterface.FastScrolling.Patches
{
	internal class PurchaseAnimalsMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.ReceiveKeyPressPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.update), new Type[] { typeof(GameTime) }),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.UpdatePostfix))
			);
		}
	}
}
