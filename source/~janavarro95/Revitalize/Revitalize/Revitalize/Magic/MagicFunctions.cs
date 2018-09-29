using Microsoft.Xna.Framework;
using Revitalize.Objects;
using Revitalize.Persistence;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Magic.MagicFunctions
{

    public class UtilitySpells
    {
        public static void warpHome(Spell s)
        {
            const int baseCost = 5;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;

            for (int i = 0; i < 12; i++)
            {
                Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float)Game1.random.Next(25, 75), 6, 1, new Vector2((float)Game1.random.Next((int)Game1.player.position.X - Game1.tileSize * 4, (int)Game1.player.position.X + Game1.tileSize * 3), (float)Game1.random.Next((int)Game1.player.position.Y - Game1.tileSize * 4, (int)Game1.player.position.Y + Game1.tileSize * 3)), false, Game1.random.NextDouble() < 0.5));
            }
            Game1.playSound("wand");


            int num = 0;
            for (int j = Game1.player.getTileX() + 8; j >= Game1.player.getTileX() - 8; j--)
            {
                Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float)j, (float)Game1.player.getTileY()) * (float)Game1.tileSize, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
                {
                    layerDepth = 1f,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0f)
                });
                num++;
            }

            Game1.warpFarmer("Farm", 64, 16, false);
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }

        public static void warpHorse(Spell s)
        {
         NPC horse= Utility.findHorse();
         if (horse == null) return;
            const int baseCost = 20;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
            Game1.warpCharacter(horse, Game1.player.currentLocation.name, Game1.player.getTileLocationPoint(), false, true);
        }


        public static void sunnyWeather(Spell s)
        {
            const int baseCost = 35;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;

            if (Game1.weatherForTomorrow != Game1.weather_sunny)
            {
                Game1.weatherForTomorrow = Game1.weather_sunny;
                MagicMonitor.consumeMagic(cost);
                MagicMonitor.consumeUsage(s);
            }
            else
            {
                Game1.showGlobalMessage("It's already going to be sunny tomorrow.");
            }
           
        }

        public static void rainyWeather(Spell s)
        {
            const int baseCost = 35;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;

            if (Game1.weatherForTomorrow != Game1.weather_rain)
            {
                Game1.weatherForTomorrow = Game1.weather_rain;
                MagicMonitor.consumeMagic(cost);
                MagicMonitor.consumeUsage(s);
            }
            else
            {
                Game1.showGlobalMessage("It's already going to be rainy tomorrow.");
            }
            
        }

        public static void stormyWeather(Spell s)
        {
            const int baseCost = 35;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;

            if (Game1.weatherForTomorrow != Game1.weather_lightning)
            {
                Game1.weatherForTomorrow = Game1.weather_lightning;
                MagicMonitor.consumeMagic(cost);
                MagicMonitor.consumeUsage(s);
            }
            else
            {
                Game1.showGlobalMessage("It's already going to be stormy tomorrow.");
            }
          
        }
    }

    public class TestingSpells
    {
        public static void showRedMessage(Spell s)
        {
            if (Class1.mouseAction == true) return; //necessary for non repeating functions/spells
            const int baseCost = 5;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;
            Game1.showRedMessage("MAGIC WORKS");
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }

       
    }

    public class PlayerSpecificSpells
    {
        /// <summary>
        /// Restore 10% of health.
        /// </summary>
        /// <param name="s"></param>
        public static void firstAide(Spell s)
        {
            if (Game1.player.health >= Game1.player.maxHealth) return;
            const int baseCost = 10;
            int healAmount = (int)(Game1.player.maxHealth * .10f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.health += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);

        }
        /// <summary>
        /// Restore 30% of health.
        /// </summary>
        /// <param name="s"></param>
        public static void heal(Spell s)
        {
            if (Game1.player.health >= Game1.player.maxHealth) return;
            const int baseCost = 30;
            int healAmount = (int)(Game1.player.maxHealth * .30f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.health += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);

        }
        /// <summary>
        /// Restore 50% of max health
        /// </summary>
        /// <param name="s"></param>
        public static void cure(Spell s)
        {
            if (Game1.player.health >= Game1.player.maxHealth) return;
            const int baseCost = 50;
            int healAmount = (int)(Game1.player.maxHealth * .50f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.health += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
        /// <summary>
        /// Restore 70% of health.
        /// </summary>
        /// <param name="s"></param>
        public static void mend(Spell s)
        {
            if (Game1.player.health >= Game1.player.maxHealth) return;
            const int baseCost = 70;
            int healAmount = (int)(Game1.player.maxHealth * .70f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.health += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
        /// <summary>
        /// Restore 10% of stamina.
        /// </summary>
        /// <param name="s"></param>
        public static void deepBreaths(Spell s)
        {
            if (Game1.player.stamina >= Game1.player.maxStamina) return;
            const int baseCost = 10;
            int healAmount = (int)(Game1.player.maxStamina * .10f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.stamina += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);

        }
        /// <summary>
        /// Restore 30% of stamina.
        /// </summary>
        /// <param name="s"></param>
        public static void refresh(Spell s)
        {
            if (Game1.player.stamina >= Game1.player.maxStamina) return;
            const int baseCost = 30;
            int healAmount = (int)(Game1.player.maxStamina * .30f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.stamina += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
        /// <summary>
        /// Restore 50% of stamina.
        /// </summary>
        /// <param name="s"></param>
        public static void replenish(Spell s)
        {
            if (Game1.player.stamina >= Game1.player.maxStamina) return;
            const int baseCost = 50;
            int healAmount = (int)(Game1.player.maxStamina * .50f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.stamina += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
        /// <summary>
        /// Restore 70% of stamina.
        /// </summary>
        /// <param name="s"></param>
        public static void rejuvinate(Spell s)
        {
            if (Game1.player.stamina >= Game1.player.maxStamina) return;
            const int baseCost = 70;
            int healAmount = (int)(Game1.player.maxStamina * .70f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.stamina += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
        /// <summary>
        /// Restore 100% of stamina.
        /// </summary>
        /// <param name="s"></param>
        public static void revitalize(Spell s)
        {
            if (Game1.player.stamina >= Game1.player.maxStamina) return;
            const int baseCost = 100;
            int healAmount = (int)(Game1.player.maxStamina * 1f);
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            Game1.player.stamina += healAmount;
            MagicMonitor.consumeMagic(cost);
            MagicMonitor.consumeUsage(s);
        }
    }

   public class CropSpells
    {
        public static void cropGrowthSpell(Spell s)
        {
            if (Class1.mouseAction == true) return; //necessary for non repeating functions/spells
            const int baseCost = 50;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;
                                                        
            
          if(Game1.player.currentLocation.isCropAtTile((int)Game1.currentCursorTile.X,(int)Game1.currentCursorTile.Y))
            {
                try
                {
                    TerrainFeature t;
                  bool f=  Game1.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out t);
                    (t as HoeDirt).crop.dayOfCurrentPhase++;
                    if ((t as HoeDirt).crop.dayOfCurrentPhase >= (t as HoeDirt).crop.phaseDays[(t as HoeDirt).crop.currentPhase])
                    {
                        (t as HoeDirt).crop.currentPhase++;
                        (t as HoeDirt).crop.dayOfCurrentPhase = 0;

                    }
                    MagicMonitor.consumeMagic(cost);
                    MagicMonitor.consumeUsage(s);
                }
                catch (Exception e)
                {
                }
            }
          else
            {
            }
            
        }
        public static void waterCropSpell(Spell s)
        {
           // if (Class1.mouseAction == true) return; //necessary for non repeating functions/spells
            const int baseCost = 5;
            //calculate all costs then factor in player proficiency
            int cost = (int)(((((baseCost) + s.spellCostModifierInt) * s.spellCostModifierPercent) * (1f - PlayerVariables.MagicProficiency)) - PlayerVariables.MagicLevel); //+s.extraCostInt-s.spellSaveInt * (1f- PlayerVariables.MagicProficiency) / s.extraCostPercent* s.spellSavePercent;


            if (Game1.player.currentLocation.isCropAtTile((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y))
            {
                try
                {
                    TerrainFeature t;
                    bool f = Game1.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out t);
               
                    if ((t as HoeDirt).state==0)
                    {
                        (t as HoeDirt).state = 1;
                        Game1.playSound("slosh");
                        DelayedAction.playSoundAfterDelay("glug", 250);
                        MagicMonitor.consumeMagic(cost);
                        MagicMonitor.consumeUsage(s);
                    }
                }
                catch (Exception e)
                {
                    //Log.AsyncR("BAD Water SPELL");
                }
            }
            else
            {
                //Log.AsyncC(Game1.currentCursorTile);
                //Log.AsyncC("Cant water here ");
            }

        }
    }
}
