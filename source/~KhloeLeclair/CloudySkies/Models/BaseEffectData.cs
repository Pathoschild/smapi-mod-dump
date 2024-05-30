/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Leclair.Stardew.CloudySkies.Serialization;

using Newtonsoft.Json;

namespace Leclair.Stardew.CloudySkies.Models;

[JsonConverter(typeof(EffectDataConverter))]
public record BaseEffectData : IEffectData {

	public string Id { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	public uint Rate { get; set; } = 60;

	#region Conditions

	public string? Condition { get; set; }

	public string? Group { get; set; }

	private TargetMapType? _TargetMapType;

	public TargetMapType TargetMapType {
		get {
			if (!_TargetMapType.HasValue) {
				if (!string.IsNullOrEmpty(Condition) && (Condition.Contains("LOCATION_IS_INDOORS") || Condition.Contains("LOCATION_IS_OUTDOORS")))
					_TargetMapType = TargetMapType.Outdoors | TargetMapType.Indoors;
				else
					_TargetMapType = TargetMapType.Outdoors;
			}

			return _TargetMapType.Value;
		}
		set {
			_TargetMapType = value;
		}
	}

	#endregion

}
