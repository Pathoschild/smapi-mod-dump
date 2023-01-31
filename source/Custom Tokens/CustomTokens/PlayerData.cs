/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System.Collections;

namespace CustomTokens
{
    public class PlayerData
    {
        public int DeathCountMarried { get; set; }
        public int PassOutCount { get; set; }
        public int CurrentMineLevel { get; set; }
        public int DeepestMineLevel { get; set; }
        public int CurrentVolcanoFloor { get; set; }
        public int DeepestVolcanoFloor { get; set; }
        public double CurrentYearsMarried { get; set; }
        public int AnniversaryDay { get; set; }
        public string AnniversarySeason { get; set; } = "No season";

        public ArrayList SpecialOrdersCompleted = new ArrayList();

        public ArrayList QuestsCompleted = new ArrayList();
    }
}
