/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chawolbaka/HaltEventTime
**
*************************************************/

using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaltEventTime
{
    public static class Time
    {
        public enum Status { Run, Stop }
        public static bool Async;
        public static Status State = Status.Run;
        private static readonly object stateLock = new object();
        private static int orginalTimeInterval;
        private static Dictionary<string, int> orginalMonsterSpeed = new Dictionary<string, int>();

        /// <summary>
        /// 恢复时间的走动
        /// </summary>
        /// <returns>是从时间停止的状态下开始的吗</returns>
        public static bool Run()
        {
            lock(stateLock)
            {
                if (State == Status.Run)
                    return false;
                State = Status.Run;
            }

            TemporarySpritesUp();
            MonsterUp();
            Game1.gameTimeInterval = orginalTimeInterval;
            return true;
        }

        /// <summary>
        /// 停止时间（需要每tick都执行）
        /// </summary>
        /// <returns>是从时间在走动的状态开始的吗</returns>
        public static bool Stop()
        {
            lock (stateLock)
            {
                if (State == Status.Stop)
                {
                    Game1.gameTimeInterval = 0;
                    TemporarySpritesDown(); //基本上是0，非0的时候基本上也是个位数，也没必要异步。
                    if (Async)
                    {
                        Task.WaitAll(new Task[] {
                            Task.Run(MonsterDown),
                            Task.Run(NPCDownAsync)
                        });
                    }
                    else
                    {
                        MonsterDown();
                        NPCDown();
                    }
                    return false;
                }
                else
                {
                    State = Status.Stop;
                    orginalTimeInterval = Game1.gameTimeInterval;
                    if (ModConfig.Instance.AsyncThreshold > 0)
                    {
                        //计算需要遍历的量，因为每tick都要遍历如果如果过多就用Task
                        int Count = Game1.currentLocation.TemporarySprites.Count + Game1.getOnlineFarmers().Count * 3 + Game1.locations.Count;
                        foreach (GameLocation location in Game1.locations)
                            Count += location.characters.Count;
                        foreach (var farmer in Game1.getOnlineFarmers())
                            Count += farmer.currentLocation.characters.Count;
                        Async = Count > ModConfig.Instance.AsyncThreshold;
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// 停止所有NPC的行动
        /// </summary>
        private static void NPCDown()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Character character in location.characters)
                {
                    if(character is NPC npc)
                        npc.movementPause = 1;
                }
            }
        }
        private static void NPCDownAsync()
        {
            Task.WaitAll(
                Task.Run(() =>
                {
                    for (int i = 0; i < Game1.locations.Count / 2; i++)
                    {
                        foreach (Character character in Game1.locations[i].characters)
                        {
                            if (character is NPC npc)
                                npc.movementPause = 1;
                        }
                    }
                }),
                Task.Run(() =>
                {
                    for (int i = Game1.locations.Count - 1; i > Game1.locations.Count / 2; i--)
                    {
                        foreach (Character character in Game1.locations[i].characters)
                        {
                            if (character is NPC npc)
                                npc.movementPause = 1;
                        }
                    }
                }));
        }

        /// <summary>
        /// 恢复所有怪物的行动
        /// </summary>
        private static void MonsterUp()
        {
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                foreach (Character character in farmer.currentLocation.characters)
                {
                    if (character is Monster monster && monster.Speed == 0 && orginalMonsterSpeed.TryGetValue(monster.getName(), out int speed))
                    {
                        monster.speed = speed;
                    }
                }
            }
            orginalMonsterSpeed.Clear();
        }

        /// <summary>
        /// 停止所有怪物的行动
        /// </summary>
        private static void MonsterDown()
        {
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                foreach (Character character in farmer.currentLocation.characters)
                {
                    if (character is Monster monster)
                    {
                        string name = monster.getName();
                        if (!orginalMonsterSpeed.ContainsKey(name))
                            orginalMonsterSpeed.Add(name, monster.speed);
                        monster.Halt();
                        monster.Speed = 0;
                        monster.xVelocity = 0;
                        monster.yVelocity = 0;
                        monster.yJumpVelocity = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 恢复炸弹这类东西的运行
        /// </summary>
        private static void TemporarySpritesUp()
        {
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                foreach (var sprites in farmer.currentLocation.TemporarySprites)
                {
                    if (sprites.bombRadius > 0)
                        sprites.paused = false;
                }
            }
        }

        /// <summary>
        /// 停止炸弹这类东西的运行
        /// </summary>
        private static void TemporarySpritesDown()
        {
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                foreach (var sprites in farmer.currentLocation.TemporarySprites)
                {
                    if (sprites.bombRadius > 0)
                        sprites.paused = true;
                }
            }
        }
    }
}
