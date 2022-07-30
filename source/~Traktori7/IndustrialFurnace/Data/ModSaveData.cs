/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace IndustrialFurnace.Data
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

				Dictionary<int, int> inputChest = ConvertItemListToDictionary(furnaces[i].input.items);
				FurnaceControllerInput.Add(inputChest);

				Dictionary<int, int> outputChest = ConvertItemListToDictionary(furnaces[i].output.items);
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

				foreach (KeyValuePair<int, int> kvp in FurnaceControllerInput[i])
				{
					Object item = new Object(kvp.Key, kvp.Value);
					controller.input.addItem(item);
				}

				foreach (KeyValuePair<int, int> kvp in FurnaceControllerOutput[i])
				{
					Object item = new Object(kvp.Key, kvp.Value);
					controller.output.addItem(item);
				}

				furnaces.Add(controller);
			}
		}


		private static Dictionary<int, int> ConvertItemListToDictionary(Netcode.NetObjectList<Item> items)
		{
			Dictionary<int, int> chestItems = new Dictionary<int, int>();

			for (int j = 0; j < items.Count; j++)
			{
				Item tempItem = items[j];

				if (chestItems.ContainsKey(tempItem.ParentSheetIndex))
				{
					chestItems[tempItem.ParentSheetIndex] += tempItem.Stack;
				}
				else
				{
					chestItems.Add(tempItem.ParentSheetIndex, tempItem.Stack);
				}
			}

			return chestItems;
		}
	}
}
