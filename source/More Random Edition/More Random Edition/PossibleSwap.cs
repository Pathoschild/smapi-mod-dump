/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer {
    public class PossibleSwap {
        public string FirstCharacter { get; }
        public string SecondCharacter { get; }

        public PossibleSwap(string firstCharacter, string secondCharacter) {
            this.FirstCharacter = firstCharacter;
            this.SecondCharacter = secondCharacter;
        }
    }
}