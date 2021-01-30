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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System;
namespace BNC.Managers.Augments
{

    public class Crabs : BaseAugment
    {
        public Crabs()
        {
            this.DisplayName = "You got crabs!";
            this.desc = "Spawns Crabs on each level";
        }

        public override void Init() { }

        public override ActionResponse MonsterTickUpdate(Monster m)
        {
            return ActionResponse.Done;
        }

        public override ActionResponse PlayerTickUpdate()
        {
            return ActionResponse.Done;
        }

        public override ActionResponse UpdateMonster(WarpedEventArgs e, Monster m)
        {
            GameLocation loc = Game1.player.currentLocation;
            int cnt = Game1.random.Next(8) + 1;
            for (int i = 0; i < cnt; i++)
                Spawner.addMonsterToSpawn(new RockCrab(Vector2.Zero), "");

            return ActionResponse.Done;
        }

        public override ActionResponse WarpLocation(WarpedEventArgs e) { return ActionResponse.Done; }
    }
}
