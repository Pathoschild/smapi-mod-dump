/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;

namespace AeroCore.Backport
{
	public class QuantityModifier
	{
		public enum QuantityModifierMode { Stack, Minimum, Maximum };
		public enum ModificationType { Add, Subtract, Multiply, Divide, Set};
		public string Id { get; set; }
		public string Condition { get; set; }
		public ModificationType Modification { get; set; }
		public float Amount { get; set; }
		public List<float> RandomAmount { get; set; }
		public static float Apply(float value, ModificationType modification, float amount)
			=> modification switch
			{
				ModificationType.Add => value + amount, 
				ModificationType.Subtract => value - amount, 
				ModificationType.Multiply => value* amount,
				ModificationType.Divide => value / amount, 
				ModificationType.Set => amount, 
				_ => value, 
			};
		public static int ModifyQuantity(List<QuantityModifier> modifiers, int value, 
			Item what = null, GameLocation where = null, Farmer who = null, Random rand = null)
		{
			var target = (float)value;
			foreach(var modifier in modifiers)
				if (GameStateQuery.CheckConditions(modifier.Condition, rand, who, what, where))
					target = Apply(target, modifier.Modification, modifier.Amount);
			return (int)(target + .5f);
		}
}
}
