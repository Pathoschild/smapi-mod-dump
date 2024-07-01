/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Netcode;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class ItemDeliveryQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static readonly Random Random = new();

    private static List<string> possibleItems = new();
    private static string universalGiftTaste = "";
    private static string npcGiftTaste = "";

    public ItemDeliveryQuestPatcher(ModConfig config)
    {
        ItemDeliveryQuestPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.loadQuestInfo)),
            transpiler: this.GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
        );
        harmony.Patch(this.RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.GetGoldRewardPerItem)),
            postfix: this.GetHarmonyMethod(nameof(GetGoldRewardPerItemPostfix))
        );
        harmony.Patch(this.RequireMethod<ItemDeliveryQuest>(nameof(ItemDeliveryQuest.checkIfComplete)),
            transpiler: this.GetHarmonyMethod(nameof(CheckIfCompleteTranspiler))
        );
    }

    // 交易任务友谊奖励修改
    private static IEnumerable<CodeInstruction> CheckIfCompleteTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4 && code.operand.Equals(150));
        codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemDeliveryQuestPatcher), nameof(GetItemDeliveryFriendshipGain)));
        return codes.AsEnumerable();
    }

    // 交易任务金钱奖励修改
    private static void GetGoldRewardPerItemPostfix(ref int __result)
    {
        __result = (int)(__result * config.ItemDeliveryRewardMultiplier);
    }

    private static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        // 随机作物逻辑
        var index = codes.FindIndex(code => code.opcode == OpCodes.Call && code.operand.Equals(AccessTools.Method(typeof(Utility), nameof(Utility.possibleCropsAtThisTime))));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_0));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.target))));
        codes.Insert(index + 3, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetString), nameof(NetString.Get))));
        codes.Insert(index + 4, new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetRandomCrops))));

        // 随机任务物品逻辑
        index = codes.FindIndex(index,
            code => code.opcode == OpCodes.Call && code.operand.Equals(AccessTools.Method(typeof(Utility), nameof(Utility.getRandomItemFromSeason),
                new[] { typeof(Season), typeof(int), typeof(bool), typeof(bool) })));
        codes[index - 3] = new CodeInstruction(OpCodes.Ldarg_0);
        codes[index - 2] = new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemDeliveryQuest), nameof(ItemDeliveryQuest.target)));
        codes[index - 1] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetString), nameof(NetString.Get)));
        codes[index].operand = GetMethod(nameof(GetRandomItem));

        // 不知道为什么,但不这么做会报错
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_R8 && code.operand.Equals(0.33));
        codes[index].operand = -0.1;

        return codes.AsEnumerable();
    }

    private static List<string> GetRandomCrops(List<string> possibleCrops, string npcName)
    {
        InitNPCGiftTaste(npcName);
        var giftTaste = universalGiftTaste + " " + npcGiftTaste;
        var temp = new List<string>(possibleCrops);
        temp = temp.Where(crop => giftTaste.Contains(crop)).ToList();

        return temp.Any() ? temp : possibleCrops;
    }

    private static string GetRandomItem(Season season, string npcName)
    {
        if (config.UseModPossibleItems)
            InitModPossibleItems(npcName);
        else
        {
            InitVanillaPossibleItems(season);
            InitNPCGiftTaste(npcName);
        }

        var result = Random.ChooseFrom(possibleItems);
        possibleItems = possibleItems.Where(IsItemAvailable).ToList();

        return possibleItems.Any() ? Random.ChooseFrom(possibleItems) : result;
    }

    public static void Init()
    {
        possibleItems.Clear();
        InitUniversalGiftTaste();
    }

    private static void InitVanillaPossibleItems(Season season)
    {
        if (possibleItems.Any()) return;

        possibleItems.AddRange(new[] { "68", "66", "78", "80", "86", "152", "167", "153", "420" });
        var allUnlockedCraftingRecipes = Utility.GetAllPlayerUnlockedCraftingRecipes();
        var allUnlockedCookingRecipes = Utility.GetAllPlayerUnlockedCookingRecipes();

        if (MineShaft.lowestLevelReached > 40 || Utility.GetAllPlayerReachedBottomOfMines() >= 1)
        {
            possibleItems.AddRange(new[] { "62", "70", "72", "84", "422" });
        }

        if (MineShaft.lowestLevelReached > 80 || Utility.GetAllPlayerReachedBottomOfMines() >= 1)
        {
            possibleItems.AddRange(new[] { "64", "60", "82" });
        }

        if (Utility.doesAnyFarmerHaveMail("ccVault"))
        {
            possibleItems.AddRange(new[] { "88", "90", "164", "165" });
        }

        if (allUnlockedCraftingRecipes.Contains("Furnace"))
        {
            possibleItems.AddRange(new[] { "334", "335", "336", "338" });
        }

        if (allUnlockedCraftingRecipes.Contains("Quartz Globe"))
        {
            possibleItems.Add("339");
        }

        switch (season)
        {
            case Season.Spring:
                possibleItems.AddRange(new[]
                {
                    "16", "18", "20", "22", "129", "131", "132", "136", "137", "142",
                    "143", "145", "147", "148", "152", "167", "267"
                });
                break;
            case Season.Summer:
                possibleItems.AddRange(new[]
                {
                    "128", "130", "132", "136", "138", "142", "144", "145", "146", "149",
                    "150", "155", "396", "398", "402", "267"
                });
                break;
            case Season.Fall:
                possibleItems.AddRange(new[]
                {
                    "404", "406", "408", "410", "129", "131", "132", "136", "137", "139",
                    "140", "142", "143", "148", "150", "154", "155", "269"
                });
                break;
            case Season.Winter:
                possibleItems.AddRange(new[]
                {
                    "412", "414", "416", "418", "130", "131", "132", "136", "140", "141",
                    "144", "146", "147", "150", "151", "154", "269"
                });
                break;
        }


        foreach (var recipe in allUnlockedCookingRecipes)
        {
            if (Random.NextDouble() < 0.4)
            {
                continue;
            }

            var cropsAvailableNow = Utility.possibleCropsAtThisTime(Game1.season, Game1.dayOfMonth <= 7);
            if (!DataLoader.CookingRecipes(Game1.content).TryGetValue(recipe, out var rawCraftingData))
            {
                continue;
            }

            var fields = rawCraftingData.Split('/');
            var ingredientsSplit = ArgUtility.SplitBySpace(ArgUtility.Get(fields, 0));
            var ingredientsAvailable = true;
            foreach (var ingredients in ingredientsSplit)
            {
                if (!possibleItems.Contains(ingredients) && !IsCategoryAvailable(ingredients) &&
                    (cropsAvailableNow == null || !cropsAvailableNow.Contains(ingredients)))
                {
                    ingredientsAvailable = false;
                    break;
                }
            }

            if (ingredientsAvailable)
            {
                var itemId = ArgUtility.Get(fields, 2);
                if (itemId != null)
                {
                    possibleItems.Add(itemId);
                }
            }
        }
    }

    private static void InitModPossibleItems(string npcName)
    {
        possibleItems.Clear();
        InitNPCGiftTaste(npcName);
        var giftTaste = universalGiftTaste + " " + npcGiftTaste;
        possibleItems.AddRange(ArgUtility.SplitBySpace(giftTaste));
    }

    private static void InitUniversalGiftTaste()
    {
        universalGiftTaste = "";
        universalGiftTaste += Game1.NPCGiftTastes["Universal_Love"];
        if (config.QuestItemRequirement > 0) universalGiftTaste += " " + Game1.NPCGiftTastes["Universal_Like"];
        if (config.QuestItemRequirement > 1) universalGiftTaste += " " + Game1.NPCGiftTastes["Universal_Neutral"];
        if (config.QuestItemRequirement > 2) universalGiftTaste += " " + Game1.NPCGiftTastes["Universal_Dislike"];
        if (config.QuestItemRequirement > 3) universalGiftTaste += " " + Game1.NPCGiftTastes["Universal_Hate"];
    }

    private static void InitNPCGiftTaste(string npcName)
    {
        npcGiftTaste = "";
        if (!Game1.NPCGiftTastes.TryGetValue(npcName, out var data)) return;
        var split = data.Split('/');
        if (split.Length < 10) return;
        npcGiftTaste += split[1];
        if (config.QuestItemRequirement > 0) npcGiftTaste += " " + split[3];
        if (config.QuestItemRequirement > 1) npcGiftTaste += " " + split[5];
        if (config.QuestItemRequirement > 2) npcGiftTaste += " " + split[7];
        if (config.QuestItemRequirement > 3) npcGiftTaste += " " + split[9];
    }

    private static bool IsItemAvailable(string itemId)
    {
        var isGiftTaste = config.UseModPossibleItems || (universalGiftTaste.Contains(itemId) && npcGiftTaste.Contains(itemId));

        if (itemId is "-5" or "-6" && isGiftTaste) return true;

        if (!ItemRegistry.Exists(itemId))
        {
            Log.Trace($"ItemId: {itemId} doesn't exist.");
            return false;
        }

        var item = new SObject(itemId, 1);

        return isGiftTaste &&
               (config.MaxPrice <= 0 || item.Price <= config.MaxPrice) &&
               (config.AllowArtisanGoods || item.Category != SObject.artisanGoodsCategory);
    }

    private static bool IsCategoryAvailable(string category)
    {
        return category.StartsWith('-') && category != "-5" && category != "-6";
    }

    private static int GetItemDeliveryFriendshipGain()
    {
        return config.ItemDeliveryFriendshipGain;
    }

    private static MethodInfo GetMethod(string name)
    {
        return AccessTools.Method(typeof(ItemDeliveryQuestPatcher), name) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(ItemDeliveryQuestPatcher), name)}.");
    }
}