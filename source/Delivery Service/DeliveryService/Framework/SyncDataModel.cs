using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DeliveryService.Framework
{
    public class SyncDataModel : SerializableChestLocation
    {
        public DeliveryOptions DeliveryOptions;
        public SyncDataModel() { }
        public SyncDataModel(DeliveryChest chest) : base(chest)
        {
            this.DeliveryOptions = chest.DeliveryOptions;
        }
    }
}
