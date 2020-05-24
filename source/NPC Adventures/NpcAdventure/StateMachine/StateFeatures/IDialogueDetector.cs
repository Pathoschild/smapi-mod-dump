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