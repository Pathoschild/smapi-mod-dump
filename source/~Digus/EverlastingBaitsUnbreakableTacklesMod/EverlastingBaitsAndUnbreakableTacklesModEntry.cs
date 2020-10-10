/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class EverlastingBaitsAndUnbreakableTacklesModEntry : Mod
    {
        public static IMonitor ModMonitor;
        internal static DataLoader DataLoader;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.ConsoleCommands.Add("player_addallbaitstacklesrecipes", "Add all everlasting baits and unbreakable tackles recipes to the player.", Commands.AddAllBaitTackleRecipes);
            helper.ConsoleCommands.Add("player_getallbaitstackles", "Get all everlasting baits and unbreakable tackles.", Commands.GetAllBaitTackle);
            helper.ConsoleCommands.Add("player_addquestfortackle", "Adds a quest for the specified unbreakable tackles.\n\nUsage: player_addsQuestForTackle <value>\n- value: name of the tackle. ex. Unbreakable Trap Bobber", Commands.AddsQuestForTackle);
            helper.ConsoleCommands.Add("player_removeblankquests", "Remove all blank quests from player log.", Commands.RemoveBlankQuests);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(Helper);

            var harmony = HarmonyInstance.Create("Digus.InfiniteBaitAndLureMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
                prefix: new HarmonyMethod(typeof(GameOverrides), nameof(GameOverrides.DoDoneFishing))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.createItem)),
                prefix: new HarmonyMethod(typeof(GameOverrides), nameof(GameOverrides.CreateItem))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
                prefix: new HarmonyMethod(typeof(GameOverrides), nameof(GameOverrides.ClickCraftingRecipe))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(GameOverrides), nameof(GameOverrides.TryToReceiveActiveObject))
            );


            if (!DataLoader.ModConfig.DisableIridiumQualityFish)
            {
                harmony.Patch(
                    original: AccessTools.Constructor(typeof(BobberBar), new[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
                    postfix: new HarmonyMethod(typeof(GameOverrides), nameof(GameOverrides.BobberBar))
                );
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            DataLoader.ReloadQuestWhenClient();
        }
    }
}
