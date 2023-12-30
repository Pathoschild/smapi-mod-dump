/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Challenge
{
    public class Tyrannus : ChallengeHandle
    {
        public bool modifiedSandDragon;
        public Reaper bossMonster;
        public Vector2 bossTile;
        public bool adjustWarp;

        public Tyrannus(Vector2 target, Rite rite, Quest quest)
          : base(target, rite, quest)
        {
            targetVector = target;
            voicePosition = new(targetVector.X * 64f, (targetVector.Y * 64f) - 32f);//Vector2.op_Addition(Vector2.op_Multiply(targetVector, 64f), new Vector2(0.0f, -32f));
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120.0;
        }

        public override void EventTrigger()
        {
            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Weald", 1f, 1f);

            ModUtility.AnimateRockfalls(targetLocation, targetPlayer.getTileLocation());

            DelayedAction.functionAfterDelay(RockfallSounds, 575);

            DelayedAction.functionAfterDelay(RockfallSounds, 675);

            DelayedAction.functionAfterDelay(RockfallSounds, 775);

            Mod.instance.RegisterEvent(this, "active");
        }

        public void RockfallSounds()
        {
            targetLocation.playSoundPitched(new Random().Next(2) == 0 ? "boulderBreak" : "boulderCrack", 800, 0);
        }

        public override bool EventActive()
        {
            if (targetPlayer.currentLocation == targetLocation && !eventAbort)
            {
                double totalSeconds = Game1.currentGameTime.TotalGameTime.TotalSeconds;
                if (expireTime < totalSeconds || expireEarly)
                    return EventExpire();
                int num = (int)Math.Round(expireTime - totalSeconds);
                if (activeCounter != 0 && num % 10 == 0 && num != 0)
                    Game1.addHUDMessage(new HUDMessage(string.Format("{0} more minutes left!", num), "2"));
                return true;
            }
            EventAbort();
            return false;
        }

        public override void RemoveMonsters()
        {
            if (bossMonster != null)
            {
                targetLocation.characters.Remove(bossMonster);
                bossMonster = null;
            }
            base.RemoveMonsters();
        }

        public override void EventRemove()
        {
            base.EventRemove();
            if (!(targetLocation is MineShaft))
                return;
            Vector2 ladderTile = bossTile;
            for (int index1 = 0; index1 < targetLocation.map.GetLayer("Buildings").LayerHeight; ++index1)
            {
                for (int index2 = 0; index2 < targetLocation.map.GetLayer("Buildings").LayerWidth; ++index2)
                {
                    if (targetLocation.map.GetLayer("Buildings").Tiles[index2, index1] != null && targetLocation.map.GetLayer("Buildings").Tiles[index2, index1].TileIndex == 115)
                    {
                        // ISSUE: explicit constructor call
                        //((Vector2)ref bossTile).\u002Ector((float)(index2 + 1), (float)(index1 + 1));
                        ladderTile = new(index2 + 1, index1 + 1);
                    }
                }
            }
            Layer layer = targetLocation.map.GetLayer("Buildings");
            layer.Tiles[(int)ladderTile.X, (int)ladderTile.Y] = new StaticTile(layer, targetLocation.map.TileSheets[0], 0, 174);
            Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int)ladderTile.X * 64, (int)ladderTile.Y * 64, 64, 64));
            Mod.instance.CastMessage("A way down has appeared");
        }

        public override bool EventExpire()
        {
            if (eventLinger == -1)
            {
                RemoveMonsters();
                eventLinger = 4;
                return true;
            }
            if (eventLinger == 3)
            {
                if (expireEarly)
                {
                    if (!questData.name.Contains("Two"))
                        new Throw().ThrowSword(Game1.player, 57, bossTile, 500);
                    Mod.instance.CompleteQuest(questData.name);
                    if (Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                        Mod.instance.dialogue["Jester"].specialDialogue.Add("quests", new List<string>()
            {
              "Jester of Fate:^Thank you for helping me put Thanatoshi to rest.",
              "I'm sorry about your kinsman.",
              "I think this cutlass is to blame"
            });
                }
                else
                {
                    Mod.instance.CastMessage("Try again tomorrow");
                    Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi... why...", -1, 2, 3000, 0);
                }
            }
            return base.EventExpire();
        }

        public override void EventInterval()
        {
            ++activeCounter;
            if (eventLinger != -1 || activeCounter == 1)
                return;
            if (activeCounter == 2)
            {
                AddTomb();
                targetVector = new Vector2(13f, 18f);
                Game1.inMine = true;
                Game1.warpFarmer("UndergroundMine145", 13, 19, 2);
                Game1.xLocationAfterWarp = 13;
                Game1.yLocationAfterWarp = 19;
                voicePosition = new(targetVector.X * 64, (targetVector.Y * 64) - 32f);//Vector2.op_Addition(Vector2.op_Multiply(targetVector, 64f), new Vector2(0.0f, -32f));
            }
            else if (activeCounter == 3)
            {
                targetPlayer.Position = new(targetVector.X * 64, targetVector.Y * 64);//Vector2.op_Multiply(targetVector, 64f);
                bossMonster = MonsterData.CreateMonster(17, new Vector2(13f, 9f), riteData.combatModifier) as Reaper;
                if (questData.name.Contains("Two"))
                    bossMonster.HardMode();
                targetLocation.characters.Add(bossMonster);
                bossMonster.currentLocation = riteData.castLocation;
                bossMonster.update(Game1.currentGameTime, riteData.castLocation);
                SetTrack("LavaMine");
                bossTile = new Vector2(13f, 9f);
            }
            else
            {
                if (activeCounter == 5 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("What a moment...", -1, 2, 3000, 0);
                if (activeCounter == 10 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi?", -1, 3, 3000, 0);
                if (activeCounter == 15 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("Farmer, it's him, The Reaper of Fate", -1, 3, 3000, 0);
                if (activeCounter == 20 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi, stop messing around!", -1, 3, 3000, 0);
                if (activeCounter == 25 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("I am a Fate too you know, the Jester?", -1, 2, 3000, 0);
                if (activeCounter == 30 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("It's no use, he's insane", -1, 2, 3000, 0);
                if (activeCounter == 35 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("That's... a cutlass... on the shaft", -1, 2, 3000, 0);
                if (activeCounter == 40 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("What has he done to himself?", -1, 2, 3000, 0);
                if (activeCounter == 40 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("I guess we have no choice...", -1, 2, 3000, 0);
                if (activeCounter == 50 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                    Mod.instance.characters["Jester"].showTextAboveHead("For Fate and Fortune!", -1, 3, 3000, 0);
                if (bossMonster.defeated || bossMonster.Health <= 0 || bossMonster == null || !targetLocation.characters.Contains(bossMonster))
                    expireEarly = true;
                else
                    bossTile = bossMonster.getTileLocation();
            }
        }

        public void AddTomb()
        {
            MineShaft mineShaft = new MineShaft(145);
            MineShaft.activeMines.Clear();
            MineShaft.activeMines.Add(mineShaft);
            mineShaft.mapPath.Value = "Maps\\Mines\\33";
            mineShaft.loadedMapNumber = 33;
            mineShaft.updateMap();
            mineShaft.mapImageSource.Value = "Maps\\Mines\\mine_desert_dark_dangerous";
            mineShaft.Map.TileSheets[0].ImageSource = "Maps\\Mines\\mine_desert_dark_dangerous";
            mineShaft.Map.LoadTileSheets(Game1.mapDisplayDevice);
            mineShaft.mineLevel = 100;
            mineShaft.chooseLevelType();
            mineShaft.mineLevel = 145;
            mineShaft.findLadder();
            targetLocation = Game1.getLocationFromName("UndergroundMine145");
            Layer layer1 = targetLocation.map.GetLayer("Back");
            Layer layer2 = targetLocation.map.GetLayer("Buildings");
            Layer layer3 = targetLocation.map.GetLayer("Front");
            TileSheet tileSheet1 = new TileSheet("zestfordragontiles99999999", targetLocation.map, Path.Combine("Maps", "DesertTiles"), new Size(16, 23), new Size(1, 1));
            targetLocation.map.AddTileSheet(tileSheet1);
            TileSheet tileSheet2 = targetLocation.map.TileSheets[0];
            layer1.Tiles[15, 11] = new StaticTile(layer1, tileSheet2, 0, 166);
            layer1.Tiles[16, 11] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[17, 11] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[18, 11] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[19, 11] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[20, 11] = new StaticTile(layer1, tileSheet2, 0, 168);
            layer1.Tiles[11, 12] = new StaticTile(layer1, tileSheet2, 0, 166);
            layer1.Tiles[12, 12] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[13, 12] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[14, 12] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[15, 12] = new StaticTile(layer1, tileSheet2, 0, 152);
            layer1.Tiles[16, 12] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[17, 12] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[18, 12] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[19, 12] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[20, 12] = new StaticTile(layer1, tileSheet2, 0, 184);
            layer1.Tiles[8, 13] = new StaticTile(layer1, tileSheet2, 0, 166);
            layer1.Tiles[9, 13] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[10, 13] = new StaticTile(layer1, tileSheet2, 0, 167);
            layer1.Tiles[11, 13] = new StaticTile(layer1, tileSheet2, 0, 152);
            layer1.Tiles[12, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[13, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[14, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[15, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[16, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[17, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[18, 13] = new StaticTile(layer1, tileSheet2, 0, 181);
            layer1.Tiles[19, 13] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[20, 13] = new StaticTile(layer1, tileSheet2, 0, 184);
            layer1.Tiles[8, 14] = new StaticTile(layer1, tileSheet2, 0, 182);
            layer1.Tiles[9, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[10, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[11, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[12, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[13, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[14, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[15, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[16, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[17, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[18, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[19, 14] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[20, 14] = new StaticTile(layer1, tileSheet2, 0, 184);
            layer1.Tiles[8, 15] = new StaticTile(layer1, tileSheet2, 0, 182);
            layer1.Tiles[9, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[10, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[11, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[12, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[13, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[14, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[15, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[16, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[17, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[18, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[19, 15] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[20, 15] = new StaticTile(layer1, tileSheet2, 0, 184);
            layer1.Tiles[8, 16] = new StaticTile(layer1, tileSheet2, 0, 182);
            layer1.Tiles[9, 16] = new StaticTile(layer1, tileSheet2, 0, 181);
            layer1.Tiles[10, 16] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[11, 16] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[12, 16] = new StaticTile(layer1, tileSheet2, 0, 181);
            layer1.Tiles[13, 16] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[14, 16] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[15, 16] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[16, 16] = new StaticTile(layer1, tileSheet2, 0, 150);
            layer1.Tiles[17, 16] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[18, 16] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[19, 16] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[20, 16] = new StaticTile(layer1, tileSheet2, 0, 200);
            layer1.Tiles[8, 17] = new StaticTile(layer1, tileSheet2, 0, 182);
            layer1.Tiles[9, 17] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[10, 17] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[11, 17] = new StaticTile(layer1, tileSheet2, 0, 165);
            layer1.Tiles[12, 17] = new StaticTile(layer1, tileSheet2, 0, 150);
            layer1.Tiles[13, 17] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[14, 17] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[15, 17] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[16, 17] = new StaticTile(layer1, tileSheet2, 0, 200);
            layer1.Tiles[8, 18] = new StaticTile(layer1, tileSheet2, 0, 198);
            layer1.Tiles[9, 18] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[10, 18] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[11, 18] = new StaticTile(layer1, tileSheet2, 0, 199);
            layer1.Tiles[12, 18] = new StaticTile(layer1, tileSheet2, 0, 200);
            layer3.Tiles[17, 13] = new StaticTile(layer3, tileSheet1, 0, 3);
            layer3.Tiles[18, 13] = new StaticTile(layer3, tileSheet1, 0, 4);
            layer2.Tiles[17, 14] = new StaticTile(layer2, tileSheet1, 0, 19);
            layer2.Tiles[18, 14] = new StaticTile(layer2, tileSheet1, 0, 20);
            layer3.Tiles[15, 13] = new StaticTile(layer3, tileSheet1, 0, 5);
            layer3.Tiles[15, 14] = new StaticTile(layer3, tileSheet1, 0, 21);
            layer2.Tiles[15, 15] = new StaticTile(layer2, tileSheet1, 0, 37);
            layer3.Tiles[14, 13] = new StaticTile(layer3, tileSheet1, 0, 5);
            layer3.Tiles[14, 14] = new StaticTile(layer3, tileSheet1, 0, 21);
            layer2.Tiles[14, 15] = new StaticTile(layer2, tileSheet1, 0, 37);
            layer3.Tiles[13, 13] = new StaticTile(layer3, tileSheet1, 0, 5);
            layer3.Tiles[13, 14] = new StaticTile(layer3, tileSheet1, 0, 21);
            layer2.Tiles[13, 15] = new StaticTile(layer2, tileSheet1, 0, 37);
        }
    }
}