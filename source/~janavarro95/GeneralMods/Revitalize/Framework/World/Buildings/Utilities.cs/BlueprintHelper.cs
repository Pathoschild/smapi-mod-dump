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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;

namespace Omegasis.Revitalize.Framework.World.Buildings.Utilities.cs
{
    /// <summary>
    /// Class for helping to generate Stardew Valley Building blueprints.
    /// </summary>
    public class BlueprintHelper
    {
        /// <summary>
        /// This will be used to load display name, desctiption, and will act as the building type.
        /// </summary>
        public string revitalizeBuildingId = "";

        /// <summary>
        /// The materials used to build the building. Note that in SDV 1.6 all item references will be using strings so this will probably need to be migrated at some point...
        /// </summary>
        public List<ItemReference> buildingMaterials = new List<ItemReference>();

        /// <summary>
        /// The bounding box tile dimensions of the building.
        /// </summary>
        public Point tileDimensions = new Point(1, 1);
        /// <summary>
        /// The door humans can interact with to go inside.
        /// </summary>
        public Point humanDoorPosition = new Point(-1, -1);

        /// <summary>
        /// The door animals can walk out of. Probably will be null?
        /// </summary>
        public Point animalDoorPosition = new Point(-1, -1);

        /// <summary>
        /// The map to warp to.
        /// </summary>
        public string mapToWarpTo = "";

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                return JsonContentPackUtilities.LoadBuildingDisplayName(this.revitalizeBuildingId);
            }
        }

        [JsonIgnore]
        public string Description
        {
            get
            {
                return JsonContentPackUtilities.LoadBuildingDescription(this.revitalizeBuildingId);
            }
        }

        [JsonIgnore]
        public string BlueprintType
        {
            get
            {
                return this.revitalizeBuildingId;
            }
        }

        /// <summary>
        /// The id for the buidlign to upgrade into. Default as none since we can't accept empty strings without causing errors. Would otehrwise be the other BuildingType.
        /// </summary>
        public string upgradeForWhichBuildingType = "none";

        public Point buildingTextureDimensions;

        [JsonIgnore]
        public Rectangle SourceRectForMenuView
        {
            get
            {
                return new Rectangle(0, 0, this.buildingTextureDimensions.X, this.buildingTextureDimensions.Y);
            }
        }

        /// <summary>
        /// Used to determine the number of animals that can be in this building at once?
        /// </summary>
        public int maxOccupants = 20;

        /// <summary>
        /// The action to do when interacting with this building. This will pretty much always be null since we can handle this with c# logic.
        /// </summary>
        public string actionBehavior = "null";

        /// <summary>
        /// The names of all of the places this building can be built. In SDV 1.5 only the Farm is allowed, but in SDV 1.6 all game locations allow for this. So maybe alos eventually add other locations as well.
        /// </summary>
        public List<string> namesOfOkBuildingLocationsList = new List<string>()
        {
            "Farm"
        };

        /// <summary>
        /// Money required to build this building.
        /// </summary>
        public int moneyRequired = 0;

        /// <summary>
        /// If true this building would be constructed instantly.
        /// </summary>
        public bool isMagical = false;

        /// <summary>
        /// The number of days to build this building. Defaults to 2.
        /// </summary>
        public int daysToConstruct = 2;

        public BlueprintHelper()
        {

        }

        public virtual string toBlueprintString()
        {
            //Convert to blueprint form here.

            List<string> strings = new List<string>();

            StringBuilder materialsListBuilder = new StringBuilder();

            if (this.buildingMaterials.Count == 0)
            {
                throw new Exception("No building materials present in the blueprint helper for building : " + this.revitalizeBuildingId);
            }

            for( int i=0; i<this.buildingMaterials.Count;i++)
            {
                ItemReference itemReference = this.buildingMaterials[i];
                //Add support for more items here later when SDV 1.6 releases.
                if(itemReference.StardewValleyItemId!= Constants.Enums.SDVObject.NULL)
                {
                    materialsListBuilder.Append(string.Format("{0} {1}", (int)itemReference.StardewValleyItemId, itemReference.StackSize));
                }
                if (i < this.buildingMaterials.Count - 1)
                {
                    materialsListBuilder.Append(" ");
                }
            }
            strings.Add(materialsListBuilder.ToString());

            strings.Add(this.tileDimensions.X.ToString());
            strings.Add(this.tileDimensions.Y.ToString());

            strings.Add(this.humanDoorPosition.X.ToString());
            strings.Add(this.humanDoorPosition.Y.ToString());

            strings.Add(this.animalDoorPosition.X.ToString());
            strings.Add(this.animalDoorPosition.Y.ToString());

            strings.Add(this.mapToWarpTo);

            strings.Add(this.DisplayName);
            strings.Add(this.Description);
            strings.Add(this.BlueprintType);
            strings.Add(this.upgradeForWhichBuildingType);

            strings.Add(this.SourceRectForMenuView.X.ToString());
            strings.Add(this.SourceRectForMenuView.Y.ToString());

            strings.Add(this.maxOccupants.ToString());
            strings.Add(this.actionBehavior);

            StringBuilder acceptableGameLocationsForBuilding = new StringBuilder();
            for (int i = 0; i < this.namesOfOkBuildingLocationsList.Count; i++)
            {
                acceptableGameLocationsForBuilding.Append(this.namesOfOkBuildingLocationsList[i]);
                if (i < this.namesOfOkBuildingLocationsList.Count - 1)
                {
                    acceptableGameLocationsForBuilding.Append(" ");
                }
            }

            strings.Add(acceptableGameLocationsForBuilding.ToString());

            strings.Add(this.moneyRequired.ToString());
            strings.Add(this.isMagical.ToString());
            strings.Add(this.daysToConstruct.ToString());


            StringBuilder result = new StringBuilder();
            for(int i=0; i < strings.Count; i++)
            {
                result.Append(strings[i]);
                if (i < strings.Count - 1)
                {
                    result.Append("/");
                }

            }
            return result.ToString();
        }
    }
}
