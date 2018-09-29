using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipAnywhere
{
    public class Configuration
    {
        public InputButton OpenShippingBox { get; set; } = new InputButton(Keys.P);
    }
}
