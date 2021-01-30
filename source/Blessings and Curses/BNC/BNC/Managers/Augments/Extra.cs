/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System;
namespace BNC.Managers.Augments
{

    public class Extra : BaseAugment
    {

        public Extra()
        {
            this.DisplayName = "More Mobs!";
            this.desc = "More mobs per level.";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)  {  return ActionResponse.Done;  }

        public override ActionResponse PlayerTickUpdate() { return ActionResponse.Done; }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc) { return ActionResponse.Done; }

        public override ActionResponse WarpLocation(WarpedEventArgs e)
        {
            GameLocation loc = Game1.player.currentLocation;
            int cnt = Game1.random.Next(8) + 1;

            NPC[] copy = new NPC[Game1.player.currentLocation.characters.Count];
            Game1.player.currentLocation.characters.CopyTo(copy, 0);

            foreach (NPC n in copy)
            {
                BNC_Core.Logger.Log("NPC", StardewModdingAPI.LogLevel.Debug);
                if (!(n is Monster)) continue;
                
                Monster monster = (Monster)n;
                int flag = Game1.random.Next(2);

                BNC_Core.Logger.Log($"Monster: {flag}", StardewModdingAPI.LogLevel.Debug);
                if (flag.Equals(1))
                {
                    int type = Game1.random.Next(2);
                    switch (type)
                    {
                        case 0:
                            BNC_Core.Logger.Log("Spawn slime", StardewModdingAPI.LogLevel.Debug);
                            Spawner.addMonsterToSpawn(new GreenSlime(Vector2.Zero), "");
                            break;
                        case 1:
                            BNC_Core.Logger.Log("Spawn slime", StardewModdingAPI.LogLevel.Debug);
                            Spawner.addMonsterToSpawn(new RockCrab(Vector2.Zero), "");
                            break;
                        default:
                            break;
                    }
                }
            }
            return ActionResponse.Done;
        }
    }
}
