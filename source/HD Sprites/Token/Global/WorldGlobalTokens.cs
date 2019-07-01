using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class FarmCaveGlobalToken : GlobalToken
    {
        public FarmCaveGlobalToken() : base("FarmCave") { }

        public override void Update()
        {
            int value = Game1.player?.caveChoice.Value ?? Farmer.caveNothing;
            this.GlobalValue = value == Farmer.caveMushrooms ? "Mushrooms" : value == Farmer.caveBats ? "Bats" : "None";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };            
        }
    }

    public class FarmhouseUpgradeGlobalToken : GlobalToken
    {
        public FarmhouseUpgradeGlobalToken() : base("FarmhouseUpgrade") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player?.HouseUpgradeLevel.ToString() ?? "0";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class FarmNameGlobalToken : GlobalToken
    {
        public FarmNameGlobalToken() : base("FarmName") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player?.farmName ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class FarmTypeGlobalToken : GlobalToken
    {
        public FarmTypeGlobalToken() : base("FarmType") { }

        public override void Update()
        {
            string type = "Standard";
            switch (Game1.whichFarm)
            {
                case 1: type = "Riverland"; break;
                case 2: type = "Forest"; break;
                case 3: type = "Hilltop"; break;
                case 4: type = "Wilderness"; break;
                case 100: type = "Custom"; break;
            }
            this.GlobalValue = type;
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class IsCommunityCenterCompleteGlobalToken : GlobalToken
    {
        public IsCommunityCenterCompleteGlobalToken() : base("IsCommunityCenterComplete") { }

        public override void Update()
        {
            this.GlobalValue = ((Game1.MasterPlayer?.mailReceived.Contains("ccIsComplete") ?? false) || (Game1.MasterPlayer?.hasCompletedCommunityCenter() ?? false)).ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class LanguageGlobalToken : GlobalToken
    {
        public LanguageGlobalToken() : base("Language") { }

        public override void Update()
        {
            this.GlobalValue = "en";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }
}
