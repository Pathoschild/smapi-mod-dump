/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;

namespace TrainStation.Framework
{
    public interface IApi
    {
        void OpenTrainMenu();
        void OpenBoatMenu();

        //If the given StopId already exists, update it with the given data. Otherwise, create a new train stop with the given data
        void RegisterTrainStation(string stopId, string targetMapName, Dictionary<string, string> localizedDisplayName, int targetX, int targetY, int cost, int facingDirectionAfterWarp, string[] conditions, string translatedName);

        //If the given StopId already exists, update it with the given data. Otherwise, create a new boat stop with the given data
        void RegisterBoatStation(string stopId, string targetMapName, Dictionary<string, string> localizedDisplayName, int targetX, int targetY, int cost, int facingDirectionAfterWarp, string[] conditions, string translatedName);
    }
}
