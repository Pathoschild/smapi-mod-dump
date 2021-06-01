/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    class ShopDiscount : SingleEffect
    {
        Shop shop;
        double discount;

        public ShopDiscount(Shop shop, double discount)
        {
            this.shop = shop;
            this.discount = discount;
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu && shopMenu.GetShop() == shop)
            {
                // reduce price of all items
                foreach(var element in shopMenu.itemPriceAndStock)
                {
                    int[] arr = element.Value;
                    if (arr != null && arr.Length > 0)
                    {
                        arr[0] = (int)Math.Max(0, arr[0] * (1 - discount));
                    }
                }
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.None, $"Get a slight discount when buying from {shop.GetShopReferral()}");
    }
}
