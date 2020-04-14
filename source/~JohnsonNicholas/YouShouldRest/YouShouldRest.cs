using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;

namespace TwilightShards.YouShouldRest
{
    //public class YouShouldRest : Mod, IAssetEditor
    public class YouShouldRest : Mod
    {
        internal Dictionary<string, string> ModDialogues;
        private readonly IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            ModDialogues = new Dictionary<string, string>();

            helper.Events.GameLoop.SaveLoaded += LoadPacksOnLoad;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void LoadPacksOnLoad(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, "dialogue.json")))
                {
                    Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}");
                    var rawData = contentPack.ReadJsonFile<List<RestModel>>("dialogue.json");
                    foreach (var r in rawData)
                    {
                        ModDialogues.Add(r.Conditions, r.Dialogue);
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
            if (Game1.player.Stamina / Game1.player.MaxStamina <= .825)
                return true;
            else if (Game1.player.health / Game1.player.maxHealth <= .825)
                return true;

            return false;
        }

        private int GetLevelOnScale(float val, float maxVal)
        {
            //0 - 5 scale; 0 > 98%, 1 80-99%, 2 50 - 80%, 3 20-50%, 4 2-20%, 5 <2%
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

        /// <summary>
        /// This function returns the key for a given dialogue. 
        /// </summary>
        /// <param name="character">NPC for the dialogue</param>
        /// <returns>The key for the dialogue</returns>
        private string GetDialogueForConditions(NPC character)
        {
            //json format ex: abigail[10]summer_4_4_spouse
            // name[heartlevel]season_healthstatus_staminastatus_timeofday_spousestatus
            int HeartLevel = Game1.player.getFriendshipHeartLevelForNPC(character.Name);
            int HealthStatus = GetLevelOnScale((float)Game1.player.health, (float)Game1.player.maxHealth);
            int StaminaStatus = Game1.player.exhausted ? 5 : GetLevelOnScale(Game1.player.Stamina, (float)Game1.player.MaxStamina);
            int TimeOfDay = GetTimeOfDay(Game1.timeOfDay);
            string SpouseStatus = Game1.player.friendshipData[character.Name].IsMarried() ? "spouse" : "";
            string seasonDay = string.Concat(Game1.currentSeason, Game1.dayOfMonth);

            string s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + HealthStatus + "_" + StaminaStatus + "_" + TimeOfDay;
            if (!string.IsNullOrEmpty(SpouseStatus))
                s += "_" + SpouseStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + HealthStatus + "_" + StaminaStatus + "_" + TimeOfDay;
            if (!string.IsNullOrEmpty(SpouseStatus))
                s += "_" + SpouseStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + HealthStatus + "_" + StaminaStatus + "_" + TimeOfDay;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + HealthStatus + "_" + StaminaStatus + "_" + TimeOfDay;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + HealthStatus + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + HealthStatus + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + seasonDay + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "[" + HeartLevel + "]_" + Game1.currentSeason + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "_" + Game1.currentSeason + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "_" + seasonDay + "_" + StaminaStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "_" + Game1.currentSeason + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            s = character.Name + "_" + seasonDay + "_" + HealthStatus;
            if (ModDialogues.ContainsKey(s))
                return s;

            //fallback to disposition text
            //age, manners?,social anxiety, optimism
            //format is:
            //age_manners_social_timeofday_optimism
            // it tries to find the longest one first, then works backwards


            //fallback to default if EVERYTHING's missing.
            return "default";
        }

        private void OnMenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox dBox && !Game1.eventUp && StaminaCheck())
            {
                var cDBU = Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").GetValue();
                Dialogue diag = Helper.Reflection.GetField<Dialogue>(dBox, "characterDialogue").GetValue();
                cDBU.Push(Helper.Translation.Get(GetDialogueForConditions(diag.speaker)));
                Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").SetValue(cDBU);
            }
        }
    }
}
