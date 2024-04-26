/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Leclair.Stardew.Common.Serialization.Converters;

namespace Leclair.Stardew.MoreNightlyEvents.Models;

[DiscriminatedType("Script")]
public class ScriptEventData : BaseEventData {

	public string? Script { get; set; }

}
