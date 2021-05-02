/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework
{
    public class DataInfo 
    {
        public string type;
        public object data;
        public string recipientID;

        public DataInfo()
        {
        }

        public DataInfo(string Type,object Data)
        {
            this.type = Type;
            this.data = Data;
            this.recipientID = "";
        }

        public DataInfo(string Type, object Data, Farmer farmer)
        {
            this.type = Type;
            this.data = Data;
            this.recipientID = farmer.UniqueMultiplayerID.ToString();
        }

        public DataInfo(string Type, object Data, string uniqueID)
        {
            this.type = Type;
            this.data = Data;
            this.recipientID = uniqueID;
        }
    }
}
