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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

namespace SkillfulClothes.Effects.Special
{
    /// <summary>
    /// Grants a discount when constructing buildings
    /// </summary>
    class ConstructDiscount : SingleEffect<ConstructDiscountParameters>
    {
        public ConstructDiscount(ConstructDiscountParameters parameters)
            : base(parameters)
        {
            // --
        }

        public ConstructDiscount(double discount)
            : base(ConstructDiscountParameters.With(discount))
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }        
        
        void applyDiscount(ref int field)
        {
            field = (int)Math.Max(0, field * (1 - Parameters.Discount));
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (Game1.currentLocation?.NameOrUniqueName == "ScienceHouse" && e.NewMenu is CarpenterMenu carpenterMenu)
            {
                var blueprints = EffectHelper.ModHelper.Reflection.GetField<List<BluePrint>>(e.NewMenu, "blueprints").GetValue();
                foreach(var blueprint in blueprints)
                {
                    applyDiscount(ref blueprint.moneyRequired);
                    applyDiscount(ref blueprint.woodRequired);

                    applyDiscount(ref blueprint.stoneRequired);
                    applyDiscount(ref blueprint.copperRequired);
                    applyDiscount(ref blueprint.IronRequired);
                    applyDiscount(ref blueprint.GoldRequired);
                    applyDiscount(ref blueprint.IridiumRequired);

                    var items = blueprint.itemsRequired.ToList();
                    blueprint.itemsRequired.Clear();
                    foreach (var item in items)
                    {
                        int amount = item.Value;
                        applyDiscount(ref amount);

                        if (amount > 0)
                        {
                            blueprint.itemsRequired.Add(item.Key, amount);
                        }
                    }                    
                }

                carpenterMenu.setNewActiveBlueprint();

                EffectHelper.Overlays.AddSparklingText(new SparklingText(Game1.dialogueFont, $"You received a discount ({Parameters.Discount * 100:0}%)", Color.LimeGreen, Color.Azure), new Vector2(64f, Game1.uiViewport.Height - 64));
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.Money, $"Get a slight discount when constructing buildings ({Parameters.Discount * 100:0}%)");
    }

    public class ConstructDiscountParameters : IEffectParameters
    {
        public double Discount { get; set; }

        public static ConstructDiscountParameters With(double discount)
        {
            return new ConstructDiscountParameters() { Discount = discount };
        }
    }
}
