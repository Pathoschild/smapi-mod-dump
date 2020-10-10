/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AxesOfEvil/SV_DeliveryService
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DeliveryService.Framework
{
    public class SaveDataModel : SerializableChestLocation
    {
        public List<string> Send = new List<string>();
        public List<string> Receive = new List<string>();
        public bool MatchColor;
        public SaveDataModel() { }
        public SaveDataModel(DeliveryChest chest) : base(chest)
        {
            MatchColor = chest.DeliveryOptions.MatchColor;
            foreach (DeliveryCategories cat in Enum.GetValues(typeof(DeliveryCategories)))
            {
                if (chest.DeliveryOptions.Send[(int)cat] > 0)
                    Send.Add(cat.ToString() + ":" + chest.DeliveryOptions.Send[(int)cat].ToString());
                if (chest.DeliveryOptions.Receive[(int)cat] > 0)
                    Receive.Add(cat.ToString() + ":" + chest.DeliveryOptions.Receive[(int)cat].ToString());
            }
        }
    }
}