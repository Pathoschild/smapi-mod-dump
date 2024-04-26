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

using Leclair.Stardew.CloudySkies.LayerData;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Models;

internal record struct EffectCache {

	public WeatherData Data { get; set; }

	public GameLocation Location { get; set; }

	public int Hour { get; set; }

	public bool EventUp { get; set; }

	public Dictionary<string, IEffect> EffectsById { get; set; }
	public Dictionary<string, BaseEffectData> DataById { get; set; }

	public List<IEffect>? Effects { get; set; }

}
