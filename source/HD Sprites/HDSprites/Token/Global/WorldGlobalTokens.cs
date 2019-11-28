using StardewValley;

namespace HDSprites.Token.Global
{
    public class FarmCaveGlobalToken : GlobalToken
    {
        public FarmCaveGlobalToken() : base("FarmCave") { }

        protected override bool Update()
        {
            int? value = Game1.player?.caveChoice.Value;
            return this.SetValue(value == Farmer.caveMushrooms ? "Mushrooms" : value == Farmer.caveBats ? "Bats" : "None");
        }
    }

    public class FarmhouseUpgradeGlobalToken : GlobalToken
    {
        public FarmhouseUpgradeGlobalToken() : base("FarmhouseUpgrade") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player?.HouseUpgradeLevel.ToString() ?? "0");
        }
    }

    public class FarmNameGlobalToken : GlobalToken
    {
        public FarmNameGlobalToken() : base("FarmName") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player?.farmName.Value);
        }
    }

    public class FarmTypeGlobalToken : GlobalToken
    {
        public FarmTypeGlobalToken() : base("FarmType") { }

        protected override bool Update()
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
            return this.SetValue(type);
        }
    }

    public class IsCommunityCenterCompleteGlobalToken : GlobalToken
    {
        public IsCommunityCenterCompleteGlobalToken() : base("IsCommunityCenterComplete") { }

        protected override bool Update()
        {
            return this.SetValue(((Game1.MasterPlayer?.mailReceived.Contains("ccIsComplete") ?? false) || (Game1.MasterPlayer?.hasCompletedCommunityCenter() ?? false)).ToString());
        }
    }

    public class LanguageGlobalToken : GlobalToken
    {
        public LanguageGlobalToken() : base("Language") { }

        protected override bool Update()
        {
            return this.SetValue("en");
        }
    }
}
