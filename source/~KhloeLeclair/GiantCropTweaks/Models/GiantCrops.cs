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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

namespace Leclair.Stardew.GiantCropTweaks.Models;

/// <inheritdoc />
public class GiantCrops : IGiantCropData {

	/// <inheritdoc />
	public string ID { get; set; } = string.Empty;

	/// <inheritdoc />
	public string Texture { get; set; } = string.Empty;

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public Point Corner { get; set; } = Point.Zero;

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public Point TileSize { get; set; } = new Point(3, 3);

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public double Chance { get; set; } = 0.01d;

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public string? HarvestedItemId { get; set; }

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public int MinYields { get; set; } = 15;

	/// <inheritdoc />
	[ContentSerializer(Optional = true)]
	public int MaxYields { get; set; } = 21;

}
