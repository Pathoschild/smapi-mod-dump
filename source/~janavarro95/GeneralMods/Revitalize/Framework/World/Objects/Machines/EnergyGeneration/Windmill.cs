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
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using System.Xml.Serialization;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Machines.EnergyGeneration.Windmill")]
    public class Windmill : Machine
    {

        public readonly NetInt maxDaysToProduceBattery = new NetInt();
        public readonly NetInt daysRemainingToProduceBattery = new NetInt();

        public Windmill() { }

        public Windmill(BasicItemInformation info, Vector2 TileLocation) : base(info, TileLocation)
        {
            this.maxDaysToProduceBattery.Value = 12;
            this.daysRemainingToProduceBattery.Value = this.maxDaysToProduceBattery;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.maxDaysToProduceBattery, this.daysRemainingToProduceBattery);
        }


        public override Item getOne()
        {
            Windmill component = new Windmill(this.getItemInformation().Copy(), this.TileLocation);
            //component.containerObject = this.containerObject;
            //component.offsetKey = this.offsetKey;
            return component;
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
            if (!this.getCurrentLocation().IsOutdoors) return;
            if (this.heldObject.Value != null) return;
            if (Game1.weatherIcon == Game1.weather_rain)
                this.daysRemainingToProduceBattery.Value -= 2;
            else if (Game1.weatherIcon == Game1.weather_lightning)
                this.daysRemainingToProduceBattery.Value -= 3;
            else if (Game1.weatherIcon == Game1.weather_debris)
                this.daysRemainingToProduceBattery.Value -= 4;
            else
                this.daysRemainingToProduceBattery.Value -= 1;
            if (this.daysRemainingToProduceBattery <= 0)
            {
                this.daysRemainingToProduceBattery.Value = this.maxDaysToProduceBattery;
                this.heldObject.Value = ObjectUtilities.getStardewObjectFromEnum(Enums.SDVObject.BatteryPack, 1);
            }
        }
    }
}
