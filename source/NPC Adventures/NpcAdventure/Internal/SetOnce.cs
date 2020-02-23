using System;

namespace NpcAdventure.Internal
{
    internal class SetOnce<T>
    {
        private bool alreadySet = false;
        private T value;

        public T Value 
        {
            get => this.value;
            set
            {
                if (this.alreadySet)
                {
                    throw new InvalidOperationException("Value already set!");
                }

                this.value = value;
                this.alreadySet = true;
            }
        }
    }
}