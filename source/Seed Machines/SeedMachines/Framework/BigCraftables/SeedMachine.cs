using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SeedMachines.Framework.BigCraftables
{
    public class SeedMachine : IBigCraftable
    {
        public SeedMachine(StardewValley.Object baseObject, IBigCraftableWrapper wrapper) 
            : base(baseObject, wrapper)
        { }

        public override void onClick(ButtonPressedEventArgs args)
        {
            ModEntry.modHelper.Input.Suppress(args.Button);
            Game1.activeClickableMenu = new ShopMenu(SalableSeedsEnumerator.getSeedsForSale(), 0, null, null, null, null);
        }
    }
}
