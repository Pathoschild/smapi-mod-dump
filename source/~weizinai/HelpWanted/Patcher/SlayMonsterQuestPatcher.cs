/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class SlayMonsterQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public SlayMonsterQuestPatcher(ModConfig config)
    {
        SlayMonsterQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<SlayMonsterQuest>(nameof(SlayMonsterQuest.loadQuestInfo)), this.GetHarmonyMethod(nameof(LoadQuestInfoPrefix))
        );
    }

    private static bool LoadQuestInfoPrefix(ref SlayMonsterQuest __instance, Random ___random)
    {
        for (var i = 0; i < ___random.Next(1, 100); i++) ___random.Next();

        if (__instance.target.Value is not null && __instance.monster is not null) return false;

        __instance.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13696");

        var possibleMonsters = new List<string>();
        InitPossibleMonsters(possibleMonsters);

        var number = __instance.monsterName.Value == null ? 1 : __instance.numberToKill.Value == 0 ? 1 : 0;
        if (number != 0) __instance.monsterName.Value = ___random.ChooseFrom(possibleMonsters);
        if (__instance.monsterName.Value is "Frost Jelly" or "Sludge" or "Big Slime")
        {
            __instance.monster!.Value = new Monster("Green Slime", Vector2.Zero);
            __instance.monster.Value.Name = __instance.monsterName.Value;
        }
        else
        {
            __instance.monster!.Value = new Monster(__instance.monsterName.Value, Vector2.Zero);
        }

        if (number != 0)
        {
            switch (__instance.monsterName.Value)
            {
                case "Bug":
                case "Grub":
                case "Fly":
                    __instance.numberToKill.Value = ___random.Next(10, 21);
                    __instance.reward.Value = __instance.numberToKill.Value * 30;
                    break;
                case "Green Slime":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.numberToKill.Value -= __instance.numberToKill.Value % 2;
                    __instance.reward.Value = __instance.numberToKill.Value * 60;
                    break;
                case "Frost Jelly":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.numberToKill.Value -= __instance.numberToKill.Value % 2;
                    __instance.reward.Value = __instance.numberToKill.Value * 85;
                    break;
                case "Sludge":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.numberToKill.Value -= __instance.numberToKill.Value % 2;
                    __instance.reward.Value = __instance.numberToKill.Value * 125;
                    break;
                case "Big Slime":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.numberToKill.Value -= __instance.numberToKill.Value % 2;
                    __instance.reward.Value = __instance.numberToKill.Value * 180;
                    break;
                case "Rock Crab":
                    __instance.numberToKill.Value = ___random.Next(2, 6);
                    __instance.reward.Value = __instance.numberToKill.Value * 75;
                    break;
                case "Lava Crab":
                    __instance.numberToKill.Value = ___random.Next(2, 6);
                    __instance.reward.Value = __instance.numberToKill.Value * 180;
                    break;
                case "Iridium Crab":
                    __instance.numberToKill.Value = ___random.Next(2, 6);
                    __instance.reward.Value = __instance.numberToKill.Value * 350;
                    break;
                case "Bat":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.reward.Value = __instance.numberToKill.Value * 60;
                    break;
                case "Frost Bat":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.reward.Value = __instance.numberToKill.Value * 90;
                    break;
                case "Lave Bat":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.reward.Value = __instance.numberToKill.Value * 120;
                    break;
                case "Iridium Bat":
                    __instance.numberToKill.Value = ___random.Next(4, 11);
                    __instance.reward.Value = __instance.numberToKill.Value * 450;
                    break;
                case "Duggy":
                    __instance.numberToKill.Value = ___random.Next(2, 4);
                    __instance.reward.Value = __instance.numberToKill.Value * 150;
                    break;
                case "Stone Golem":
                case "Ghost":
                    __instance.numberToKill.Value = ___random.Next(2, 4);
                    __instance.reward.Value = __instance.numberToKill.Value * 250;
                    break;
                case "Carbon Ghost":
                    __instance.numberToKill.Value = ___random.Next(2, 4);
                    __instance.reward.Value = __instance.numberToKill.Value * 300;
                    break;
                case "Dust Spirit":
                    __instance.numberToKill.Value = ___random.Next(10, 21);
                    __instance.reward.Value = __instance.numberToKill.Value * 60;
                    break;
                case "Skeleton":
                    __instance.numberToKill.Value = ___random.Next(6, 12);
                    __instance.reward.Value = __instance.numberToKill.Value * 100;
                    break;
                case "Shadow Brute":
                case "Shadow Shaman":
                    __instance.numberToKill.Value = ___random.Next(6, 12);
                    __instance.reward.Value = __instance.numberToKill.Value * 200;
                    break;
                case "Metal Head":
                    __instance.numberToKill.Value = ___random.Next(2, 6);
                    __instance.reward.Value = __instance.numberToKill.Value * 275;
                    break;
                case "Squid Kid":
                    __instance.numberToKill.Value = ___random.Next(1, 3);
                    __instance.reward.Value = __instance.numberToKill.Value * 350;
                    break;
                case "Mummy":
                    __instance.numberToKill.Value = ___random.Next(2, 4);
                    __instance.reward.Value = __instance.numberToKill.Value * 400;
                    break;
                case "Serpent":
                    __instance.numberToKill.Value = ___random.Next(6, 12);
                    __instance.reward.Value = __instance.numberToKill.Value * 300;
                    break;
                case "Pepper Rex":
                    __instance.numberToKill.Value = ___random.Next(1, 3);
                    __instance.reward.Value = __instance.numberToKill.Value * 500;
                    break;
                default:
                    __instance.numberToKill.Value = ___random.Next(3, 7);
                    __instance.reward.Value = __instance.numberToKill.Value * 120;
                    break;
            }
        }

        __instance.reward.Value = (int)(__instance.reward.Value * config.SlayMonstersRewardMultiplier);

        switch (__instance.monsterName.Value)
        {
            case "Green Slime":
            case "Frost Jelly":
            case "Sludge":
            case "Big Slime":
                __instance.parts.Clear();
                __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13723", __instance.numberToKill.Value,
                    __instance.monsterName.Value.Equals("Frost Jelly")
                        ? new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13725")
                        : __instance.monsterName.Value.Equals("Sludge")
                            ? new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13727")
                            : new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13728")));
                __instance.target.Value = "Lewis";
                __instance.dialogueparts.Clear();
                __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13730");
                if (___random.NextBool())
                {
                    __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13731");
                    __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + ___random.Choose("13732", "13733"));
                    __instance.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13734",
                        new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + ___random.Choose("13735", "13736")),
                        new DescriptionElement("Strings\\StringsFromCSFiles:Dialogue.cs." + ___random.Choose<string>("795", "796", "797", "798", "799", "800", "801",
                            "802", "803", "804", "805", "806", "807", "808", "809", "810")),
                        new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + ___random.Choose("13740", "13741", "13742"))));
                }
                else
                {
                    __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13744");
                }

                break;
            case "Rock Crab":
            case "Lava Crab":
            case "Iridium Crab":
                __instance.parts.Clear();
                __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13747", __instance.numberToKill.Value));
                __instance.target.Value = "Demetrius";
                __instance.dialogueparts.Clear();
                __instance.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13750", __instance.monster.Value));
                break;
            case "Duggy":
                __instance.parts.Clear();
                __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13711", __instance.numberToKill.Value));
                __instance.target.Value = "Clint";
                break;
            default:
                __instance.parts.Clear();
                __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13752",
                    __instance.monster.Value, __instance.numberToKill.Value,
                    new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs." + ___random.Choose("13755", "13756", "13757"))));
                __instance.target.Value = "Wizard";
                __instance.dialogueparts.Clear();
                __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13760");
                break;
        }

        if (__instance.target.Value.Equals("Wizard") && !Utility.doesAnyFarmerHaveMail("wizardJunimoNote") && !Utility.doesAnyFarmerHaveMail("JojaMember"))
        {
            __instance.parts.Clear();
            __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13764",
                __instance.numberToKill.Value, __instance.monster.Value));
            __instance.target.Value = "Lewis";
            __instance.dialogueparts.Clear();
            __instance.dialogueparts.Add("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13767");
        }

        __instance.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", __instance.reward.Value));
        __instance.objective.Value =
            new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13770", "0", __instance.numberToKill.Value, __instance.monster.Value);

        return false;
    }

    private static void InitPossibleMonsters(List<string> possibleMonsters)
    {
        InitPossibleMineShaftMonsters(possibleMonsters);
        if (config.MoreSlayMonsterQuest)
        {
            InitPossibleSkullCavernMonsters(possibleMonsters);
            InitPossibleVolcanoDungeonMonsters(possibleMonsters);
        }
    }

    // 初始化矿井怪物清单
    private static void InitPossibleMineShaftMonsters(List<string> possibleMonsters)
    {
        var mineLevel = Utility.GetAllPlayerDeepestMineLevel();
        // 原版杀怪任务怪物清单
        switch (mineLevel)
        {
            case <= 40:
            {
                // 绿色史莱姆
                possibleMonsters.Add("Green Slime");
                // 岩石蟹
                if (mineLevel > 10) possibleMonsters.Add("Rock Crab");
                // 掘地虫
                if (mineLevel > 30) possibleMonsters.Add("Duggy");
                break;
            }
            case <= 80:
            {
                // 冰冻史莱姆
                possibleMonsters.Add("Frost Jelly");
                // 灰尘精灵
                possibleMonsters.Add("Dust Spirit");
                // 幽灵
                if (mineLevel > 50) possibleMonsters.Add("Ghost");
                // 骷髅
                if (mineLevel > 70) possibleMonsters.Add("Skeleton");
                break;
            }
            default:
                // 熔岩史莱姆
                possibleMonsters.Add("Sludge");
                // 熔岩蟹
                possibleMonsters.Add("Lava Crab");
                // 鱿鱼娃
                if (mineLevel > 90) possibleMonsters.Add("Squid Kid");
                break;
        }

        if (!config.MoreSlayMonsterQuest) return;

        // 模组杀怪任务怪物清单
        switch (mineLevel)
        {
            case <= 40:
            {
                // 臭虫
                possibleMonsters.Add("Bug");
                // 蛆 苍蝇
                if (mineLevel > 10) possibleMonsters.AddRange(new[] { "Grub", "Fly" });
                // 蝙蝠 石魔
                if (mineLevel > 30) possibleMonsters.AddRange(new[] { "Bat", "Stone Golem" });
                break;
            }
            case <= 80:
            {
                // 冰霜蝙蝠
                possibleMonsters.Add("Frost Bat");
                break;
            }
            default:
            {
                // 熔岩蝙蝠 暗影狂徒 暗影萨满 金属大头
                possibleMonsters.AddRange(new[] { "Lava Bat", "Shadow Brute", "Shadow Shaman", "Metal Head" });
                break;
            }
        }
    }

    // 初始化沙漠矿洞怪物清单 
    private static void InitPossibleSkullCavernMonsters(List<string> possibleMonsters)
    {
        var mineLevel = Utility.GetAllPlayerDeepestMineLevel();
        if (mineLevel <= 120) return;
        // 大史莱姆 木乃伊 飞蛇 石碳幽灵 霸王喷火龙
        possibleMonsters.AddRange(new[] { "Big Slime", "Mummy", "Serpent", "Carbon Ghost", "Pepper Rex" });
        // 铱蟹
        if (mineLevel > 145) possibleMonsters.Add("Iridium Crab");
        // 铱蝠
        if (mineLevel > 170) possibleMonsters.Add("Iridium Bat");
    }

    // 初始化火山怪物请单
    private static void InitPossibleVolcanoDungeonMonsters(List<string> possibleMonsters)
    {
    }
}