/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    interface IBossMonster
    {
        string DisplayName { get; }

        string MapPath { get; }

        string TextureName { get; }

        Vector2 SpawnLocation { get; }

        List<string> MusicTracks { get; }

        bool InitializeWithHealthbar { get; }

        float Difficulty { get; set; }
    }
}
