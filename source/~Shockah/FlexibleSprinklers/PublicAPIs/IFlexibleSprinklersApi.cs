/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

#nullable enable
namespace Shockah.FlexibleSprinklers
{
	/// <summary>The API which provides access to Flexible Sprinklers for other mods.</summary>
	public interface IFlexibleSprinklersApi
	{
		/// <summary>Returns the mod's configuration that can used in a read-only way.</summary>
		ModConfig Config { get; }

		/// <summary>Returns whether the current configuration allows independent sprinkler activation.</summary>
		bool IsSprinklerBehaviorIndependent { get; }

		/// <summary>
		/// Register a new sprinkler coverage provider, to add support for Flexible Sprinklers for your custom <c>StardewValley.Object</c> sprinklers in your mod or override existing ones.<br/>
		/// Returned tile coverage should be relative.<br />
		/// Return `null` if you don't want to modify this specific coverage.
		/// </summary>
		void RegisterSprinklerCoverageProvider(Func<SObject, IReadOnlySet<IntPoint>?> provider);

		/// <summary>
		/// Register a new sprinkler info interceptor, allowing for modifying any type of (custom) sprinklers for use with Flexible Sprinklers.
		/// </summary>
		void RegisterSprinklerInfoInterceptor(Action<GameLocation, ISet<SprinklerInfo>> provider);

		/// <summary>
		/// Registers a new custom waterable tile provider, to make some tiles count as waterable or not.<br/>
		/// Return `true` if the tile should be waterable no matter what; return `false` if the tile should not be waterable no matter what; return `null` if you don't want to modify this specific tile.
		/// </summary>
		void RegisterCustomWaterableTileProvider(Func<GameLocation, IntPoint, bool?> provider);

		/// <summary>Activates all sprinklers in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
		void ActivateAllSprinklers();

		/// <summary>Activates sprinklers in specified location in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
		void ActivateSprinklersInLocation(GameLocation location);

		/// <summary>Activates a sprinkler, taking into account the Flexible Sprinklers mod behavior.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
		void ActivateSprinkler(SObject sprinkler);

		/// <summary>Returns the sprinkler's power after config modifications (that is, the number of tiles it will water).</summary>
		int GetSprinklerPower(SObject sprinkler);

		/// <summary>Returns a sprinkler's spread range (that is, how many tiles away will it look for tiles to water) for a given sprinkler power (if evenly spread around it).</summary>
		int GetSprinklerSpreadRange(int power);

		/// <summary>Returns a sprinkler's focused range (that is, how many tiles away will it look for tiles to water) for a given unmodified coverage (if focused in one direction).</summary>
		int GetSprinklerFocusedRange(IntRectangle occupiedSpace, IReadOnlyCollection<IntPoint> coverage);

		/// <summary>Returns a sprinkler's max range (that is, how many tiles away will it look for tiles to water) for a given sprinkler (the highest of the spread and focused ranges).</summary>
		int GetSprinklerMaxRange(SObject sprinkler);

		/// <summary>Returns a sprinkler's max range (that is, how many tiles away will it look for tiles to water) for a given sprinkler info (the highest of the spread and focused ranges).</summary>
		int GetSprinklerMaxRange(SprinklerInfo info);

		/// <summary>Get the relative tile coverage by supported sprinkler ID. This API takes into consideration the position. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
		IReadOnlySet<IntPoint> GetUnmodifiedSprinklerCoverage(SObject sprinkler);

		/// <summary>Get the relative tile coverage by supported sprinkler ID, modified by the Flexible Sprinklers mod. This API takes into consideration the location and position. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
		IReadOnlySet<IntPoint> GetModifiedSprinklerCoverage(SObject sprinkler);

		/// <summary>Returns whether a given tile is in range of any sprinkler in the location.</summary>
		bool IsTileInRangeOfAnySprinkler(GameLocation location, IntPoint tileLocation);

		/// <summary>Returns whether a given tile is in range of the specified sprinkler.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
		bool IsTileInRangeOfSprinkler(SObject sprinkler, IntPoint tileLocation);

		/// <summary>Returns whether a given tile is in range of specified sprinklers.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
		bool IsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, IntPoint tileLocation);

		/// <summary>Returns whether a given tile is in range of specified sprinklers.</summary>
		/// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
		bool IsTileInRangeOfSprinklers(IEnumerable<SprinklerInfo> sprinklers, GameLocation location, IntPoint tileLocation);

		/// <summary>Returns all tiles that are currently in range of any sprinkler in the location.</summary>
		IReadOnlySet<IntPoint> GetAllTilesInRangeOfSprinklers(GameLocation location);

		/// <summary>Returns all sprinklers of any nature in the given location.</summary>
		IReadOnlySet<SprinklerInfo> GetAllSprinklers(GameLocation location);

		/// <summary>Displays the sprinkler coverage for the specified time.</summary>
		/// <param name="seconds">The amount of seconds to display the coverage for. Pass `null` to use the value configured by the user.</param>
		void DisplaySprinklerCoverage(float? seconds = null);
	}
}