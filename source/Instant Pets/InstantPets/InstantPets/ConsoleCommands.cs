/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/InstantPets
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstantPets
{
	internal static class ConsoleCommands
	{
		private static IModHelper Helper;
		private static IMonitor Monitor;
		private static readonly Dictionary<string, string> Definitions = new Dictionary<string, string>
		{
			{
				nameof(pet_reset),
				$"Reset your pet to its default state.{Environment.NewLine}This will erase pet friendship progress."
			},
			{
				nameof(pet_species),
				$"Swap the farm pet from cat to dog, and vice versa.{Environment.NewLine}Pet friendship will be preserved."
			},
			{
				nameof(pet_breed),
				$"Set the breed for your pet. Use 1/2/3 to choose the breed.{Environment.NewLine}Pet friendship will be preserved."
			},
			{
				nameof(pet_rename),
				$"Rename the farm pet.{Environment.NewLine}Pet friendship will be preserved."
			},
			{
				nameof(pet_clear),
				$"Clear all farm pets.{Environment.NewLine}This will erase pet friendship progress."
			},
			{
				nameof(cave_type),
				$"Swap the farm cave from mushroom to fruit, and vice versa.{Environment.NewLine}This will clear any mushroom boxes."
			},
			{
				nameof(cave_clear),
				$"Clear the farm cave to neutral, producing neither mushrooms or fruit.{Environment.NewLine}This will clear any mushroom boxes."
			},
			{
				nameof(misc_explode),
				$"Blow up Marnie and Demetrius for wasting so much of my time."
			}
		};


		internal static void AddConsoleCommands(IModHelper helper, IMonitor monitor)
		{
			ConsoleCommands.Helper = helper;
			ConsoleCommands.Monitor = monitor;
			Utils.Init(helper: helper, monitor: monitor);

			System.Reflection.MethodInfo[] methods =
				ConsoleCommands.Definitions
					.Keys
					.Select(name => typeof(ConsoleCommands).GetMethod(
						name: name, 
						bindingAttr: System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static))
					.ToArray();

			foreach (System.Reflection.MethodInfo method in methods)
			{
				string[] prefixAndName = method.Name.Split('_');
				ConsoleCommands.Helper.ConsoleCommands.Add(
					name: string.Join(".", prefixAndName),
					documentation: ConsoleCommands.Definitions[method.Name],
					callback: (Action<string, string[]>)Delegate.CreateDelegate(
						type: typeof(Action<string, string[]>),
						target: typeof(ConsoleCommands),
						method: method.Name));
			}
		}

		private static IEnumerable<Pet> CommandPetCheck(string[] args, bool isNameRequired, string matchName = null)
		{
			const LogLevel logLevel = LogLevel.Debug;
			IEnumerable<Pet> pets = Utils.GetPets();

			if (!pets.Any())
			{
				ConsoleCommands.Monitor.Log("No pets were found.", logLevel);
				return null;
			}

			if (isNameRequired)
			{
				if (string.IsNullOrWhiteSpace(matchName))
				{
					int numOfPets = pets.Count();
					if (numOfPets > 1 && args.Length <= 1)
					{
						ConsoleCommands.Monitor.Log($"{numOfPets} pets were found. Please also provide the current name of the pet.", logLevel);
						return null;
					}
				}
				else
				{
					Pet pet = pets.FirstOrDefault(p => p.Name == matchName);
					if (pet == null)
					{
						ConsoleCommands.Monitor.Log($"No pets were found under the name '{matchName}'.", logLevel);
						return null;
					}
				}
			}

			return pets;
		}

		private static void pet_reset(string s, string[] args)
		{
			Utils.ClearPets(isMessageVisible: true);
			Utils.NewPetMenu(isMessageVisible: true);
		}

		private static void pet_breed(string s, string[] args)
		{
			IEnumerable<Pet> pets = ConsoleCommands.CommandPetCheck(args: args, isNameRequired: true);
			if (pets == null)
				return;

			if (args.Length < 1 || !int.TryParse(args[0], out int whichBreed) || whichBreed < 1 || whichBreed > 3)
			{
				ConsoleCommands.Monitor.Log($"Use 1/2/3 to choose the breed.", LogLevel.Debug);
				return;
			}

			Utils.SetBreed(pet: pets.First(), whichBreed: --whichBreed, setForPlayer: pets.Count() == 1, isMessageVisible: true);
		}

		private static void pet_species(string s, string[] args)
		{
			string name = args.Length <= 1 ? null : args[1];

			IEnumerable<Pet> pets = ConsoleCommands.CommandPetCheck(args: args, isNameRequired: true, matchName: name);
			if (pets == null)
				return;

			Pet pet = pets.First();
			Utils.SetSpecies(pet: pet, isCat: !(pet is Cat), isMessageVisible: true);
		}

		private static void pet_rename(string s, string[] args)
		{
			if (args.Length == 0)
			{
				ConsoleCommands.Monitor.Log($"Provide a name to rename your pet.", LogLevel.Debug);
				return;
			}

			string newName = args[0];
			string oldName = args.Length <= 1 ? null : args[1];

			IEnumerable<Pet> pets = ConsoleCommands.CommandPetCheck(args: args, isNameRequired: true, matchName: oldName);
			if (pets == null)
				return;

			Utils.RenamePet(pet: pets.First(), name: newName, isMessageVisible: true);
		}

		private static void pet_clear(string s, string[] args)
		{
			IEnumerable<Pet> pets = ConsoleCommands.CommandPetCheck(args: args, isNameRequired: false);
			if (pets == null)
				return;

			Utils.ClearPets(isMessageVisible: true, isExplosive: args.Length > 0);
			Utils.SetRejectedMail(isRejected: true);
		}

		private static void cave_type(string s, string[] args)
		{
			Utils.ToggleCave(isMessageVisible: true);
		}

		private static void cave_clear(string s, string[] args)
		{
			Utils.ClearCave(isMessageVisible: true);
		}

		private static void misc_explode(string s, string[] args)
		{
			Utils.ExplodeIdiots();
		}
	}
}
