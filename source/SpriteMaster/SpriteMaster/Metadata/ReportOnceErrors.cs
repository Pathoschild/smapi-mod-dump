/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;

namespace SpriteMaster.Metadata;

[Flags]
enum ReportOnceErrors : uint {
	OverlappingSource =			1U << 0,
	InvertedSource =		1U << 1,
	DegenerateSource =	1U << 2,
}
