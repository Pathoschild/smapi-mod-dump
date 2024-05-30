/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EverlastingBaitsAndUnbreakableTacklesMod.utility;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class BaitTackle : Enumeration
    {

        public static readonly BaitTackle EverlastingBait = new ("685", "Everlasting Bait");
        public static readonly BaitTackle EverlastingWildBait = new ("774", "Everlasting Wild Bait");
        public static readonly BaitTackle EverlastingMagnet = new ("703", "Everlasting Magnet");
        public static readonly BaitTackle UnbreakableSpinner = new ("686", "Unbreakable Spinner");
        public static readonly BaitTackle UnbreakableLeadBobber = new ("692", "Unbreakable Lead Bobber");
        public static readonly BaitTackle UnbreakableTrapBobber = new ("694", "Unbreakable Trap Bobber");
        public static readonly BaitTackle UnbreakableSonarBobber = new ("SonarBobber", "Unbreakable Sonar Bobber");
        public static readonly BaitTackle UnbreakableCorkBobber = new ("695", "Unbreakable Cork Bobber");
        public static readonly BaitTackle UnbreakableTreasureHunter = new ("693", "Unbreakable Treasure Hunter");
        public static readonly BaitTackle UnbreakableBarbedHook = new ("691", "Unbreakable Barbed Hook");
        public static readonly BaitTackle UnbreakableDressedSpinner = new ("687", "Unbreakable Dressed Spinner");

        public BaitTackle(string id, string description) : base(id, description)
        {
        }

        public static BaitTackle GetFromDescription(string value)
        {
            foreach (BaitTackle baitTackle in Enumeration.GetAll<BaitTackle>())
            {
                if (baitTackle.Description.Equals(value))
                {
                    return baitTackle;
                }
            }
            return null;
        }

        public static BaitTackle GetFromId(string value)
        {
            foreach (BaitTackle baitTackle in Enumeration.GetAll<BaitTackle>())
            {
                if (baitTackle.Id.Equals(value))
                {
                    return baitTackle;
                }
            }
            return null;
        }

        public string GetQuestName()
        {
            return "Quest" + Id;
        }
    }
}
