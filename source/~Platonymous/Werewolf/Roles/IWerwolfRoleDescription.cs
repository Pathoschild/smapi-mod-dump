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
using System.Collections.Generic;
using Werewolf.Game;
using Werewolf.Roles.Actions;

namespace Werewolf.Roles
{
    public interface IWerwolfRoleDescription
    {
        string RoleID { get; }

        WerwolfRoleTarget Target { get; }

        WerewolfRoleType Type { get; }

        string ModID { get; }

        string Name { get; }

        string Description { get; }

        bool IsMayor { get; }

        bool CanVote { get; }

        List<IWerwolfRoleAction> RoleActions { get; }

        WerwolfPlayer Player { get; }

        bool IsWolf(bool truth);

        bool ReadyToProgress();

        bool Remove { get; set; }

        void PreGame(WerwolfGame game, Action callback);

        List<string> KnownRole(WerwolfPlayer player, List<string> roles, bool truth);

        void OnDeath(WerwolfGame game, WerwolfPlayer killed);

        void BeforeKills(WerwolfGame game, List<long> players, Action callback);

        List<WerwolfPlayer> SetupVote(List<WerwolfPlayer> players);

        void AfterVote(WerwolfGame game, Action callback, WerwolfVotes votes);

        IWerwolfRoleDescription GetAssigned(WerwolfPlayer player);
    }


}
