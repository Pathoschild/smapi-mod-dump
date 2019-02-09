namespace TehPers.CoreMod.Api.ContentPacks {
    public interface IContextSpecific {
        /// <summary>Whether this is valid in the given context.</summary>
        /// <param name="context">The context this will be used in.</param>
        /// <returns>True if valid in this context, false otherwise.</returns>
        bool IsValidInContext(IContext context);
    }
}