/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using LandGrants.Game;

namespace LandGrants.Roles
{
    public class WerwolfRoleDescriptionVillager : WerwolfRoleDescriptionBase
    {
        public WerwolfRoleDescriptionVillager(WerwolfPlayer player) : base(player)
        {
        }

        public override string Name { get; } = "Villager";

        public override string Description { get; } = "Can vote for other villagers to be hanged. Wins when all the wolves are dead.";

        public override void OnDeath(WerwolfGame game, WerwolfPlayer killed)
        {
            if (game.GameIsActive && game.Players.Where(p => p.IsAlive && p.IsWolf(true)).Count() == 0)
                game.End(game.Villagers, "All wolves are dead. The townsfolk has won.", false);

            base.OnDeath(game, killed);
        }

    }


}
