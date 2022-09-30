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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Factories;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using MaddUtil;



namespace ItemPipes.Framework.Patches
{
	[HarmonyPatch]
	public static class CJBItemSpawnerIntegration
    {
		public static void Apply(Harmony harmony)
		{
			try
			{
				Type modEntry = AccessTools.TypeByName("CJBItemSpawner.ModEntry") ??
					throw new Exception("CJBItemSpawner not found!");
				harmony.Patch(
					original: AccessTools.Method(AccessTools.TypeByName("CJBItemSpawner.ModEntry"), "GetSpawnableItems"),
					postfix: new HarmonyMethod(typeof(CJBItemSpawnerIntegration), nameof(CJBItemSpawnerIntegration.ModEntry_GetSpawnableItems_Postfix))
				);
			}
			catch (Exception ex)
			{
				Printer.Error($"Failed to add CJBItemSpawnerIntegration patches: {ex}");
			}
		}

		private static void ModEntry_GetSpawnableItems_Postfix(ref object __result)
		{
			var resType = __result.GetType();
			__result = AccessTools.Method(typeof(CJBItemSpawnerIntegration), nameof(GetDefaultPlusCustomItems))
				.MakeGenericMethod(AccessTools.TypeByName("CJBItemSpawner.Framework.SpawnableItem"))
				.Invoke(null, new object[] { __result })!;
		}

		private static IEnumerable<T> GetDefaultPlusCustomItems<T>(IEnumerable<T> items)
		{
			//Re-yield all previous items
			using var enumerator = items.GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}

			//Create constructorInfo for spawnableItem & searchableItem
			Type spawnableItem = AccessTools.TypeByName("CJBItemSpawner.Framework.SpawnableItem");
			ConstructorInfo spawnableItemCtor = spawnableItem.GetConstructor(new[]
			{
				AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.SearchableItem").GetTypeInfo(),
				typeof(string)
			});
			Type searchableItem = AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.SearchableItem");
			ConstructorInfo[] constList = searchableItem.GetConstructors();
			ConstructorInfo searchableItemCtor = constList[0];

			//Get itemType enum values for searchableItem constructor
			Type itemType = AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.ItemType");
			Array enumValues = Enum.GetValues(itemType);

			//Create delegate with expression delegate for searchableItem constructor
			CreateExpressionDelegate createExpressionDelegate = new CreateExpressionDelegate(CreateExpression);

			//Load custom items by id by creating spawnableItem instances
			foreach (int id in DataAccess.GetDataAccess().ItemIDs.Values)
			{
				object searchableItemInstance = searchableItemCtor.Invoke(new object[]
				{
					enumValues.GetValue(6), //Object item type
					id,
					createExpressionDelegate.Invoke(id)
				});
				T spawnableItemInstance = (T)spawnableItemCtor.Invoke(new object[]
				{
					searchableItemInstance,
					"Crafting"
				});
				if (spawnableItemInstance != null)
				{
					yield return spawnableItemInstance;
				}
			}
			
		}

		private delegate Delegate CreateExpressionDelegate(int id);

		private static Delegate CreateExpression(int id)
		{
			var searchableItem = Expression.Parameter
			(
				AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.SearchableItem"), 
				"inItem"
			);
			//Create your custom item instance
			Item newItem;
			//Tool
			if (id == 222661)
            {
				newItem = ItemFactory.CreateTool("wrench");
			}
			else
            {
				newItem = ItemFactory.CreateItemFromID(id);
			}

			/*
			//OPTION 1) Get item via item.getOne() method
			//Make sure your item implements this method
			//Get info of getOne() method
			MethodInfo methodinfo = typeof(Item).GetMethod("getOne");
			//Get getOne() method of newItem by createing a call
			var call = Expression.Call(Expression.Constant(newItem), methodinfo);
			//Create delegate
			var lambda = Expression.Lambda(call, searchableItem).Compile();
			*/
			
			//-----------------------------------------------------------------
			//OPTION 2) Get item via expression magic (uses above created newItem)
			//Create a dummy item to swap for the real one
			var dummyItem = Expression.Parameter(typeof(Item), "dummyItem");
			var block = Expression.Block
			(
				// Add a local variable
				new[] { dummyItem },			
				// Assigns item value to dummy item
				Expression.Assign
				(
					dummyItem, 
					Expression.Constant(newItem, typeof(Item))
				)
			);
			//This has to be done because it wont accept a variable in Expression.Constant()
			//And I couldn't get Expression.Variable to work

			//Create delegate
			var lambda = Expression.Lambda(block, searchableItem).Compile();
			
			return lambda;
		}
	}
}
