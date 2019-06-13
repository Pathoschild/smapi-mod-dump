using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class HasFlagGlobalToken : GlobalToken
    {
        public HasFlagGlobalToken() : base("HasFlag") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            foreach (var entry in player.mailReceived)
            {
                this.GlobalValues.Add(new ValueExt(entry, new List<string>()));
            }

            foreach (var entry in player.mailForTomorrow)
            {
                this.GlobalValues.Add(new ValueExt(entry, new List<string>()));
            }

            foreach (var entry in player.mailbox)
            {
                this.GlobalValues.Add(new ValueExt(entry, new List<string>()));
            }
        }
    }

    public class HasProfessionGlobalToken : GlobalToken
    {
        public HasProfessionGlobalToken() : base("HasProfession") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            foreach (var id in player.professions)
            {
                string name = "";
                switch (id)
                {
                    case Farmer.acrobat: name = "Acrobat"; break;
                    case Farmer.brute: name = "Brute"; break;
                    case Farmer.defender: name = "Defender "; break;
                    case Farmer.desperado: name = "Desperado"; break;
                    case Farmer.fighter: name = "Fighter"; break;
                    case Farmer.scout: name = "Scout"; break;
                    case Farmer.agriculturist: name = "Agriculturist"; break;
                    case Farmer.artisan: name = "Artisan"; break;
                    case Farmer.butcher: name = "Butcher"; break;
                    case Farmer.rancher: name = "Rancher"; break;
                    case Farmer.shepherd: name = "Shepherd"; break;
                    case Farmer.tiller: name = "Tiller"; break;
                    case Farmer.angler: name = "Angler"; break;
                    case Farmer.fisher: name = "Fisher"; break;
                    case Farmer.baitmaster: name = "Mariner"; break;
                    case Farmer.pirate: name = "Pirate"; break;
                    case Farmer.mariner: name = "Luremaster "; break;
                    case Farmer.trapper: name = "Trapper"; break;
                    case Farmer.botanist: name = "Botanist"; break;
                    case Farmer.forester: name = "Forester"; break;
                    case Farmer.gatherer: name = "Gatherer"; break;
                    case Farmer.lumberjack: name = "Lumberjack"; break;
                    case Farmer.tapper: name = "Tapper"; break;
                    case Farmer.tracker: name = "Tracker"; break;
                    case Farmer.blacksmith: name = "Blacksmith"; break;
                    case Farmer.excavator: name = "Excavator"; break;
                    case Farmer.gemologist: name = "Gemologist"; break;
                    case Farmer.geologist: name = "Geologist"; break;
                    case Farmer.miner: name = "Miner"; break;
                    case Farmer.burrower: name = "Prospector "; break;
                }
                this.GlobalValues.Add(new ValueExt(name, new List<string>()));
            }
        }
    }

    public class HasReadLetterGlobalToken : GlobalToken
    {
        public HasReadLetterGlobalToken() : base("HasReadLetter") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            foreach (var entry in player.mailReceived)
            {
                this.GlobalValues.Add(new ValueExt(entry, new List<string>()));
            }
        }
    }

    public class HasSeenEventGlobalToken : GlobalToken
    {
        public HasSeenEventGlobalToken() : base("HasSeenEvent") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            foreach (var entry in player.eventsSeen)
            {
                this.GlobalValues.Add(new ValueExt(entry.ToString(), new List<string>()));
            }
        }
    }

    public class HasWalletItemGlobalToken : GlobalToken
    {
        public HasWalletItemGlobalToken() : base("HasWalletItem") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            if (player.canUnderstandDwarves) this.GlobalValues.Add(new ValueExt("DwarvishTranslationGuide", new List<string>()));
            if (player.hasRustyKey) this.GlobalValues.Add(new ValueExt("RustyKey", new List<string>()));
            if (player.hasClubCard) this.GlobalValues.Add(new ValueExt("ClubCard", new List<string>()));
            if (player.hasSpecialCharm) this.GlobalValues.Add(new ValueExt("SpecialCharm", new List<string>()));
            if (player.hasSkullKey) this.GlobalValues.Add(new ValueExt("SkullKey", new List<string>()));
            if (player.hasMagnifyingGlass) this.GlobalValues.Add(new ValueExt("MagnifyingGlass", new List<string>()));
            if (player.hasDarkTalisman) this.GlobalValues.Add(new ValueExt("DarkTalisman", new List<string>()));
            if (player.hasMagicInk) this.GlobalValues.Add(new ValueExt("MagicInk", new List<string>()));
            if (player.eventsSeen.Contains(2120303)) this.GlobalValues.Add(new ValueExt("BearsKnowledge", new List<string>()));
            if (player.eventsSeen.Contains(3910979)) this.GlobalValues.Add(new ValueExt("SpringOnionMastery", new List<string>()));
        }
    }

    public class IsMainPlayerGlobalToken : GlobalToken
    {
        public IsMainPlayerGlobalToken() : base("IsMainPlayer") { }

        public override void Update()
        {
            this.GlobalValue = Context.IsMainPlayer.ToString();
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class IsOutdoorsGlobalToken : GlobalToken
    {
        public IsOutdoorsGlobalToken() : base("IsOutdoors") { }

        public override void Update()
        {
            this.GlobalValue = Game1.currentLocation?.IsOutdoors.ToString() ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class LocationNameGlobalToken : GlobalToken
    {
        public LocationNameGlobalToken() : base("LocationName") { }

        public override void Update()
        {
            this.GlobalValue = Game1.currentLocation?.Name ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class PlayerGenderGlobalToken : GlobalToken
    {
        public PlayerGenderGlobalToken() : base("PlayerGender") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player.IsMale ? "Male" : "Female" ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class PlayerNameGlobalToken : GlobalToken
    {
        public PlayerNameGlobalToken() : base("PlayerName") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player.Name ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class PreferredPetGlobalToken : GlobalToken
    {
        public PreferredPetGlobalToken() : base("PreferredPet") { }

        public override void Update()
        {
            this.GlobalValue = Game1.player.catPerson ? "Cat" : "Dog" ?? "";
            this.GlobalValues = new List<ValueExt>() { new ValueExt(this.GlobalValue, new List<string>()) };
        }
    }

    public class SkillLevelGlobalToken : GlobalToken
    {
        public SkillLevelGlobalToken() : base("SkillLevel") { }

        public override void Update()
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();

            Farmer player = Game1.player;
            if (player == null) return;

            List<string> combatLevels = new List<string>();
            for (int i = 0; i <= player.CombatLevel; ++i) combatLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Combat", combatLevels));

            List<string> farmingLevels = new List<string>();
            for (int i = 0; i <= player.FarmingLevel; ++i) farmingLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Farming", farmingLevels));

            List<string> fishingLevels = new List<string>();
            for (int i = 0; i <= player.FishingLevel; ++i) fishingLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Fishing", fishingLevels));

            List<string> foragingLevels = new List<string>();
            for (int i = 0; i <= player.ForagingLevel; ++i) foragingLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Foraging", foragingLevels));

            List<string> luckLevels = new List<string>();
            for (int i = 0; i <= player.LuckLevel; ++i) luckLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Luck", luckLevels));

            List<string> miningLevels = new List<string>();
            for (int i = 0; i <= player.MiningLevel; ++i) miningLevels.Add(i.ToString());
            this.GlobalValues.Add(new ValueExt("Mining", miningLevels));
        }
    }
}
