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

using Leclair.Stardew.MoreNightlyEvents.Serialization;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace Leclair.Stardew.MoreNightlyEvents.Models;

[JsonConverter(typeof(EventDataConverter))]
public class BaseEventData {

	#region Identity

	public string Id { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	#endregion

	#region Selection

	public int Priority { get; set; } = 0;

	public List<EventCondition> Conditions { get; set; } = new();

	#endregion

	#region Side Effects

	public List<SideEffectData> SideEffects { get; set; } = new();

	#endregion

	#region Shared Data

	public string? TargetMap { get; set; }

	public Point? TargetPoint { get; set; }

	public string? OverrideWeather { get; set; }

	public int? TimeOfDay { get; set; }

	public Color? AmbientLight { get; set; }

	#endregion

}
