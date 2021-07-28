/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace TheftOfTheWinterStar.Framework
{
    internal class SaveData
    {
        public ArenaStage ArenaStage { get; set; } = ArenaStage.NotTriggered;
        public bool DidProjectilePuzzle { get; set; } = false;
        public bool BeatBoss { get; set; } = false;
    }
}
