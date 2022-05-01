/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using GenericModConfigMenu;
using Helpers;


namespace TemplateMod
{
    public interface IEasierMonsterEradicationApi
    {
        /// <summary>Return the modified monster eradication goal value. returns -1 if the passed monster could not be identified.</summary>
        /// <param name="nameOfMonster">You pass the generic monster name as indentified by the game code.
        /// "Slimes", "DustSprites", "Bats", "Serpent", "VoidSpirits", "MagmaSprite", "CaveInsects", "Mummies", "RockCrabs", "Skeletons", "PepperRex", "Duggies".
        /// You can also pass specific game monster names like "Green Slime" if that is more convenient.
        /// </param>
        public int GetMonsterGoal(string nameOfMonster);

    }

    public class ModEntry : Mod
    {
        public ModConfig Config;
        private Logger Log;

        internal IModHelper MyHelper;

        public String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;
            Log = new Logger(Monitor);

            MyHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            MyHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            MyHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after the game has loaded and all Mods are loaded. Here we load the config.json file and setup GMCM </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {

        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Config = MyHelper.ReadConfig<ModConfig>();

            MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            //MyHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            var api = MyHelper.ModRegistry.GetApi<IEasierMonsterEradicationApi>("NormanPCN.EasierMonsterEradication");
            if (api != null)
                Log.Debug($"GetApi Slimes={api.GetMonsterGoal("Slimes")}");
            else
                Log.Debug("GetApi EasierMonsterEradicationApi == null");

            object rapi = MyHelper.ModRegistry.GetApi("NormanPCN.EasierMonsterEradication");
            if (api != null)
                Log.Debug($"Relection Slimes={MyHelper.Reflection.GetMethod(rapi, "GetMonsterGoal").Invoke<int>("Slimes")}");
            else
                Log.Debug("Reflection EasierMonsterEradicationApi == null");
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            //MyHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.
        /// This method implements...
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (
                (e.Button == SButton.F5) &&
                Context.IsPlayerFree
               )
            {
            }
        }

        /// <summary>Raised just after the game state is updated (≈60 times per second).
        /// This method implements facing direction change correction.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicked(object sender, EventArgs e)
        {
        }
    }
}

