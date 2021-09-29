/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableBundleCosts
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigurableBundleCosts
{
	public class ContentPackHelper
	{

		public static IContentPatcherAPI api = null;
		public static bool contentPacksLoaded = false;

		private static List<ContentPackData> packDataList = new();
		private static List<ContentPackItem> patchList = new();

		private static readonly List<string> validTargets = new()
		{
			"buscost",
			"minecartscost",
			"bridgecost",
			"greenhousecost",
			"panningcost",
			"movietheatercost",
			"bundle1",
			"bundle2",
			"bundle3",
			"bundle4"
		};

		public static bool TryLoadContentPatcherAPI()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
				{
					Globals.Monitor.Log("Content Patcher not present");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log($"Failed to register ContentPatcher API: {e.Message}", LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Adds all config values as tokens to ContentPatcher so that they can be referenced dynamically by patches
		/// </summary>
		public static void RegisterTokens()
		{
			if (api == null) return;

			Dictionary<string, string> numeralsToWords = new()
			{
				["1"] = "One",
				["2"] = "Two",
				["3"] = "Three",
				["4"] = "Four"
			};

			List<FieldInfo> jojaFields = typeof(ModConfig.JojaConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
			List<FieldInfo> vaultFields = typeof(ModConfig.VaultConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

			foreach (FieldInfo field in jojaFields)
			{
				string name = field.Name == "applyValues" ? "JojaApplyValues" : field.Name;
				api.RegisterToken(Globals.Manifest, name, () =>
					{
						return name == "JojaApplyValues" ? new[] { ((bool)field.GetValue(Globals.CurrentValues.Joja)).ToString() } : new[] { ((int)field.GetValue(Globals.CurrentValues.Joja)).ToString() };
					}
				);
			}

			foreach (FieldInfo field in vaultFields)
			{
				string name = field.Name == "applyValues" ? "VaultApplyValues" : numeralsToWords.Aggregate(field.Name, (result, num) => result.Replace(num.Key, num.Value));
				api.RegisterToken(Globals.Manifest, name, () =>
				{
					string bundleName = numeralsToWords.Aggregate(name, (result, num) => result.Replace(num.Key, num.Value));
					return name == "VaultApplyValues" ? new[] { ((bool)field.GetValue(Globals.CurrentValues.Vault)).ToString() } : new[] { ((int)field.GetValue(Globals.CurrentValues.Vault)).ToString() };
				}
				);
			}
		}

		/// <summary>
		/// Sets config values by hierarchical priority. The save's existing config values are respected unless <c>forceReload</c> is true. If no saved config values exist, the values are set thusly:
		/// config.override.json values take priority if defined, followed by any values supplied by content packs, followed lastly by any values set in the config.json in the mod folder.
		/// </summary>
		/// <param name="forceReload"> Determines whether or not to force a reload</param>
		public static void ProcessConfigOverrides(bool forceReload = false)
		{

			// check to see if config.override.json exists
			Globals.Override = Globals.Helper.Data.ReadJsonFile<ContentPackConfig>("config.override.json");

			ModConfig initVal = Globals.InitialValues;
			ModConfig savedVal = Globals.Helper.Data.ReadSaveData<ModConfig>("Saved_Config");

			if (savedVal == null || forceReload)
			{
				savedVal = new ModConfig();

				foreach (ContentPackData pack in packDataList)
				{
					ContentPackConfig packVal = pack.Default;

					// set actual config values to:
					// 									override values if they exist		->		content pack values if provided ->	base config values as a last resort
					savedVal.Joja.applyValues		  =	Globals.Override?.Joja.applyValues		 ??	packVal.Joja.applyValues		 ??	initVal.Joja.applyValues;
					savedVal.Joja.busCost			  =	Globals.Override?.Joja.busCost			 ??	packVal.Joja.busCost			 ??	initVal.Joja.busCost;
					savedVal.Joja.minecartsCost		  =	Globals.Override?.Joja.minecartsCost	 ??	packVal.Joja.minecartsCost		 ??	initVal.Joja.minecartsCost;
					savedVal.Joja.bridgeCost		  =	Globals.Override?.Joja.bridgeCost		 ??	packVal.Joja.bridgeCost			 ??	initVal.Joja.bridgeCost;
					savedVal.Joja.greenhouseCost	  =	Globals.Override?.Joja.greenhouseCost	 ??	packVal.Joja.greenhouseCost		 ??	initVal.Joja.greenhouseCost;
					savedVal.Joja.panningCost		  =	Globals.Override?.Joja.panningCost		 ??	packVal.Joja.panningCost		 ??	initVal.Joja.panningCost;
					savedVal.Joja.movieTheaterCost    =	Globals.Override?.Joja.movieTheaterCost  ?? packVal.Joja.movieTheaterCost	 ??	initVal.Joja.movieTheaterCost;

					savedVal.Vault.applyValues		  =	Globals.Override?.Vault.applyValues		 ??	packVal.Vault.applyValues		 ??	initVal.Vault.applyValues;
					savedVal.Vault.bundle1			  =	Globals.Override?.Vault.bundle1			 ??	packVal.Vault.bundle1			 ??	initVal.Vault.bundle1;
					savedVal.Vault.bundle2			  =	Globals.Override?.Vault.bundle2			 ??	packVal.Vault.bundle2			 ??	initVal.Vault.bundle2;
					savedVal.Vault.bundle3			  =	Globals.Override?.Vault.bundle3			 ??	packVal.Vault.bundle3			 ??	initVal.Vault.bundle3;
					savedVal.Vault.bundle4			  =	Globals.Override?.Vault.bundle4			 ??	packVal.Vault.bundle4			 ??	initVal.Vault.bundle4;
				}
			}

			Globals.CurrentValues = savedVal;
		}

		/// <summary>
		/// Runs each day. Processes any patch for which the conditions are met and the patch has not been processed too recently, according to the patch's Frequency.
		/// </summary>
		public static void ProcessDailyUpdates()
		{
			SDate today = SDate.Now();

			foreach (ContentPackItem patch in patchList)
			{
				IManagedConditions con = patch.GetManagedConditions();
				if (con != null)
				{
					con.UpdateContext();
					if (con.IsMatch && CheckFrequency(patch.Frequency, patch.GetDateApplied(), today))
					{
						ProcessPatch(patch);
					}
				}
				else if (CheckFrequency(patch.Frequency, patch.GetDateApplied(), today))
				{
					ProcessPatch(patch);
				}

			}
		}

		public static void ProcessPatch(ContentPackItem patch)
		{
			try
			{

				// logging
				Globals.Monitor.Log($"Processing patch '{patch.Name}'");

				FieldInfo field = ModConfig.GetMatchingField(patch.Target.ToLower().Replace("joja.", "").Replace("vault.", "").Trim());
				if (field == null)
				{
					Globals.Monitor.Log($"Target field not found - aborting patch {patch.Name}");
					return;
				}

				FieldInfo overrideField = ContentPackConfig.GetMatchingField(patch.Target.ToLower().Replace("joja.", "").Replace("vault.", "").Trim());
				ContentPackConfig.SubConfig overrideInst = patch.Target.Contains("bundle") ? Globals.Override?.Vault : Globals.Override?.Joja;
				if (overrideInst != null)
				{
					if (overrideField.GetValue(overrideInst) != null)
					{
						Globals.Monitor.Log($"config.override.json contains value for '{overrideField.Name}'");
						Globals.Monitor.Log($"config.override.json has priority - aborting patch '{patch.Name}'");
						return;
					}
					else if (overrideInst.applyValues == false)
					{
						string configName = overrideInst.ToString().Substring(overrideInst.ToString().LastIndexOf('+') + 1);
						Globals.Monitor.Log($"config.override.json 'applyValues' set to false for '{configName}'");
						Globals.Monitor.Log($"config.override.json has priority - aborting patch '{patch.Name}'");
						return;
					}
				}

				ModConfig.SubConfig inst = patch.Target.Contains("bundle") ? Globals.CurrentValues.Vault : Globals.CurrentValues.Joja;

				int? currentValue = (int?)field.GetValue(inst);
				float pValue = patch.Value ?? 0f;
				int newVal;

				switch (patch.Action)
				{
					case Actions.Set:
						newVal = (int)pValue;
						field.SetValue(inst, newVal);
						// logging
						Globals.Monitor.Log($"Set value of {field.Name} to {newVal}");
						break;
					case Actions.Add:
						newVal = (int)(currentValue + pValue);
						field.SetValue(inst, newVal);
						// logging
						Globals.Monitor.Log($"Added {pValue} to {field.Name}. New value: {newVal}");
						break;
					case Actions.Subtract:
						newVal = (int)(currentValue - pValue);
						field.SetValue(inst, newVal);
						// logging
						Globals.Monitor.Log($"Subtracted value of {pValue} from {field.Name}. New value: {newVal}");
						break;
					case Actions.Divide:
						if (pValue == 0f)
						{
							Globals.Monitor.Log($"Aborted patch - cannot divide by 0.");
							break;
						}
						newVal = (int)(currentValue / pValue);
						field.SetValue(inst, newVal);
						// logging
						Globals.Monitor.Log($"Divided value of {field.Name} by {pValue}. New value: {newVal}");
						break;
					case Actions.Multiply:
						newVal = (int)(currentValue * pValue);
						field.SetValue(inst, newVal);
						// logging
						Globals.Monitor.Log($"Multiplied value of {field.Name} by {pValue}. New value: {newVal}");
						break;
					default:
						Globals.Monitor.Log($"Unrecognized 'Action' token - aborting patch {patch.Name}");
						return;
				}

				patch.SetDateApplied(SDate.Now());

				Globals.Helper.Data.WriteSaveData("Saved_Patches", patchList);
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while performing patch {patch.Name}: {ex}", LogLevel.Error);
			}
		}

		public static bool CheckFrequency(string frequency, SDate dateApplied, SDate today)
		{
			// shortcut - if dateApplied never assigned, patch hasn't triggered yet
			if (dateApplied == null)
			{
				return true;
			}

			int offset;

			string normalizedFrequency = frequency.ToLower().Trim();
			switch (normalizedFrequency)
			{
				case "yearly":
					offset = 112;
					break;
				case "monthly":
					offset = 28;
					break;
				case "weekly":
					offset = 7;
					break;
				case "daily":
					offset = 1;
					break;
				case "once":
					return false;
				default:
					offset = 0;
					break;
			}

			return offset <= today.DaysSinceStart - dateApplied.DaysSinceStart;
		}

		public static void CheckForValidContentPacks()
		{
			foreach (IContentPack contentPack in Globals.Helper.ContentPacks.GetOwned())
			{
				if (!contentPack.HasFile("content.json"))
				{
					Globals.Monitor.Log($"Required file missing in content pack {contentPack.Manifest.Name}: content.json", LogLevel.Warn);
					Globals.Monitor.Log("Skipping content pack", LogLevel.Warn);
				}
				else
				{
					Globals.Monitor.Log($"Located content.json in content pack {contentPack.Manifest.Name}");
					contentPacksLoaded = true;
				}
			}
		}

		public static void GetContentPacks(bool forcePatchReload = false)
		{
			foreach (IContentPack contentPack in Globals.Helper.ContentPacks.GetOwned())
			{
				ContentPackData packData = contentPack.ReadJsonFile<ContentPackData>("content.json");
				packData.SetFolderName(contentPack.Manifest.Name);
				packDataList.Add(packData);

				patchList = patchList.Concat(ParseContentPackPatches(packData, forcePatchReload)).ToList();
			}
		}

		public static void ReloadContentPacks(bool forcePatchReload = false)
		{
			packDataList = new();
			patchList = forcePatchReload ? new() : patchList;
			GetContentPacks(forcePatchReload);

			Globals.Monitor.Log("Reloaded content packs");
		}

		private static List<ContentPackItem> ParseContentPackPatches(ContentPackData packData, bool forceReload = false)
		{
			List<ContentPackItem> patches = null;

			if (Context.IsWorldReady && !forceReload)
			{
				patches = Globals.Helper.Data.ReadSaveData<List<ContentPackItem>>("Saved_Patches");
			}

			if (api == null)
			{
				Globals.Monitor.Log("API not yet initialized - halting content pack parsing");
				return patches;
			}

			if (patches == null)
			{
				patches = new();

				foreach (ContentPackItem patch in packData.Patches.ToList())
				{
					patch.SetContentPack(packData);

					if (patch.Action != null)
					{
						if (patch.Value != null)
						{
							patches = patches.Concat(ParsePatchTargetOrTargets(patch)).ToList();
						}
						else
						{
							Globals.Monitor.Log($"Patch '{patch.Name}' has a null 'Value' field - skipping patch '{patch.Name}'", LogLevel.Warn);
							continue;
						}
					}
					else
					{
						Globals.Monitor.Log($"Patch '{patch.Name}' has a null 'Action' field - skipping patch '{patch.Name}'", LogLevel.Warn);
						continue;
					}
				}
			}

			return patches;
		}

		private static List<ContentPackItem> ParsePatchTargetOrTargets(ContentPackItem patch)
		{
			List<ContentPackItem> patches = new();

			if (patch.Target.Contains(","))
			{
				string[] targets = patch.Target.Split(',');

				foreach (string target in targets)
				{
					string newTarget = target.Trim();

					ContentPackItem singlePatch = new(patch);
					singlePatch.Target = newTarget;

					if (patch.Name == "" || patch.Name == null)
					{
						singlePatch.Name = CreatePatchName(singlePatch);
					}
					else
					{
						singlePatch.Name = patch.Name + "_" + newTarget;
					}

					patches = patches.Concat(ParsePatchTargetOrTargets(singlePatch)).ToList();
				}
			}
			else
			{

				if (patch.Name == "" || patch.Name == null)
				{
					patch.Name = CreatePatchName(patch);
				}

				string normalizedTarget = patch.Target.ToLower().Replace("joja.", "").Replace("vault.", "").Trim();
				if (validTargets.Contains(normalizedTarget))
				{
					IManagedConditions conditions = api.ParseConditions(Globals.Manifest, patch.Conditions, new SemanticVersion("1.23.0"));
					patch.SetManagedConditions(conditions);

					patches.Add(patch);
				}
				else
				{
					Globals.Monitor.Log($"Patch '{patch.Name}' has an unrecognized 'Target' value - skipping patch '{patch.Name}'", LogLevel.Warn);
				}
			}

			return patches;
		}

		public static List<ContentPackData> GetContentPackList()
		{
			return packDataList;
		}

		public static List<ContentPackItem> GetPatchList()
		{
			return patchList;
		}

		private static string CreatePatchName(ContentPackItem patch)
		{
			string desc = patch.Action switch
			{
				Actions.Set => $"Set {patch.Target} to {patch.Value} {patch.Frequency}",
				Actions.Add => $"Add {patch.Value} to {patch.Target} {patch.Frequency}",
				Actions.Subtract => $"Subtract {patch.Value} from {patch.Target} {patch.Frequency}",
				Actions.Multiply => $"Multiply {patch.Target} by {patch.Value} {patch.Frequency}",
				Actions.Divide => $"Divide {patch.Target} by {patch.Value} {patch.Frequency}",
				_ => "Unknown patch type"
			};

			return (patch.GetContentPack()?.GetFolderName() ?? "UnknownFolderName") + " - " + desc;
		}
	}
}
