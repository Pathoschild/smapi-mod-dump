/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    class ShopDiscount : SingleEffect<ShopDiscountParameters>
    {
        public ShopDiscount(ShopDiscountParameters parameters)
            : base(parameters)
        {
            // --
        }

        public ShopDiscount(Shop shop, double discount)
            : base(ShopDiscountParameters.With(shop, discount))
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu && shopMenu.GetShop() == Parameters.Shop)
            {
                // reduce price of all items
                foreach(var element in shopMenu.itemPriceAndStock)
                {
                    int[] arr = element.Value;
                    if (arr != null && arr.Length > 0)
                    {
                        arr[0] = (int)Math.Max(0, arr[0] * (1 - Parameters.Discount));
                    }
                }

                EffectHelper.Overlays.AddSparklingText(new SparklingText(Game1.dialogueFont, $"You received a discount ({Parameters.Discount * 100:0}%)", Color.LimeGreen, Color.Azure), new Vector2(64f, Game1.uiViewport.Height - 64));                
            }
        }      

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.Money, $"Get a slight discount when buying from {Parameters.Shop.GetShopReferral()} ({Parameters.Discount * 100:0}%)");
    }

    public class ShopDiscountParameters : IEffectParameters
    {
        public Shop Shop { get; set; }
        public double Discount { get; set; }

        public static ShopDiscountParameters With(Shop shop, double discount)
        {
            return new ShopDiscountParameters() { Shop = shop, Discount = discount };
        }
    }
}
