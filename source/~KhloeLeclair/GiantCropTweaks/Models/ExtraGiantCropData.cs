/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using StardewValley.GameData.Crops;
using StardewValley.GameData.GiantCrops;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Leclair.Stardew.Common.Serialization.Converters;

namespace Leclair.Stardew.GiantCropTweaks.Models;

public class ExtraGiantCropData : IExtraGiantCropData {

	public string Id { get; set; } = string.Empty;

	#region Behaviors

	public bool CanGrowWhenNotFullyRegrown { get; set; } = true;

	public ReplantBehavior ShouldReplant { get; set; } = ReplantBehavior.WhenRegrowing;

	#endregion

	#region Colors

	/// </summary>
	[JsonProperty(ItemConverterType = typeof(ColorConverter))]
	public List<Color>? Colors { get; set; }

	public bool UseBaseCropTintColors { get; set; } = false;

	public List<string>? HarvestItemsToColor { get; set; }

	public bool RandomizeHarvestItemColors { get; set; } = false;

	#endregion

	#region Overlay Texture

	public string? OverlayTexture { get; set; }

	public bool OverlayPrismatic { get; set; } = false;

	public Point? OverlayPosition { get; set; }

	public Point? OverlaySize { get; set; }

	public Point OverlayOffset { get; set; } = Point.Zero;

	#endregion

}
