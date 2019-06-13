using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class HeartsGlobalToken : GlobalToken
    {
        public HeartsGlobalToken() : base("Hearts") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;
            
            foreach(var entry in player.friendshipData.Pairs)
            {
                int value = (int)(entry.Value.Points / NPC.friendshipPointsPerHeartLevel);
                List<string> values = new List<string>();
                for (int i = 0; i <= value; ++i) values.Add(i.ToString());
                this.GlobalValues.Add(new ValueExt(entry.Key, values));
            }
        }
    }

    public class RelationshipGlobalToken : GlobalToken
    {
        public RelationshipGlobalToken() : base("Relationship") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            foreach (var entry in player.friendshipData.Pairs)
            {
                this.GlobalValues.Add(new ValueExt(entry.Key, new List<string>() { entry.Value.Status.ToString() }));
            }
        }
    }

    public class SpouseGlobalToken : GlobalToken
    {
        public SpouseGlobalToken() : base("Spouse") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player?.spouse ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };            
        }
    }
}
