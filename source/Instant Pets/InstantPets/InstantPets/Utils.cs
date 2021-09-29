/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/InstantPets
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantPets
{
	public static class Utils
	{
		private static IModHelper Helper;
		private static IMonitor Monitor;

		private const int EventIdCat = 1590166;
		private const int EventIdDog = 897405;
		private const int EventIdCave = 65;
		private const string MailIdRejected = "rejectedPet";
		private const string MailIdLoved = "petLoveMessage";
		private const int CaveIdMushroom = 2;
		private const int CaveIdFruit = 1;
		private const int CaveIdNone = 0;
		private const int ObjectIdMushroomBox = 128;

		internal static void Init(IModHelper helper, IMonitor monitor)
		{
			Utils.Helper = helper;
			Utils.Monitor = monitor;
		}

		public static void InstantPet(bool isMessageVisible)
		{
			bool isFirstDay = Game1.dayOfMonth == 1 && Utility.getSeasonNumber(Game1.currentSeason) == 0 && Game1.year == 1;
			if (isFirstDay || !Utils.HasMadePetSelection())
			{
				if (ModEntry.Config.NoPets)
				{
					Utils.Monitor.Log($"Set the farm to have no pets.",
						isMessageVisible ? LogLevel.Debug : LogLevel.Trace);

					Utils.SetRejectedMail(isRejected: true);
					return;
				}

				if (ModEntry.Config.InstantPets)
				{
					Utils.SetPetPreference(isCatPerson: ModEntry.Config.CatPerson, isMessageVisible: false);
					Utils.NewPetMenu(isMessageVisible: false);
					return;
				}
			}

			Utils.Monitor.Log($"No pets to manage, going to sleep. zzzz",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static void InstantCave(bool isMessageVisible)
		{
			bool isFirstDay = Game1.dayOfMonth == 1 && Utility.getSeasonNumber(Game1.currentSeason) == 0 && Game1.year == 1;
			if (isFirstDay || !Utils.HasMadeCaveSelection())
			{
				if (ModEntry.Config.EmptyCave)
				{
					Utils.Monitor.Log($"Set the farm to have a neutral cave.",
						isMessageVisible ? LogLevel.Debug : LogLevel.Trace);

					Utils.SetCaveEvent(isSeen: true);
					return;
				}

				if (ModEntry.Config.InstantCave)
				{
					Utils.SetCave(isMushroom: ModEntry.Config.MushroomPerson, isMessageVisible: false);
					return;
				}
			}

			Utils.Monitor.Log($"No caves to change, going to sleep. zzzz",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static void SetCaveEvent(bool isSeen)
		{
			if (isSeen)
			{
				Game1.MasterPlayer.eventsSeen.Add(Utils.EventIdCave);
			}
			else
			{
				Game1.MasterPlayer.eventsSeen.Remove(Utils.EventIdCave);
			}
		}

		public static void SetRejectedMail(bool isRejected)
		{
			if (isRejected)
			{
				if (!Game1.MasterPlayer.hasOrWillReceiveMail(Utils.MailIdRejected))
				{
					Game1.MasterPlayer.mailReceived.Add(Utils.MailIdRejected);
					Game1.MasterPlayer.mailReceived.Remove(Utils.MailIdLoved);
				}
			}
			else
			{
				Game1.MasterPlayer.mailReceived.Remove(Utils.MailIdRejected);
			}
		}

		public static void NewPetMenu(bool isMessageVisible)
		{
			Utils.ClearPets(isMessageVisible: isMessageVisible);
			new Event().answerDialogue(questionKey: "pet", answerChoice: 0);
		}

		public static IEnumerable<Pet> GetPetsForLocation(GameLocation location)
		{
			return location.characters.OfType<Pet>();
		}

		public static IEnumerable<GameLocation> GetPossiblePetLocations()
		{
			List<GameLocation> locations = new List<GameLocation> { Game1.getFarm() };
			locations.AddRange(Game1.getAllFarmers().Select(f => Utility.getHomeOfFarmer(f)).Cast<GameLocation>().ToList());
			return locations;
		}

		public static IEnumerable<Pet> GetPets()
		{
			List<Pet> pets = new List<Pet> { Game1.MasterPlayer.getPet() };
			IEnumerable<GameLocation> locations = Utils.GetPossiblePetLocations();
			pets.AddRange(locations.SelectMany(l => Utils.GetPetsForLocation(l)));

			return pets.Distinct().Where(p => p != null);
		}

		public static void SetPetPreference(bool isCatPerson, bool isMessageVisible)
		{
			Utils.SetRejectedMail(isRejected: false);
			Game1.MasterPlayer.eventsSeen.Remove(isCatPerson ? Utils.EventIdDog : Utils.EventIdCat);
			Game1.MasterPlayer.eventsSeen.Add(isCatPerson ? Utils.EventIdCat : Utils.EventIdDog);
			Game1.MasterPlayer.catPerson = isCatPerson;

			Utils.Monitor.Log($"Set pet preference to {(isCatPerson ? "cat" : "dog")}.",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static void SetBreed(Pet pet, int whichBreed, bool setForPlayer, bool isMessageVisible)
		{
			pet.whichBreed.Value = whichBreed;
			if (setForPlayer)
			{
				Game1.MasterPlayer.whichPetBreed = whichBreed;
			}

			pet.playContentSound();
			Utils.Monitor.Log($"'{pet.Name}' is now breed {whichBreed}.",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static void SetSpecies(Pet pet, bool isCat, bool isMessageVisible)
		{
			if ((pet is Cat && isCat) || (pet is Dog && !isCat))
			{
				Utils.Monitor.Log($"'{pet.Name}' is already a {pet.GetType().Name}.",
					isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
				return;
			}

			Vector2 tileLocation = Utility.recursiveFindOpenTileForCharacter(
				c: pet, l: Game1.currentLocation, tileLocation: Game1.player.getTileLocation(), maxIterations: 30, allowOffMap: false);
			Pet newPet = isCat
				? (Pet)(new Cat(xTile: (int)tileLocation.X, yTile: (int)tileLocation.Y, breed: pet.whichBreed.Value))
				: (Pet)(new Dog(xTile: (int)tileLocation.X, yTile: (int)tileLocation.Y, breed: pet.whichBreed.Value));

			newPet.whichBreed.Value = pet.whichBreed.Value;
			newPet.isSleepingOnFarmerBed.Value = pet.isSleepingOnFarmerBed.Value;
			newPet.friendshipTowardFarmer.Value = pet.friendshipTowardFarmer.Value;
			newPet.grantedFriendshipForPet.Value = pet.grantedFriendshipForPet.Value;
			newPet.lastPetDay = pet.lastPetDay;
			newPet.displayName = pet.displayName;
			newPet.flip = pet.flip;

			pet.currentLocation.characters.Remove(pet);
			pet.currentLocation.addCharacter(newPet);
		}

		public static void RenamePet(Pet pet, string name, bool isMessageVisible)
		{
			pet.playContentSound();
			Utils.Monitor.Log($"'{pet.Name}' is now named '{name}'.",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);

			pet.displayName = name;
		}

		public static void ClearPets(bool isMessageVisible, bool isExplosive = false)
		{
			StringBuilder message = new StringBuilder();
			IEnumerable<Pet> pets = GetPets();

			if (!pets.Any())
			{
				Utils.Monitor.Log("No pets were found.",
					isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
				return;
			}

			foreach (Pet pet in pets)
			{
				if (pet.currentLocation.characters.Remove(pet))
				{
					message.Append($"{pet.Name} was {(isExplosive ? "blasted" : "moved")} out of the {pet.currentLocation.NameOrUniqueName.ToLower()}.");
					if (isExplosive)
					{
						Utils.Explode(chara: pet);
					}
					else
					{
						pet.currentLocation.explode(tileLocation: pet.getTileLocation(), radius: 0, who: Game1.player, damageFarmers: false, damage_amount: 0);
					}
				}
				else
				{
					message.Append($"Couldn't remove {pet.Name} from the {pet.currentLocation.NameOrUniqueName}.");
				}
			}

			if (isExplosive)
			{
				Game1.flashAlpha = 0.75f;
			}
			Game1.playSound(isExplosive ? "explosion" : "fireball");
			Utils.Monitor.Log(message.ToString(),
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
			
			Utils.SetRejectedMail(isRejected: false);
		}

		public static bool HasMadePetSelection()
		{
			return Game1.MasterPlayer.eventsSeen.Contains(Utils.EventIdCat)
				|| Game1.MasterPlayer.eventsSeen.Contains(Utils.EventIdCat)
				|| Game1.MasterPlayer.mailReceived.Contains(Utils.MailIdRejected)
				|| Game1.MasterPlayer.hasPet();
		}

		public static bool HasMadeCaveSelection()
		{
			return Game1.MasterPlayer.eventsSeen.Contains(Utils.EventIdCave);
		}

		public static void ToggleCave(bool isMessageVisible)
		{
			Utils.SetCave(isMushroom: Game1.MasterPlayer.caveChoice.Value != Utils.CaveIdMushroom, isMessageVisible: isMessageVisible);
		}

		public static void SetCave(bool isMushroom, bool isMessageVisible)
		{
			Utils.SetCaveEvent(isSeen: true);

			string caveName = isMushroom ? "mushroom" : "fruit";
			if (isMushroom && Game1.MasterPlayer.caveChoice.Value == Utils.CaveIdMushroom)
			{
				Utils.Monitor.Log($"The farm already has a {caveName} cave.",
					isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
				return;
			}
			FarmCave farmCave = Utils.GetFarmCave();
			Game1.MasterPlayer.caveChoice.Value = isMushroom ? Utils.CaveIdMushroom : Utils.CaveIdFruit;
			if (isMushroom)
			{
				farmCave.setUpMushroomHouse();
			}
			else
			{
				Utils.RemoveMushroomBoxesFromCave(farmCave);
			}

			Utils.Monitor.Log($"Set the farm to have a {caveName} cave.",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static void ClearCave(bool isMessageVisible)
		{
			FarmCave farmCave = Utils.GetFarmCave();
			Utils.RemoveMushroomBoxesFromCave(farmCave);
			Game1.MasterPlayer.caveChoice.Value = Utils.CaveIdNone;

			Utils.Monitor.Log($"Set the farm cave to neutral.",
				isMessageVisible ? LogLevel.Debug : LogLevel.Trace);
		}

		public static FarmCave GetFarmCave()
		{
			return Game1.getLocationFromName("FarmCave") as FarmCave;
		}

		public static void RemoveMushroomBoxesFromCave(FarmCave farmCave)
		{
			foreach (Vector2 key in farmCave.Objects.Keys.Where(key => farmCave.Objects[key].ParentSheetIndex == Utils.ObjectIdMushroomBox).ToList())
			{
				farmCave.Objects.Remove(key);
			}
			farmCave.UpdateReadyFlag();
		}

		public static void Explode(NPC chara)
		{
			string[] text = new [] { "Ooof!", "Ouch!", "Argh!", "Ahhh!", "My bones!", "Aaaugh!", "Blast!", "KA-POW!" };
			chara.currentLocation.explode(tileLocation: chara.getTileLocation(), radius: 2, who: Game1.player, damageFarmers: false, damage_amount: 0);
			chara.showTextAboveHead(text[Game1.random.Next(0, text.Length)]);
			Game1.flashAlpha = 0.75f;
		}

		public static void ExplodeIdiots()
		{
			NPC marnie = Game1.getCharacterFromName("Marnie");
			NPC demetrius = Game1.getCharacterFromName("Demetrius");
			List<NPC> idiots = new List<NPC> { marnie, demetrius };
			foreach (NPC idiot in idiots)
			{
				if (Game1.currentLocation == idiot.currentLocation)
				{
					Utils.Explode(chara: idiot);
				}
			}

			Utils.Monitor.Log($"the deed is done", LogLevel.Debug);
		}
	}
}
