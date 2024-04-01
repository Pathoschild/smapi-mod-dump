/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ItemExtensions.Models.Contained;

public class MonsterSpawnData
{
    public string Name { get; set; } = null;
    public int Health { get; set; } = -1;
    public bool Hardmode { get; set; }
    public Vector2 Distance { get; set; } = new();
    public bool ExcludeOriginalDrops { get; set; }
    public List<ExtraSpawn> ExtraDrops { get; set; } = null;
}