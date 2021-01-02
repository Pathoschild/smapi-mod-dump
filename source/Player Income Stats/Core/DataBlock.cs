/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace PlayerIncomeStats.Core
{
    public class DataBlock : IEquatable<DataBlock>
    {
        public long UniqueID { get; set; }
        public string Name { get; set; }
        public int MoneyMade { get; set; } = 0;
        public int MoneyMadeTemp { get; set; } = 0;
        public string EntryString { get; set; } = string.Empty;

        public Color justChangedColor = Color.Green;
        public int justChangedTimer = 0;
        public string nameS = string.Empty;
        public string overallIncomeS = string.Empty;
        public string tempIncomeS = string.Empty;
        public float nameSWidth;
        public float overallIncomeSWidth;
        public float tempIncomeSWidth;

        public DataBlock(long id, string name)
        {
            UniqueID = id;
            Name = name;
        }

        public void CalculateEntryString()
        {
            nameS = Name.Substring(0, Name.Length > 10 ? 10 : Name.Length);
            overallIncomeS = MoneyMade.ToString();
            tempIncomeS = "(+" + MoneyMadeTemp.ToString() + ")";
            nameSWidth = Game1.smallFont.MeasureString(nameS).X;
            overallIncomeSWidth = Game1.smallFont.MeasureString(overallIncomeS).X;
            tempIncomeSWidth = Game1.smallFont.MeasureString(tempIncomeS).X;
        }

        public bool Equals(DataBlock other) => UniqueID == other.UniqueID;
    }
}
