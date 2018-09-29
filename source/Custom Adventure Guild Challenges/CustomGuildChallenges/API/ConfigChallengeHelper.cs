using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomGuildChallenges.API
{
    public class ConfigChallengeHelper : ICustomChallenges
    {
        // Location Names to find dying monsters
        public readonly string FarmLocationName = "Farm";
        public readonly string BugLocationName = "BugLand";

        /// <summary>
        ///     Mod configuration
        /// </summary>
        internal static ModConfig Config;

        /// <summary>
        ///     Configuration and challenge list
        /// </summary>
        internal static IList<SlayerChallenge> ChallengeList;      

        /// <summary>
        ///     Mod's version of the Adventure Guild
        /// </summary>
        internal readonly CustomAdventureGuild customAdventureGuild;

        /// <summary>
        ///     Vanilla version of the Adventure Guild
        /// </summary>
        internal readonly AdventureGuild adventureGuild;

        /// <summary>
        ///     SMAPI API - used for saving and loading JSON files
        /// </summary>
        internal readonly IModHelper modHelper;       

        /// <summary>
        ///     Is invoked each time a monster is killed
        /// </summary>
        public event EventHandler<Monster> MonsterKilled;

        /// <summary>
        ///     Creates guild and sets up events
        /// </summary>
        /// <param name="guild"></param>
        public ConfigChallengeHelper(IModHelper helper, ModConfig config)
        {
            modHelper = helper;
            Config = config;

            ChallengeList = new List<SlayerChallenge>();               
            adventureGuild = new AdventureGuild(CustomAdventureGuild.StandardMapPath, CustomAdventureGuild.StandardMapName);
            customAdventureGuild = new CustomAdventureGuild();

            if(ChallengeList == null || ChallengeList.Count == 0)
            {
                foreach (var info in config.Challenges) ChallengeList.Add(new SlayerChallenge() { Info = info });
            }
            
            SaveEvents.AfterCreate += SetupMonsterKilledEvent;
            SaveEvents.AfterLoad += SetupMonsterKilledEvent;
            
            SaveEvents.BeforeSave += PresaveData;
            SaveEvents.AfterSave += InjectGuild;
            SaveEvents.AfterLoad += InjectGuild;
            SaveEvents.AfterCreate += InjectGuild;

            MonsterKilled += Events_MonsterKilled;
        }

        /// <summary>
        ///     Add a challenge for the player to complete. The global config will not be updated.
        /// </summary>
        /// <param name="challengeName"></param>
        /// <param name="killCount"></param>
        /// <param name="rewardItemType"></param>
        /// <param name="rewardItemNumber"></param>
        /// <param name="monsterNames"></param>
        public void AddChallenge(string challengeName, int killCount, int rewardItemType, int rewardItemNumber, int rewardItemStack, IList<string> monsterNames)
        {
            var challenge = new SlayerChallenge()
            {
                CollectedReward = false,
                Info = new ChallengeInfo()
                {
                    ChallengeName = challengeName,
                    RequiredKillCount = killCount,
                    RewardType = rewardItemType,
                    RewardItemNumber = rewardItemNumber,
                    RewardItemStack = rewardItemStack,
                    MonsterNames = monsterNames.ToList()
                }
            };

            ChallengeList.Add(challenge);
        }

        /// <summary>
        ///     Remove a challenge from the challenge list. The global config will not be updated.
        /// </summary>
        /// <param name="challengeName"></param>
        public void RemoveChallenge(string challengeName)
        {
            for(int i = 0; i < ChallengeList.Count; i++)
            {
                if(ChallengeList[i].Info.ChallengeName == challengeName)
                {
                    ChallengeList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        ///     Set the dialogue for Gil
        /// </summary>
        /// <param name="initialNoRewardDialogue"></param>
        /// <param name="secondNoRewardDialogue"></param>
        /// <param name="specialRewardDialogue"></param>
        public void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue, string specialRewardDialogue)
        {
            Config.GilNoRewardDialogue= initialNoRewardDialogue;
            Config.GilSleepingDialogue = secondNoRewardDialogue;
            Config.GilSpecialGiftDialogue = specialRewardDialogue;
        }

        /// <summary>
        ///     Setup event that detects whether monsters are killed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SetupMonsterKilledEvent(object sender, EventArgs e)
        {
            // Inject into all mines
            MineEvents.MineLevelChanged += MineEvents_MineLevelChanged;
            // Inject into all locations that spawn monsters that are not in the mines
            foreach (var location in Game1.locations)
            {
                if (location.Name == FarmLocationName || location.Name == BugLocationName)
                {
                    location.characters.OnValueRemoved += Characters_OnValueRemoved;
                }
            }
        }

        /// <summary>
        ///     Sets up detection for when a monster dies in the mines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MineEvents_MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (Game1.mine != null) Game1.mine.characters.OnValueRemoved += Characters_OnValueRemoved;
        }

        /// <summary>
        ///     Fires the MonsterKilled event if the removed NPC is a monster and has 0 or less health
        ///     or its a grub that doesn't have a fixed health value of -500 for when it transforms into a fly
        /// </summary>
        /// <param name="value"></param>
        private void Characters_OnValueRemoved(NPC value)
        {
            // Grub at -500 health means it transformed
            // This is a hacky way to detect transformation, but the alternative is reflection
            if (value is Monster monster && monster.Health <= 0 && (!(value is Grub grub) || grub.Health != -500))
            {                
                MonsterKilled.Invoke(Game1.currentLocation, monster);
            }
        }

        /// <summary>
        ///     Saves the status of challenges and switches the
        ///     CustomAdventureGuild with AdventureGuild to prevent
        ///     crashing the save process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void PresaveData(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = new SaveData();

            foreach (var slayerChallenge in ChallengeList)
            {
                var save = new ChallengeSave()
                {
                    ChallengeName = slayerChallenge.Info.ChallengeName,
                    Collected = slayerChallenge.CollectedReward
                };

                saveData.Challenges.Add(save);
            }

            modHelper.WriteJsonFile(saveDataPath, saveData);

            // Remove custom location and add back the original location
            Game1.locations.Remove(customAdventureGuild);
            Game1.locations.Add(adventureGuild);
        }

        /// <summary>
        ///     Read the save data file and replace the AdventureGuild with
        ///     CustomAdventureGuild
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void InjectGuild(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = modHelper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

            foreach (var savedChallenge in saveData.Challenges)
            {
                foreach (var slayerChallenge in ChallengeList)
                {
                    if (savedChallenge.ChallengeName == slayerChallenge.Info.ChallengeName)
                    {
                        slayerChallenge.CollectedReward = savedChallenge.Collected;
                        break;
                    }
                }
            }

            if (Game1.player.IsMainPlayer && !customAdventureGuild.HasMarlon()) customAdventureGuild.AddMarlon();

            // Kill old guild, replace with new guild
            Game1.locations.Remove(adventureGuild);
            Game1.locations.Add(customAdventureGuild);
        }

        /// <summary>
        ///     Adds kills for monsters on the farm (if enabled) and Wilderness Golems
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_MonsterKilled(object sender, Monster e)
        {
            if (!(sender is GameLocation location)) return;
            if (Game1.player.currentLocation.Name != location.Name) return;

            string monsterName = e.Name;
            if (location.IsFarm && (Config.CountKillsOnFarm || monsterName == Monsters.WildernessGolem))
            {
                Game1.player.stats.monsterKilled(monsterName);
            }
            else if (location.Name == BugLocationName)
            {
                string mutantName = "Mutant " + monsterName;
                Game1.player.stats.monsterKilled(mutantName);
                Game1.player.stats.specificMonstersKilled[monsterName]--;
                monsterName = mutantName;
            }

            //if (Config.DebugMonsterKills) Monitor.Log(monsterName + " killed for total of " + Game1.player.stats.getMonstersKilled(e.Name));

            NotifyIfChallengeComplete(monsterName);
        }

        /// <summary>
        ///     Display message to see Gil if the challenge just completed
        /// </summary>
        private void NotifyIfChallengeComplete(string monsterKilled)
        {
            bool hasMonster;
            int kills;

            foreach (var challenge in ChallengeList)
            {
                if (challenge.CollectedReward) continue;

                kills = 0;
                hasMonster = false;

                foreach (var monsterName in challenge.Info.MonsterNames)
                {
                    kills += Game1.player.stats.getMonstersKilled(monsterName);
                    if (monsterName == monsterKilled) hasMonster = true;
                }

                if (hasMonster && kills == challenge.Info.RequiredKillCount)
                {
                    string message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Stats.cs.5129");
                    if (!IsVanillaChallenge(challenge.Info)) Game1.showGlobalMessage(message);
                    break;
                }
            }
        }

        /// <summary>
        ///     Check to see if challenge is vanilla
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool IsVanillaChallenge(ChallengeInfo info)
        {
            foreach (var challenge in CustomGuildChallengeMod.VanillaChallenges)
            {
                if (challenge.RequiredKillCount == info.RequiredKillCount && challenge.ChallengeName == info.ChallengeName
                    && challenge.MonsterNames.All(x => info.MonsterNames.Any(y => x == y)) && info.MonsterNames.All(x => challenge.MonsterNames.Any(y => y == x)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
