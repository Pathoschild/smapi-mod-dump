using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Framework
{
    public class SerializableChestLocation
    {
        public string Location = "None";
        public int X = 0; 
        public int Y = 0;
        public bool isFridge = false;
        public SerializableChestLocation() { }
        public SerializableChestLocation(string location, int x, int y, bool is_fridge)
        {
            Location = location;
            X = x;
            Y = y;
            isFridge = is_fridge;
        }
        public SerializableChestLocation(DeliveryChest chest)
        {
            if (chest == null || chest.Location == null)
                return;
            if (chest.Location.Location != null)
                Location = chest.Location.Location.NameOrUniqueName;
            X = (int)chest.Location.TileLocation.X;
            Y = (int)chest.Location.TileLocation.Y;
            isFridge = chest.IsFridge();
        }
    }
}
