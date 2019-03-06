using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class EverlastingBaitsAndUnbreakableTacklesModEntery : Mod
    {
        public static IMonitor ModMonitor;
        internal static DataLoader DataLoader;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            DataLoader = new DataLoader(helper);

            helper.Events.GameLoop.SaveLoaded += (sender, e) => DataLoader.ReloadQuestWhenClient();

            var harmony = HarmonyInstance.Create("Digus.InfiniteBaitAndLureMod");

            var fishingRodDoDoneFishing =
                typeof(FishingRod).GetMethod("doDoneFishing", BindingFlags.NonPublic | BindingFlags.Instance);
            var fishingRodOverridesDoDoneFishing = typeof(GameOverrides).GetMethod("DoDoneFishing");
            harmony.Patch(fishingRodDoDoneFishing, new HarmonyMethod(fishingRodOverridesDoDoneFishing), null);

            var craftingRecipeCreateItem = typeof(CraftingRecipe).GetMethod("createItem");
            var fishingRodOverridesCreateItem = typeof(GameOverrides).GetMethod("CreateItem");
            harmony.Patch(craftingRecipeCreateItem, new HarmonyMethod(fishingRodOverridesCreateItem), null);

            var craftingPageClickCraftingRecipe = typeof(CraftingPage).GetMethod("clickCraftingRecipe",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var gameOverridesClickCraftingRecipe = typeof(GameOverrides).GetMethod("ClickCraftingRecipe");
            harmony.Patch(craftingPageClickCraftingRecipe, new HarmonyMethod(gameOverridesClickCraftingRecipe), null);

            var npcTryToReceiveActiveObject = typeof(NPC).GetMethod("tryToReceiveActiveObject");
            var gameOverridesTryToReceiveActiveObject = typeof(GameOverrides).GetMethod("TryToReceiveActiveObject");
            harmony.Patch(npcTryToReceiveActiveObject, new HarmonyMethod(gameOverridesTryToReceiveActiveObject), null);

            if (!DataLoader.ModConfig.DisableIridiumQualityFish)
            {
                var bobberBarConstructor = typeof(BobberBar).GetConstructor(new []{ typeof(int), typeof(float), typeof(bool), typeof(int)});
                var gameOverridesBobberBar = typeof(GameOverrides).GetMethod("BobberBar");
                harmony.Patch(bobberBarConstructor, null, new HarmonyMethod(gameOverridesBobberBar));
            }

            helper.ConsoleCommands.Add("player_addallbaitstacklesrecipes",
                "Add all everlasting baits and unbreakable tackles recipes to the player.", Commands.AddAllBaitTackleRecipes);
            helper.ConsoleCommands.Add("player_getallbaitstackles", "Get all everlasting baits and unbreakable tackles.", Commands.GetAllBaitTackle);
            helper.ConsoleCommands.Add("player_addquestfortackle", "Adds a quest for the specified unbreakable tackles.\n\nUsage: player_addsQuestForTackle <value>\n- value: name of the tackle. ex. Unbreakable Trap Bobber", Commands.AddsQuestForTackle);
            helper.ConsoleCommands.Add("player_removeblankquests", "Remove all blank quests from player log.", Commands.RemoveBlankQuests);
        }

        
    }
}
