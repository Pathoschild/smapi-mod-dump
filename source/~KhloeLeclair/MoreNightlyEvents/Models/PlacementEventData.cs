/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using System.Collections.Generic;

using Leclair.Stardew.Common.Serialization.Converters;

using Microsoft.Xna.Framework;

using StardewValley.GameData;
using StardewValley.GameData.FarmAnimals;

namespace Leclair.Stardew.MoreNightlyEvents.Models;

[DiscriminatedType("Placement")]
public class PlacementEventData : BaseEventData {

	public string? SoundName { get; set; }

	public int MessageDelay { get; set; } = 7000;

	public string? Message { get; set; }

	public List<PlacementItemData>? Output { get; set; }

}


public class PlacementItemData : GenericSpawnItemDataWithCondition {

	// Used for everything, to determine the bounds that they can spawn within.
	/// <summary>
	/// An optional list of rectangles of tiles that, if set, will be the
	/// only locations where things can be placed by this event.
	/// </summary>
	public List<Rectangle>? SpawnAreas { get; set; }

	/// <summary>
	/// What type of placeable is this event for.
	/// </summary>
	public PlaceableType Type { get; set; } = PlaceableType.Item;

	/// <summary>
	/// By default, we will continue the event if at least one thing is
	/// placeable. Set this to true to require at least <see cref="ISpawnItemData.MinStack"/>
	/// placeable things.
	/// </summary>
	public bool RequireMinimumSpots { get; set; } = false;


	// Building Fields

	public List<string>? RandomSkinId { get; set; }

	public string? SkinId { get; set; }

	public List<PlacementAnimalData>? Animals { get; set; }


	// Item Fields

	/// <summary>
	/// If placing an item with an inventory, such as a Chest, its contents
	/// will be initialized with the items in this list.
	/// </summary>
	public List<GenericSpawnItemDataWithCondition>? Contents { get; set; }


	// Tree Fields

	/// <summary>
	/// The initial growth stage of crops and tres. For crops, the maximum
	/// growth state varies but for trees it is for. Use -1 for fully grown.
	/// </summary>
	public int GrowthStage { get; set; } = -1;

	/// <summary>
	/// If spawning a fruit tree, attempt to spawn them with this many fruit
	/// present. Please note that this will obey seasonal growth restrictions
	/// unless paired with <see cref="IgnoreSeasons"/>.
	/// </summary>
	public int InitialFruit { get; set; } = 0;

	/// <summary>
	/// Whether the crop or tree should ignore seasonal restrictions. If this
	/// is set to <see cref="IgnoreSeasonsMode.DuringSpawn"/> then the season
	/// will only be ignored when spawning the crop or tree, while setting
	/// it to <see cref="IgnoreSeasonsMode.Always"/> will cause that crop or
	/// tree to remain permanently unaffected by seasons.
	/// </summary>
	public IgnoreSeasonsMode IgnoreSeasons { get; set; } = IgnoreSeasonsMode.Never;


	// Resource Clump Fields
	public int ClumpId { get; set; } = -1;
	public bool ClumpStrictPlacement { get; set; } = false;

	public int ClumpWidth { get; set; } = 2;
	public int ClumpHeight { get; set; } = 2;
	public int? ClumpHealth { get; set; }
	public string? ClumpTexture { get; set; }

}


public class PlacementAnimalData {

	public string Id { get; set; } = string.Empty;

	public string? AnimalId { get; set; }

	public List<string>? RandomAnimalId { get; set; }

	public string? SkinId { get; set; }

	public List<string>? RandomSkinId { get; set; }

	//public FarmAnimalGender Gender { get; set; } = FarmAnimalGender.MaleOrFemale;

	public string? Name { get; set; }

	public List<string>? RandomName { get; set; }

	public int MinHappiness { get; set; }

	public int MaxHappiness { get; set; } = -1;

	public int MinFriendship { get; set; }

	public int MaxFriendship { get; set; } = -1;

}


public enum IgnoreSeasonsMode {
	Never,
	DuringSpawn,
	Always
}


public enum PlaceableType {
	Building,
	Item,
	WildTree,
	FruitTree,
	Crop,
	GiantCrop,
	ResourceClump
}
