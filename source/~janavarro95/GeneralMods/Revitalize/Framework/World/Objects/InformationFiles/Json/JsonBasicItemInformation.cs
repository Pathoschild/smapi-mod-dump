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
using Omegasis.Revitalize.Framework.Content.JsonContent.Animations;

namespace Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json
{
    /// <summary>
    /// Used to store basic information regarding object files.
    /// </summary>
    public class JsonBasicItemInformation
    {
        public string id;
        public string name;
        public string description;
        public string categoryId;

        public int healthRestoredOnEating;
        public int staminaRestoredOnEating;

        public int fraglility;
        public int sellingPrice;

        public bool canBeSetIndoors;
        public bool canBeSetOutdoors;

        public bool ignoreBoundingBox;
        public Vector2 boundingBoxTileDimensions;

        public Vector2 drawTileOffset;

        public Color drawColor;

        public JsonAnimationManager animationManager;

        public JsonBasicItemInformation()
        {
            this.id = "";
            this.name = "";
            this.description = "";
            this.categoryId = "";

            //-300 is inedible.
            this.healthRestoredOnEating = -300;
            this.staminaRestoredOnEating = -300;

            this.fraglility = 0;
            this.sellingPrice = 0;

            this.canBeSetIndoors = true;
            this.canBeSetOutdoors = true;

            this.ignoreBoundingBox = false;
            this.boundingBoxTileDimensions = new Vector2(0, 0);

            this.drawTileOffset = new Vector2(0, 0);

            this.drawColor = Color.White;

            this.animationManager = new JsonAnimationManager();
        }


        public virtual BasicItemInformation toBasicItemInformation()
        {
            return new BasicItemInformation(this);
        }
    }
}
