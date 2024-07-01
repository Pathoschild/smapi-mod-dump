/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    public abstract class BuildingMachine
        : GameMachine
    {
        protected BuildingMachine(Building building, Point accessPoint)
            : base(building, accessPoint)
        {
        }

        public static BuildingMachine? TryCreate(Building building, Point accessPoint)
        {
            if (building is FishPond fishPond)
            {
                return new FishPondMachine(fishPond, accessPoint);
            }
            else
            {
                return null;
            }
        }

        protected Building Building => (Building)base.GameObject;

        public override string ToString()
        {
            return IF($"{this.Building.buildingType} at {this.Building.tileX.Value},{this.Building.tileY.Value}");
        }
    }
}
