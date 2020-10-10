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
using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BNC
{
    public class Spawner
    {
        private static Dictionary<NPC, string> SpawnList_AroundPlayer = new Dictionary<NPC, string>();
        private static List<ITwitchMonster> MobsSpawned = new List<ITwitchMonster>();

        public static void Init()
        {
            if(BNC_Core.config.Use_Bits_To_Spawn_Mobs)
                GraphicsEvents.OnPostRenderHudEvent += new EventHandler(OnPostRender);
        }


        public static void UpdateTick()
        {
            if (!Context.CanPlayerMove || Game1.CurrentEvent != null || Game1.isFestival() || Game1.weddingToday)
                return;

            // Spawns Up to three per tick
            for (int i = 0; i < 3; i++) {
                if (SpawnList_AroundPlayer.Count > 0 && canSpawn())
                {

                    bool flag = false;
                    NPC npc = SpawnList_AroundPlayer.Keys.ElementAt(0);
                    if (npc is Monster)
                    {
                        Monster m = (Monster)npc;
                        flag = tryToSpawnNPC(npc, SpawnList_AroundPlayer.Values.ElementAt(0), getRangeFromPlayer(10, 4));
                    }
                    else if (npc is Junimo)
                    {
                        Junimo j = (Junimo)npc;
                        j.stayPut.Value = false;
                        flag = tryToSpawnNPC(j, SpawnList_AroundPlayer.Values.ElementAt(0), getRangeFromPlayer(8));
                    }

                    if (flag) SpawnList_AroundPlayer.Remove(SpawnList_AroundPlayer.Keys.ElementAt(0));
                }
                else
                    break;
            }
        }

        private static Vector2 getRangeFromPlayer(int range, int minRange = 3)
        {
            int xStart = Game1.player.getTileX() - range;
            int yStart = Game1.player.getTileY() - range;

            int randX = Game1.random.Next(range * 2 + 2);
            int randY = Game1.random.Next(range * 2 + 2);


            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            while (Vector2.Distance(vector, Game1.player.getTileLocation()) < minRange) {
                vector.X = xStart + Game1.random.Next(range * 2 + 2);
                vector.Y = yStart + Game1.random.Next(range * 2 + 2);
            }
            return vector;
        }

        public static bool canSpawn()
        {
            if (Game1.player.currentLocation.Name.Equals("Hospital") || Game1.player.currentLocation.Name.Equals("FarmHouse"))
                return false;
            else
                return true;
        }

        public static Monster UpdateDifficulty(Monster m)
        {
            int minelvl = Game1.player.deepestMineLevel;
            WorldDate date = Game1.Date;
            bool beforeSword = date.DayOfMonth < 5 && date.Year == 1 && date.SeasonIndex == 1;

            if(beforeSword)
            {
                m.MaxHealth = m.MaxHealth / 4; ;
                m.Health = m.MaxHealth;
                m.DamageToFarmer = m.DamageToFarmer / 3;
            }

            if(minelvl <= 10)
            {
                m.MaxHealth = m.MaxHealth / 2;
                m.Health = m.MaxHealth;
                m.DamageToFarmer = m.DamageToFarmer / 2;
            }

            if (m.DamageToFarmer <= 0) m.DamageToFarmer = 1;
            if (m.MaxHealth <= 0) m.MaxHealth = 1;
            if (m.Health <= 0) m.Health = 1;

            return m;
        }

        public static void addMonsterToSpawn(Monster m, string username)
        {
           SpawnList_AroundPlayer.Add(UpdateDifficulty(m), username);
        }

        public static void addSubToSpawn(NPC sub, string username)
        {
            SpawnList_AroundPlayer.Add(sub, username);
        }

        public static void SpawnTwitchJunimo(string name)
        {
            if (!BNC_Core.config.Spawn_Subscriber_Junimo)
                return;
            Junimo j = new TwitchJunimo(Vector2.Zero);
            Spawner.addSubToSpawn(j, name);
        }

        private static bool tryToSpawnNPC(NPC m, string username, Vector2 pos)
        {
            if (!canSpawn() || pos.Equals(Game1.player.getTileLocation()) || !Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(pos) || Game1.player.currentLocation.isTileOccupied(pos, ""))
            {
                if(Config.ShowDebug())
                    BNC_Core.Logger.Log($"Faild to spawn {m.displayName}:{username} at Tile:{pos.X},{pos.Y} Adding Back into Queue");
                return false;
            }

            ((ITwitchMonster)m).setTwitchName(username);

            m.setTilePosition((int)pos.X, (int)pos.Y);
            Game1.player.currentLocation.characters.Add((NPC) m);
            MobsSpawned.Add((ITwitchMonster)m);

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Spawning {m.displayName}:{username} at Tile:{pos.X},{pos.Y}..");
            if (m is GreenSlime)
                Game1.player.currentLocation.playSoundAt("slime", pos);

            return true;
        }

        public static void ClearMobs()
        {
            foreach (GameLocation location in Game1.locations.ToArray())
            {
                foreach (NPC mob in location.characters.ToArray())
                {
                    if(mob is ITwitchMonster && Config.Should_Respawn(Game1.random))
                    {
                        location.characters.Remove(mob);
                    }
                }
            }
            MobsSpawned.Clear();
        }

        public static void SpawnTwitchNPC(string username, Monster mob)
        {
            addMonsterToSpawn(mob, username);
        }

        public static void AddMonsterToSpawnFromType(TwitchMobType type, string name, bool tiny = false)
        {
            switch (type)
            {

                case TwitchMobType.Slime:
                    TwitchSlime slime = new TwitchSlime(Vector2.Zero, Game1.player.deepestMineLevel);
                    if (tiny)
                    {
                        slime.willDestroyObjectsUnderfoot = false;
                        slime.moveTowardPlayer(4);
                        slime.Scale = (float)(0.50 + (double)Game1.random.Next(-5, 10) / 100.0);
                        slime.MaxHealth = slime.MaxHealth / 4;
                        slime.Health = slime.MaxHealth;
                    }
                    SpawnTwitchNPC(name, slime);
                    break;
                case TwitchMobType.Crab:
                    SpawnTwitchNPC(name, new TwitchCrab(Vector2.Zero));
                    break;
                case TwitchMobType.Bug:
                    TwitchBug bug = new TwitchBug(Vector2.Zero, -1);
                    bug.faceDirection(Game1.random.Next(4));
                    SpawnTwitchNPC(name, bug);
                    break;
                case TwitchMobType.Fly:
                    TwitchFly fly = new TwitchFly(Vector2.Zero, false);
                    fly.focusedOnFarmers = true;
                    SpawnTwitchNPC(name, fly);
                    break;
                case TwitchMobType.Bat:
                    TwitchBat bat = SpecialBatSpawn();
                    bat.focusedOnFarmers = true;
                    SpawnTwitchNPC(name, bat);
                    break;
                case TwitchMobType.BigSlime:
                    SpawnTwitchNPC(name, new TwitchBigSlime(Vector2.Zero));
                    break;
            }
        }

        public static bool IsMonsterEnabled(TwitchMobType type)
        {

            switch (type)
            {
                case TwitchMobType.Slime:
                    return BNC_Core.config.Bits_To_Spawn_Slimes_Range != null;
                case TwitchMobType.Crab:
                    return BNC_Core.config.Bits_To_Spawn_Crabs_Range != null;
                case TwitchMobType.Bug:
                    return BNC_Core.config.Bits_To_Spawn_Bugs_Range != null;
                case TwitchMobType.Fly:
                    return BNC_Core.config.Bits_To_Spawn_Fly_Range != null;
                case TwitchMobType.Bat:
                    return BNC_Core.config.Bits_To_Spawn_Bat_Range != null;
                case TwitchMobType.BigSlime:
                    return BNC_Core.config.Bits_To_Spawn_Big_Slimes_Range != null;
            }
            return false;
        }

        public static TwitchMobType? GetMonsterFromBits(int bit)
        {
            TwitchMobType? TypeReturn = null;

            foreach (TwitchMobType type in Enum.GetValues(typeof(TwitchMobType)))
            {
                if (!IsMonsterEnabled(type))
                    continue;

                int[] range;
                switch (type)
                {
                    case TwitchMobType.Slime:
                        range = GetBitRangeFromMonster(TwitchMobType.Slime);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.Slime;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    case TwitchMobType.Crab:
                        range = GetBitRangeFromMonster(TwitchMobType.Crab);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.Crab;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    case TwitchMobType.Bug:
                        range = GetBitRangeFromMonster(TwitchMobType.Bug);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.Bug;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    case TwitchMobType.Fly:
                        range = GetBitRangeFromMonster(TwitchMobType.Fly);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.Fly;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    case TwitchMobType.Bat:
                        range = GetBitRangeFromMonster(TwitchMobType.Bat);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.Bat;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    case TwitchMobType.BigSlime:
                        range = GetBitRangeFromMonster(TwitchMobType.BigSlime);
                        if (bit >= range[0] && bit <= range[1])
                            TypeReturn = TwitchMobType.BigSlime;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"{type.ToString()}: Are bits({bit}) between {range[0]} & {range[1]}? {bit >= range[0] && bit <= range[1]}");
                        break;
                    default:
                        range = new int[] { -1, -1 };
                        break;
                }
            }
            return TypeReturn;  
        }


        private static int[] GetBitRangeFromMonster(TwitchMobType type)
        {
            int[] ranges = new int[]{ -1, -2 };
            switch (type)
            {
                case TwitchMobType.Slime:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Slimes_Range);
                    break;
                case TwitchMobType.Crab:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Crabs_Range);
                    break;
                case TwitchMobType.Bug:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Bugs_Range);
                    break;
                case TwitchMobType.Fly:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Fly_Range);
                    break;
                case TwitchMobType.Bat:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Bat_Range);
                    break;
                case TwitchMobType.BigSlime:
                    ranges = CalculateRanges(BNC_Core.config.Bits_To_Spawn_Big_Slimes_Range);
                    break;
                default:
                    ranges = new int[] { -1, -2 };
                    break;

            }
            return ranges;
        }

        private static int[] CalculateRanges(int[] ranges)
        {
            if (ranges.Length >= 2)
            {
                int min = Math.Min(ranges[0], ranges[1]);
                int max = Math.Max(ranges[0], ranges[1]);
                return new int[] { min, max };
            }
            else if (ranges.Length == 1)
            {
                return new int[] { ranges[0], int.MaxValue };
            }
            return new int[] { -100, -200 };
        }

        private static TwitchBat SpecialBatSpawn()
        {
            TwitchBat bat = new TwitchBat(Vector2.Zero, 1); ;

            if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() < 0.25)
            {
                bat = new TwitchBat(Vector2.Zero, 9999);
                bat.focusedOnFarmers = true;
                bat.wildernessFarmMonster = true;
            }
            else if (Game1.player.CombatLevel >= 8 && Game1.random.NextDouble() < 0.5)
            {
                bat = new TwitchBat(Vector2.Zero, 81);
                bat.focusedOnFarmers = true;
                bat.wildernessFarmMonster = true;
            }
            else if (Game1.player.CombatLevel >= 5 && Game1.random.NextDouble() < 0.5)
            {
                bat = new TwitchBat(Vector2.Zero, 41);
                bat.focusedOnFarmers = true;
                bat.wildernessFarmMonster = true;
            }
            else
            {
                bat = new TwitchBat(Vector2.Zero, 1);
                bat.focusedOnFarmers = true;
                bat.wildernessFarmMonster = true;
            }
            return bat;
        }

        //## Client Rendering Code for Twitch Name over twitch mobs. 
        private static void OnPostRender(object sender, EventArgs e)
        {
            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {

                //                foreach (string name in MobsSpawned.Keys) Tried something different
                foreach (NPC npc in Game1.currentLocation.getCharacters())
                {
                    if (npc is ITwitchMonster)
                    {
                        int localX = npc.GetBoundingBox().Center.X - Game1.viewport.X - ((int)Game1.smallFont.MeasureString(((ITwitchMonster)npc).GetTwitchName()).Length() / 2);
                        int localY = npc.GetBoundingBox().Y - Game1.viewport.Y - (npc is Monster ? 60 : 30);
                        Utility.drawTextWithColoredShadow(Game1.spriteBatch, $"{((ITwitchMonster)npc).GetTwitchName()}", Game1.smallFont, new Vector2(localX, localY), Color.Wheat, Color.Black);
                    }
                }
            }
        }

        public enum TwitchMobType   { Slime, Crab, Bug, Fly, Bat, BigSlime }
    }
}
