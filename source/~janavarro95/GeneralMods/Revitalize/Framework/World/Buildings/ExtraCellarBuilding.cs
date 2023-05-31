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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Buildings;
using Omegasis.Revitalize.Framework.World.GameLocations;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using StardewValley.Buildings;

namespace Omegasis.Revitalize.Framework.World.Buildings
{
    [XmlType("Mods_Omegasis.Revitalize.Buildings.ExtraCellarBuilding")]
    public class ExtraCellarBuilding : CustomBuilding
    {
        private static readonly BluePrint Blueprint = new(BuildingIds.ExtraCellar);

        public ExtraCellarBuilding()
            : base(ExtraCellarBuilding.Blueprint, Vector2.Zero) { }

        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            return new ExtraCellarLocation();
        }



        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            if (this.isInteractingWithBuilding(tileLocation, who))
            {
                who.currentLocation.playSoundAt("doorClose", tileLocation);
                SoundUtilities.PlaySoundAt(Enums.StardewSound.doorClose, this.indoors.Value, Game1.player.getTileLocation());
                bool isStructure = this.indoors.Value != null;
                Game1.warpFarmer(this.indoors.Value.NameOrUniqueName, this.indoors.Value.warps[0].X, this.indoors.Value.warps[0].Y + 1, (int)Enums.Direction.Down, isStructure);
                return true;
            }
            return false;
        }
    }
}
