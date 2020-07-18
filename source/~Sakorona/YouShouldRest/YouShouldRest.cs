using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace TwilightShards.YouShouldRest
{
    public class YouShouldConfig
    {
        public double CommentChance = .35;
        public double StaminaThreshold = .785;
        public double HealthThreshold = .785;
    }

    public class DialogueText
    {
        public string OriginMod { get; set; }
        public string Text { get; set; }

        public DialogueText()
        {

        }

        public DialogueText(string origin, string text)
        {
            OriginMod = origin;
            Text = text;
        }
    }

    public class YouShouldRest : Mod
    {
        internal Dictionary<string,DialogueText> ModDialogues;
        private IModHelper helper;
        private YouShouldConfig ModConfig;
        private List<string> NPCCommenters;

        public override void Entry(IModHelper Helper)
        {
            NPCCommenters = new List<string>();
            ModDialogues = new Dictionary<string, DialogueText>();
            ModConfig = Helper.ReadConfig<YouShouldConfig>();
            helper = Helper;
            LoadPacksOnLoad();

            Helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            NPCCommenters.Clear();
        }

        private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            NPCCommenters.Clear();
        }

        private void LoadPacksOnLoad()
        {
            Monitor.Log("Scanning for content packs", LogLevel.Info);
            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, "dialogue.json")))
                {
                    Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Info);
                    var rawData = contentPack.ReadJsonFile<Dictionary<string,string>>("dialogue.json");
                    foreach (var r in rawData)
                    {
                        if (ModDialogues.ContainsKey(r.Key))
                        {
                            Monitor.Log(Helper.Translation.Get("dupAddMsg", new { addingMod = contentPack.Manifest.Name, key = r.Key, orgMod = ModDialogues[r.Key].OriginMod }), LogLevel.Warn);
                            ModDialogues.Remove(r.Key);
                        }
                        else
                            ModDialogues.Add(r.Key, new DialogueText(contentPack.Manifest.Name,r.Value));
                    }
                }
                else
                {
                    Monitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an dialogue.json file.", LogLevel.Warn);
                }
            }
        }

        private bool StaminaCheck()
        {
            if (Game1.player.Stamina / Game1.player.MaxStamina <= ModConfig.StaminaThreshold)
                return true;
            if (Game1.player.health / Game1.player.maxHealth <= ModConfig.HealthThreshold)
                return true;

            return false;
        }

        private string GetLevelOnScale(float val, float maxVal)
        {
            float valPercent = val / maxVal;
            if (valPercent > .75)
                return "High";
            if (valPercent > .35 && valPercent <= .75)
                return "Medium";
            if (valPercent > .02 && valPercent <= .35)
                return "Low";
            if (valPercent <= .02)
                return "Very Low";

            return "";
        }
        
        public static int GetTimeOfDay(int timeOfDay)
        {
            //converts the time of day into a period
            //periods are 0 - early morning, 1 - morning, 2 - afternoon, 3 - evening, 4 - night, 5 - late night
            // It's probably unlikely 0 will ever be used. And 5. But just in case!
            if (timeOfDay <= 700)
                return 0;
            if (timeOfDay > 700 && timeOfDay <= 1200)
                return 1;
            if (timeOfDay > 1200 && timeOfDay <= Game1.getStartingToGetDarkTime())
                return 2;
            if (timeOfDay > Game1.getStartingToGetDarkTime() && timeOfDay <= Game1.getTrulyDarkTime())
                return 3;
            if (timeOfDay > Game1.getTrulyDarkTime() && timeOfDay < 2200)
                return 4;
            return 5;            
        }

        private IEnumerable<string> GetKeys(CharacterDetails detailSet)
        {
            //specific to inspecific tied to specific NPCs
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}_{detailSet.TimeOfDay}";

            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDayFB}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDay}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}";

            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.HealthStatus}";


            yield return $"{detailSet.Name}_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Name}_{detailSet.SeasonDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Name}_{detailSet.SeasonDayFB}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Name}_{detailSet.SeasonDay}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Name}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}";

            //fallback to disposition text
            //age, manners?,social anxiety, optimism
            //format is:
            //age_manners_social_timeofday_optimism
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.Optimism}_{detailSet.HealthStatus}";

            //-Optimism
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}";

            //-Time Of Day
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.StaminaStatus}";

            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDay}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.SocialAnxiety}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}";

            // -Social -Seasonday! :D
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.Manners}_{detailSet.HealthStatus}";

            //+SeasonDay. -Manners :v
            yield return $"{detailSet.Age}_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.SeasonDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.SeasonDayFB}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.SeasonDay}_{detailSet.HealthStatus}";
            yield return $"{detailSet.Age}_{detailSet.SeasonDayFB}_{detailSet.HealthStatus}";

            //just timeOfDay + Manners
            yield return $"{detailSet.Manners}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Manners}_{detailSet.TimeOfDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Manners}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}";

            //just timeOfDay + Age
            yield return $"{detailSet.Age}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.TimeOfDay}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.TimeOfDay}_{detailSet.HealthStatus}";

            //just age
            yield return $"{detailSet.Age}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Age}_{detailSet.HealthStatus}";

            //just manners
            yield return $"{detailSet.Manners}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Manners}_{detailSet.StaminaStatus}";
            yield return $"{detailSet.Manners}_{detailSet.HealthStatus}";
        }

        /// <summary>
        /// This function returns the key for a given dialogue. 
        /// </summary>
        /// <param name="character">NPC for the dialogue</param>
        /// <returns>The key for the dialogue</returns>
        private string GetDialogueForConditions(NPC character)
        {
            //json format ex: abigail[10]summer_4_4_spouse
            // name[heartlevel]season_healthstatus_staminastatus_timeofday_spousestatus
            CharacterDetails newDetails = new CharacterDetails
            {
                Name = character.Name,
                Age = character.Age,
                Manners = character.Manners,
                SocialAnxiety = character.SocialAnxiety,
                Optimism = character.Optimism,
                HeartLevel = GetHeartLevel(character), //Game1.player.getFriendshipHeartLevelForNPC(character.Name),
                HealthStatus = GetLevelOnScale(Game1.player.health, Game1.player.maxHealth),
                StaminaStatus = Game1.player.exhausted.Value ? "Very Low" : GetLevelOnScale(Game1.player.Stamina, Game1.player.MaxStamina),
                TimeOfDay = GetTimeOfDay(Game1.timeOfDay),
                SeasonDay = string.Concat(Game1.currentSeason, Game1.dayOfMonth),
                SeasonDayFB = Game1.currentSeason
            };

            Monitor.Log(newDetails.ToString(), LogLevel.Info);

            foreach (string key in GetKeys(newDetails))
            {
                if (ModDialogues.ContainsKey(key))
                    return key;
            }

            //fallback to default if EVERYTHING's missing.
            return "";
        }

        private string GetHeartLevel(NPC character)
        {
            int level = Game1.player.getFriendshipHeartLevelForNPC(character.Name);

            if (level < 2)
                return "Acquaintance";
            if (level >= 2 && level < 6)
                return "Friend";
            if (level >= 6 && level < 8)
                return "Close Friend";
            if (level >= 8 && level < 10 && Game1.player.friendshipData[character.Name].IsDating())
                return "Dating";
            if (level >= 8 && level < 10 && !Game1.player.friendshipData[character.Name].IsDating())
                return "Best Friend";
            if (level >= 10 && Game1.player.friendshipData[character.Name].IsMarried())
                return "Married";
            if (level >= 10 && !Game1.player.friendshipData[character.Name].IsDating())
                return "Platonic Soulmate";

            return "";

        }


        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox dBox && !Game1.eventUp && StaminaCheck() && Game1.random.NextDouble() < ModConfig.CommentChance)
            {
                var cDBU = Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").GetValue();
                Dialogue diag = Helper.Reflection.GetField<Dialogue>(dBox, "characterDialogue").GetValue();
                if (diag != null && diag.speaker != null && !NPCCommenters.Contains(diag.speaker.Name))
                {
                    if (diag.temporaryDialogue == Helper.Translation.Get("Strings\\UI:Carpenter_DemolishCabinConfirm") ||
                        diag.temporaryDialogue == Helper.Translation.Get("Data\\ExtraDialogue:Robin_Instant") ||
                        diag.temporaryDialogue == Helper.Translation.Get("Data\\ExtraDialogue:Robin_UpgradeConstruction") ||
                        diag.temporaryDialogue == Helper.Translation.Get("Data\\ExtraDialogue:Robin_NewConstruction") ||
                        diag.temporaryDialogue == Helper.Translation.Get("Data\\ExtraDialogue:Robin_UpgradeConstructionFestival") || diag.temporaryDialogue == Helper.Translation.Get("Data\\ExtraDialogue:Robin_NewConstructionFestival")
                        ) { 
                        return;
                        }
                    NPCCommenters.Add(diag.speaker.Name);
                    string key = GetDialogueForConditions(diag.speaker);
                    string ret;
                    if (!string.IsNullOrEmpty(key) && ModDialogues.ContainsKey(key))
                    {
                        ret = ModDialogues[key].Text;
                        cDBU.Push(ret);
                    }
                    Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").SetValue(cDBU);
                }
            }
        }
    }
}
