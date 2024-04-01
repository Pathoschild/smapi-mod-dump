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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HappyHomeDesigner.Integration
{
	internal class CustomFurniture
	{
		public static IEnumerable<ISalable> customFurniture;
		public static bool Installed;
		
		public static void Init(IModHelper helper)
		{
			Installed = false;
			if (!helper.ModRegistry.IsLoaded("Platonymous.CustomFurniture"))
				return;

			ModEntry.monitor.Log("Custom Furniture detected! Integrating...", LogLevel.Debug);

			if (!ModUtilities.TryFindAssembly("CustomFurniture", out var asm))
			{
				ModEntry.monitor.Log("Failed to find CF assembly, could not integrate.", LogLevel.Warn);
				return;
			}

			var bindings = BindingFlags.Public | BindingFlags.NonPublic;
			var type = asm.GetType("CustomFurniture.CustomFurnitureMod");
			var field = type?.GetField("furniture", BindingFlags.Static | bindings);
			var modInst = type?.GetField("instance", BindingFlags.Static | bindings)?.GetValue(null);
			var checker = 
				modInst is null ?
				(s) => true :
				type.GetMethod("meetsConditions", BindingFlags.Instance | bindings)?.ToDelegate<Func<string, bool>>(modInst);

			if (field is null)
			{
				ModEntry.monitor.Log("Failed to find furniture list, could not integrate.", LogLevel.Warn);
				return;
			}
			if (modInst is null)
			{
				ModEntry.monitor.Log("Failed to capture Mod instance. Conditions for Custom Furniture will not be checked!", LogLevel.Warn);
			}

			var furnitures = ((dynamic)field.GetValue(null)).Values as IEnumerable<dynamic>;

			customFurniture = 
				from furn in furnitures
				where furn.data.sellAtShop && (furn.data.conditions is "none" || checker(furn.data.conditions))
				select furn as ISalable;

			Installed = true;
			ModEntry.monitor.Log("Custom Furniture successfully integrated!", LogLevel.Debug);
		}
	}
}
