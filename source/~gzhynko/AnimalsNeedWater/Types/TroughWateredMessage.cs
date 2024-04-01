/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;

namespace AnimalsNeedWater.Types
{
    public class TroughWateredMessage
    {
        public readonly string BuildingType;
        public readonly string BuildingUniqueName;

        public TroughWateredMessage(string buildingType, string buildingUniqueName)
        {
            BuildingType = buildingType;
            BuildingUniqueName = buildingUniqueName;
        }
    }
}