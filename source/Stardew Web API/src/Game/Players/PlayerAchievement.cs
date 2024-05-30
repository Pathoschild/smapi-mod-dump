/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewWebApi.Game.Items;

namespace StardewWebApi.Game.Players;

public class PlayerAchievement : Achievement
{
    private readonly Farmer _player;

    public PlayerAchievement(int achievementId, Farmer player) : base(achievementId)
    {
        _player = player;
        HasPlayerUnlocked = _player.achievements.Contains(Id);
    }

    public bool HasPlayerUnlocked { get; }
    public bool CanPlayerSeeDetails => HasPlayerUnlocked
        || (ShowIfPrerequisiteMet && (!HasPrerequisiteAchievement || _player.achievements.Contains(PrerequisiteAchievement!.Value)));
}