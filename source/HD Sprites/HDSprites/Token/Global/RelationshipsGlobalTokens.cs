/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class HeartsGlobalToken : GlobalToken
    {
        public HeartsGlobalToken() : base("Hearts") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            this.GlobalValue = "";
            foreach(var entry in player.friendshipData.Pairs)
            {
                int value = entry.Value.Points / NPC.friendshipPointsPerHeartLevel;
                List<string> values = new List<string>();
                for (int i = 0; i <= value; ++i)
                    values.Add(i.ToString());
                this.GlobalValues.Add(new ValueExt(entry.Key, values));
            }
            return true;
        }
    }

    public class RelationshipGlobalToken : GlobalToken
    {
        public RelationshipGlobalToken() : base("Relationship") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            this.GlobalValue = "";
            foreach (var entry in player.friendshipData.Pairs)
                this.GlobalValues.Add(new ValueExt(entry.Key, new List<string> { entry.Value.Status.ToString() }));
            return true;
        }
    }

    public class SpouseGlobalToken : GlobalToken
    {
        public SpouseGlobalToken() : base("Spouse") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player?.spouse);
        }
    }
}
