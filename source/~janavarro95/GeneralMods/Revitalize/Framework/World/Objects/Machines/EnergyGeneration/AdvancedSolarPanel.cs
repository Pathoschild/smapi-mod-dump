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
using StardewValley;
using System.Xml.Serialization;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.WorldUtilities;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration.AdvancedSolarPanel")]
    public class AdvancedSolarPanel : Machine
    {
        public readonly NetInt daysRemainingToProduceBattery = new NetInt();
        public readonly NetEnum<SolarPanelTier> solarPanelTier = new NetEnum<SolarPanelTier>();

        public enum SolarPanelTier
        {
            Advanced,
            Superior
        }

        public AdvancedSolarPanel() { }

        public AdvancedSolarPanel(BasicItemInformation info, SolarPanelTier SolarPanelTier) : this(info,Vector2.Zero,SolarPanelTier)
        {
        }

        public AdvancedSolarPanel(BasicItemInformation info, Vector2 TilePosition, SolarPanelTier SolarPanelTier) : base(info,TilePosition)
        {
            this.solarPanelTier.Value = SolarPanelTier;
            this.daysRemainingToProduceBattery.Value = this.getDaysToProduceABattery();
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.solarPanelTier, this.daysRemainingToProduceBattery);
        }

        public override Item getOne()
        {
            AdvancedSolarPanel component = new AdvancedSolarPanel(this.basicItemInformation.Copy(),this.solarPanelTier.Value);
            return component;
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
            if (!this.getCurrentLocation().IsOutdoors) return;
            if (Game1.IsRainingHere(location) || Game1.IsLightningHere(location) || Game1.IsSnowingHere(location)) return;
            if (this.heldObject.Value != null) return;

            this.daysRemainingToProduceBattery.Value -= 1;
            if (this.daysRemainingToProduceBattery.Value == 0)
            {
                this.daysRemainingToProduceBattery.Value = this.getDaysToProduceABattery();
                this.heldObject.Value = ObjectUtilities.getStardewObjectFromEnum(Enums.SDVObject.BatteryPack, 1);
            }

        }

        public override bool shouldDoDayUpdate()
        {
            return this.dayUpdateCounter.Value>=2;
        }

        public virtual int getDaysToProduceABattery()
        {
            switch (this.solarPanelTier.Value)
            {
                case SolarPanelTier.Advanced:
                    return 5;
                case SolarPanelTier.Superior:
                    return 3;
                default:
                    return 7;
            }
        }

    }
}
