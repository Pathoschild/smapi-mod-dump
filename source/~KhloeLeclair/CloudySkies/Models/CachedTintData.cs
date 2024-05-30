/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Models;

internal record struct CachedTintData {

	public WeatherData Data { get; set; }

	public GameLocation Location { get; set; }

	public bool EventUp { get; set; }

	public int StartTime { get; set; }

	public int EndTime { get; set; }

	public int DurationInTenMinutes { get; set; }

	public int FullyDarkTime { get; set; }
	public int AmbientStartTime { get; set; }
	public int AmbientEndTime { get; set; }
	public int AmbientDurationInTenMinutes { get; set; }

	public bool HasAmbientColor { get; set; }

	public bool HasLightingTint { get; set; }

	public bool HasPostLightingTint { get; set; }

	public Color StartAmbientColor { get; set; }

	public Color EndAmbientColor { get; set; }

	public float StartAmbientOutdoorOpacity { get; set; }

	public float EndAmbientOutdoorOpacity { get; set; }

	public Color StartLightingTint { get; set; }

	public Color EndLightingTint { get; set; }

	public float StartLightingTintOpacity { get; set; }

	public float EndLightingTintOpacity { get; set; }

	public Color StartPostLightingTint { get; set; }

	public Color EndPostLightingTint { get; set; }

	public float StartPostLightingTintOpacity { get; set; }

	public float EndPostLightingTintOpacity { get; set; }

}
