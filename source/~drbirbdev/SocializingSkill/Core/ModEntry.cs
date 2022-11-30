/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared;
using BirbShared.Asset;
using BirbShared.Command;
using BirbShared.Config;
using HarmonyLib;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SocializingSkill
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal static Config Config;
        internal static Assets Assets;

        internal ITranslationHelper I18n => this.Helper.Translation;

        internal static readonly PerScreen<List<string>> BelovedCheckedToday = new();

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);

            Config = helper.ReadConfig<Config>();

            Assets = new Assets();
            new AssetClassParser(this, Assets).ParseAssets();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            SpaceCore.Events.SpaceEvents.AfterGiftGiven += this.SpaceEvents_AfterGiftGiven;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            new Harmony(this.ModManifest.UniqueID).PatchAll();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();
            Skills.RegisterSkill(new SocializingSkill());
        }

        // Beloved Profession
        //  - reset which villagers have been checked for bonus gifts today for each player.
        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            BelovedCheckedToday.Value = new List<string>();
        }

        // Grant XP
        // Gifter Profession
        //  - Give extra friendship
        private void SpaceEvents_AfterGiftGiven(object sender, SpaceCore.Events.EventArgsGiftGiven e)
        {
            int taste = e.Npc.getGiftTasteForThisItem(e.Gift);
            if (Game1.player.HasCustomProfession(SocializingSkill.Gifter))
            {
                int extraFriendship = 0;
                switch (taste)
                {
                    case 0:
                        extraFriendship = ModEntry.Config.GifterLovedGiftExtraFriendship;
                        break;
                    case 2:
                        extraFriendship = ModEntry.Config.GifterLikedGiftExtraFriendship;
                        break;
                    case 8:
                        extraFriendship = ModEntry.Config.GifterNeutralGiftExtraFriendship;
                        break;
                }
                Game1.player.changeFriendship(extraFriendship, e.Npc);
            }

            if (taste <= 2)
            {
                float exp = ModEntry.Config.ExperienceFromGifts;
                if (taste == 0)
                {
                    exp *= ModEntry.Config.LovedGiftExpMultiplier;
                }
                if (e.Npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                {
                    exp *= ModEntry.Config.BirthdayGiftExpMultiplier;
                }
                Skills.AddExperience(Game1.player, "drbirbdev.Socializing", (int)exp);
            }
        }
    }
}
