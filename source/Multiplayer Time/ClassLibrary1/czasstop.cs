
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StardewValley.Events;

namespace MultiplayerTime
{
    public class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.F3;
        public bool Active { get; set; } = true;
    }
    class MultiplayerTimeMod : Mod
    {
        /*********
       ** Public methods
       *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        int timeinterval=-1;
        int orginal;
        bool difference;
        Color textColor=Game1.textColor;
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = (ModConfig)helper.ReadConfig<ModConfig>();
            if (this.Config.Active)
            {
                Helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
                Helper.Events.Display.RenderingHud += this.PreRenderHud;
                Helper.Events.Display.RenderedHud += this.PostRenderHud;
                Helper.Events.Display.Rendered += this.RenderClock;
                Helper.Events.GameLoop.Saved += this.Statement;
            }
            Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.ButtonPressed);
            Helper.Events.GameLoop.Saving += this.Save;
            Helper.Events.GameLoop.Saved += this.Save;
        }
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button== this.Config.ActivationKey)
            {
                if (this.Config.Active)
                {
                    this.Config.Active = false;
                    Helper.Events.GameLoop.UpdateTicked -= this.GameEvents_UpdateTick;
                    Helper.Events.Display.RenderingHud -= this.PreRenderHud;
                    Helper.Events.Display.RenderedHud -= this.PostRenderHud;
                    Helper.Events.Display.Rendered -= this.RenderClock;
                    this.Helper.WriteConfig(Config);
                    if (Context.IsWorldReady)
                    {
                        Game1.chatBox.addMessage("Multiplayer time Mod turned off", Color.White);
                        if (difference)
                        {

                            orginal = Game1.player.MagneticRadius;
                        }
                        else
                        {
                            orginal = orginal + Game1.player.MagneticRadius;
                        }
                        Game1.player.MagneticRadius = orginal;
                        difference = true;
                    }
                }
                else
                {
                    this.Config.Active = true;
                    Helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
                    Helper.Events.Display.RenderingHud += this.PreRenderHud;
                    Helper.Events.Display.RenderedHud += this.PostRenderHud;
                    Helper.Events.Display.Rendered += this.RenderClock;
                    this.Helper.WriteConfig(Config);
                    if (Context.IsWorldReady)
                    {
                        Game1.chatBox.addMessage("Multiplayer time Mod turned on", Color.White);
                        difference = true;
                        timeinterval = -1;
                    }
                }
            }
        }
        private void Statement(object sender, EventArgs e)
        {
            if (this.Config.Active)
            {
                Game1.chatBox.addMessage("Multiplayer time Mod turned on", Color.White);
            }
            else
            {
                Game1.chatBox.addMessage("Multiplayer time Mod turned off", Color.White);
            }
        }
        private void Save(object sender, EventArgs e)
        {
            Game1.player.MagneticRadius = 128;
            if (Game1.player.rightRing != null && Game1.player.rightRing.Value != null && Game1.player.rightRing.Value.indexInTileSheet == 518)
            {
                Game1.player.MagneticRadius += 64;
            }
            if (Game1.player.leftRing != null && Game1.player.leftRing.Value != null && Game1.player.leftRing.Value.indexInTileSheet == 518)
            {
                Game1.player.MagneticRadius += 64;
            }
            if (Game1.player.rightRing != null && Game1.player.rightRing.Value != null && Game1.player.rightRing.Value.indexInTileSheet == 519)
            {
                Game1.player.MagneticRadius += 128;
            }
            if (Game1.player.leftRing != null && Game1.player.leftRing.Value != null && Game1.player.leftRing.Value.indexInTileSheet == 519)
            {
                Game1.player.MagneticRadius += 128;
            }
            if (Game1.player.rightRing != null && Game1.player.rightRing.Value != null && Game1.player.rightRing.Value.indexInTileSheet == 527)
            {
                Game1.player.MagneticRadius += 128;
            }
            if (Game1.player.leftRing != null && Game1.player.leftRing.Value != null && Game1.player.leftRing.Value.indexInTileSheet == 527)
            {
                Game1.player.MagneticRadius += 128;
            }
            difference = true;
        }
        private void PreRenderHud(object sender, EventArgs e)
        {
            if (!shouldTimePass())
            {
                Game1.textColor *= 0f;
                Game1.dayTimeMoneyBox.timeShakeTimer = 0;
            }
        }
        private void PostRenderHud(object sender, EventArgs e)
        {
            if (!shouldTimePass())
            {
                drawfade(Game1.spriteBatch);
            }
            Game1.textColor = textColor;
        }
        private void RenderClock(object sender, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (!Game1.isFestival() && !(Game1.farmEvent!=null && (Game1.farmEvent is FairyEvent || Game1.farmEvent is WitchEvent || Game1.farmEvent is SoundInTheNightEvent)) && (Game1.eventUp || Game1.currentMinigame != null || Game1.activeClickableMenu is AnimalQueryMenu || Game1.activeClickableMenu is PurchaseAnimalsMenu || Game1.activeClickableMenu is CarpenterMenu || Game1.freezeControls))
                {
                    Game1.textColor *= 0f;
                    Game1.dayTimeMoneyBox.timeShakeTimer = 0;
                    Game1.spriteBatch.End();
                    Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                    Game1.dayTimeMoneyBox.draw(Game1.spriteBatch);
                    Game1.textColor = textColor;
                    drawfade(Game1.spriteBatch);
                }
            }
        }
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (difference)
                {
                    orginal = Game1.player.MagneticRadius;
                }
                else
                {
                    orginal = orginal + Game1.player.MagneticRadius;
                }
                if (!Context.IsPlayerFree || (Game1.currentMinigame != null && Game1.currentMinigame.minigameId() == "PrairieKing") || Game1.player.isEating || Game1.freezeControls)
                {
                    Game1.player.MagneticRadius = 0;
                    difference = false;
                }
                if ((Context.IsPlayerFree && !Game1.player.isEating && Game1.currentMinigame==null && !Game1.freezeControls) || Game1.activeClickableMenu is BobberBar)
                {
                    Game1.player.MagneticRadius = orginal;
                    difference = true;
                }
                if (Context.IsMainPlayer && !shouldTimePass())
                {
                    foreach (GameLocation location in Game1.locations)
                    {
                        foreach(Character NPCs in location.characters)
                        {
                            if(NPCs is NPC)
                            {
                                (NPCs as NPC).movementPause = 1;
                            }
                        }
                    }
                    foreach(Farmer gracz in Game1.getOnlineFarmers())
                    {
                        if (gracz.currentLocation.currentEvent != null)
                        {
                            if(gracz.currentLocation is Farm)
                            {
                                foreach (FarmAnimal animal in (gracz.currentLocation as Farm).getAllFarmAnimals())
                                {
                                    animal.pauseTimer = 100;
                                }
                            }
                            if (gracz.currentLocation is AnimalHouse)
                            {
                                foreach (FarmAnimal animal in (gracz.currentLocation as AnimalHouse).animals.Values)
                                {
                                    animal.pauseTimer = 100;
                                }
                            }
                            foreach(Character Monsters in gracz.currentLocation.characters)
                            {
                                if (Monsters !=null && Monsters is NPC && Monsters is Monster)
                                {
                                    if(!(Monsters is Bug))
                                    {
                                        (Monsters as Monster).Halt();
                                    }
                                    if(Monsters is Bat || Monsters is Ghost || Monsters is DustSpirit)
                                    {
                                        (Monsters as Monster).xVelocity = 0f;
                                        (Monsters as Monster).yVelocity = 0f;
                                        if(Monsters is DustSpirit)
                                        {
                                            (Monsters as Monster).yJumpVelocity = 0f;
                                        }
                                    }
                                    if(Monsters is GreenSlime || Monsters is SquidKid)
                                    {
                                        (Monsters as Monster).moveTowardPlayer(0);
                                    }
                                    if(Monsters is Fly || Monsters is Serpent)
                                    {
                                        (Monsters as Monster).setInvincibleCountdown(100);
                                        (Monsters as Monster).stopGlowing();
                                    }
                                    (Monsters as Monster).Speed = 0;
                                }
                            }
                        }
                    }
                }
                if(Context.IsMainPlayer && shouldTimePass())
                {
                    foreach (Farmer gracz in Game1.getOnlineFarmers())
                    {
                        foreach (Character Monsters in gracz.currentLocation.characters)
                        {
                            if (Monsters is NPC && Monsters is Monster && (Monsters as Monster).Speed==0 )
                            {
                                (Monsters as Monster).Speed=Convert.ToInt32(monsterInfo((Monsters as Monster).Name)[10]);
                                if(Monsters is GreenSlime || Monsters is SquidKid)
                                {
                                    (Monsters as Monster).moveTowardPlayer(Convert.ToInt32(monsterInfo((Monsters as Monster).Name)[9]));
                                }
                            }
                        }
                    }
                }
                if (Game1.currentLocation != null)
                {
                    for (int k = Game1.currentLocation.TemporarySprites.Count - 1; k >= 0; k--)
                    {
                        if (Game1.currentLocation.TemporarySprites[k].bombRadius > 0 && shouldTimePass())
                        {
                            Game1.currentLocation.TemporarySprites[k].paused = false;
                        }
                        if (Game1.currentLocation.TemporarySprites[k].bombRadius > 0 && !shouldTimePass())
                        {
                            Game1.currentLocation.TemporarySprites[k].paused = true;
                        }
                    }
                }
                if (Context.IsMainPlayer)
                {
                    if (shouldTimePass())
                    {
                        timeinterval = -1;
                        return;
                    }
                    if (timeinterval == -1)
                    {
                        if (Game1.gameTimeInterval > 6800)
                        {
                            timeinterval = 6800;
                        }
                        else
                        {
                            timeinterval = Game1.gameTimeInterval;
                        }
                    }
                    else
                    {
                        Game1.gameTimeInterval = timeinterval;
                    }
                }
                return;
            }
        }
        private string[] monsterInfo(string name) {
            return Game1.content.Load<Dictionary<string, string>>("Data\\Monsters")[name].Split(new char[]
            {
                '/'
            });
        }
        private bool shouldTimePass()
        {
            foreach (Farmer gracz in Game1.getOnlineFarmers())
            {
                if (gracz.MagneticRadius != 0)
                {
                    return true;
                }
            }
            return false;
        }
        private void drawfade(SpriteBatch b)
        {
            string text = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) + ". " + Game1.dayOfMonth;
            Vector2 dayPosition = new Vector2((float)Math.Floor(183.5f - Game1.dialogueFont.MeasureString(text).X / 2), (float)18);
            b.DrawString(Game1.dialogueFont, text, Game1.dayTimeMoneyBox.position + dayPosition, textColor);
            string timeofDay = (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400) ? " am" : " pm";
            string zeroPad = (Game1.timeOfDay % 100 == 0) ? "0" : "";
            string hours = (Game1.timeOfDay / 100 % 12 == 0) ? "12" : string.Concat(Game1.timeOfDay / 100 % 12);
            string time = string.Concat(new object[]
            {
                hours,
                ":",
                Game1.timeOfDay % 100,
                zeroPad,
                timeofDay
            });
            Vector2 timePosition = new Vector2((float)Math.Floor(183.5 - Game1.dialogueFont.MeasureString(time).X/2), (float)108);
            bool nofade = shouldTimePass() || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 > 1000.0;
            b.DrawString(Game1.dialogueFont, time, Game1.dayTimeMoneyBox.position + timePosition, (Game1.timeOfDay >= 2400) ? Color.Red : (textColor * (nofade ? 1f : 0.5f)));
        }
    }
}
