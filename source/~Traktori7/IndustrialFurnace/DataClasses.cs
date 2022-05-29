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
using StardewValley;

namespace IndustrialFurnace
{
	/// <summary>
	/// Data class for the save data.
	/// </summary>
	public class ModSaveData
	{
		public List<int> FurnaceControllerId { get; set; }
		public List<bool> FurnaceControllerCurrentlyOn { get; set; }
		public List<Dictionary<int, int>> FurnaceControllerInput { get; set; }
		public List<Dictionary<int, int>> FurnaceControllerOutput { get; set; }


		public ModSaveData()
		{
			FurnaceControllerId = new List<int>();
			FurnaceControllerCurrentlyOn = new List<bool>();
			FurnaceControllerInput = new List<Dictionary<int, int>>();
			FurnaceControllerOutput = new List<Dictionary<int, int>>();
		}


		public void ClearOldData()
		{
			FurnaceControllerId.Clear();
			FurnaceControllerCurrentlyOn.Clear();
			FurnaceControllerInput.Clear();
			FurnaceControllerOutput.Clear();
		}


		/// <summary>Parses the save data from the furnace controller data</summary>
		public void ParseControllersToModSaveData(List<IndustrialFurnaceController> furnaces)
		{
			for (int i = 0; i < furnaces.Count; i++)
			{
				FurnaceControllerId.Add(furnaces[i].ID);
				FurnaceControllerCurrentlyOn.Add(furnaces[i].CurrentlyOn);

				Dictionary<int, int> inputChest = new Dictionary<int, int>();

				for (int j = 0; j < furnaces[i].input.items.Count; j++)
				{
					Item tempItem = furnaces[i].input.items[j];

					if (inputChest.ContainsKey(tempItem.ParentSheetIndex))
						inputChest[tempItem.ParentSheetIndex] += tempItem.Stack;
					else
						inputChest.Add(tempItem.ParentSheetIndex, tempItem.Stack);
				}

				FurnaceControllerInput.Add(inputChest);


				Dictionary<int, int> outputChest = new Dictionary<int, int>();

				for (int j = 0; j < furnaces[i].output.items.Count; j++)
				{
					Item tempItem = furnaces[i].output.items[j];

					if (outputChest.ContainsKey(tempItem.ParentSheetIndex))
						outputChest[tempItem.ParentSheetIndex] += tempItem.Stack;
					else
						outputChest.Add(tempItem.ParentSheetIndex, tempItem.Stack);
				}

				FurnaceControllerOutput.Add(outputChest);
			}
		}


		/// <summary>Parses the furnace controller data from the save data</summary>
		public void ParseModSaveDataToControllers(List<IndustrialFurnaceController> furnaces, ModEntry mod)
		{
			// Assume the lists are equally as long

			for (int i = 0; i < FurnaceControllerId.Count; i++)
			{
				IndustrialFurnaceController controller = new IndustrialFurnaceController(FurnaceControllerId[i], FurnaceControllerCurrentlyOn[i], mod);

				Dictionary<int, int> tempDictionary = FurnaceControllerInput[i];
				foreach (KeyValuePair<int, int> kvp in tempDictionary)
				{
					StardewValley.Object item = new StardewValley.Object(kvp.Key, kvp.Value);
					controller.input.addItem(item);
				}

				tempDictionary = FurnaceControllerOutput[i];
				foreach (KeyValuePair<int, int> kvp in tempDictionary)
				{
					StardewValley.Object item = new StardewValley.Object(kvp.Key, kvp.Value);
					controller.output.addItem(item);
				}

				furnaces.Add(controller);
			}
		}
	}


	/// <summary>
	/// The blueprint data class.
	/// </summary>
	public class BlueprintData
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? BlueprintType { get; set; }
		public string? NameOfBuildingToUpgrade { get; set; }
		public string? MaxOccupants { get; set; }
		public string? Width { get; set; }
		public string? Height { get; set; }
		public string? HumanDoorX { get; set; }
		public string? HumanDoorY { get; set; }
		public string? AnimalDoorX { get; set; }
		public string? AnimalDoorY { get; set; }
		public string? MapToWarpTo { get; set; }
		public string? SourceRectForMenuViewX { get; set; }
		public string? SourceRectForMenuViewY { get; set; }
		public string? ActionBehaviour { get; set; }
		public string? NamesOfBuildingLocations { get; set; }
		public string? Magical { get; set; }
		public string? DaysToBuild { get; set; }
		public string? MoneyRequired { get; set; }
		public List<RequiredItem> ItemsRequired { get; set; }


		public BlueprintData()
		{
			ItemsRequired = new List<RequiredItem>();
		}


		/// <summary>Convert the blueprint data to a string stardew valley understands, and replace the name and description with the current language variants, if present.</summary>
		/// <param name="i18n"></param>
		/// <returns>The blueprint in the format Stardew Valley understands</returns>
		public string ToBlueprintString(ITranslationHelper i18n)
		{
			string s;

			string items = string.Join(" ", ItemsRequired);

#pragma warning disable CS8601 // Possible null reference assignment.
			s = string.Join("/", new string[] {items, Width, Height, HumanDoorX, HumanDoorY, AnimalDoorX, AnimalDoorY, MapToWarpTo, i18n.Get("industrial-furnace.name"), i18n.Get("industrial-furnace.description"),
				BlueprintType, NameOfBuildingToUpgrade, SourceRectForMenuViewX, SourceRectForMenuViewY, MaxOccupants, ActionBehaviour, NamesOfBuildingLocations, MoneyRequired, Magical, DaysToBuild});
#pragma warning restore CS8601 // Possible null reference assignment.

			return s;
		}
	}


	/// <summary>
	/// The data class for blueprint's item requirements.
	/// </summary>
	public class RequiredItem
	{
		public string? ItemName { get; set; }
		public int ItemAmount { get; set; }
		public int ItemID { get; set; }


		public override string ToString()
		{
			return ItemID + " " + ItemAmount;
		}
	}


	/// <summary>
	/// The data class for the smelting rules.
	/// 
	/// NOTE: Relying on RequiredModID doesn't work if the item is provided by Json Assets.
	/// The token evaluation {{ItemId: Item name}} returns an error for unknown names and blocks
	/// the Content Patcher edit.
	/// </summary>
	public class SmeltingRulesContainer
	{
		public List<SmeltingRule> SmeltingRules { get; set; }


		public SmeltingRulesContainer(Dictionary<string, string> dict, IMonitor monitor)
		{
			SmeltingRules = new List<SmeltingRule>();

			foreach (var kvp in dict)
			{
				SmeltingRule smeltingRule = new();

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
		public SmeltingRule? GetSmeltingRuleFromInputID(int inputItemID)
		{
			foreach (SmeltingRule rule in SmeltingRules)
			{
				if (rule.InputItemID == inputItemID)
					return rule;
			}

			return null;
		}
	}


	/// <summary>
	/// The data class for a smelting rule.
	/// </summary>
	public class SmeltingRule
	{
		public int InputItemID { get; set; }
		public int InputItemAmount { get; set; }
		public int OutputItemID { get; set; }
		public int OutputItemAmount { get; set; }
		public string[]? RequiredModID { get; set; }
	}


	public class SmokeAnimationData
	{
		public bool UseCustomSprite { get; set; } = false;
		public uint SpawnFrequency { get; set; } = 500;
		public int SpawnXOffset { get; set; } = 68;
		public int SpawnYOffset { get; set; } = -64;
		public int SpriteSizeX { get; set; } = 10;
		public int SpriteSizeY { get; set; } = 10;
		public float SmokeScale { get; set; } = 2;
		public float SmokeScaleChange { get; set; } = 0.02f;
	}


	public class FireAnimationData
	{
		public bool UseCustomSprite { get; set; } = false;
		public uint SpawnFrequency { get; set; } = 500;
		public float SpawnChance { get; set; } = 0.2f;
		public int SpawnXOffset { get; set; } = 64;
		public int SpawnYOffset { get; set; } = 18;
		public int SpawnXRandomOffset { get; set; } = 16;
		public int SpawnYRandomOffset { get; set; } = 4;
		public int SpriteSizeX { get; set; } = 64;
		public int SpriteSizeY { get; set; } = 64;
		public int AnimationSpeed { get; set; } = 100;
		public int AnimationLength { get; set; } = 4;
		public float SoundEffectChance { get; set; } = 0.05f;
		public int LightSourceXOffset { get; set; } = 96;
		public int LightSourceYOffset { get; set; } = 96;
		public float LightSourceScaleMultiplier { get; set; } = 1.5f;
	}
}
