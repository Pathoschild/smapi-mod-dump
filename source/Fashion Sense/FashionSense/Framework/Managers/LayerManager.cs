/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances;
using FashionSense.Framework.Models.Appearances.Accessory;
using FashionSense.Framework.Models.Appearances.Hair;
using FashionSense.Framework.Models.Appearances.Hat;
using FashionSense.Framework.Models.Appearances.Pants;
using FashionSense.Framework.Models.Appearances.Shirt;
using FashionSense.Framework.Models.Appearances.Shoes;
using FashionSense.Framework.Models.Appearances.Sleeves;
using FashionSense.Framework.Models.General;
using FashionSense.Framework.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace FashionSense.Framework.Managers
{
    internal class LayerManager
    {
        private IMonitor _monitor;
        private int _facingDirection;

        public LayerManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public List<LayerData> SortModelsForDrawing(Farmer who, int facingDirection, List<AppearanceMetadata> metadata)
        {
            // Set the required variables
            _facingDirection = facingDirection;
            List<AppearanceModel> models = metadata.Where(m => m.Model is not null).Select(m => m.Model).ToList();

            // Establish the rawLayerData list
            List<LayerData> rawLayerData = new List<LayerData>();

            // Add in LayerData for vanilla appearances, if models doesn't contain them
            AddVanillaLayerData(models, ref rawLayerData);

            // Add in existing models, defaulting to vanilla if certain conditions are not met
            foreach (var data in metadata)
            {
                switch (data.Model)
                {
                    case PantsModel pantsModel:
                        AddPants(who, pantsModel, data.Color, ref rawLayerData);
                        break;
                    case ShoesModel shoesModel:
                        AddShoes(who, shoesModel, data.Color, ref rawLayerData);
                        break;
                    case ShirtModel shirtModel:
                        AddShirt(who, shirtModel, data.Color, ref rawLayerData);
                        break;
                    case SleevesModel sleevesModel:
                        AddSleeves(who, sleevesModel, data.Color, ref rawLayerData);
                        break;
                    case AccessoryModel accessoryModel:
                        AddAccessory(who, accessoryModel, data.Color, ref rawLayerData);
                        break;
                    case HairModel hairModel:
                        AddHair(who, hairModel, data.Color, ref rawLayerData);
                        break;
                    case HatModel hatModel:
                        AddHat(who, hatModel, data.Color, ref rawLayerData);
                        break;
                }
            }

            // Establish the initial sorted order, assuming no conditional changes are required
            List<LayerData> sortedLayerData = new List<LayerData>()
            {
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Player),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Pants),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Shoes),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Shirt),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Sleeves),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Hair),
                rawLayerData.First(d => d.AppearanceType is AppearanceContentPack.Type.Hat),
            };
            sortedLayerData.InsertRange(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Sleeves) + 1, rawLayerData.Where(d => d.AppearanceType is AppearanceContentPack.Type.Accessory));

            // Sort the models in the actual correct order
            foreach (var layerData in sortedLayerData.ToList())
            {
                // If the LayerData is using vanilla logic, skip any conditional checks
                if (layerData.IsVanilla)
                {
                    continue;
                }

                switch (layerData.AppearanceType)
                {
                    case AppearanceContentPack.Type.Player:
                        SortPlayer(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Pants:
                        SortPants(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Shoes:
                        SortShoes(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Shirt:
                        SortShirt(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Accessory:
                        SortAccessory(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Hair:
                        SortHair(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Sleeves:
                        SortSleeves(layerData, ref sortedLayerData);
                        break;
                    case AppearanceContentPack.Type.Hat:
                        SortHat(layerData, ref sortedLayerData);
                        break;
                }
            }

            return sortedLayerData;
        }

        #region Conditional check methods
        private bool IsHatHidingHair(List<LayerData> rawLayerData)
        {
            return rawLayerData.Any(d => d.AppearanceModel is HatModel hatModel && hatModel.HideHair is true);
        }

        private bool AreSleevesForcedHidden(List<LayerData> rawLayerData)
        {
            return rawLayerData.Any(d => d.AppearanceModel is not null && d.AppearanceModel.HideSleeves is true);
        }
        #endregion

        private void AddVanillaLayerData(List<AppearanceModel> models, ref List<LayerData> rawLayerData)
        {
            rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Player, null, isVanilla: true));
            if (models.Any(m => m is PantsModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Pants, null, isVanilla: true));
            }
            if (models.Any(m => m is ShoesModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Shoes, null, isVanilla: true));
            }
            if (models.Any(m => m is ShirtModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Shirt, null, isVanilla: true));
            }
            if (models.Any(m => m is SleevesModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Sleeves, null, isVanilla: true));
            }
            if (models.Any(m => m is AccessoryModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Accessory, null, isVanilla: true));
            }
            if (models.Any(m => m is HairModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Hair, null, isVanilla: true));
            }
            if (models.Any(m => m is HatModel) is false)
            {
                rawLayerData.Add(new LayerData(AppearanceContentPack.Type.Hat, null, isVanilla: true));
            }
        }
        private void MoveLayerDataItem(int index, LayerData layerData, ref List<LayerData> sourceList)
        {
            sourceList.Remove(layerData);
            sourceList.Insert(index, layerData);
        }

        #region Add methods for rawLayerData
        private void AddPants(Farmer who, PantsModel pantsModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Pants, pantsModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, pantsModel))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddShoes(Farmer who, ShoesModel shoesModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Shoes, shoesModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, shoesModel) || AppearanceHelpers.ShouldHideLegs(who, _facingDirection))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddShirt(Farmer who, ShirtModel shirtModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Shirt, shirtModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, shirtModel))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddAccessory(Farmer who, AccessoryModel accessoryModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Accessory, accessoryModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, accessoryModel))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddHair(Farmer who, HairModel hairModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Hair, hairModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, hairModel))
            {
                if (IsHatHidingHair(rawLayerData))
                {
                    layerData.IsVanilla = true;
                }
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddSleeves(Farmer who, SleevesModel sleevesModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Sleeves, sleevesModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, sleevesModel) || AreSleevesForcedHidden(rawLayerData))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }

        private void AddHat(Farmer who, HatModel hatModel, Color color, ref List<LayerData> rawLayerData)
        {
            var layerData = new LayerData(AppearanceContentPack.Type.Hat, hatModel);
            if (AppearanceHelpers.ShouldHideWhileSwimmingOrWearingBathingSuit(who, hatModel))
            {
                layerData.IsVanilla = true;
            }
            layerData.Color = color;

            rawLayerData.Add(layerData);
        }
        #endregion

        #region Sort methods for sortedLayerData
        private void SortPlayer(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            // Player layer has no conditional checks
        }

        private void SortPants(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            // Pants have no conditional checks
        }

        private void SortShoes(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            var shoesModel = layerData.AppearanceModel as ShoesModel;
            if (shoesModel.DrawBeforePants)
            {
                MoveLayerDataItem(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Pants), layerData, ref sortedLayerData);
            }
        }

        private void SortShirt(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            // Shirts have no conditional checks
        }

        private void SortAccessory(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            var accessoryModel = layerData.AppearanceModel as AccessoryModel;
            if (accessoryModel.DrawAfterPlayer)
            {
                // Move to bottom of list
                sortedLayerData.Remove(layerData);
                sortedLayerData.Add(layerData);
            }
            else if (accessoryModel.DrawBeforeBase)
            {
                MoveLayerDataItem(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Player), layerData, ref sortedLayerData);
            }
            else if (accessoryModel.DrawAfterSleeves)
            {
                MoveLayerDataItem(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Sleeves) + 1, layerData, ref sortedLayerData);
            }
        }

        private void SortHair(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            // Hair has no conditional checks
        }

        private void SortSleeves(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            var sleevesModel = layerData.AppearanceModel as SleevesModel;
            if (sleevesModel.DrawBeforeShirt)
            {
                MoveLayerDataItem(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Shirt), layerData, ref sortedLayerData);
            }
            else if (sleevesModel.DrawBeforeBase)
            {
                MoveLayerDataItem(sortedLayerData.FindIndex(d => d.AppearanceType is AppearanceContentPack.Type.Player), layerData, ref sortedLayerData);
            }
        }

        private void SortHat(LayerData layerData, ref List<LayerData> sortedLayerData)
        {
            // Hat has no conditional checks
        }
        #endregion
    }
}
