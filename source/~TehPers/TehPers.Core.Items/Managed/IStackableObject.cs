namespace TehPers.Core.Items.Managed {
    public interface IStackableObject : IApiManagedObject {
        /// <summary>The quantity of this object.</summary>
        int StackSize { get; }

        /// <summary>The max quantity of this object.</summary>
        int MaxStackSize { get; }

        /// <summary>Adds items to this stack.</summary>
        /// <param name="amount">The amount to increase the stack size by.</param>
        void AddToStack(int amount);

        /// <summary>Checks if this can be stacked with another <see cref="IStackableObject"/>.</summary>
        /// <param name="other">The other <see cref="IStackableObject"/>.</param>
        /// <returns>True if the two objects can be stacked, false otherwise.</returns>
        bool CanStackWith(IStackableObject other);
    }
}