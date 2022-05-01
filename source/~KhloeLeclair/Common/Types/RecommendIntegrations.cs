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

namespace Leclair.Stardew.Common.Types;

public record struct RecommendedIntegration(
	string Id,
	string Name,
	string Url,

	string[] Mods
);
