/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;


namespace IndustrialFurnace
{
	/// <summary>
	/// The data class for the smelting rules.
	/// 
	/// NOTE: Relying on RequiredModID doesn't work if the item is provided by Json Assets.
	/// The token evaluation {{ItemId: Item name}} returns an error for unknown names and blocks
	/// the Content Patcher edit.
	/// </summary>
	public class SmeltingRulesContainer
	{
		public List<Data.SmeltingRule> SmeltingRules { get; set; }


		public SmeltingRulesContainer(Dictionary<string, string> dict, IMonitor monitor)
		{
			SmeltingRules = new List<Data.SmeltingRule>();

			foreach (var kvp in dict)
			{
				Data.SmeltingRule smeltingRule = new();

				// Try to parse the smelting rule. If it fails, print error and skip.
				try
				{
					string[] s = kvp.Value.Split('/');
					
					smeltingRule.InputItemID = int.Parse(kvp.Key);
					smeltingRule.InputItemAmount = int.Parse(s[0]);
					smeltingRule.OutputItemID = int.Parse(s[1]);
					smeltingRule.OutputItemAmount = int.Parse(s[2]);

					// The fourth entry is space delimited list of required mod IDs
					if (s[3].Length > 0)
					{
						string[] modIDs = s[3].Split(' ', StringSplitOptions.RemoveEmptyEntries);
						smeltingRule.RequiredModID = modIDs.Length > 0 ? modIDs : null;
					}
				}
				catch (Exception ex)
				{
					monitor.Log("Mod failed while parsing smelting rules. Ignoring the faulty rule.", LogLevel.Error);
					monitor.Log(ex.ToString(), LogLevel.Error);
					continue;
				}
				
				SmeltingRules.Add(smeltingRule);
			}
		}


		/// <summary>Returns the smelting rule that matches the input item's ID or null if no matches were found.</summary>
		/// <param name="inputItemID"></param>
		/// <returns></returns>
		public Data.SmeltingRule? GetSmeltingRuleFromInputID(int inputItemID)
		{
			foreach (Data.SmeltingRule rule in SmeltingRules)
			{
				if (rule.InputItemID == inputItemID)
					return rule;
			}

			return null;
		}
	}
}
