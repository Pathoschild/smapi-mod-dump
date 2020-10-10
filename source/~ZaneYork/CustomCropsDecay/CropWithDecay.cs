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
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;
namespace CustomCropsDecay
{
    class CropWithDecay : SObject, ICropWithDecay
    {
        public float decayDays { get; set; } = int.MaxValue;
        public SDate harvestDate { get; set; } = SDate.Now();

        public CropWithDecay() : base()
        {
        }

        public CropWithDecay(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false) : base(tileLocation, parentSheetIndex, isRecipe)
        {
        }

        public CropWithDecay(Vector2 tileLocation, int parentSheetIndex, int initialStack) : base(tileLocation, parentSheetIndex, initialStack)
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
                CropWithDecay junk = copyFrom(new SObject(168, Stack));
                assignFrom(junk);
            }
            Item item = base.getOne();
            item.Stack = this.Stack;
            return item;
        }

        private void assignFrom(CropWithDecay junk)
        {
            ParentSheetIndex = junk.ParentSheetIndex;
            Name = junk.Name;
            DisplayName = junk.DisplayName;
            Price = junk.Price;
            Edibility = junk.Edibility;
            Type = junk.Type;
            Category = junk.Category;
            setOutdoors.Value = junk.setOutdoors.Value;
            setIndoors.Value = junk.setIndoors.Value;
            Fragility = junk.Fragility;
            isLamp.Value = junk.isLamp;
            SpecialVariable = junk.SpecialVariable;
            Scale = junk.scale;
            Quality = junk.quality;
            IsSpawnedObject = junk.isSpawnedObject;
            IsRecipe = junk.isRecipe;
            HasBeenInInventory = junk.HasBeenInInventory;
            HasBeenPickedUpByFarmer = junk.HasBeenPickedUpByFarmer;
            uses.Value = junk.uses.Value;
            questItem.Value = junk.questItem;
            questId.Value = junk.questId;
            preserve.Value = junk.preserve.Value;
            preservedParentSheetIndex.Value = junk.preservedParentSheetIndex.Value;
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
            return CropWithDecay.copyFrom((SObject)replacement);
        }
        public override Item getOne()
        {
            CropWithDecay one = copyFrom(this);
            one.Stack = 1;
            one.harvestDate = harvestDate;
            one.decayDays = decayDays;
            return one;
        }

        public static CropWithDecay copyFrom(SObject replacement)
        {
            if (replacement.bigCraftable)
            {
                int parentSheetIndex = replacement.ParentSheetIndex;
                if (replacement.name.Contains("Seasonal"))
                {
                    parentSheetIndex = replacement.ParentSheetIndex - replacement.ParentSheetIndex % 4;
                }
                return new CropWithDecay(replacement.tileLocation, parentSheetIndex)
                {
                    IsRecipe = replacement.isRecipe,
                    name = replacement.name,
                    DisplayName = replacement.DisplayName,
                    SpecialVariable = replacement.SpecialVariable
                };
            }
            CropWithDecay @object = new CropWithDecay(replacement.tileLocation, replacement.parentSheetIndex, 1);
            @object.Scale = replacement.scale;
            @object.Quality = replacement.quality;
            @object.IsSpawnedObject = replacement.isSpawnedObject;
            @object.IsRecipe = replacement.isRecipe;
            @object.Stack = replacement.Stack;
            @object.SpecialVariable = replacement.SpecialVariable;
            @object.Price = replacement.price;
            @object.name = replacement.name;
            @object.DisplayName = replacement.DisplayName;
            @object.HasBeenInInventory = replacement.HasBeenInInventory;
            @object.HasBeenPickedUpByFarmer = replacement.HasBeenPickedUpByFarmer;
            @object.uses.Value = replacement.uses.Value;
            @object.questItem.Value = replacement.questItem;
            @object.questId.Value = replacement.questId;
            @object.preserve.Value = replacement.preserve.Value;
            @object.preservedParentSheetIndex.Value = replacement.preservedParentSheetIndex.Value;
            return @object;
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            float decayPercent = 1 - ((SDate.Now().DaysSinceStart - harvestDate.DaysSinceStart) * 1.0f / decayDays);
            if (decayPercent <= 1 && decayPercent >= 0)
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
            if (!(other is CropWithDecay))
            {
                return false;
            }
            CropWithDecay otherCrop = (other as CropWithDecay);
            return (maximumStackSize() > 1) && (other.maximumStackSize() > 1) && (this.ParentSheetIndex == otherCrop.ParentSheetIndex) && (this.bigCraftable.Value == otherCrop.bigCraftable.Value) && (this.quality.Value == otherCrop.quality.Value) && this.harvestDate == otherCrop.harvestDate && (int)this.decayDays == (int)otherCrop.decayDays;
        }
    }
}
