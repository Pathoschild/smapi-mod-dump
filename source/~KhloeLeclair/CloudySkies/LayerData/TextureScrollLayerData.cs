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

[DiscriminatedType("TextureScroll")]
public record TextureScrollLayerData : BaseLayerData {

	public string? Texture { get; set; }

	public Rectangle? Source { get; set; }

	public float Scale { get; set; } = 4;

	public bool FlipHorizontal { get; set; }

	public bool FlipVertical { get; set; }

	public Vector2 Speed { get; set; } = Vector2.Zero;

	[JsonConverter(typeof(ColorConverter))]
	public Color? Color { get; set; }

	public float Opacity { get; set; } = 1f;

}
