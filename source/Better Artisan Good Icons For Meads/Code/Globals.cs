/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/bagi-meads
**
*************************************************/

using System;

namespace DaLion.Meads;

internal class Globals
{
    public const int MEAD_INDEX_I = 459;

    public static readonly object MeadAsArtisanGoodEnum =
        Enum.ToObject("BetterArtisanGoodIcons.ArtisanGood".ToType(), MEAD_INDEX_I);
}