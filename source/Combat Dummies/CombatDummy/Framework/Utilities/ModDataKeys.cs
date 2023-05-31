/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CombatDummy
**
*************************************************/

namespace CombatDummy.Framework.Utilities
{
    public class ModDataKeys
    {
        // Entity related
        internal const string PRACTICE_DUMMY_NAME = "PracticeDummy";
        internal const string KNOCKBACK_DUMMY_NAME = "KnockbackDummy";
        internal const string MAX_HIT_DUMMY_NAME = "MaxHitDummy";
        internal const string MONSTER_DUMMY_FLAG = "PeacefulEnd.PracticeDummy.MonsterDummy";

        // Monster related
        internal const string MONSTER_HOME_POSITION_X = "PeacefulEnd.PracticeDummy.MonsterDummy.Home.X";
        internal const string MONSTER_HOME_POSITION_Y = "PeacefulEnd.PracticeDummy.MonsterDummy.Home.Y";

        // Animation related
        internal const string IS_DUMMY_ANIMATING = "PeacefulEnd.PracticeDummy.IsAnimating";
        internal const string DUMMY_ANIMATION_FRAME = "PeacefulEnd.PracticeDummy.AnimationFrame";
        internal const string DUMMY_ANIMATION_COUNTDOWN = "PeacefulEnd.PracticeDummy.AnimationCountdown";

        // Knockback related
        internal const string DUMMY_NEXT_POSITION_X = "PeacefulEnd.PracticeDummy.NextPosition.X";
        internal const string DUMMY_NEXT_POSITION_Y = "PeacefulEnd.PracticeDummy.NextPosition.Y";
        internal const string DUMMY_LAST_POSITION_X = "PeacefulEnd.PracticeDummy.LastPosition.X";
        internal const string DUMMY_LAST_POSITION_Y = "PeacefulEnd.PracticeDummy.LastPosition.Y";
        internal const string DUMMY_VELOCITY_X = "PeacefulEnd.PracticeDummy.Velocity.X";
        internal const string DUMMY_VELOCITY_Y = "PeacefulEnd.PracticeDummy.Velocity.Y";
        internal const string DUMMY_KNOCKBACK_COUNTDOWN = "PeacefulEnd.PracticeDummy.KnockbackCountdown";

        // Combat related
        internal const string IS_DUMMY_INVINCIBLE = "PeacefulEnd.PracticeDummy.IsInvincible";
        internal const string DUMMY_COLLECTIVE_DAMAGE = "PeacefulEnd.PracticeDummy.CollectiveDamage";
        internal const string DUMMY_INVINCIBLE_COUNTDOWN = "PeacefulEnd.PracticeDummy.InvincibleCountdown";
        internal const string DUMMY_DAMAGE_COUNTDOWN = "PeacefulEnd.PracticeDummy.DamageCountdown";
    }
}
