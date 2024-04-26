/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using MoonShared;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buffs;
using StardewValley.BellsAndWhistles;
using BirbCore.Attributes;
using Object = StardewValley.Object;
using ArchaeologySkill.Objects.Water_Shifter;

namespace ArchaeologySkill
{
    public class Utilities
    {
        /// <summary>
        /// Apply the basic Archaeology skill items. Give who the exp, check to see if they have the gold rush profession, and spawn bonus loot
        /// </summary>
        /// <param name="who"> The player</param>
        /// <param name="bonusLoot"> Do they get bonus loot or not</param>
        /// <param name="Object"> the int that the bonus loot should be</param>
        /// <param name="xLocation">the player's x location</param>
        /// <param name="yLocation">the player's y location</param>
        /// <param name="panning">is the effect from panning, since bonus loot works differently there.</param>
        /// <param name="exactItem">What bonus item if it passes the checks do you want to give the player</param>
        public static void ApplyArchaeologySkill(Farmer who, int EXP, bool bonusLoot = false, int xLocation = 0, int yLocation = 0, bool panning = false, string exactItem = "")
        {
            var farmer = Game1.getFarmer(who.UniqueMultiplayerID);

            //Give the player EXP
            BirbCore.Attributes.Log.Trace("Archaeology Skll: Adding EXP to the player");

            AddEXP(farmer, EXP);

            //If the player has the gold rush profession, give them a speed buff
            BirbCore.Attributes.Log.Trace("Archaeology Skll: Does the player have gold rusher?");
            if (farmer.HasCustomProfession(Archaeology_Skill.Archaeology10b2))
            {
                BirbCore.Attributes.Log.Trace("Archaeology Skll: The player does have gold rusher!");
                ApplySpeedBoost(farmer);
            } else
            {
                BirbCore.Attributes.Log.Trace("Archaeology Skll: the player does not have gold rusher");
            }

            //Check to see if the player wins the double loot chance roll if they are not panning.
            if (!panning)
            {
                BirbCore.Attributes.Log.Trace("Archaeology Skll: Does the player get bonus loot?");
                double doubleLootChance = GetLevel(farmer) * 0.05;
                double diceRoll = Game1.random.NextDouble();
                bool didTheyWin = (diceRoll < doubleLootChance);
                BirbCore.Attributes.Log.Trace("Archaeology Skll: The dice roll is... " + diceRoll.ToString() + ". The player's chance is... " + doubleLootChance.ToString() + ". ");
                if (didTheyWin || bonusLoot)
                {
                    BirbCore.Attributes.Log.Trace("Archaeology Skill: They do get bonus loot!");
                    string objectID = exactItem != "" ? exactItem : ModEntry.BonusLootTable.RandomChoose(Game1.random, "390");
                    Game1.createMultipleObjectDebris(objectID, xLocation, yLocation, 1, farmer.UniqueMultiplayerID);
                }
                else
                {
                    BirbCore.Attributes.Log.Trace("Archaeology Skill: They do not get bonus loot!");
                }
            }
        }

        //For the goldrush profession
        public static bool ApplySpeedBoost(Farmer who)
        {
            //Get the player
            var player = Game1.getFarmer(who.UniqueMultiplayerID);
            //check to see the player who is doing the request is the same one as this player. 
            if (player != Game1.player)
                return false;

            Buff buff = new(
                id: "Archaeology:profession:haste",
                displayName: ModEntry.Instance.I18N.Get("Archaeology10b2.buff"), // can optionally specify description text too
                iconTexture: ModEntry.Assets.Gold_Rush_Buff,
                iconSheetIndex: 0,
                duration: 6_000*GetLevel(player), // 60 seconds by default. Can go higher with buffs.
                effects: new BuffEffects()
                {
                    Speed = { 3 } // shortcut for buff.Speed.Value = 5
                }
            );
            //Check to see if the player already has the haste buff. if so, don't refresh it and return false.
            if (player.hasBuff(buff.id))
                return false;


            //Apply the buff make sure we have it have a custon name.
            player.applyBuff(buff);

            //get the player's tile positon as a vector2
            Vector2 tile = new(
                x: (int)(player.position.X / Game1.tileSize),
                y: (int)(player.position.Y / Game1.tileSize)
            );

            //Play a sound to give feedback that this profession is working
            player.currentLocation.localSound("debuffHit", tile);
            return true;
        }

        public static void AddEXP(StardewValley.Farmer who, int amount)
        {
            SpaceCore.Skills.AddExperience(Game1.getFarmer(who.UniqueMultiplayerID), "moonslime.Archaeology", amount);
        }

        public static int GetLevel(StardewValley.Farmer who)
        {
            var player = Game1.getFarmer(who.UniqueMultiplayerID);
            return SpaceCore.Skills.GetSkillLevel(player, "moonslime.Archaeology") + SpaceCore.Skills.GetSkillBuffLevel(player, "moonslime.Archaeology");
        }

    }
}
