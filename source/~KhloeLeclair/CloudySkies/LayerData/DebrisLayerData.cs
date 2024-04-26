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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Serialization.Converters;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace Leclair.Stardew.CloudySkies.LayerData;

[DiscriminatedType("Debris")]
public record DebrisLayerData : BaseLayerData {

	public string? Texture { get; set; }

	public List<Rectangle>? Sources { get; set; }

	public int UseSeasonal { get; set; } = -1;

	public int MinTimePerFrame { get; set; } = 76;

	public int MaxTimePerFrame { get; set; } = 126;

	public float Scale { get; set; } = 3f;

	public bool FlipHorizontal { get; set; }

	public bool FlipVertical { get; set; }

	public Vector2 Speed { get; set; } = Vector2.Zero;

	public int MinCount { get; set; } = 16;
	public int MaxCount { get; set; } = 64;

	public bool ShouldAnimate { get; set; } = true;

	public bool CanBlow { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? Color { get; set; }

	public float Opacity { get; set; } = 1f;

}
