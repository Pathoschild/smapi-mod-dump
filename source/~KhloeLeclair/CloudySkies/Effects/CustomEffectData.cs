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

using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.CloudySkies.Effects;

[DiscriminatedType("Custom")]
public record CustomEffectData : BaseEffectData, ICustomEffectData {

	[JsonExtensionData]
	public IDictionary<string, JToken> Fields { get; set; } = new ValueEqualityDictionary<string, JToken>();

}
