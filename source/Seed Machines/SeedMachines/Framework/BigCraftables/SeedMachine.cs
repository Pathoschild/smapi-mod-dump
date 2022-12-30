/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

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
    [XmlType("Mods_SeedMachines_SeedMachine")]
    public class SeedMachine : IBigCraftable
    {
        public SeedMachine() : base()
        {
            this.wrapperName = "Seed Machine";
            this.animate();
        }
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
