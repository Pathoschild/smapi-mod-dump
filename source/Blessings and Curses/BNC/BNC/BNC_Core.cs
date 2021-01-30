/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Configs;
using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using BNC.TwitchAppIntergration;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using BNC.Actions;
using static BNC.BuffManager;
using StardewValley.Locations;
using BNC.Managers.Augments;
using BNC.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace BNC
{


    public class BNC_Core : Mod
    {

        public static Texture2D meteorTileSheet;

        public static SaveFile BNCSave = new SaveFile();
        public static readonly string saveFileName = "bncsave";
        public static IModHelper helper;
        public static IMonitor Logger;
        public static Config config = new Config();
        private static AppIntergration connection;

        public static ActionManager actionManager;
        public static AugmentManager augmentManager;

        public override void Entry(IModHelper helperIn)
        {
            helper = helperIn;
            Logger = this.Monitor;
            config = helper.ReadConfig<Config>();

            if (config.Enable_Twitch_Integration)
                TwitchIntergration.LoadConfig(helperIn);

            actionManager = new ActionManager();
            augmentManager = new AugmentManager();

            connection = new TwitchAppIntergration.AppIntergration("GenDeathrow_Stardew");
            connection.Start();

            BuffManager.Init();
            MineBuffManager.Init();
            Spawner.Init();
            augmentManager.Init();

            // read an image file
            meteorTileSheet = helper.Content.Load<Texture2D>("assets/Meteor.png", ContentSource.ModFolder);

            //helper.Events.Player.Warped += MineBuffManager.mineLevelChanged;
            helper.Events.GameLoop.UpdateTicked += this.updateTick;

            helper.Events.GameLoop.DayStarted += NewDayEvent;

            helper.Events.GameLoop.Saving += BeforeSaveEvent;
            helper.Events.GameLoop.Saved += SaveEvent;

            helper.Events.GameLoop.SaveLoaded += LoadEvent;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            //Debug button
            helper.Events.Input.ButtonPressed += this.InputEvents_ButtonPressed;
        }

        Spawner spawner = new Spawner();
        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            this.Monitor.Log(e.Button.ToString());
            if (!Game1.IsMasterGame) return;


            if (e.Button.Equals(SButton.J))
            {
                Gift gift = new Gift();

                gift.item = 421;

                gift.cnt = 1;

                gift.Handle();
            }

            if (e.Button.Equals(SButton.N))
            {
                //Game1.game1.parseDebugInput("minelevel 1");

                MeteorStorm storm = new MeteorStorm();
                storm.from = "test";
                storm.stormtype = "apocalyptic";
                storm.Handle();
            }
            if (e.Button.Equals(SButton.B))
            {
                for (int i = 0; i < 1; i++)
                {

                    //Game1.player.currentLocation.temporarySprites.Add(new CosmeticDebris(new Fence().fenceTexture.Value, new Vector2(Game1.player.getTileX() * 64f + 32f, Game1.player.getTileY() * 64f + 32f), (float)Game1.random.Next(-5, 5) / 100f, (float)Game1.random.Next(-64, 64) / 30f, (float)Game1.random.Next(-800, -100) / 100f, (int)((Game1.player.getTileY() + 1f) * 64f), new Rectangle(32 + Game1.random.Next(2) * 16 / 2, 96 + Game1.random.Next(2) * 16 / 2, 8, 8), Color.White, (Game1.soundBank != null) ? Game1.soundBank.GetCue("shiny4") : null, null, 0, 200));
                    //Meteor projectile = new Meteor(getRangeFromPlayer(1000));
                    //Meteor projectile = new Meteor(getRangeFromViewPort(700), Game1.random.Next(100, 600), Game1.player.currentLocation);
                    //projectile.height.Value = 24f;
                    //projectile.ignoreMeleeAttacks.Value = true;
                    //projectile.hostTimeUntilAttackable = 0.1f;
                    //Game1.player.currentLocation.projectiles.Add(projectile);
                    
                    FireballEvent storm = new FireballEvent();

                    storm.from = "test";
                    storm.stormtype = "insane";
                    actionManager._actionQueue.Enqueue(storm);

                    
                }


                //SpawnCat.tryMoveCats();
            }
        }


        private static Vector2 getRangeFromViewPort(int range, int minRange = 3)
        {

            int xStart = (int)Game1.viewport.X;
            int yStart = (int)Game1.viewport.Y;

            int randX = -Game1.random.Next(range * 2 + 2);
            int randY = -Game1.random.Next(range * 2 + 2);


            Vector2 vector = new Vector2(xStart + randX , yStart + randY);
            /*
            while (Vector2.Distance(vector, Game1.player.getTileLocation()) < minRange)
            {
                vector.X = xStart + (Game1.random.Next(range * 2 + 2));
                vector.Y = yStart + (Game1.random.Next(range * 2 + 2));
            }*/
            return vector;
        }

        private void OnReturnToTitle(object sender, EventArgs e)
        {
            BNCSave.clearData();
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            BNCSave.LoadModData(helper);
        }

        private void BeforeSaveEvent(object sender, EventArgs e)
        {
            Spawner.ClearMobs();
        }

        private void SaveEvent(object sender, EventArgs e) {
            BNCSave.SaveModData(helper);
        }

        private void NewDayEvent(object sender, EventArgs e) {
            if (!Context.IsWorldReady)
                return;
            if (BNC_Core.config.Random_Day_Buffs)
            {
                BuffManager.UpdateDay();
            }

            //Allow Weather to change again. 
            Weather.clearForNewDay();

            // Run Cat removeal
            //SpawnCat.tryRemoveCat();

        }

        private bool markedForDeath = false;
        private BaseAugment blood = new Tired();
        private void updateTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Make Death Events not as bad
            if (Game1.killScreen && markedForDeath == false){
                markedForDeath = true;
            }
            if (Game1.killScreen == false && markedForDeath && !Game1.eventUp && (Game1.player.currentLocation.name.Equals("Hospital") || Game1.player.currentLocation.name.Equals("Mine") || Game1.player.currentLocation is IslandLocation)) {
                BuffManager.buffPlayer(new BuffOption("rush", "Adrenaline", true, 100).add_attack(6).addShortDesc("Buff Attack +6").setGlow(Color.OrangeRed));
                BuffManager.buffPlayer(new BuffOption("pain", "Pain Medicine", true, 100).add_defense(3).addShortDesc("Buff Defense +3"));
                BuffManager.buffPlayer(new BuffOption("speed", "Fight or Flight", true, 100).add_speed(4).addShortDesc("Buff Speed +4").setGlow(Color.LightBlue));
                markedForDeath = false;
            }
            // End of Death Event

            if (e.IsMultipleOf(15))
            {
                BuffManager.UpdateTick();
                Spawner.UpdateTick();
                actionManager.Update();

                if (!Game1.player.currentLocation.name.Equals("Hospital"))
                    if (!IsBusyDoingSomething()) 
                        BombEvent.UpdateTick();

            }

            if (e.IsOneSecond)
            {
                //MineBuffManager.UpdateTick();
                augmentManager.UpdateTick();
                BuffManager.Update();

                if (!Game1.player.currentLocation.name.Equals("Hospital") || Game1.player.currentLocation.Name.Equals("FarmHouse"))
                    if (!IsBusyDoingSomething() &&
                        !Game1.paused &&
                        !Game1.menuUp &&
                        !Game1.isTimePaused &&
                        Game1.shouldTimePass())
                    {
                        MeteorStorm.UpdateTick();
                        FireballEvent.UpdateTick();
                    }
                        
            }
        }

        public bool IsBusyDoingSomething()
        {
            if (Game1.eventUp)
            {
                return true;
            }
            if (Game1.fadeToBlack)
            {
                return true;
            }
            if (Game1.currentMinigame != null)
            {
                return true;
            }
            if (Game1.isWarping)
            {
                return true;
            }
             if (Game1.killScreen)
            {
                return true;
            }
            if (!Game1.player.CanMove)
            {
                return false;
            }
            return false;
        }

    }
}

