/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

namespace StackEverythingRedux.UI
{
    internal sealed class Caret
    {
        /// <summary>Current index of the caret.</summary>
        public int Index { get; private set; } = 0;

        /// <summary>Optional maximum index of the caret. 0 = no limit.</summary>
        private readonly int MaxLength = 0;

        /// <summary>Constructor.</summary>
        /// <param name="maxLength">The optional max length of the string the caret is navigating.</param>
        public Caret(int maxLength)
        {
            // The caret must always be 1 index ahead so it must be able to be positioned thusly
            MaxLength = maxLength > 0 ? maxLength + 1 : maxLength;
        }

        /// <summary>Moves the caret forward.</summary>
        /// <param name="amount">The amount to advance by.</param>
        public void Advance(int amount, int textLength)
        {
            UpdateIndex(Index + amount, textLength);
        }

        /// <summary>Moves the caret back.</summary>
        /// <param name="amount">The amount to move back by.</param>
        public void Regress(int amount)
        {
            UpdateIndex(Index - amount);
        }

        /// <summary>Move the caret to the start.</summary>
        public void Start()
        {
            Index = 0;
        }

        /// <summary>Move the caret to the end of the text.</summary>
        /// <param name="textLength">Current length of the text.</param>
        public void End(int textLength)
        {
            Index = MaxLength > 0 ? Math.Min(MaxLength, textLength) : textLength;
        }

        /// <summary>Moves the caret to the specified index if it's within bounds.</summary>
        /// <param name="newIndex">The new caret index.</param>
        private void UpdateIndex(int newIndex, int textLength = 0)
        {
            if (newIndex >= 0
                && (MaxLength == 0 || newIndex < MaxLength)
                && (textLength == 0 || newIndex <= textLength)
                )
            {
                Index = newIndex;
            }
        }
    }
}
