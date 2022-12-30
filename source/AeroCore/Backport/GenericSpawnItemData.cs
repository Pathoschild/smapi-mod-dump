/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace AeroCore.Backport
{
	public class GenericSpawnItemData : ISpawnItemData
	{
		public string ItemId { get; set; }
		public List<string> RandomItemId { get; set; }
		public int MinStack { get; set; } = -1;
		public int MaxStack { get; set; } = -1;
		public int Quality { get; set; } = -1;
		public int ToolUpgradeLevel { get; set; } = -1;
		public bool IsRecipe { get; set; }
		public string Condition { get; set; }
		public List<QuantityModifier> StackModifiers { get; set; }
		public QuantityModifier.QuantityModifierMode StackModifierMode { get; set; }
		public List<QuantityModifier> QualityModifiers { get; set; }
		public QuantityModifier.QuantityModifierMode QualityModifierMode { get; set; }
		public virtual Item Spawn(Farmer who = null, GameLocation where = null)
		{
			var id = ItemId;
			if (id is null || !id.TryGetItem(out var item))
			{
				var rand = RandomItemId;
				if (rand is null || rand.Count <= 0 || !Game1.random.Next(rand).TryGetItem(out item))
					return null;
			}
			item.Stack = Math.Max(1, Modify(StackModifierMode, StackModifiers, MinStack, MaxStack, who, where, item));
			if (item is SObject obj)
			{
				int quality = Math.Clamp(Modify(QualityModifierMode, QualityModifiers, Quality, -1, who, where, item), 0, 3);
				obj.Quality = quality == 3 ? 4 : quality;
				obj.IsRecipe = IsRecipe;
			}
			return item;
		}
		private static int Modify(QuantityModifier.QuantityModifierMode mode, List<QuantityModifier> modifiers, int min, int max, Farmer who, GameLocation where, Item what)
		{
			if (min < 0)
				min = 1;
			switch (mode)
			{
				case QuantityModifier.QuantityModifierMode.Stack:
					max = max < 0 ? min : max;
					return QuantityModifier.ModifyQuantity(modifiers, Game1.random.Next(min, max), what, where, who);
				case QuantityModifier.QuantityModifierMode.Minimum:
					min = QuantityModifier.ModifyQuantity(modifiers, min, what, where, who);
					if (max < 0)
						max = min;
					else
						min = Math.Min(min, max);
					break;
				case QuantityModifier.QuantityModifierMode.Maximum:
					max = max < 0 ? min : max;
					max = QuantityModifier.ModifyQuantity(modifiers, max, what, where, who);
					break;
			}
			return Game1.random.Next(min, max);
		}
	}

	public interface ISpawnItemData
	{
		string ItemId { get; set; }
		List<string> RandomItemId { get; set; }
		int MinStack { get; set; }
		int MaxStack { get; set; }
		int Quality { get; set; }
		int ToolUpgradeLevel { get; set; }
		bool IsRecipe { get; set; }
		List<QuantityModifier> StackModifiers { get; set; }
		QuantityModifier.QuantityModifierMode StackModifierMode { get; set; }
		List<QuantityModifier> QualityModifiers { get; set; }
		QuantityModifier.QuantityModifierMode QualityModifierMode { get; set; }

		public Item Spawn(Farmer who = null, GameLocation where = null);
	}
}
