using System.Collections.Generic;
using System.Linq;
using AdvancedKeyBindings.Extensions;
using AdvancedKeyBindings.Extensions.MenuExtensions;
using AdvancedKeyBindings.StaticHelpers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace AdvancedKeyBindings.KeyHandlers
{
    public class PanScreenHandler : IKeyHandler
    {
        private SButton[] PanScreenScrollLeft { get; }
        private SButton[] PanScreenScrollRight { get; }
        private SButton[] PanScreenScrollUp { get; }
        private SButton[] PanScreenScrollDown { get; }
        private SButton[] PanScreenPreviousBuilding { get; }
        private SButton[] PanScreenNextBuilding { get; }
        private Building _currentlySelectedBuilding;
        private List<Building> _currentBuildingList;

        public PanScreenHandler(SButton[] panScreenScrollLeft, SButton[] panScreenScrollRight,
            SButton[] panScreenScrollUp, SButton[] panScreenScrollDown, SButton[] panScreenPreviousBuilding,
            SButton[] panScreenNextBuilding)
        {
            PanScreenScrollLeft = panScreenScrollLeft;
            PanScreenScrollRight = panScreenScrollRight;
            PanScreenScrollUp = panScreenScrollUp;
            PanScreenScrollDown = panScreenScrollDown;
            PanScreenPreviousBuilding = panScreenPreviousBuilding;
            PanScreenNextBuilding = panScreenNextBuilding;
        }

        public bool ReceiveButtonPress(SButton input)
        {
            if (CanPan())
            {
                if (HandlePanning(input))
                {
                    return true;
                }
            }

            if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu)
            {
                if (carpenterMenu.IsMovingPlacementMode())
                {
                    _currentBuildingList = Game1.getFarm().GetMovableBuildings();
                    
                    if (HandleBuildingSelection(input))
                    {
                        return true;
                    }
                }
                if (carpenterMenu.IsUpgradingPlacementMode())
                {
                    _currentBuildingList = carpenterMenu.CurrentBlueprint.GetUpgradeableBuildings();

                    if (HandleBuildingSelection(input))
                    {
                        return true;
                    }
                }

                if (carpenterMenu.IsDemolishingPlacementMode())
                {
                    _currentBuildingList = Game1.getFarm().GetDemolishableBuildings();

                    if (HandleBuildingSelection(input))
                    {
                        return true;
                    }
                }
            }

            if (Game1.activeClickableMenu is PurchaseAnimalsMenu menu && menu.IsAnimalPlacementMode())
            {
                _currentBuildingList = menu.GetAnimalBeingPurchased().GetPossibleHomeBuildings();

                if (HandleBuildingSelection(input))
                {
                    return true;
                }
            }

            if (Game1.activeClickableMenu is AnimalQueryMenu animalQueryMenu && animalQueryMenu.IsAnimalPlacementMode())
            {
                _currentBuildingList = animalQueryMenu.GetAnimal().GetPossibleHomeBuildings();

                if (HandleBuildingSelection(input))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HandleBuildingSelection(SButton input)
        {
            if (PanScreenPreviousBuilding.Contains(input))
            {
                PreviousBuilding();

                _currentlySelectedBuilding?.PanToBuilding(true, true);
                return true;
            }

            if (PanScreenNextBuilding.Contains(input))
            {
                NextBuilding();
                _currentlySelectedBuilding?.PanToBuilding(true, true);

                return true;
            }

            return false;
        }

        private bool CanPan()
        {
            if (Game1.activeClickableMenu is PurchaseAnimalsMenu animalsMenu)
            {
                if (animalsMenu.IsAnimalPlacementMode())
                {
                    return true;
                }
            }

            if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu)
            {
                if (carpenterMenu.InPlacementMode())
                {
                    return true;
                }
            }

            if (Game1.activeClickableMenu is AnimalQueryMenu animalQueryMenu)
            {
                if (animalQueryMenu.IsAnimalPlacementMode())
                {
                    return true;
                }
            }

            return false;
        }

        private bool HandlePanning(SButton input)
        {
            var panSize = Game1.pixelZoom * 16 * 10;

            if (PanScreenScrollLeft.Contains(input))
            {
                SmoothPanningHelper.GetInstance().RelativePanTo(-panSize, 0);
                return true;
            }

            if (PanScreenScrollRight.Contains(input))
            {
                SmoothPanningHelper.GetInstance().RelativePanTo(panSize, 0);
                return true;
            }

            if (PanScreenScrollUp.Contains(input))
            {
                SmoothPanningHelper.GetInstance().RelativePanTo(0, -panSize);
                return true;
            }

            if (PanScreenScrollDown.Contains(input))
            {
                SmoothPanningHelper.GetInstance().RelativePanTo(0, panSize);
                return true;
            }

            return false;
        }

        private void NextBuilding()
        {
            if (_currentBuildingList.Count == 0)
            {
                _currentlySelectedBuilding = null;
                return;
            }

            if (_currentlySelectedBuilding == null)
            {
                _currentlySelectedBuilding = _currentBuildingList.First();
            }
            else
            {
                var buildingIndex = _currentBuildingList.IndexOf(_currentlySelectedBuilding);
                buildingIndex++;

                if (buildingIndex >= _currentBuildingList.Count)
                {
                    buildingIndex = 0;
                }

                _currentlySelectedBuilding = _currentBuildingList[buildingIndex];
            }
        }

        private void PreviousBuilding()
        {
            if (_currentBuildingList.Count == 0)
            {
                _currentlySelectedBuilding = null;
                return;
            }

            if (_currentlySelectedBuilding == null)
            {
                _currentlySelectedBuilding = _currentBuildingList.Last();
            }
            else
            {
                var buildingIndex = _currentBuildingList.IndexOf(_currentlySelectedBuilding);

                buildingIndex--;

                if (buildingIndex < 0)
                {
                    buildingIndex = _currentBuildingList.Count - 1;
                }

                _currentlySelectedBuilding = _currentBuildingList[buildingIndex];
            }
        }
    }
}