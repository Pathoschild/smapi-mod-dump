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

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Models;

internal record struct LayerCache {

	public WeatherData Data { get; set; }

	public GameLocation Location { get; set; }

	public int Hour { get; set; }

	public bool EventUp { get; set; }

	public bool HasShaders { get; set; }

	public Dictionary<string, IWeatherLayer> LayersById { get; set; }
	public Dictionary<string, ILayerData> DataById { get; set; }

	public List<IWeatherLayer>? Layers { get; set; }

}
