/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omegasis.HappyBirthday.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Monsters;
using Omegasis.HappyBirthday.Framework.ContentPack;
using Omegasis.HappyBirthday.Framework.Utilities;
using Omegasis.HappyBirthday.Framework.Configs;
using Omegasis.HappyBirthday.Framework.Menus;
using Omegasis.HappyBirthday.Framework.Events;
using Omegasis.HappyBirthday.Framework.Gifts;
using Omegasis.StardustCore.Events;

namespace Omegasis.HappyBirthday
{
    /// <summary>The mod entry point.</summary>
    public class HappyBirthdayModCore : Mod
    {
        /*********
        ** Fields
        *********/

        /// <summary>
        /// Manages all of the configs for Happy Birthday.
        /// </summary>
        public static ConfigManager Configs;

        /// <summary>Class to handle all birthday messages for this mod.</summary>
        public BirthdayMessages birthdayMessages;

        /// <summary>Class to handle all birthday gifts for this mod.</summary>
        public GiftManager giftManager;

        public static HappyBirthdayModCore Instance;

        public HappyBirthdayContentPackManager happyBirthdayContentPackManager;

        /// <summary>Handles different translations of files.</summary>
        public TranslationInfo translationInfo;

        /// <summary>
        /// Utilities for checking if it's a player's birthday, seeing if npcs have given birthday wishes already, etc.
        /// </summary>
        public BirthdayManager birthdayManager;

        public bool contentPacksInitalized;

        public IStardewAccessApi screenreader;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            Instance = this;
            Configs = new ConfigManager();
            Configs.initializeConfigs();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;

            this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            this.Helper.Events.GameLoop.DayEnding += this.OnDayEnded;

            this.Helper.Events.GameLoop.SaveCreated += this.GameLoop_SaveCreated;

            this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.Helper.Events.GameLoop.Saving += this.OnSaving;

            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            this.Helper.Events.Display.MenuChanged += MenuUtilities.OnMenuChanged;

            this.Helper.Events.Display.RenderedActiveMenu += RenderUtilities.OnRenderedActiveMenu;
            this.Helper.Events.Display.RenderedHud += RenderUtilities.OnRenderedHud;

            this.Helper.Events.Multiplayer.ModMessageReceived += MultiplayerUtilities.Multiplayer_ModMessageReceived;
            this.Helper.Events.Multiplayer.PeerDisconnected += MultiplayerUtilities.Multiplayer_PeerDisconnected;

            this.Helper.Events.Player.Warped += BirthdayEventUtilities.Player_Warped;

            this.Helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;

            this.Helper.Events.Content.AssetRequested += this.Content_AssetRequested;

            this.birthdayManager = new BirthdayManager();

            this.happyBirthdayContentPackManager = new HappyBirthdayContentPackManager();

            this.translationInfo = new TranslationInfo();

            LocalizedContentManager.OnLanguageChange += this.LocalizedContentManager_OnLanguageChange;

        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.BaseName.Equals(@"Data/mail"))
            {
                e.Edit(MailUtilities.EditMailAsset);
            }
        }

        private void GameLoop_SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            this.initalizeHappyBirthdayContent();
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.birthdayManager.reset();
        }

        public override object GetApi()
        {
            return new HappyBirthday.Framework.API.HappyBirthdayAPI();
        }


        /*********
        ** Private methods
        *********/

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            BirthdayEventUtilities.BirthdayEventManager = new EventManager();

            this.birthdayMessages = new BirthdayMessages();
            this.giftManager = new GiftManager();
            MenuUtilities.IsDailyQuestBoard = false;

            BirthdayEventUtilities.InitializeBirthdayEventCommands();

            screenreader = Helper.ModRegistry.GetApi<IStardewAccessApi>("shoaib.stardewaccess");

        }

        private void LocalizedContentManager_OnLanguageChange(LocalizedContentManager.LanguageCode code)
        {
            //Reload the birthday gifts since they are local to the content packs.
            this.giftManager.reloadBirthdayGifts();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {

            this.initalizeHappyBirthdayContent();

            SaveManager.OnDayStarted(sender, e);
            this.birthdayManager.onDayStarted(sender, e);

            BirthdayEventUtilities.ClearEventsFromFarmer();
            BirthdayEventUtilities.OnDayStarted();
        }

        private void OnDayEnded(object sender, DayEndingEventArgs e)
        {
            SaveManager.OnDayEnded(sender, e);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // show birthday selection menu
            if (Game1.activeClickableMenu != null) return;
            if (Context.IsPlayerFree && !this.birthdayManager.hasChosenBirthday() && e.Button == Configs.modConfig.KeyBinding)
                Game1.activeClickableMenu = new BirthdayMenu(this.birthdayManager.playerBirthdayData.BirthdaySeason, this.birthdayManager.playerBirthdayData.BirthdayDay, this.birthdayManager.setBirthday);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.initalizeHappyBirthdayContent();
        }

        protected virtual void initalizeHappyBirthdayContent()
        {
            if (this.contentPacksInitalized) return;
            if (this.Helper.ContentPacks.GetOwned().Count() == 0)
            {
                throw new InvalidDataException("There are ZERO Happy birthday content packs found for the mod. Without at least one installed there is no guaranteed that this mod will work due to missing dialogue errors. Please install at least one HappyBirthdayContent pack before continuing. One can be found at https://www.nexusmods.com/stardewvalley/mods/11148 for English dialogue. Thank you!");
            }

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.happyBirthdayContentPackManager.registerNewContentPack(contentPack);
            }
            this.giftManager.addInGiftsFromLoadedContentPacks();
            MailUtilities.RemoveAllBirthdayMail();

            BirthdayEventUtilities.InitializeBirthdayEvents();
            this.contentPacksInitalized = true;
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            //SaveManager.Save(Game1.player.uniqueMultiplayerID);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {

            if (Game1.CurrentEvent != null)
            {
                BirthdayEventUtilities.UpdateEventManager();
                return;
            }

            if (!Context.IsWorldReady || Game1.isFestival())
            {
            }


            //Below code sets up menus for selecting the new birthday for the player.


            if (!this.birthdayManager.hasCheckedForBirthday() && Game1.activeClickableMenu == null)
            {
                this.birthdayManager.setCheckedForBirthday(true);

                this.birthdayManager.setUpPlayersBirthday();
            }

        }

        public void SayWithMenuChecker(string text, bool interrupt)
        {
            if (this.screenreader != null)
                this.screenreader.SayWithMenuChecker(text, interrupt);
        }
    }
}
