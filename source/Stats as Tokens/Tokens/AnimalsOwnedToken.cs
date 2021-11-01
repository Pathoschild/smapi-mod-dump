/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
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

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatsAsTokens
{
	internal class AnimalsOwnedToken : BaseToken
	{
		public static List<Character> cachedAnimals;
		public static Dictionary<Guid, SDate> horseAges;
		public static List<string> validArgs;
		public string tokenType = "";
		public string outputSeparator = null;

		public AnimalsOwnedToken(string field)
		{
			cachedAnimals = new List<Character>();
			horseAges = new Dictionary<Guid, SDate>();
			validArgs = GetValidAnimalNames();
			tokenType = field;
		}

		public override bool CanHaveMultipleValues(string input = null)
		{
			return (cachedAnimals.Count > 1);
		}

		public override bool TryValidateInput(string input, out string error)
		{
			error = "";
			string[] args = input.ToLower().Split('|');
			string validArgsText = @"'[White/Brown/Blue/Void/Golden] Chicken', 'Duck', 'Rabbit', 'Dinosaur', '[White/Brown] Cow', 'Goat', 'Pig', 'Hog', 'Sheep', 'Ostrich', 'Horse', 'Any', 'Barn', 'Coop' (or matching the name of a custom animal, such as one added by BFAV)";

			if (args.Count() > 0)
			{
				if (!args[0].Contains("type="))
				{
					error += "Named argument 'type' not provided. ";
				}
				else if (args[0].IndexOf('=') == args[0].Length - 1)
				{
					error += $"Named argument 'type' not provided a value. Must be one of the following values: {validArgsText}. ";
				}
				else
				{
					// accept hostplayer or host, localplayer or local
					string animalType = args[0].Substring(args[0].IndexOf('=') + 1).Replace(" ", "");

					bool matched = false;

					if (animalType.Equals("any"))
					{
						matched = true;
					}

					foreach (string animal in validArgs)
					{
						if (matched) break;

						if (animal.ToLower().Replace(" ", "").Contains(animalType))
						{
							matched = true;
						}
					}
						
					if (!matched)
					{
						error += $"Named argument 'type' must be one of the following values: {validArgsText}. ";
					}
				}

				if (args.Count() == 2)
				{
					if (!args[1].Contains("separator="))
					{
						error += $"Second argument must be 'separator' and consist of a string to use as a separator";
					}
					else if (args[1].IndexOf('=') == args[1].Length - 1)
					{
						error += $"If 'separator' argument is provided, it must be provided an argument consisting of an alphanumeric string. ";
					}
				}
			}
			else
			{
				error += $"Wrong number of arguments provided. A 'type' argument (one of the following values: {validArgsText}) must be provided, and optionally a 'separator' argument consisting of an alphanumeric string to separate return values. ";
			}

			return error.Equals("");
		}

		public override bool IsReady()
		{
			return Context.IsWorldReady;
		}

		protected override bool DidStatsChange()
		{
			bool hasChanged = false;
			List<FarmAnimal> currentAnimals = Game1.getFarm().getAllFarmAnimals();
			List<Horse> currentHorses = GetAllHorses();

			// if cached data differs from current data, update and return true

			// Check to see if currently owned animals are cached - add to cache if not
			foreach (FarmAnimal animal in currentAnimals)
			{
				if (!cachedAnimals.Contains(animal))
				{
					hasChanged = true;
					cachedAnimals.Add(animal);
				}
			}

			// Check to see if currently owned horses are cached - add to cache if not
			foreach (Horse horse in currentHorses)
			{
				if (!cachedAnimals.Contains(horse))
				{
					hasChanged = true;
					cachedAnimals.Add(horse);
					horseAges.Add(horse.HorseId, SDate.Now());
				}
			}

			// Check to see if all animals in cache are currently owned - remove if not
			foreach (Character animal in cachedAnimals.ToList())
			{
				if (animal is FarmAnimal fAnimal)
				{
					if (!currentAnimals.Contains(fAnimal))
					{
						hasChanged = true;
						cachedAnimals.Remove(fAnimal);
					}
				}
				else if (animal is Horse horse)
				{
					if (!currentHorses.Contains(horse))
					{
						hasChanged = true;
						cachedAnimals.Remove(horse);
						horseAges.Remove(horse.HorseId);
					}
				}
			}

			return hasChanged;
		}

		public override IEnumerable<string> GetValues(string input)
		{
			// sort first by Age, then Name
			// finally, just to be safe, sort by internal ID, so if you somehow end up with two animals with the same name and age, sorting will be consistent.
			// it's extremely important to sort consistently so that the valueAt argument behaves consistently.
			cachedAnimals = cachedAnimals.OrderByDescending(x => AnimalAge(x)).ThenBy(x => AnimalName(x)).ThenBy(x => AnimalId(x)).ToList();

			List<string> output = new();

			// sanitize inputs
			string[] args = input.Split('|');
			string inputAnimal = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");

			foreach (Character animal in cachedAnimals)
			{
				if (inputAnimal is "barn" or "coop")
				{
					if (animal is FarmAnimal farmAnimal)
					{
						if (farmAnimal.buildingTypeILiveIn.Value.ToLower().Contains(inputAnimal))
						{
							#if DEBUG
							Globals.Monitor.Log($"Matched '{inputAnimal}' to {AnimalType(farmAnimal)} {AnimalName(farmAnimal)} [{AnimalId(farmAnimal)}] which is age {AnimalAge(farmAnimal)}.");
							#endif

							output.Add(GetReturnValue(animal, tokenType));
						}
					}
				}
				else if (AnimalType(animal).ToLower().Replace(" ", "").Contains(inputAnimal) || inputAnimal is "any")
				{

					#if DEBUG
					Globals.Monitor.Log($"Matched '{inputAnimal}' to {AnimalType(animal)} {AnimalName(animal)} [{AnimalId(animal)}] which is age {AnimalAge(animal)}.");
					#endif

					output.Add(GetReturnValue(animal, tokenType));
				}
			}

			if (args.Count() == 2)
			{
				string sep = args[1].Substring(args[1].IndexOf('=') + 1);

				output = new() { string.Join(sep, output) };
			}

			return output;

		}

		private string GetReturnValue(Character animal, string type)
		{
			if (animal is FarmAnimal farmAnimal)
			{
				return type switch
				{
					"name" => farmAnimal.displayName,
					"age" => GetAge(farmAnimal).ToString(),
					"type" => farmAnimal.displayType,
					"id" => farmAnimal.myID.ToString(),
					_ => null,
				};
			}
			else if (animal is Horse horse)
			{
				return type switch
				{
					"name" => horse.Name,
					"age" => GetAge(horse).ToString(),
					"type" => "Horse",
					"id" => horse.HorseId.ToString(),
					_ => null,
				};
			}
			
			return null;
		}

		private List<Horse> GetAllHorses()
		{
			List<Horse> horses = new();

			foreach (Building b in Game1.getFarm().buildings)
			{
				if (b is Stable stable)
				{
					Horse h = stable.getStableHorse();
					if (h?.Name is not null && h.Name != "")
					{
						horses.Add(h);
					}
				}
			}

			return horses;
		}

		private string AnimalType(Character animal) => GetReturnValue(animal, "type");
		private string AnimalName(Character animal) => GetReturnValue(animal, "name");
		private string AnimalAge(Character animal) => GetReturnValue(animal, "age");
		private string AnimalId(Character animal) => GetReturnValue(animal, "id");

		private int GetAge(Character animal)
		{
			if (animal is FarmAnimal fAnimal)
			{
				return fAnimal.age.Value;
			}
			else if (animal is Horse h)
			{
				return SDate.Now().DaysSinceStart - horseAges[h.HorseId].DaysSinceStart;
			}

			return 0;
		}

		private List<string> GetValidAnimalNames()
		{
			return Globals.Helper.Content.Load<Dictionary<string, string>>("Data/FarmAnimals", ContentSource.GameContent).Keys.Concat(new List<string>() { "Horse", "Any", "Barn", "Coop" }).ToList();
		}
	}
}
