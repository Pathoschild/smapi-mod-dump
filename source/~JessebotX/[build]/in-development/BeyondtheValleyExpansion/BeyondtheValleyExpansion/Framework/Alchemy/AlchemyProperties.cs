using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondTheValleyExpansion.Framework.Alchemy.Properties;

namespace BeyondTheValleyExpansion.Framework.Alchemy
{
    class AlchemyProperties
    {
        /*********
        ** Fields
        *********/
        /* -- General -- */
        /// <summary> Alchemy property: 'Potency'. </summary>
        public PotencyProperty Potency = new PotencyProperty();
        /// <summary> Alchemy property: 'Density'. </summary>
        public DensityProperty Density = new DensityProperty();
        /// <summary> Alchemy property: 'Growth'. </summary>
        public GrowthProperty Growth = new GrowthProperty();

        /* -- Specific -- */
        /// <summary> Alchemy property: 'Fire'. </summary>
        public FireProperty Fire = new FireProperty();
        /// <summary> Alchemy property: 'Water'. </summary>
        public WaterProperty Water = new WaterProperty();
        /// <summary> Alchemy property: 'Purity'. </summary>
        public PurityProperty Purity = new PurityProperty();

        /*********
         ** Methods
         *********/
        /// <summary> 
        /// All potion properties: <paramref name="potency"/>, <paramref name="density"/>, <paramref name="growth"/>, <paramref name="fire"/>, <paramref name="water"/> and <paramref name="nature"/>. 
        /// </summary>
        /// <param name="potency"> <see cref="Potency"/>: the strength of the potion effects. </param>
        /// <param name="density"> <see cref="Density"/>: how solid the substance is, lowers reactivity but increases damage.</param>
        /// <param name="growth"> <see cref="Growth"/>: how long a potion effect lasts. </param>
        /// <param name="fire"> <see cref="Fire"/> determines damage stat </param>
        /// <param name="water"> <see cref="Water"/> determines defence stat </param>
        /// <param name="purity"> <see cref="Purity"/> determines support/utility stat </param>
        public void PotionProperties(float potency, float density, float growth, float fire, float water, float purity)
        {
            this.Potency.Amount += potency * RefMod.Config.Alchemy.PropertyMultiplier["Potency"];
            this.Density.Amount += density * RefMod.Config.Alchemy.PropertyMultiplier["Density"];
            this.Growth.Amount += growth * RefMod.Config.Alchemy.PropertyMultiplier["Growth"];
            this.Fire.Amount += fire * RefMod.Config.Alchemy.PropertyMultiplier["Fire"];
            this.Water.Amount += water * RefMod.Config.Alchemy.PropertyMultiplier["Water"];
            this.Purity.Amount += purity * RefMod.Config.Alchemy.PropertyMultiplier["Purity"];
        }

        public void AlchemyGemCategory(Farmer who)
        {
            AlchemyFramework.ItemsInUsed.Add(Game1.player.CurrentItem.ParentSheetIndex);
            Game1.player.removeItemsFromInventory(Game1.player.CurrentItem.ParentSheetIndex, 1);

            if (AlchemyFramework.GemCategoryItems == 0f)
            {
                this.PotionProperties(
                     /* Potency: */ -10f,
                     /* Density: */ (Convert.ToSingle(who.CurrentItem.salePrice()) / 20f),
                     /* Growth: */  -10f,
                     /* Fire: */    10f,
                     /* Water: */   0f,
                     /* Purity: */  0f
                 );

                AlchemyFramework.GemCategoryItems += 1f;
            }

            else if (AlchemyFramework.GemCategoryItems >= 1f)
            {
                this.PotionProperties(
                    /* Potency: */ -10f,
                    /* Density: */ (Convert.ToSingle(who.CurrentItem.salePrice()) / 20f) - (Convert.ToSingle(who.CurrentItem.salePrice()) / 2 * AlchemyFramework.GemCategoryItems),
                    /* Growth: */  -10f,
                    /* Fire: */    10f,
                    /* Water: */   0f,
                    /* Purity: */  0f
                );
            }
        }
    }
}
