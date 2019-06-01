using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Mine_Changes.MineChanges.Config;
using Mine_Changes.MineChanges.Hook;
using StardewModdingAPI;
using StardewValley;

namespace Mine_Changes.MineChanges
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static HarmonyInstance hInstance;
        public static MineConfig config;
        public static Mod instance;
        public static bool jsonAssetsLoaded = false;
        public static bool spaceCoreLoaded = false;
        public static JsonAssetsAPI jsonAssets = null;
        public static SpaceCoreAPI spaceCoreAPI = null;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<MineConfig>() ?? new MineConfig();
            tryLoadAPIs(helper);
            try
            {
                hInstance = HarmonyInstance.Create("jpan.mine_changes");
                MineHooks.addTrans(this, hInstance);
                LocationHooks.addBreak(this, hInstance);
            }
            catch(Exception ex)
            {
                Monitor.Log("Could not patch one of the Hooks: " + ex, LogLevel.Error);
            }
        }

        private void tryLoadAPIs(IModHelper helper)
        {
            tryLoadJsonAssets(helper);
            tryLoadSpaceCore(helper);
        }

        private void tryLoadJsonAssets(IModHelper helper)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Monitor.Log("JsonAssets is not loaded. Will not be able to add defined objects to seed drops.", LogLevel.Info);
                return;
            }
            jsonAssetsLoaded = true;
            return;
        }

        private void tryLoadSpaceCore(IModHelper helper)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            {
                Monitor.Log("SpaceCore is not loaded. Will not be able to add Custom Skills to Skill-based chance and count modifiers.", LogLevel.Info);
                return;
            }
            spaceCoreLoaded = true;
            return;
        }


        public static int getObjectIDFromAsset(string oreName)
        {
            if (jsonAssetsLoaded)
            {
                if (jsonAssets == null)
                {
                    jsonAssets = Mod.instance.Helper.ModRegistry.GetApi<JsonAssetsAPI>("spacechase0.JsonAssets");
                    if (jsonAssets == null)
                    {
                        Mod.instance.Monitor.Log("Tried to load object " + oreName + " but could not find JsonAssets.", StardewModdingAPI.LogLevel.Error);
                        //jsonAssetsLoaded = false;
                        return -1000;
                    }
                }
                int ans = Mod.jsonAssets.GetObjectId(oreName);
                if (ans < 0)
                {
                    Mod.instance.Monitor.Log("Tried to load object " + oreName + " but JsonAssets could not find it.", StardewModdingAPI.LogLevel.Error);
                    return -1000;
                }
                return ans;
            }
            return -1000;
        }

        public static int getSkillLevel(Farmer who, string skill)
        {
            switch (skill)
            {
                case "0":
                case "farming":
                case "Farming":
                    return who.FarmingLevel;
                case "1":
                case "fishing":
                case "Fishing":
                    return who.FishingLevel;
                case "2":
                case "foraging":
                case "Foraging":
                    return who.ForagingLevel;
                case "3":
                case "mining":
                case "Mining":
                    return who.MiningLevel;
                case "4":
                case "combat":
                case "Combat":
                    return who.CombatLevel;
                case "6":
                case "luck":
                case "Luck":
                    return who.LuckLevel;
                default:
                    if (spaceCoreLoaded)
                    {

                        //spaceCore currently doesn't declare its api
                        /*if(spaceCoreAPI == null)
                        {
                            spaceCoreAPI = Mod.instance.Helper.ModRegistry.GetApi<SpaceCoreAPI>("spacechase0.SpaceCore");
                            if (spaceCoreAPI == null)
                            {
                                Mod.instance.Monitor.Log("Tried to load custom skill " + skill + " but could not find Space Core API.", StardewModdingAPI.LogLevel.Error);
                                return -1;
                            }
                        }*/



                        List<string> availableSkills = new List<string>();
                        Type Skills = Type.GetType("SpaceCore.Skills" + ", SpaceCore, Version = 1.2.2.0, Culture = neutral, PublicKeyToken = null");
                        if (Skills == null)
                        {
                            Mod.instance.Monitor.Log("Tried to load custom skill " + skill + " but could not find Skills class.", StardewModdingAPI.LogLevel.Error);
                            return -1;
                        }
                        MethodInfo getSkills = Skills.GetMethod("GetSkillList", AccessTools.all);
                        if (getSkills == null)
                        {
                            Mod.instance.Monitor.Log("Tried to load custom skill " + skill + " but could not find GetSkillList.", StardewModdingAPI.LogLevel.Error);
                            return -1;
                        }
                        availableSkills.AddRange((string[])(getSkills.Invoke(null, null)));
                        //availableSkills.AddRange(spaceCoreAPI.GetCustomSkills());
                        /*if (!doOnce)
                        {
                            Mod.instance.Monitor.Log("Skills Available:", StardewModdingAPI.LogLevel.Info);
                            foreach (string s in availableSkills)
                            {
                                Mod.instance.Monitor.Log(s, StardewModdingAPI.LogLevel.Info);
                            }
                            Mod.instance.Monitor.Log("Professions added:", StardewModdingAPI.LogLevel.Info);


                            doOnce = true;
                        }*/
                        if (!availableSkills.Contains(skill))
                        {
                            Mod.instance.Monitor.Log("Tried to load custom skill " + skill + " but could not find it", StardewModdingAPI.LogLevel.Error);
                            return -1;
                        }
                        MethodInfo levelOfCustomSkill = Skills.GetMethod("GetSkillLevel", AccessTools.all);
                        if (levelOfCustomSkill == null)
                        {
                            Mod.instance.Monitor.Log("Tried to load custom skill " + skill + " but could not find GetSkillLevel.", StardewModdingAPI.LogLevel.Error);
                            return -1;
                        }

                        //return spaceCoreAPI.GetLevelForCustomSkill(who, skill);
                        return (int)levelOfCustomSkill.Invoke(null, new object[] { who, skill });
                    }
                    break;
            }
            return -1;
        }
    }
}
