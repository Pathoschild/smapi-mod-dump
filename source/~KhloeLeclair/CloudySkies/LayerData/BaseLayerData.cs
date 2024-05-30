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

namespace Leclair.Stardew.CloudySkies.LayerData;

[JsonConverter(typeof(LayerDataConverter))]
public record BaseLayerData : ILayerData {

	public string Id { get; set; } = string.Empty;

	public string Type { get; set; } = string.Empty;

	#region Conditions

	public string? Condition { get; set; }

	public string? Group { get; set; }

	public TargetMapType TargetMapType { get; set; } = TargetMapType.Outdoors;

	#endregion

	#region Shared Rendering

	public LayerDrawType Mode { get; set; } = LayerDrawType.Normal;

	#endregion

}
