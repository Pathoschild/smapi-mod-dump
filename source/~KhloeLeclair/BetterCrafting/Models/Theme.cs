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

using Newtonsoft.Json;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Serialization.Converters;

namespace Leclair.Stardew.BetterCrafting.Models;

public class Theme {

	[JsonConverter(typeof(ColorConverter))]
	public Color? SearchHighlightColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? QuantityCriticalTextColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? QuantityCriticalShadowColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? QuantityWarningTextColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? QuantityWarningShadowColor { get; set; }


	public bool CustomTooltip { get; set; }
	public bool CustomScroll { get; set; }
	public bool CustomMouse { get; set; }


	[JsonConverter(typeof(ColorConverter))]
	public Color? TextColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? TextShadowColor { get; set; }


	[JsonConverter(typeof(ColorConverter))]
	public Color? TooltipTextColor { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? TooltipTextShadowColor { get; set; }

}
