/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;
namespace CustomCropsDecay
{
    class ColoredCropWithDecay : ColoredObject, ICropWithDecay
    {
        public float decayDays { get; set; } = int.MaxValue;
        public SDate harvestDate { get; set; } = SDate.Now();


        public ColoredCropWithDecay() : base()
        {
        }

        public ColoredCropWithDecay(int parentSheetIndex, int stack, Color color) : base(parentSheetIndex, stack, color)
        {
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("decayDays", decayDays.ToString());
            savedata.Add("harvestDay", harvestDate.Day.ToString());
            savedata.Add("harvestSeason", harvestDate.Season);
            savedata.Add("harvestYear", harvestDate.Year.ToString());
            return savedata;
        }

        public dynamic getReplacement()
        {
            if (this.harvestDate.AddDays((int)this.decayDays) < SDate.Now())
            {
                ColoredCropWithDecay coloredJunk = copyFrom(new ColoredObject(168, Stack, color));
                assignFrom(coloredJunk);
            }
            Item item = base.getOne();
            item.Stack = this.Stack;
            return item;
        }

        private void assignFrom(ColoredCropWithDecay coloredJunk)
        {
            ParentSheetIndex = coloredJunk.ParentSheetIndex;
            Name = coloredJunk.Name;
            DisplayName = coloredJunk.DisplayName;
            Price = coloredJunk.Price;
            Edibility = coloredJunk.Edibility;
            color.Value = coloredJunk.color.Value;
            Type = coloredJunk.Type;
            Category = coloredJunk.Category;
            setOutdoors.Value = coloredJunk.setOutdoors.Value;
            setIndoors.Value = coloredJunk.setIndoors.Value;
            Fragility = coloredJunk.Fragility;
            isLamp.Value = coloredJunk.isLamp;
            SpecialVariable = coloredJunk.SpecialVariable;
            Scale = coloredJunk.scale;
            Quality = coloredJunk.quality;
            IsSpawnedObject = coloredJunk.isSpawnedObject;
            IsRecipe = coloredJunk.isRecipe;
            HasBeenInInventory = coloredJunk.HasBeenInInventory;
            HasBeenPickedUpByFarmer = coloredJunk.HasBeenPickedUpByFarmer;
            uses.Value = coloredJunk.uses.Value;
            questItem.Value = coloredJunk.questItem;
            questId.Value = coloredJunk.questId;
            preserve.Value = coloredJunk.preserve.Value;
            preservedParentSheetIndex.Value = coloredJunk.preservedParentSheetIndex.Value;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            if (additionalSaveData.TryGetValue("decayDays", out string decayDays)
                && additionalSaveData.TryGetValue("harvestDay", out string harvestDay)
                && additionalSaveData.TryGetValue("harvestSeason", out string harvestSeason)
                && additionalSaveData.TryGetValue("harvestYear", out string harvestYear)
                )
            {
                if (float.TryParse(decayDays, out float floatDecayDays)
                    && int.TryParse(harvestDay, out int intHarvestDay)
                    && int.TryParse(harvestYear, out int intHarvestYear)
                    )
                {
                    this.decayDays = floatDecayDays;
                    this.harvestDate = new SDate(intHarvestDay, harvestSeason, intHarvestYear);
                }
            }
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return ColoredCropWithDecay.copyFrom((ColoredObject)replacement);
        }
        public override Item getOne()
        {
            ColoredCropWithDecay one = copyFrom(this);
            one.Stack = 1;
            one.harvestDate = harvestDate;
            one.decayDays = decayDays;
            return one;
        }

        public static ColoredCropWithDecay copyFrom(ColoredObject replacement)
        {
            ColoredCropWithDecay obj1 = new ColoredCropWithDecay((int)replacement.parentSheetIndex, replacement.Stack, (Color)replacement.color)
            {
                Quality = replacement.quality,
                Price = replacement.price,
                HasBeenInInventory = replacement.HasBeenInInventory,
                HasBeenPickedUpByFarmer = replacement.hasBeenPickedUpByFarmer,
                SpecialVariable = replacement.SpecialVariable
            };
            obj1.preserve.Set(replacement.preserve.Value);
            obj1.preservedParentSheetIndex.Set(replacement.preservedParentSheetIndex.Value);
            obj1.Name = replacement.Name;
            return obj1;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            float decayPercent = 1 - ((SDate.Now().DaysSinceStart - harvestDate.DaysSinceStart) * 1.0f / decayDays);
            if(decayPercent <= 1 && decayPercent >= 0)
            {
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X + 6, (int)(location.Y + (2f * scaleSize)), (int)(52f * scaleSize), (int)(8f * scaleSize)), Color.White * 0.7f);
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X + 8, (int)(location.Y + (4f * scaleSize)), (int)(48f * scaleSize * decayPercent), (int)(4f * scaleSize)), Utility.getRedToGreenLerpColor(decayPercent));
                }
                else
                {
                    int itemSlotSize = ModEntry._reflection.GetProperty<int>(this, "itemSlotSize").GetValue();
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)(location.X + 6.0), (int)(location.Y + 2.0), itemSlotSize - 12, 8), Color.White * 0.7f);
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)(location.X + 8.0), (int)(location.Y + 4.0), (int)((itemSlotSize - 16) * decayPercent), 4), Utility.getRedToGreenLerpColor(decayPercent));
                }
            }
        }
        public override bool canStackWith(ISalable other)
        {
            if (other == null)
            {
                return false;
            }
            if (!(other is ColoredCropWithDecay))
            {
                return false;
            }
            ColoredCropWithDecay otherCrop = (other as ColoredCropWithDecay);
            if (!this.color.Value.Equals(otherCrop.color.Value))
            {
                return false;
            }
            return (maximumStackSize() > 1) && (other.maximumStackSize() > 1) && (this.ParentSheetIndex == otherCrop.ParentSheetIndex) && (this.bigCraftable.Value == otherCrop.bigCraftable.Value) && (this.quality.Value == otherCrop.quality.Value) && this.harvestDate == otherCrop.harvestDate && (int)this.decayDays == (int)otherCrop.decayDays;
        }
    }
}
