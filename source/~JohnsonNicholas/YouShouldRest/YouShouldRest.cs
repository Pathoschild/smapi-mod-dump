using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;

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

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            NPCCommenters.Clear();
        }

        private void OnReturnToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
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
            else if (Game1.player.health / Game1.player.maxHealth <= ModConfig.HealthThreshold)
                return true;

            return false;
        }

        private int GetLevelOnScale(float val, float maxVal)
        {
            //0 - 5 scale; 0 > 98%, 1 80-98%, 2 50 - 80%, 3 20-50%, 4 2-20%, 5 <2%
            float valPercent = val / maxVal;
            if (valPercent >= .98f)
                return 0;
            else if (valPercent >= .8f && valPercent < .98)
                return 1;
            else if (valPercent >= .5f && valPercent < .8)
                return 2;
            else if (valPercent >= .2f && valPercent < .5)
                return 3;
            else if (valPercent >= .02f && valPercent < .2)
                return 4;
            else if (valPercent < .02f)
                return 5;

            return 0;
        }
        
        public static int GetTimeOfDay(int timeOfDay)
        {
            //converts the time of day into a period
            //periods are 0 - earlyt morning, 1 - morning, 2 - afternoon, 3 - evening, 4 - night, 5 - late night
            // It's probably unlikely 0 will ever be used. And 5. But just in case!
            if (timeOfDay <= 700)
                return 0;
            else if (timeOfDay > 700 && timeOfDay <= 1200)
                return 1;
            else if (timeOfDay > 1200 && timeOfDay <= Game1.getStartingToGetDarkTime())
                return 2;
            else if (timeOfDay > Game1.getStartingToGetDarkTime() && timeOfDay <= Game1.getTrulyDarkTime())
                return 3;
            else if (timeOfDay > Game1.getTrulyDarkTime() && timeOfDay < 2200)
                return 4;
            return 5;            
        }

        private IEnumerable<string> GetKeys(CharacterDetails detailSet)
        {
            //specific to inspecific tied to specific NPCs
            yield return $"{detailSet.Name}[{detailSet.HeartLevel}]_{detailSet.SeasonDay}_{detailSet.HealthStatus}_{detailSet.StaminaStatus}_{detailSet.TimeOfDay}_{detailSet.SpouseStatus}";
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
                HeartLevel = Game1.player.getFriendshipHeartLevelForNPC(character.Name),
                HealthStatus = GetLevelOnScale((float)Game1.player.health, (float)Game1.player.maxHealth),
                StaminaStatus = Game1.player.exhausted ? 5 : GetLevelOnScale(Game1.player.Stamina, (float)Game1.player.MaxStamina),
                TimeOfDay = GetTimeOfDay(Game1.timeOfDay),
                SpouseStatus = Game1.player.friendshipData[character.Name].IsMarried() ? "spouse" : "",
                SeasonDay = string.Concat(Game1.currentSeason, Game1.dayOfMonth),
                SeasonDayFB = Game1.currentSeason
            };

            Monitor.Log(newDetails.ToString(), LogLevel.Info);

            foreach (string key in this.GetKeys(newDetails))
            {
                if (ModDialogues.ContainsKey(key))
                    return key;
            }

            //fallback to default if EVERYTHING's missing.
            return "";
        }

        private void OnMenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
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
