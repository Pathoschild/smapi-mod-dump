/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

namespace CombatControlsRedux
{
    public class ModConfig
    {
        public bool MouseFix = true;
        public bool ControllerFix = false;
        public bool RegularToolsFix = false;

        public bool AutoSwing = false;
        public bool AutoSwingDagger = true;

        public bool ClubSpecialSpamAttack = false;

        public bool SlickMoves = true;
        public bool SwordSpecialSlickMove = true;
        public bool ClubSpecialSlickMove = false;

        public float SlideVelocity = 4f;
        public float SpecialSlideVelocity = 2.8f;

        //undocumented options
        public bool Debug = false;//unused right now.
        public bool NearTileFacingFix = false;
        public int CountdownStart = 6;
        public int CountdownFastDaggerOffset = 2;
        public int CountdownRepeat = 2;
        public int ClubSpamCount = 3;
    }
}

