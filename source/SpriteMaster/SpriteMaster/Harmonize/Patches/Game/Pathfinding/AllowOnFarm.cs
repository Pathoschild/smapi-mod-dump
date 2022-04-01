/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

static partial class Pathfinding {
	private static readonly Func<NPC, int>? GetDefaultFacingDirection = typeof(NPC).GetFieldGetter<NPC, int>("defaultFacingDirection");
	private static readonly Func<NPC?, string?, int, int, string?, int, int, int, string?, string?, SchedulePathDescription>? PathfindToNextScheduleLocation =
		typeof(NPC).GetMethod("pathfindToNextScheduleLocation", BindingFlags.Instance | BindingFlags.NonPublic)?.
		CreateDelegate<Func<NPC?, string?, int, int, string?, int, int, int, string?, string?, SchedulePathDescription>>();

	/*
	[Harmonize(
		typeof(NPC),
		"prepareToDisembarkOnNewSchedulePath",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool PrepareToDisembarkOnNewSchedulePath(NPC __instance) {
		if (!Config.Enabled || !Config.Extras.AllowNPCsOnFarm || !Config.Extras.OptimizeWarpPoints) {
			return true;
		}

		//__instance.finishEndOfRouteAnimation();
		__instance.doingEndOfRouteAnimation.Value = false;
		//__instance.currentlyDoingEndOfRouteAnimation = false;

		return false;
	}
	*/

	// Override 'warpCharacter' so that family entities (pets, spouses, children) actually path to destinations rather than warping.
	[Harmonize(
		typeof(Game1),
		"warpCharacter",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool WarpCharacter(NPC? character, GameLocation? targetLocation, XNA.Vector2 position) {
		if (!Config.Enabled || !Config.Extras.AllowNPCsOnFarm || !Config.Extras.OptimizeWarpPoints) {
			return true;
		}

		if (PathfindToNextScheduleLocation is null) {
			return true;
		}

		if (character is null || Game1.player is not Farmer player) {
			return true;
		}

		if (character.currentLocation is null) {
			return true;
		}

		if (targetLocation is null) {
			return true;
		}

		bool isFamily = false;
		// If the player has friendship data, check if the given character is married or a roommate.
		if (player.friendshipData.TryGetValue(character.Name, out var value)) {
			isFamily = value.IsMarried() || value.IsRoommate();
		}
		// Check if the given character is a child.
		if (!isFamily && (player.getChildren()?.Contains(character) ?? false)) {
			isFamily = true;
		}

		// https://github.com/aedenthorn/StardewValleyMods/blob/master/FreeLove/FarmerPatches.cs
		// If the character is family, a pet, or a spouse, run the logic.
		if (isFamily || character == player.getSpouse() || character == player.getPet()) {
			// If the character is still sleeping, warp it because otherwise it just glides on the floor creepily.
			if (character.isSleeping.Value || character.layingDown) {
				return true;
			}

			// If it's a pet, it appears that 'behavior 1' indicates sleeping.
			if (character is Pet pet && pet.CurrentBehavior is Pet.behavior_Sleep or Pet.behavior_SitDown or Cat.behavior_Flop or Dog.behavior_SitSide) {
				return true;
			}

			// If the character is invisible, don't bother having it move.
			if (character.IsInvisible) {
				return true;
			}

			if (Game1.IsClient) {
				return true;
			}

			// TODO : Child.newborn/baby?

			// Do _not_ execute this logic for Events.
			var trace = new StackTrace();
			foreach (var frame in trace.GetFrames()) {
				if (frame.GetMethod() is MethodBase method) {
					if (method.DeclaringType == typeof(Event) || method.Name.Contains("parseDebugInput")) {
						return true;
					}
				}
			}

			// Try to path. If we fail, revert to default logic.
			int direction = (GetDefaultFacingDirection is null) ? -1 : GetDefaultFacingDirection(character);

			if (character.currentLocation == targetLocation) {
				character.temporaryController = new PathFindController(c: character, location: targetLocation, endPoint: Utility.Vector2ToPoint(position), finalFacingDirection: direction); // TODO: we often do know the expected final facing direction.
				if (character.temporaryController.pathToEndPoint is null || character.temporaryController.pathToEndPoint.Count <= 0) {
					character.temporaryController = null;
					return true;
				}
			}
			else {
				var pathDescription = PathfindToNextScheduleLocation(
					character,
					character.currentLocation.Name,
					(int)character.Position.X,
					(int)character.Position.Y,
					targetLocation.Name,
					(int)position.X,
					(int)position.Y,
					direction,
					null,
					null
				);

				if (pathDescription is null) {
					return true;
				}

				character.queuedSchedulePaths.Insert(0, new KeyValuePair<int, SchedulePathDescription>(0, pathDescription));
			}


			// Logic copied from the vanilla method
			switch (targetLocation.Name) {
				case "Beach" when (Game1.currentSeason == "winter" && Game1.dayOfMonth.WithinInclusive(15, 17)):
					targetLocation = Game1.getLocationFromName("BeachNightMarket");
					break;
				case "Trailer" when Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"):
					targetLocation = Game1.getLocationFromName("Trailer_Big");
					if (position == new XNA.Vector2(12f, 9f)) {
						position = new XNA.Vector2(13f, 24f);
					}
					break;
			}

			// Also copied from vanilla method
			character.isCharging = false;
			character.speed = 2;
			character.blockedInterval = 0;
			if (character.isVillager()) {
				string? targetLocationOverride = null;
				switch (character.Name) {
					case "Maru":
						targetLocationOverride = "Hospital";
						break;
					case "Shane":
						targetLocationOverride = "JojaMart";
						break;
				}
				if (targetLocationOverride is not null) {
					string textureFileName;
					if (targetLocation.Name == targetLocationOverride) {
						textureFileName = $"{character.Name}_{targetLocation.Name}";
					}
					else if (character.Sprite.textureName.Value != character.Name) {
						textureFileName = character.Name;
					}
					else {
						textureFileName = NPC.getTextureNameForCharacter(character.Name);
					}

					character.Sprite.LoadTexture(@$"Characters\{textureFileName}");
				}
			}

			// ALSO copied
			if (character.CurrentDialogue.Count > 0 && character.CurrentDialogue.Peek().removeOnNextMove && !character.getTileLocation().Equals(character.DefaultPosition / 64f)) {
				character.CurrentDialogue.Pop();
			}

			return false;
		}

		return true;
	}

	// TODO: prepareToDisembarkOnNewSchedulePath : need to override Farm-specific logic
	// TODO: reference pathfindToNextScheduleLocation

	// Prevent NPCs from destroying items on the farm.
	[Harmonize(
		typeof(GameLocation),
		"characterDestroyObjectWithinRectangle",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool CharacterDestroyObjectWithinRectangle(GameLocation __instance, ref bool __result, XNA.Rectangle rect, bool showDestroyedObject) {
		if (__instance.IsFarm || __instance.IsGreenhouse) {
			__result = false;
			return false;
		}

		return true;
	}
}
