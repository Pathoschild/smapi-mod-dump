/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Objects;

namespace RemoteFridgeStorage.API
{
    public class RemoteFridgeApi : IRemoteFridgeApi
    {
        private readonly ModEntry _modEntry;

        public RemoteFridgeApi(ModEntry modEntry)
        {
            _modEntry = modEntry;
        }

        public IEnumerable<Chest> GetFridgeChests()
        {
            return _modEntry.ChestController.GetChests();
        }  
    }
}