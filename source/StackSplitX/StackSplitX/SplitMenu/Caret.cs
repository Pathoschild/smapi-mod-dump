using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackSplitX
{
    sealed class Caret
    {
        // TODO: add draw calls/blink timer (or do that in a different class)
        /// <summary>Current index of the caret.</summary>
        public int Index { get; private set; } = 0;

        /// <summary>Optional maximum index of the caret. 0 = no limit.</summary>
        private int MaxLength = 0;

        /// <summary>Constructor.</summary>
        /// <param name="maxLength">The optional max length of the string the caret is navigating.</param>
        public Caret(int maxLength)
        {
            // The caret must always be 1 index ahead so it must be able to be positioned thusly
            this.MaxLength = maxLength > 0 ? maxLength + 1 : maxLength;
        }

        /// <summary>Moves the caret forward.</summary>
        /// <param name="amount">The amount to advance by.</param>
        public void Advance(int amount, int textLength)
        {
            UpdateIndex(this.Index + amount, textLength);
        }

        /// <summary>Moves the caret back.</summary>
        /// <param name="amount">The amount to move back by.</param>
        public void Regress(int amount)
        {
            UpdateIndex(this.Index - amount);
        }

        /// <summary>Move the caret to the start.</summary>
        public void Start()
        {
            this.Index = 0;
        }

        /// <summary>Move the caret to the end of the text.</summary>
        /// <param name="textLength">Current length of the text.</param>
        public void End(int textLength)
        {
            this.Index = (this.MaxLength > 0) ? Math.Min(this.MaxLength, textLength) : textLength;
        }

        /// <summary>Moves the caret to the specified index if it's within bounds.</summary>
        /// <param name="newIndex">The new caret index.</param>
        private void UpdateIndex(int newIndex, int textLength = 0)
        {
            if (newIndex >= 0 &&
                (this.MaxLength == 0 || newIndex < this.MaxLength) &&
                (textLength == 0 || newIndex <= textLength))
            {
                this.Index = newIndex;
            }
        }
    }
}
