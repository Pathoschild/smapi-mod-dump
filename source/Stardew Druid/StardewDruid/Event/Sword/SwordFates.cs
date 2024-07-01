/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event.Access;
using StardewDruid.Location;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Threading;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Sword
{
    internal class SwordFates : EventHandle
    {

        public bool entranceFound;

        public Vector2 statueVector;

        public Dictionary<Vector2, Tile> heldTiles = new();

        public SwordFates()
        {

        }

        public override void TriggerInterval()
        {
            
            if (!entranceFound)
            {
                
                Vector2 entrance = (location as MineShaft).tileBeneathLadder;

                origin = (entrance + new Vector2(0, 2)) * 64;

                statueVector = new Vector2(30,6) * 64;

                for (int x = 28; x < 32; x++)
                {

                    for (int y = 4; y < 8; y++)
                    {

                        if (location.isActionableTile(x, y, Game1.player))
                        {

                            foreach (Layer layer in location.map.Layers)
                            {

                                if (layer.Tiles[x, y] != null)
                                {

                                    Vector2 tileVector = new(x, y);

                                    heldTiles[tileVector] = layer.Tiles[x, y];

                                    layer.Tiles[x, y] = new StaticTile(layer, heldTiles[tileVector].TileSheet, BlendMode.Alpha, heldTiles[tileVector].TileIndex);

                                }

                            }

                        };

                    }

                }

                entranceFound = true;

            }

            base.TriggerInterval();

        }

        public override void EventRemove()
        {

            companions.Clear();
            
            base.EventRemove();

            foreach(KeyValuePair<Vector2,Tile> tile in heldTiles)
            {

                Layer layer = tile.Value.Layer;

                layer.Tiles[(int)tile.Key.X, (int)tile.Key.Y] = tile.Value;

            }

            heldTiles.Clear();

        }


        public override void EventActivate()
        {

            base.EventActivate();

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 90; i += 2)
            {

                monsterHandle.spawnSchedule.Add(i, new() { new(MonsterHandle.bosses.spectre, 4, Mod.instance.randomIndex.Next(2)) });

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(Game1.player.Position);

            monsterHandle.spawnRange = new(15, 15);

            monsterHandle.spawnWater = true;

            monsterHandle.spawnVoid = true;

            eventProximity = -1;

            activeLimit = 90;

            EventBar("The Reaper's Trail", 0);

        }

        public override void EventInterval()
        {

            activeCounter++;

            DialogueCue(activeCounter);

            if (activeCounter <= 10)
            {

                EventPartOne();

            }
            else
            if(activeCounter <= 90)
            {

                EventPartTwo();

            }
            else
            if(activeCounter <= 120)
            {

                EventPartThree();

            }
            else
            {

                EventPartFour();

            }

        }

        public void EventPartOne()
        {

            switch (activeCounter)
            {
                
                case 1:

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Jester, Character.Character.mode.track);

                    companions[0] = Mod.instance.characters[CharacterHandle.characters.Jester];

                    narrators = DialogueData.DialogueNarrators(eventId);

                    cues = DialogueData.DialogueScene(eventId);

                    voices[0] = companions[0];
                    
                    break;

                case 6:
                case 8:
                    location.playSound("ghost");

                    break;
                
                case 10:

                    Mod.instance.CastDisplay("Get to the end of the tunnel before time runs out!", 2);

                    location.playSound("ghost");
                        
                    SetTrack("cowboy_outlawsong");

                    break;

            }

        }


        public void EventPartTwo()
        {
            
            if (activeCounter % 10 == 0)
            {

                Mod.instance.trackers[CharacterHandle.characters.Jester].WarpToPlayer();

                if (Mod.instance.trackers.ContainsKey(CharacterHandle.characters.Effigy))
                {

                    Mod.instance.trackers[CharacterHandle.characters.Effigy].WarpToPlayer();

                }

                Mod.instance.CastDisplay("Get to the end of the tunnel before time runs out!", 2);

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(Game1.player.Position);

            monsterHandle.SpawnCheck();

            monsterHandle.SpawnInterval();

            if(Vector2.Distance(Game1.player.Position,statueVector) <= 384)
            {

                activeCounter = 90;

                activeLimit = 160;

            }

        }

        public void EventPartThree()
        {

            if (Game1.player.currentLocation.Name == LocationData.druid_court_name)
            {

                inabsentia = false;

                activeCounter = 120;

                return;

            }

            switch (activeCounter)
            {

                case 91:

                    companions[0].Position = new Vector2(27, 6) * 64;

                    companions[0].Halt();

                    companions[0].idleTimer = 600;

                    companions[0].netDirection.Set(1);

                    break;

                case 93:

                    if (!Game1.player.mailReceived.Contains("gotGoldenScythe"))
                    {
                        Game1.playSound("parry");

                        Game1.player.mailReceived.Add("gotGoldenScythe");

                        location.setMapTileIndex(29, 4, 245, "Front");

                        location.setMapTileIndex(30, 4, 246, "Front");

                        location.setMapTileIndex(29, 5, 261, "Front");

                        location.setMapTileIndex(30, 5, 262, "Front");

                        location.setMapTileIndex(29, 6, 277, "Buildings");

                        location.setMapTileIndex(30, 56, 278, "Buildings");

                        StardewValley.Tools.MeleeWeapon scythe = new("53");

                        ThrowHandle swordThrow = new(Game1.player, companions[0].Position, scythe);

                        swordThrow.register();
                    
                    }

                    break;

                case 98:

                    Event.Access.AccessHandle CourtAccess = new();

                    CourtAccess.AccessSetup("UndergroundMine77377", LocationData.druid_court_name, new(29, 8), new(45, 20));

                    CourtAccess.location = location;

                    CourtAccess.AccessStair();

                    Event.Access.AccessHandle TunnelAccess = new();

                    TunnelAccess.AccessSetup(LocationData.druid_court_name, "UndergroundMine77377", new(43, 20), new(29, 10));

                    TunnelAccess.location = Mod.instance.locations[LocationData.druid_court_name];

                    TunnelAccess.AccessStair();

                    break;

                case 99:

                    location.localSound("secret1");

                    break;

                case 102:

                    Game1.warpFarmer(LocationData.druid_court_name, 43, 20, 1);

                    Game1.xLocationAfterWarp = 43;

                    Game1.yLocationAfterWarp = 20;

                    inabsentia = true;

                    location = Mod.instance.locations[LocationData.druid_court_name];

                    CharacterMover.Warp(Mod.instance.locations[LocationData.druid_court_name], companions[0], new Vector2(41, 18) * 64);

                    break;

                case 106:

                    inabsentia = false;

                    activeCounter = 120;

                    break;


            }

        }


        public void EventPartFour()
        {

            switch (activeCounter)
            {

                case 121:

                    DialogueCue(121);

                    break;

                case 124:

                    companions[0].LookAtTarget(Game1.player.Position);

                    DialogueSetups(companions[0], 1);

                    break;

                case 131:

                    companions[0].LookAtTarget(Game1.player.Position);

                    DialogueSetups(companions[0], 2);

                    break;

                case 141:

                    companions[0].LookAtTarget(Game1.player.Position);

                    DialogueSetups(companions[0], 3);

                    break;

                case 151:

                    companions[0].LookAtTarget(Game1.player.Position);

                    DialogueSetups(companions[0], 4);

                    break;

                case 152:

                    Mod.instance.questHandle.CompleteQuest(eventId);

                    eventComplete = true;

                    Event.Access.AccessHandle CourtAccess = new();

                    CourtAccess.AccessSetup("UndergroundMine77377", LocationData.druid_court_name, new(29, 8), new(45, 20));

                    CourtAccess.location = location;

                    CourtAccess.AccessWarps();

                    Event.Access.AccessHandle TunnelAccess = new();

                    TunnelAccess.AccessSetup(LocationData.druid_court_name, "UndergroundMine77377", new(43, 20), new(29, 10));

                    TunnelAccess.location = Mod.instance.locations[LocationData.druid_court_name];

                    TunnelAccess.AccessWarps();

                    break;


            }


        }

        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {

                default: // introOne

                    intro = "The Jester of Fate: Are you as creeped out as I am farmer?";

                    break;

                case 2:

                    intro = "The Jester of Fate: You know what, after all that, I'm actually relieved we didnt run into Thanatoshi, because, to be honest, it's all a bit overwhelming. And what would I say if we even find him? ";

                    break;

                case 3:

                    intro = "These monuments are arranged like the court of the Fates. The Artisans, the Priesthood, The Morticians, and Chaos. " +
                        "I think I know this place. The Justiciar of the Morticians came here to fix all the problems caused by the dragons and elderfolk and humans. " +
                        "This was where Thanatoshi was sent after the fallen Star. This is the last place he was seen.";

                    break;

                case 4:

                    intro = "I hope that statue in the tunnel isn't all that's left of the great Thanatoshi, otherwise that's a bit stink. This whole thing gives me the wierd feels. " +
                        "I think the switch opened a door to the outside in the south part of this cave. How about we go back to your place and practice tricks until I feel better.";

                    DialogueDraw(npc, intro);

                    return;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //introOne

                    responseList.Add(new Response("1a", "After a hundred descents into the mines, I have become a master dungeon-explorer."));
                    responseList.Add(new Response("1a", "I saw a lot of body-less heads but not a lot of headless bodies. I suspect someone's stealing bodies."));
                    responseList.Add(new Response("1a", "(Say nothing)"));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "Odd to think the locals produced this. I assume most of them are unaware of the mysteries of the Fates."));
                    responseList.Add(new Response("2a", "I don't want to throw shade at the Fates and all, but I'm getting really big cult vibes from this setup."));
                    responseList.Add(new Response("2a", "(Say nothing)"));

                    break;

                case 3:

                    responseList.Add(new Response("2a", "He's cursed a great number of souls, innocent or not, and I intend to hold him accountable."));
                    responseList.Add(new Response("2a", "Judging by his likeness in stone, I imagine he'll be warm and approachable."));
                    responseList.Add(new Response("2a", "(Say nothing)"));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogueId)
        {

            switch (dialogueId)
            {

                case "1a":
                default:

                    activeCounter = Math.Max(130, activeCounter);

                    break;

                case "2a":

                    activeCounter = Math.Max(140, activeCounter);

                    break;

                case "3a":

                    activeCounter = Math.Max(150, activeCounter);

                    break;

            }

        }

    }

}