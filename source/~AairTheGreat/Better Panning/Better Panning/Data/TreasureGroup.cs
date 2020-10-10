/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace BetterPanning.Data
{
    public enum TREASURE_GROUP
    {
        Rares = 0,
        Geodes,
        GeodeMinerals,
        Boots,
        Rings,
        Minerals,
        Artifacts,
        Gems,
        Ores,
        Bars,
        Tackle,
        Totems,
        SpringSeeds,
        SummerSeeds,
        FallSeeds,
        WinterSeeds,
        Other,
        Custom
    }

    public class TreasureGroup : IWeighted
    {
        public TREASURE_GROUP GroupID { get; set; }

        public double GroupChance { get; set; }

        public bool Enabled { get; set; }

        public bool ManualOverride { get; set; }

        public List<TreasureData> treasureList { get; set; }

        public TreasureGroup(TREASURE_GROUP id, double chance, bool enabled, bool manualOverride)
        {
            this.GroupID = id;
            this.GroupChance = chance;
            this.Enabled = enabled;
            this.ManualOverride = manualOverride;
        }

        public double GetWeight()
        {
            return this.GroupChance;
        }

        public bool GetEnabled()
        {
            return this.Enabled;
        }

        public void SetEnableFlagOnAllTreasures(bool enable)
        {
            if (!this.ManualOverride)
            {
                foreach (var treasure in this.treasureList)
                {
                    treasure.Enabled = enable;
                }
                CheckEnableFlag();
            }
        }

        public void SetEnableTreasure(int itemID, bool enable)
        {
            if (!this.ManualOverride)
            {
                foreach (var treasure in this.treasureList)
                {
                    if (treasure.Id == itemID)
                    {
                        treasure.Enabled = enable;
                        break;
                    }
                }
                CheckEnableFlag();
            }
        }

        public void CheckGroupTreasuresStatus()
        {
            if (!this.ManualOverride)
            {
                if (this.GroupID == TREASURE_GROUP.Artifacts)
                {
                    SetArtifactTreasuresEnableFlag();
                }
                if (this.GroupID == TREASURE_GROUP.GeodeMinerals)
                {
                    SetGeodeMineralsTreasuresEnableFlag();
                }
            }
        }

        private void CheckEnableFlag()
        {
            if (!this.ManualOverride)
            {
                this.Enabled = (this.treasureList.Count(loot => loot.Enabled) != 0);
            }
        }


        private void SetArtifactTreasuresEnableFlag()
        {
            if (!this.ManualOverride)
            {
                // Need to find a cleaner way...
                if (this.GroupID == TREASURE_GROUP.Artifacts)
                {
                    foreach (var treasure in this.treasureList)
                    {
                        treasure.Enabled = !Game1.player.archaeologyFound.ContainsKey(treasure.Id);
                    }
                    CheckEnableFlag();
                }
            }
        }

        private void SetGeodeMineralsTreasuresEnableFlag()
        {
            if (!this.ManualOverride)
            {
                // Need to find a cleaner way...
                if (this.GroupID == TREASURE_GROUP.GeodeMinerals)
                {
                    foreach (var treasure in this.treasureList)
                    {
                        treasure.Enabled = Game1.player.mineralsFound.ContainsKey(treasure.Id);
                    }
                    CheckEnableFlag();
                }
            }
        }
    }
}
