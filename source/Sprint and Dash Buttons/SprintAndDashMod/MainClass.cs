/*
    Copyright 2016 Maurício Gomes (Speeder)

    Sprint and Dash mod is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Sprint and Dash mod is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Sprint and Dash mod. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SprintAndDashMod
{   
    public class MainModClass : Mod
    {
        const int kSprintBuff = 58012395;
        const int kDashBuff = 623165;
        const int kDashCDBuff = 6890125;

        public static SprintAndDashConfig ModConfig { get; protected set; }

        public static bool needCdBuff = false;

        public override void Entry(params object[] objects)
        {
            ModConfig = new SprintAndDashConfig().InitializeConfig(BaseConfigPath);
            GameEvents.UpdateTick += UpdateTickEvent;
        }

        static void UpdateTickEvent(object sender, EventArgs e)
        {
            if(!Game1.player.canMove || Game1.activeClickableMenu != null)
            {
                return;
            }

            if(needCdBuff)
            {
                bool canAddCDBuff = true;
                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff.which == kDashCDBuff || buff.which == kDashBuff)
                    {
                        canAddCDBuff = false;
                    }
                }

                if(canAddCDBuff)
                {
                    needCdBuff = false;
                    Buff cdnbuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -2, -2, 1, "Combat Dash Cooldown");
                    cdnbuff.millisecondsDuration = 5000;
                    cdnbuff.which = kDashCDBuff;
                    cdnbuff.glow = Color.DarkRed;
                    Game1.buffsDisplay.addOtherBuff(cdnbuff);
                }                
            }

            KeyboardState currentKeyboardState = Keyboard.GetState();
            if(currentKeyboardState.IsKeyDown(ModConfig.sprintKey) && Game1.player.stamina > 30f)
            {
                foreach(Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if(buff.which == kSprintBuff)
                    {
                        if(buff.millisecondsDuration <= 35)
                        {
                            buff.millisecondsDuration = 1000;
                            Game1.player.Stamina -= 1f;
                        }                        
                        return;
                    }                    
                }

                Buff nbuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, "Sprinting");
                nbuff.millisecondsDuration = 1000;
                nbuff.which = kSprintBuff;
                Game1.buffsDisplay.addOtherBuff(nbuff);
                Game1.player.Stamina -= 1f;
            }

            if (currentKeyboardState.IsKeyDown(ModConfig.dashKey) && !needCdBuff)
            {
                foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff.which == kDashCDBuff || buff.which == kDashBuff)
                    {
                        return;
                    }
                }

                int speed = Game1.player.FarmingLevel / 2;
                int defense = Game1.player.ForagingLevel / 2 + Game1.player.FishingLevel / 3;
                int attack = Game1.player.CombatLevel;
                Buff nbuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed, defense, attack, 1, "Combat Dash");
                nbuff.millisecondsDuration = 2000;
                nbuff.which = kDashBuff;
                nbuff.glow = Color.AntiqueWhite;
                Game1.buffsDisplay.addOtherBuff(nbuff);
                float staminaToConsume = speed + defense + attack + 10;
                float healthToConsume = 0f;
                if(staminaToConsume > Game1.player.Stamina)
                {
                    healthToConsume = staminaToConsume - Game1.player.Stamina;
                    staminaToConsume = Math.Max(Game1.player.Stamina - 1, 0f);
                }
                Game1.player.Stamina -= staminaToConsume;
                Game1.player.health = Math.Max(0, Game1.player.health - (int)healthToConsume);
                needCdBuff = true;

                if(healthToConsume == 0)
                {
                    //Game1.playSound("tinyWhip");
                    //Game1.playSound("toolCharge");                    
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
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f, 0, -1, -1f, -1, 0));
                tileLocation.X -= 1;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f, 0, -1, -1f, -1, 0));
                tileLocation.X += 2;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f, 0, -1, -1f, -1, 0));
                tileLocation.X -= 1;
                tileLocation.Y -= 1;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f, 0, -1, -1f, -1, 0));
                tileLocation.Y += 2;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(initialTile, tileLocation) * 30f, 0, -1, -1f, -1, 0));
            }
        }
    }

    public class SprintAndDashConfig : Config
    {
        public Keys sprintKey;
        public Keys dashKey;

        public override T GenerateDefaultConfig<T>()
        {
            sprintKey = Keys.LeftControl;
            dashKey = Keys.N;
            return this as T;
        }
    }
}
