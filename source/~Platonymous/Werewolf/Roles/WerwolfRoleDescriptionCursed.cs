/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Linq;
using Werewolf.Game;

namespace Werewolf.Roles
{
    public class WerwolfRoleDescriptionCursed : WerwolfRoleDescriptionBase
    {
        public WerwolfRoleDescriptionCursed(WerwolfPlayer player) : base(player)
        {
        }

        public override WerwolfRoleTarget Target => WerwolfRoleTarget.NONE;
        public override WerewolfRoleType Type => WerewolfRoleType.INGAME;

        public override string Name { get; } = "Cursed";

        public override string Description { get; } = "Till the game ends, the cursed show the mirror of their true soul on death or to the seer.";


        public override void PreGame(WerwolfGame game, Action callback)
        {
            base.PreGame(game, callback);
        }

        public override bool IsWolf(bool truth)
        {
            return !Player.Roles.Where(r => r is not WerwolfRoleDescriptionCursed).Any(wr => wr.IsWolf(false));
        }

    }


}
