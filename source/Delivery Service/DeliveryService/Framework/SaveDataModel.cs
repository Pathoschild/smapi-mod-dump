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