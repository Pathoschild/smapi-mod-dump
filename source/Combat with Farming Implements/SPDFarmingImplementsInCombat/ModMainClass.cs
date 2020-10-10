/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/speeder1/farming-implements-combat
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPDFarmingImplementsInCombat
{
    public class ModMainClass : Mod
    {
        const int kScytheBuff = 571921611;
        const int kPickaxeBuff = 19257915;
        const int kAxeBuff = 258016;

        public override void Entry(params object[] objects)
        {
            GameEvents.UpdateTick += OnUpdateTick;
        }

        static void OnUpdateTick(object sender, EventArgs e)
        {
            if(Game1.activeClickableMenu == null && Game1.CurrentEvent == null && Game1.gameMode == Game1.playingGameMode)
            {
                if(Game1.player.CurrentTool != null)
                {
                    if(Game1.player.CurrentTool.name == "Scythe") //seriously, string comparison is bad :( but the game give me no choice here
                    {
                        //Log.Debug("we are holding a scythe");
                        MeleeWeapon scythe = Game1.player.CurrentTool as MeleeWeapon;
                        //scythe.addedAreaOfEffect = Game1.tileSize*2;                        
                        /*scythe.minDamage = Game1.player.FarmingLevel*2 + (int)Math.Ceiling(Math.Log((Game1.stats.cropsShipped+200)/200)*100);
                        scythe.maxDamage = (int)(scythe.minDamage*1.5);*/
                        foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                        {
                            if (buff.which == kScytheBuff || buff.which == kPickaxeBuff || buff.which == kAxeBuff) return;
                        }
                        //game damage formula is: basedamage+weapon*critwhatnot + buff*5 thus if we want to add 5 of damage, the buff value must be 1
                        //farming level attack bonus: from 0 to 25
                        //grass in silos attack bonus: logarithmic, from 0, about +50 damage with 8 full silos.
                        //scythe-based crops: logarithmic, from 0, about +50 damage if you harvested 2000 crops with scythe
                        int scytheCropShipped = 0;
                        if (Game1.player.basicShipped.ContainsKey(300))
                        {
                            scytheCropShipped += Game1.player.basicShipped[300];
                        }
                        if (Game1.player.basicShipped.ContainsKey(250))
                        {
                            scytheCropShipped += Game1.player.basicShipped[250];
                        }
                        if (Game1.player.basicShipped.ContainsKey(262))
                        {
                            scytheCropShipped += Game1.player.basicShipped[262];
                        }
                        int bonusAttack = Game1.player.FarmingLevel/2 + (int)Math.Ceiling(Math.Log(((Game1.getLocationFromName("Farm") as Farm).piecesOfHay + 200) / 200) * 10) + (int)Math.Ceiling(Math.Log((scytheCropShipped + 200) / 200) * 10);
                        Buff scytheBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, bonusAttack, 1, "Scythe Usage Experience");
                        scytheBuff.millisecondsDuration = 200;
                        scytheBuff.which = kScytheBuff;
                        Game1.buffsDisplay.addOtherBuff(scytheBuff);
                    }
                    else if(Game1.player.CurrentTool is Pickaxe)
                    {
                        //Log.Debug("we are holding a pickaxe");
                        foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                        {
                            if (buff.which == kScytheBuff || buff.which == kPickaxeBuff || buff.which == kAxeBuff) return;
                        }
                        //game damage formula is: basedamage+weapon*critwhatnot + buff*5 thus if we want to add 5 of damage, the buff value must be 1
                        //mining level attack bonus: from 0 to 25
                        //minerals found: 1 damage for each new mineral found (or being more precise, +5 for each 5 minerals found)
                        //rocks smashed: logarithmic, from 0, about +25 damage if you harvested 2000 rocks with pickaxe
                        int bonusAttack = Game1.player.MiningLevel / 2 + Game1.player.mineralsFound.Count/5 + (int)Math.Ceiling(Math.Log((Game1.stats.rocksCrushed + 200) / 200) * 5);
                        Buff pickaxeBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, bonusAttack, 1, "Pickaxe Usage Experience");
                        pickaxeBuff.millisecondsDuration = 200;
                        pickaxeBuff.which = kPickaxeBuff;
                        Game1.buffsDisplay.addOtherBuff(pickaxeBuff);
                    }
                    else if (Game1.player.CurrentTool is Axe)
                    {
                        //Log.Debug("we are holding an axe");
                        foreach (Buff buff in Game1.buffsDisplay.otherBuffs)
                        {
                            if (buff.which == kScytheBuff || buff.which == kPickaxeBuff || buff.which == kAxeBuff) return;
                        }
                        //game damage formula is: basedamage+weapon*critwhatnot + buff*5 thus if we want to add 5 of damage, the buff value must be 1
                        //foraging level attack bonus: from 0 to 50                       
                        //stumps cut: logarithmic, from 0, about +100 damage if you harvested 2000 stumps with an axe
                        int bonusAttack = Game1.player.ForagingLevel + (int)Math.Ceiling(Math.Log((Game1.stats.stumpsChopped + 200) / 200) * 20);
                        Buff axeBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, bonusAttack, 1, "Axe Usage Experience");
                        axeBuff.millisecondsDuration = 200;
                        axeBuff.which = kAxeBuff;
                        Game1.buffsDisplay.addOtherBuff(axeBuff);
                    }
                }
            }
        }
    }
}
