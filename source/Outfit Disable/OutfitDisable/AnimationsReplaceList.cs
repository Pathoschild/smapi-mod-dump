/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/OutfitDisable
**
*************************************************/

namespace OutfitDisable
{
    public sealed class AnimationsReplaceList
    {
        public class AnimationReplaceTarget
        {
            public readonly Dictionary<string, string>? referenceAnimations;
            public readonly Dictionary<string, string>? manualAnimations;

            public AnimationReplaceTarget(Dictionary<string, string>? referenceAnimations, Dictionary<string, string>? manualAnimations)
            {
                this.referenceAnimations = referenceAnimations;
                this.manualAnimations = manualAnimations;
            }
        }

        // Relative NPC -> animation to replace and animation to use
        public Dictionary<string, AnimationReplaceTarget> Targets { get; set; }

        public AnimationsReplaceList()
        {
            Targets = new Dictionary<string, AnimationReplaceTarget>();
        }
    }
}