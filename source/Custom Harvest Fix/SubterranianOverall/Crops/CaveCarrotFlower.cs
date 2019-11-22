using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SubterranianOverhaul.Crops
{
    public class CaveCarrotFlower : Crop
    {
        /*public readonly NetIntList phaseDays = new NetIntList();
        [XmlElement("rowInSpriteSheet")]
        public readonly NetInt rowInSpriteSheet = new NetInt();
        [XmlElement("phaseToShow")]
        public readonly NetInt phaseToShow = new NetInt(-1);
        [XmlElement("currentPhase")]
        public readonly NetInt currentPhase = new NetInt();
        [XmlElement("harvestMethod")]
        public readonly NetInt harvestMethod = new NetInt();
        [XmlElement("indexOfHarvest")]
        public readonly NetInt indexOfHarvest = new NetInt();
        [XmlElement("regrowAfterHarvest")]
        public readonly NetInt regrowAfterHarvest = new NetInt();
        [XmlElement("dayOfCurrentPhase")]
        public readonly NetInt dayOfCurrentPhase = new NetInt();
        [XmlElement("minHarvest")]
        public readonly NetInt minHarvest = new NetInt();
        [XmlElement("maxHarvest")]
        public readonly NetInt maxHarvest = new NetInt();
        [XmlElement("maxHarvestIncreasePerFarmingLevel")]
        public readonly NetInt maxHarvestIncreasePerFarmingLevel = new NetInt();
        [XmlElement("daysOfUnclutteredGrowth")]
        public readonly NetInt daysOfUnclutteredGrowth = new NetInt();
        [XmlElement("whichForageCrop")]
        public readonly NetInt whichForageCrop = new NetInt();
        public readonly NetStringList seasonsToGrowIn = new NetStringList();
        [XmlElement("tintColor")]
        public readonly NetColor tintColor = new NetColor();
        [XmlElement("flip")]
        public readonly NetBool flip = new NetBool();
        [XmlElement("fullGrown")]
        public readonly NetBool fullyGrown = new NetBool();
        [XmlElement("raisedSeeds")]
        public readonly NetBool raisedSeeds = new NetBool();
        [XmlElement("programColored")]
        public readonly NetBool programColored = new NetBool();
        [XmlElement("dead")]
        public readonly NetBool dead = new NetBool();
        [XmlElement("forageCrop")]
        public readonly NetBool forageCrop = new NetBool();
        [XmlElement("chanceForExtraCrops")]
        public readonly NetDouble chanceForExtraCrops = new NetDouble(0.0);
        [XmlIgnore]
        public readonly NetInt netSeedIndex = new NetInt(-1);
        
        public NetFields NetFields { get; } = new NetFields();
        */

        private static int itemIndex = -1;

        public static void setIndex()
        {
            if (CaveCarrotFlower.itemIndex == -1)
            {
                CaveCarrotFlower.itemIndex = IndexManager.getUnusedObjectIndex();
            }
        }

        public static int getIndex()
        {
            if (CaveCarrotFlower.itemIndex == -1)
            {
                VoidshroomSpore.setIndex();
            }

            return CaveCarrotFlower.itemIndex;
        }

        public CaveCarrotFlower()
        {
            this.NetFields.AddFields((INetSerializable)this.phaseDays, (INetSerializable)this.rowInSpriteSheet, (INetSerializable)this.phaseToShow, (INetSerializable)this.currentPhase, (INetSerializable)this.harvestMethod, (INetSerializable)this.indexOfHarvest, (INetSerializable)this.regrowAfterHarvest, (INetSerializable)this.dayOfCurrentPhase, (INetSerializable)this.minHarvest, (INetSerializable)this.maxHarvest, (INetSerializable)this.maxHarvestIncreasePerFarmingLevel, (INetSerializable)this.daysOfUnclutteredGrowth, (INetSerializable)this.whichForageCrop, (INetSerializable)this.seasonsToGrowIn, (INetSerializable)this.tintColor, (INetSerializable)this.flip, (INetSerializable)this.fullyGrown, (INetSerializable)this.raisedSeeds, (INetSerializable)this.programColored, (INetSerializable)this.dead, (INetSerializable)this.forageCrop, (INetSerializable)this.chanceForExtraCrops, (INetSerializable)this.netSeedIndex);
        }

        public CaveCarrotFlower(bool forageCrop, int which, int tileX, int tileY)
          : this()
        {
            this.forageCrop.Value = forageCrop;
            this.whichForageCrop.Value = which;
            this.fullyGrown.Value = true;
            this.currentPhase.Value = 5;
        }


    }
}
