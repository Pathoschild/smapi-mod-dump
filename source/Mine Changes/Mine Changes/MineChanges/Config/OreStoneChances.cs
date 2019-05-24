using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mine_Changes.MineChanges.Config
{
    public class OreStoneChances
    {
        public double CopperChanceUpToLevel40 = 0.029f;
        public double IronChanceFromLevel40To80 = 0.029f;
        public double GoldChanceFromLevel80To120 = 0.029f;
        public double SkullMineBaseOreChance = 0.02f;
        public double SkullMineBaseOreChancePerLevel = 0.0005f;
        public double IridiumBaseChance = 0.0003f;
        public double GoldChanceSkullMine = 0.01f;
        public double GoldChanceSkullMinePerLevelBoost = 0.0005f;
        public double SkullMineMaxIronChance = 0.5f;
        public double IronChanceSkullMinePerLevelBoost = 0.005f;
        public double ChanceModifierPerMiningLevel = 0.0005f;
        public double GemChancePerLevelDenominator = 24000.0f;
        public double ChanceForPurpleStonePerMiningLevel = 0.008f;
        public double ChanceForMysthicStonePerMiningLevel = 0.008f;
    }
}
