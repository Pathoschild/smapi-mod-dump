/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.StardustCore.Animations;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Structures
{
    public class BuildingBlueprintExtended:BluePrint
    {
        public AnimationManager animationManager;

        public BuildingBlueprintExtended():base("Info Tool")
        {

        }

        public BuildingBlueprintExtended(string DisplayName, string Description, int MoneyRequired, Dictionary<Enums.SDVObject, int> ItemsRequired, int DaysToConstruct, AnimationManager AnimationManager ,Rectangle sourceRectangleForMenuView, Vector2 TilesDimensions, Point HumanDoorLocation, string MapToWarpTo, List<string> NamesOfOkBuildingLocations, bool IsMagical, string NameOfBuildingToUpgrade="") : this(DisplayName,Description,AnimationManager,sourceRectangleForMenuView,MoneyRequired,"",TilesDimensions,HumanDoorLocation,new Point(-1,-1),MapToWarpTo,NameOfBuildingToUpgrade,0,NamesOfOkBuildingLocations,IsMagical,DaysToConstruct,ItemsRequired)
        {
        }

        public BuildingBlueprintExtended(string DisplayName, string Description, AnimationManager AnimationManager  ,Rectangle sourceRectangleForMenuView, int MoneyRequired, string BlueprintType, Vector2 TilesDimensions, Point HumanDoorLocation, Point AnimalDoorLocation, string MapToWarpTo, string NameOfBuildingToUpgrade, int MaxOccupants, List<string> NamesOfOkBuildingLocations, bool IsMagical, int DaysToConstruct,Dictionary<Enums.SDVObject, int> ItemsRequired ) : base("Info Tool")
        {
            this.name = DisplayName;
            this.displayName = DisplayName;
            this.description = Description;
            this.sourceRectForMenuView = sourceRectangleForMenuView;
            this.moneyRequired = MoneyRequired;
            this.blueprintType = BlueprintType;
            this.tilesWidth = (int)TilesDimensions.X;
            this.tilesHeight = (int)TilesDimensions.Y;
            this.humanDoor = HumanDoorLocation;
            this.animalDoor = AnimalDoorLocation;

            this.mapToWarpTo = MapToWarpTo;
            this.nameOfBuildingToUpgrade = NameOfBuildingToUpgrade;
            this.maxOccupants = MaxOccupants;
            this.namesOfOkayBuildingLocations = NamesOfOkBuildingLocations;
            this.magical = IsMagical;
            this.daysToConstruct = DaysToConstruct;
            foreach(var v in ItemsRequired)
            {
                this.itemsRequired.Add((int)v.Key, v.Value);
            }
            this.animationManager = AnimationManager;
        }

        /// <summary>
        /// TODO: Fill this out.
        /// </summary>
        /// <returns></returns>
        public virtual KeyValuePair<string,string> toBlueprintData() {
            return new KeyValuePair<string, string>();
        }
    }
}
