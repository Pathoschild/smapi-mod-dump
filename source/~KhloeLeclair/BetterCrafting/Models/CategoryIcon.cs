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

using Leclair.Stardew.Common.Enums;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace Leclair.Stardew.BetterCrafting.Models;

public class CategoryIcon {

	public enum IconType {
		Item,
		Texture
	}

	public IconType Type { get; set; } = IconType.Item;

	// Item
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string? RecipeName { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string? ItemId { get; set; }

	// Texture
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public GameTexture? Source { get; set; } = null;
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public string? Path { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public Rectangle? Rect { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public float Scale { get; set; } = 1;

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public int Frames { get; set; } = 1;

}
