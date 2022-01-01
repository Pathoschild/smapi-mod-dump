using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Reflection;

using StardewValley;
using System.Linq;
using System.Collections.Generic;

namespace RejuvenatingForest
{
	class HarmonyPatcher
	{
		// NOTE: All Monitor code is experimental and may not be correct

		private static Harmony harmony;
		public static IMonitor Monitor; // ref to ModEntry.cs monitor
		
		static HarmonyPatcher()
		{
			harmony = new(Globals.Manifest.UniqueID);
		}

		internal static void ApplyPatches()
		{
			Globals.Monitor.Log("Applying patches...", LogLevel.Debug);

			// Get all HarmonyPatches child classes
			List<Type> patchClasses = new List<Type>();
			foreach (Type type in
				Assembly.GetAssembly(typeof(HarmonyPatches)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(HarmonyPatches))))
			{
				patchClasses.Add(type);
			}

			// For each class, add the methods to a list
			MethodInfo[] patchMethods = new MethodInfo[0];
			foreach(Type patchClass in patchClasses)
            {
				// Get a reference to all _Patch methods in the class
				MethodInfo[] patchMethodsInClass =
				patchClass
				    .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(m => m.Name.Contains("_Patch")).ToArray();

				// Append the methods to the current list of methods
				patchMethods = patchMethods.Concat<MethodInfo>(patchMethodsInClass).ToArray<MethodInfo>();
			}

			foreach (MethodInfo patchMethod in patchMethods)
			{
				patchMethod.Invoke(null, null);
			}
		}

		internal static void ApplyPatch(MethodInfo caller, MethodInfo original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			if (original is null)
			{
				Globals.Monitor.Log($"Aborting {caller.Name}: Method to patch cannot be null.", LogLevel.Error);
			}

			if (prefix is null && postfix is null && transpiler is null)
			{
				Globals.Monitor.Log($"Aborting {caller.Name}: At least one valid patch method must be specified.", LogLevel.Error);
			}

			try
			{
				harmony.Patch(
					original: original,
					prefix: prefix,
					postfix: postfix,
					transpiler: transpiler
				);

				Globals.Monitor.Log($"Successfully patched {original.DeclaringType}::{original.Name}.");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching {original.DeclaringType}::{original.Name}: {ex}", LogLevel.Error);
				Globals.Monitor.Log($"Aborting {caller.Name}.", LogLevel.Error);
			}
		}
	}
}