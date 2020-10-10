/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewValley;

namespace NpcAdventure.StateMachine.StateFeatures
{
    internal interface IDialogueDetector
    {
        /// <summary>
        /// Handles after NPC's dialogue spoken.
        /// </summary>
        /// <param name="speakedDialogue">The last spoken dialogue</param>
        void OnDialogueSpoken(Dialogue speakedDialogue);
    }
}