/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Linq;

using Leclair.Stardew.Common;

using StardewValley;

namespace Leclair.Stardew.Almanac;

public record struct CropInfo(
	string Id,
	Item Item,
	string Name,
	SpriteInfo? Sprite,
	bool IsTrellisCrop,
	bool IsGiantCrop,
	SpriteInfo? GiantSprite,
	Item[]? Seeds,
	int[] Phases,
	SpriteInfo?[]? PhaseSprites,
	int Regrow,
	bool IsPaddyCrop,
	WorldDate? StartDate,
	WorldDate? EndDate
) {

	public int Days => Phases.Sum();

};

public struct OldCropInfo {

	// Deduplication
	public string Id { get; }

	// Main Display
	public Item Item { get; }
	public string Name { get; }
	public SpriteInfo Sprite { get; }

	public bool IsTrellisCrop { get; }
	public bool IsGiantCrop { get; }
	public SpriteInfo GiantSprite { get; }


	// Seeds
	public Item[] Seeds { get; }


	// Phases
	public int[] Phases { get; }
	public SpriteInfo[] PhaseSprites { get; }

	public int Days { get; }
	public int Regrow { get; }
	public bool IsPaddyCrop { get; }

	public WorldDate StartDate { get; }
	public WorldDate EndDate { get; }

	public OldCropInfo(string id, Item item, string name, SpriteInfo sprite, bool giantCrop, SpriteInfo giantSprite, Item[] seeds, bool trellisCrop, int[] phases, int regrow, bool paddyCrop, SpriteInfo[] phaseSprites, WorldDate startDate, WorldDate endDate) {
		Id = id;
		Item = item;
		Name = name;
		Sprite = sprite;
		IsGiantCrop = giantCrop;
		GiantSprite = giantSprite;
		Seeds = seeds;
		IsTrellisCrop = trellisCrop;
		Phases = phases;
		Regrow = regrow;
		IsPaddyCrop = paddyCrop;
		PhaseSprites = phaseSprites;
		Days = Phases.Sum();
		StartDate = startDate;
		EndDate = endDate;
	}
}
