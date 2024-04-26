/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using LandGrants.Game;

namespace LandGrants.Roles.Actions
{
    public interface IWerwolfRoleAction
    {
        string ID { get; }
        string Name { get; }

        string Description { get; }

        bool IsRequired { get; }

        bool IsActive { get; set; }

        bool IsUnique { get; }

        WerwolfPlayer Player { get; }

        IWerwolfRoleDescription Role { get; }

        void Deactivate();
        void Reactivate();

        bool CanPerform(WerwolfGame game, WerwolfPlayer onPlayer);

        void Perform(WerwolfGame game, WerwolfPlayer onPlayer);

        void BotPerform(WerwolfGame game);

        IWerwolfRoleAction GetAssigned(WerwolfPlayer player, IWerwolfRoleDescription role);

        void AfterRound(WerwolfGame game);

    }


}
