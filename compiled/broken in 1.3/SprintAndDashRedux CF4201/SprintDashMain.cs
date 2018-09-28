/*
    This is an adaption of the work of Maurício Gomes (OrSpeeder), who created
    the original Sprint and Dash Mod, and released it under the  GNU General 
    Public License. Sprint and Dash Mod Redux will also use it.

    Sprint and Dash Mod Redux is free software: you can redistribute it 
    and/or modify it under the terms of the GNU General Public License as 
    published by the Free Software Foundation, either version 3 of the 
    License, or (at your option) any later version.

    For information on this license, see: <http://www.gnu.org/licenses/>

    Sprint and Dash Mod Redux mod is distributed in the hope that it will be 
    useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
 */

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SprintAndDashRedux
{
    public class SprintDashMain : Mod
    {
        /*********
        ** Properties
        *********/
        private SprintDashConfig Config;

        /// <summary>The stamina cost per tick for sprinting.</summary>
        private float StamCost;

        /// <summary>Length of time combat dash lasts in milliseconds.</summary>
        private int DashDuration;

        /// <summary>2.5x duration.</summary>
        private int DashCooldown;

        /// <summary>Number of seconds of sprinting before player is winded, or 0 to disable windedness.</summary>
        private int WindedStep;

        /// <summary>Whether the player can get winded.</summary>
        private bool EnableWindedness;

        /// <summary>Whether we're operating the button as a toggle.</summary>
        private bool EnableToggle;

        /// <summary>How long player has been sprinting.</summary>
        private int SprintTime;

        /// <summary>How many "stages" of winding (+1 per windedStep seconds) the player has accumulated.</summary>
        private int StepsProgressed;

        /// <summary>Multiplier to <see cref="StamCost"/> based on windedness.</summary>
        private int WindedAccumulated;

        /// <summary>When winded but no longer sprinting, this governs how quickly windedness goes away.</summary>
        private int WindedCooldownStep;

        /// <summary>Whether the sprint function is toggled on.</summary>
        private bool SprintToggledOn;

        //Not really sure I understand why it was necessary to use these identifier ints rather than just comparing references to Buff objects, but these probably should be deprecated.
        private const int SprintBuffID = 58012395;
        private const int DashBuffID = 623165;
        private const int CooldownBuffID = 6890125;

        private Buff SprintBuff;
        private Buff DashBuff;
        private Buff CooldownBuff;

        /// <summary>Does nothing, exists to time cooldown of windedness.</summary>
        private Buff WindedBuff;

        private KeyboardState CurrentKeyboardState;

        private Keys[] RunKey;

        /// <summary>Don't need to cooldown for dash by default</summary>
        private bool NeedCooldown;

        /// <summary>The sprint time.</summary>
        private readonly int SprintBuffDuration = 1000;

        /// <summary>When to check to refresh buffs.</summary>
        private readonly int TimeoutCheck = 35;

        /// <summary>The current milliseconds left for a buff.</summary>
        private int CurrentTimeLeft;

        /// <summary>How long to refresh a status if relevant.</summary>
        private int RefreshTime;

        /// <summary>How little stamina player must have for sprint to refresh.</summary>
        private float MinStaminaToRefresh = 30f;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.SprintBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, "Sprinting", "Sprinting");
            this.CooldownBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -2, -2, 1, "Combat Dash Cooldown", "Combat Dash Cooldown");
            this.WindedBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "Winded", "Winded");

            // read config
            this.Config = helper.ReadConfig<SprintDashConfig>();
            this.StamCost = Math.Max(1, this.Config.StamCost);
            this.DashDuration = Math.Max(1, Math.Min(10, this.Config.DashDuration)) * 1000; // 1-10 seconds
            this.DashCooldown = (int)(this.DashDuration * 2.5);
            this.WindedStep = this.Config.WindedStep;
            if (this.WindedStep > 0)
            {
                this.EnableWindedness = true;
                this.WindedCooldownStep = this.WindedStep * 200;  //Recovering from winded-ness take 1/5 the time spent being winded.
                this.WindedStep *= 1000; // convert config-based times to ms
            }
            this.EnableToggle = this.Config.ToggleMode;
            this.RunKey = null;

            // hook events
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;

            // log info
            this.Monitor.Log($"Stamina cost: {this.StamCost}, dash duration: {this.DashDuration}, dash cooldown: {this.DashCooldown}, winded step: {this.WindedStep}, toggle mode: {this.EnableToggle}", LogLevel.Trace);
        }


        /*********
        ** Private methods
        *********/
        private void WindedTest()
        {
            this.Monitor.Log($"(Winded Status) sprint time: {this.SprintTime}, steps progressed: {this.StepsProgressed}, winded accumulated: {this.WindedAccumulated}");
        }

        /// <summary>Detect key press.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            // do nothing if the conditions aren't favorable
            if (!Game1.shouldTimePass() || Game1.player.isRidingHorse())
                return;

            Keys pressed = e.KeyPressed;

            // dashing is a time-limited thing, just do it on a press
            if (pressed == this.Config.DashKey && !this.NeedCooldown)
            {
                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff == this.CooldownBuff || buff == DashBuff)
                        return;
                }

                int speed = Game1.player.FarmingLevel / 2;
                int defense = Game1.player.ForagingLevel / 2 + Game1.player.FishingLevel / 3;
                int attack = Game1.player.CombatLevel;

                DashBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed, defense, attack, 1, "Combat Dash", "Combat Dash");
                DashBuff.millisecondsDuration = this.DashDuration;
                DashBuff.which = SprintDashMain.DashBuffID;
                DashBuff.glow = Color.AntiqueWhite;
                Game1.buffsDisplay.addOtherBuff(DashBuff);
                float staminaToConsume = speed + defense + attack + 10;
                float healthToConsume = 0f;
                if (staminaToConsume > Game1.player.Stamina)
                {
                    healthToConsume = staminaToConsume - Game1.player.Stamina;
                    staminaToConsume = Math.Max(Game1.player.Stamina - 1, 0f);
                }
                Game1.player.Stamina -= staminaToConsume;
                Game1.player.health = Math.Max(0, Game1.player.health - (int)healthToConsume);
                this.NeedCooldown = true;

                if (healthToConsume == 0)
                {
                    Game1.playSound("hoeHit");
                    Game1.playSound("hoeHit");
                    Game1.playSound("hoeHit");
                }
                else
                {
                    Game1.playSound("hoeHit");
                    Game1.playSound("ow");
                }

                Vector2 tileLocation = Game1.player.getTileLocation();
                Vector2 initialTile = Game1.player.getTileLocation();
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f));
                tileLocation.X -= 1;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f));
                tileLocation.X += 2;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f));
                tileLocation.X -= 1;
                tileLocation.Y -= 1;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f));
                tileLocation.Y += 2;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f));

                this.Monitor.Log($"Activating dash for {DashBuff.millisecondsDuration}ms with buff of +{speed} speed, +{defense} defense, +{attack} attack");
            }
            else if (pressed == this.Config.SprintKey && this.EnableToggle)
            {
                this.SprintToggledOn = !this.SprintToggledOn;

                //Re-enable autorun if sprinting because sprint-walking is, uh, dumb.
                if (!Game1.options.autoRun && this.SprintToggledOn)
                    Game1.options.autoRun = true;
            }
            //At this point, the run button is basically a toggle for autorun. Not sure if this is the best feature honestly but eh.
            else if (this.RunKey.Contains(pressed) && this.EnableToggle)
            {
                Game1.options.autoRun = !Game1.options.autoRun;

                //Disable sprinting if we're no longer running because sprint-walking is, uh, dumb.
                if (this.SprintToggledOn && !Game1.options.autoRun)
                    this.SprintToggledOn = false;
            }
        }

        //Do this every tick. Checks for persistent effects, does first-run stuff.
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            //This is complicated and necessary because SDV stores the run button as an array of buttons. Theoretically we may have more than one.
            if (this.RunKey == null)
            {
                this.RunKey = new Keys[Game1.options.runButton.Length];

                int i = 0;

                foreach (InputButton button in Game1.options.runButton)
                {
                    this.RunKey[i] = button.key;
                    i++;
                }
            }

            //Cancel toggled sprinting if on horseback
            if (Game1.player.isRidingHorse() && this.EnableToggle && this.SprintToggledOn)
                this.SprintToggledOn = false;

            //If time cannot pass we should return (desireable??)
            if (!Game1.shouldTimePass())
                return;

            //Apply the cooldown bluff if needed.
            if (this.NeedCooldown)
            {
                bool canAddCooldown = true;
                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff == this.CooldownBuff || buff == DashBuff)
                        canAddCooldown = false;
                }

                if (canAddCooldown)
                {
                    this.NeedCooldown = false;
                    this.CooldownBuff.millisecondsDuration = this.DashCooldown;
                    this.CooldownBuff.which = SprintDashMain.CooldownBuffID;
                    this.CooldownBuff.glow = Color.DarkRed;
                    Game1.buffsDisplay.addOtherBuff(this.CooldownBuff);

                    this.Monitor.Log($"Dash cooldown activated for {this.CooldownBuff.millisecondsDuration}ms.");
                }
            }

            //Apply sprint buff if needed.
            this.CurrentKeyboardState = Keyboard.GetState();
            if ((this.CurrentKeyboardState.IsKeyDown(this.Config.SprintKey) || this.SprintToggledOn) && !Game1.player.isRidingHorse())
            {
                if (this.SprintTime < 0)
                    this.SprintTime = 0;

                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff == SprintBuff)
                    {
                        this.CurrentTimeLeft = buff.millisecondsDuration;

                        if (this.CurrentTimeLeft <= this.TimeoutCheck)
                        {
                            this.RefreshTime = this.SprintBuffDuration - this.CurrentTimeLeft;
                            this.SprintTime += this.RefreshTime;

                            if (this.EnableWindedness)
                            {
                                if (this.SprintTime > this.WindedStep)
                                {
                                    this.StepsProgressed = (int)Math.Floor((double)(this.SprintTime / this.WindedStep));
                                    this.WindedAccumulated = (int)(this.StamCost * this.StepsProgressed);
                                }

                                this.WindedTest();
                            }
                            else
                                this.Monitor.Log("Refreshing sprint...");

                            //Only refresh duration if more than min stam remains.
                            if (Game1.player.stamina > this.MinStaminaToRefresh)
                                buff.millisecondsDuration += this.RefreshTime;

                            //These are checks so that, if somehow we end up with a total stamina cost greater than current stamina, we won't get a negative result. (Not sure if needed?)
                            if (Game1.player.stamina > (this.StamCost + this.WindedAccumulated))
                                Game1.player.stamina -= (this.StamCost + this.WindedAccumulated);
                            else
                                Game1.player.stamina = 0;
                        }
                        return;
                    }
                }

                //Only grant the buff if player has more than min stam
                if (Game1.player.stamina > this.MinStaminaToRefresh)
                {
                    this.Monitor.Log("Starting to sprint...");

                    SprintBuff.millisecondsDuration = this.SprintBuffDuration;
                    SprintBuff.which = SprintDashMain.SprintBuffID;
                    Game1.buffsDisplay.addOtherBuff(SprintBuff);
                    Game1.player.Stamina -= (this.StamCost + this.WindedAccumulated);
                }
            }
            else if (this.EnableWindedness && this.SprintTime > 0)
            {
                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff == WindedBuff)
                    {
                        this.CurrentTimeLeft = buff.millisecondsDuration;

                        if (this.CurrentTimeLeft <= this.TimeoutCheck)
                        {
                            this.RefreshTime = this.WindedCooldownStep - this.CurrentTimeLeft;

                            if (this.WindedAccumulated > 0)
                            {
                                this.StepsProgressed -= 1;
                                this.WindedAccumulated -= (int)this.StamCost;
                                buff.millisecondsDuration += this.RefreshTime;
                                this.SprintTime -= this.WindedStep;
                            }
                            else
                                this.SprintTime -= this.RefreshTime;

                            if (this.WindedAccumulated < 0)
                                this.WindedAccumulated = 0; // just in case

                            this.WindedTest();
                        }
                        return;
                    }
                }

                WindedBuff.millisecondsDuration = this.WindedCooldownStep;
                this.WindedBuff.glow = this.SprintTime > this.WindedStep
                    ? Color.Khaki
                    : Color.Transparent;
                Game1.buffsDisplay.addOtherBuff(WindedBuff);

                this.WindedTest();
            }
        }
    }
}
