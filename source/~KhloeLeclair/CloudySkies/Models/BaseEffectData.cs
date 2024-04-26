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

using Leclair.Stardew.CloudySkies.Serialization;

using Newtonsoft.Json;

namespace Leclair.Stardew.CloudySkies.Models;

[JsonConverter(typeof(EffectDataConverter))]
public record BaseEffectData {

	/// <summary>
	/// An identifier for this specific effect within its parent
	/// weather condition. This need only be unique within the weather
	/// condition itself, so you can feel free to use Ids like <c>cold</c>
	/// </summary>
	public string Id { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// How often should this effect update. A value of <c>60</c> is
	/// once per second. Defaults to <c>60</c>.
	/// </summary>
	public uint Rate { get; set; } = 60;

	#region Conditions

	/// <summary>
	/// A condition that must evaluate to true for this effect to
	/// become active. If not set, this effect will always be active. This
	/// condition is only reevaluated upon location change, an event
	/// starting, or the hour changing.
	/// </summary>
	public string? Condition { get; set; }

	/// <summary>
	/// If you set a group, only the first effect in a group will be
	/// active at any given time.
	/// </summary>
	public string? Group { get; set; }

	#endregion

}
