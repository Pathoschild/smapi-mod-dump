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
using static StardewValley.Menus.CarpenterMenu;
using Force.DeepCloner;
using StardewValley.GameData.Buildings;

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
        
        int applyDiscount(int value)
        {
            return (int)Math.Max(0, value * (1 - Parameters.Discount));
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (Game1.currentLocation?.NameOrUniqueName == "ScienceHouse" && e.NewMenu is CarpenterMenu carpenterMenu)
            {                
                foreach(var blueprint in carpenterMenu.Blueprints)
                {
                    // apply the discount as a skin, otherwise we alter the game's base data directly
                    BuildingSkin discountedSkin;

                    if (blueprint.Skin == null)
                    {
                        discountedSkin = new BuildingSkin();
                        discountedSkin.BuildCost = applyDiscount(blueprint.Data.BuildCost);

                        if (blueprint.Data.BuildMaterials != null)
                        {
                            discountedSkin.BuildMaterials = blueprint.Data.BuildMaterials.Select(x => x.DeepClone()).ToList();
                        }                        
                    } else
                    {
                        discountedSkin = blueprint.Skin.DeepClone<BuildingSkin>();
                        discountedSkin.BuildCost = applyDiscount(blueprint.Skin.BuildCost.Value);
                        
                    }

                    if (discountedSkin.BuildMaterials != null)
                    {
                        foreach (var material in discountedSkin.BuildMaterials)
                        {
                            material.Amount = applyDiscount(material.Amount);
                        }
                    }

                    var skinProp = EffectHelper.ModHelper.Reflection.GetProperty<BuildingSkin>(blueprint, nameof(BlueprintEntry.Skin));
                    skinProp.SetValue(discountedSkin);                    
                }
                carpenterMenu.SetNewActiveBlueprint(0);

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
