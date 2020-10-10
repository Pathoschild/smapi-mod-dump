/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace HDSprites.Token.Global
{
    public class HasFlagGlobalToken : GlobalToken
    {
        public HasFlagGlobalToken() : base("HasFlag") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            return this.SetValues(
                player.mailReceived
                .Concat(player.mailForTomorrow)
                .Concat(player.mailbox)
            );
        }
    }

    public class HasProfessionGlobalToken : GlobalToken
    {
        public HasProfessionGlobalToken() : base("HasProfession") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            return this.SetValues(player.professions.Select(id =>
            {
                switch (id)
                {
                    case Farmer.acrobat: return "Acrobat";
                    case Farmer.brute: return "Brute";
                    case Farmer.defender: return "Defender ";
                    case Farmer.desperado: return "Desperado";
                    case Farmer.fighter: return "Fighter";
                    case Farmer.scout: return "Scout";
                    case Farmer.agriculturist: return "Agriculturist";
                    case Farmer.artisan: return "Artisan";
                    case Farmer.butcher: return "Butcher";
                    case Farmer.rancher: return "Rancher";
                    case Farmer.shepherd: return "Shepherd";
                    case Farmer.tiller: return "Tiller";
                    case Farmer.angler: return "Angler";
                    case Farmer.fisher: return "Fisher";
                    case Farmer.baitmaster: return "Mariner";
                    case Farmer.pirate: return "Pirate";
                    case Farmer.mariner: return "Luremaster ";
                    case Farmer.trapper: return "Trapper";
                    case Farmer.botanist: return "Botanist";
                    case Farmer.forester: return "Forester";
                    case Farmer.gatherer: return "Gatherer";
                    case Farmer.lumberjack: return "Lumberjack";
                    case Farmer.tapper: return "Tapper";
                    case Farmer.tracker: return "Tracker";
                    case Farmer.blacksmith: return "Blacksmith";
                    case Farmer.excavator: return "Excavator";
                    case Farmer.gemologist: return "Gemologist";
                    case Farmer.geologist: return "Geologist";
                    case Farmer.miner: return "Miner";
                    case Farmer.burrower: return "Prospector ";
                    default: return "";
                }
            }));
        }
    }

    public class HasReadLetterGlobalToken : GlobalToken
    {
        public HasReadLetterGlobalToken() : base("HasReadLetter") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            return this.SetValues(player.mailReceived);
        }
    }

    public class HasSeenEventGlobalToken : GlobalToken
    {
        public HasSeenEventGlobalToken() : base("HasSeenEvent") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            return this.SetValues(player.eventsSeen);
        }
    }

    public class HasWalletItemGlobalToken : GlobalToken
    {
        public HasWalletItemGlobalToken() : base("HasWalletItem") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            List<string> values = new List<string>();
            if (player.canUnderstandDwarves) values.Add("DwarvishTranslationGuide");
            if (player.hasRustyKey) values.Add("RustyKey");
            if (player.hasClubCard) values.Add("ClubCard");
            if (player.hasSpecialCharm) values.Add("SpecialCharm");
            if (player.hasSkullKey) values.Add("SkullKey");
            if (player.hasMagnifyingGlass) values.Add("MagnifyingGlass");
            if (player.hasDarkTalisman) values.Add("DarkTalisman");
            if (player.hasMagicInk) values.Add("MagicInk");
            if (player.eventsSeen.Contains(2120303)) values.Add("BearsKnowledge");
            if (player.eventsSeen.Contains(3910979)) values.Add("SpringOnionMastery");

            return this.SetValues(values);
        }
    }

    public class IsMainPlayerGlobalToken : GlobalToken
    {
        public IsMainPlayerGlobalToken() : base("IsMainPlayer") { }

        protected override bool Update()
        {
            return this.SetValue(Context.IsMainPlayer.ToString());
        }
    }

    public class IsOutdoorsGlobalToken : GlobalToken
    {
        public IsOutdoorsGlobalToken() : base("IsOutdoors") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.currentLocation?.IsOutdoors.ToString());
        }
    }

    public class LocationNameGlobalToken : GlobalToken
    {
        public LocationNameGlobalToken() : base("LocationName") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.currentLocation?.Name);
        }
    }

    public class PlayerGenderGlobalToken : GlobalToken
    {
        public PlayerGenderGlobalToken() : base("PlayerGender") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player.IsMale ? "Male" : "Female");
        }
    }

    public class PlayerNameGlobalToken : GlobalToken
    {
        public PlayerNameGlobalToken() : base("PlayerName") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player.Name);
        }
    }

    public class PreferredPetGlobalToken : GlobalToken
    {
        public PreferredPetGlobalToken() : base("PreferredPet") { }

        protected override bool Update()
        {
            return this.SetValue(Game1.player.catPerson ? "Cat" : "Dog");
        }
    }

    public class SkillLevelGlobalToken : GlobalToken
    {
        public SkillLevelGlobalToken() : base("SkillLevel") { }

        protected override bool Update()
        {
            Farmer player = Game1.player;
            if (player == null)
                return false;

            this.GlobalValue = "";
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
            return true;
        }
    }
}
