/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Compat.v100;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Data
{
    internal class VersionData
    {

        public static StaticData Reconfigure()
        {

            StaticData v220 = new();

            if (!Context.IsMainPlayer)
            {

                return v220;

            }

            StardewDruid.Compat.v100.StaticData v100 = Mod.instance.Helper.Data.ReadSaveData<StardewDruid.Compat.v100.StaticData>("staticData");

            if (v100 != null)
            {

                // ---------------------------------------------

                switch (v100.activeBlessing)
                {
                    case "Earth":
                    case "Weald":

                        v220.rite = Cast.Rite.rites.weald;

                        break;

                    case "Water":
                    case "Mists":

                        v220.rite = Cast.Rite.rites.mists;

                        break;

                    case "Stars":

                        v220.rite = Cast.Rite.rites.stars;

                        break;

                    case "Fates":

                        v220.rite = Cast.Rite.rites.fates;

                        break;

                    case "Ether":

                        v220.rite = Cast.Rite.rites.ether;

                        break;

                    default:

                        v220.rite = Cast.Rite.rites.none;

                        break;

                }

                // ---------------------------------------------

                Dictionary<string, string> questIds = new()
                {

                    ["280729871"] = "approachEffigy",
                    ["280729872"] = "challengeEarth",
                    ["280729873"] = "challengeWater",
                    ["280729874"] = "challengeStars",
                    ["280729875"] = "swordEarth",
                    ["280729876"] = "swordWater",
                    ["280729877"] = "swordStars",
                    ["280719871"] = "approachEffigy",
                    ["280719872"] = "challengeEarth",
                    ["280719873"] = "challengeWater",
                    ["280719874"] = "challengeStars",
                    ["280719875"] = "swordEarth",
                    ["280719876"] = "swordWater",
                    ["280719877"] = "swordStars",
                    ["18465001"] = "approachEffigy",
                    ["18465005"] = "swordEarth",
                    ["18465011"] = "lessonVillager",
                    ["18465012"] = "lessonCreature",
                    ["18465013"] = "lessonForage",
                    ["18465014"] = "lessonCrop",
                    ["18465015"] = "lessonRockfall",
                    ["18465002"] = "challengeEarth",
                    ["18465006"] = "swordWater",
                    ["18465016"] = "lessonTotem",
                    ["18465017"] = "lessonCookout",
                    ["18465018"] = "lessonFishspot",
                    ["18465019"] = "lessonSmite",
                    ["18465020"] = "lessonPortal",
                    ["18465003"] = "challengeWater",
                    ["18465007"] = "swordStars",
                    ["18465021"] = "lessonMeteor",
                    ["18465004"] = "challengeStars",
                    ["18465031"] = "challengeCanoli",
                    ["18465032"] = "challengeMariner",
                    ["18465033"] = "challengeSandDragon",
                    ["18465034"] = "challengeGemShrine",
                    ["18465035"] = "challengeMuseum",
                    ["18465040"] = "approachJester",
                    ["18465047"] = "swordFates",
                    ["18465041"] = "lessonWhisk",
                    ["18465042"] = "lessonTrick",
                    ["18465043"] = "lessonEnchant",
                    ["18465044"] = "lessonGravity",
                    ["18465045"] = "lessonDaze",
                    ["18465046"] = "challengeFates",
                    ["18465050"] = "swordEther",
                    ["18465051"] = "lessonTransform",
                    ["18465052"] = "lessonFlight",
                    ["18465053"] = "lessonBlast",
                    ["18465054"] = "lessonDive",
                    ["18465055"] = "lessonTreasure",
                    ["18465056"] = "challengeEther",
                    ["18465057"] = "approachShadowtin",
                    ["18465061"] = "heartEffigy",
                    ["18465062"] = "heartJester",


                };

                for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
                {

                    string gameId = Game1.player.questLog[num].id.Value;

                    if (gameId == null)
                    {

                        if (Game1.player.questLog[num].questTitle == null)
                        {

                            Game1.player.questLog.RemoveAt(num);

                        }

                        continue;

                    }

                    if (questIds.ContainsKey(gameId))
                    {

                        Game1.player.questLog.RemoveAt(num);

                        continue;

                    }

                }

                // ---------------------------------------------


                foreach (KeyValuePair<int, string> attunement in v100.weaponAttunement)
                {

                    switch (attunement.Value)
                    {
                        case "Earth":
                        case "Weald":

                            v220.attunement.Add(attunement.Key, Cast.Rite.rites.weald);

                            break;

                        case "Water":
                        case "Mists":

                            v220.attunement.Add(attunement.Key, Cast.Rite.rites.mists);

                            break;

                        case "Stars":

                            v220.attunement.Add(attunement.Key, Cast.Rite.rites.stars);

                            break;

                        case "Fates":

                            v220.attunement.Add(attunement.Key, Cast.Rite.rites.fates);

                            break;

                        case "Ether":

                            v220.attunement.Add(attunement.Key, Cast.Rite.rites.ether);

                            break;

                        default:

                            break;

                    }

                }
                /*
                v220.milestone = QuestHandle.milestones.effigy;

                Dictionary<int, QuestHandle.milestones> progression = new()
                {
                    [0] = QuestHandle.milestones.weald_weapon,
                    [1] = QuestHandle.milestones.weald_lessons,
                    [6] = QuestHandle.milestones.weald_challenge,
                    [7] = QuestHandle.milestones.mists_weapon,
                    [8] = QuestHandle.milestones.mists_lessons,
                    [13] = QuestHandle.milestones.mists_challenge,
                    [14] = QuestHandle.milestones.stars_weapon,
                    [15] = QuestHandle.milestones.stars_lessons,
                    [16] = QuestHandle.milestones.stars_challenge,
                    [17] = QuestHandle.milestones.stars_threats,
                    [18] = QuestHandle.milestones.jester,
                    [19] = QuestHandle.milestones.fates_weapon,
                    [20] = QuestHandle.milestones.fates_lessons,
                    [25] = QuestHandle.milestones.fates_challenge,
                    [26] = QuestHandle.milestones.ether_weapon,
                    [27] = QuestHandle.milestones.ether_lessons,
                    [32] = QuestHandle.milestones.ether_challenge,
                    [33] = QuestHandle.milestones.shadowtin,
                    [33] = QuestHandle.milestones.effigy_heart,
                    [33] = QuestHandle.milestones.jester_heart,

                };

                foreach(KeyValuePair<int, QuestHandle.milestones> progress in progression)
                {

                    if(v100.activeProgress > progress.Key)
                    {

                        v220.milestone = progress.Value;

                    }

                }*/

            }

            return v220;

        }

    }


}
