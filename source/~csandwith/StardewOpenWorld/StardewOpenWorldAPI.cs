/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using System;

namespace StardewOpenWorld
{
    public interface IStardewOpenWorldAPI
    {
        public void RegisterBiome(string id, Func<int, int, int, WorldTile> func);
    }
    public class StardewOpenWorldAPI : IStardewOpenWorldAPI
    {
        public void RegisterBiome(string id, Func<int, int, int, WorldTile> func)
        {
            ModEntry.biomes[id] = func;
        }
    }
}