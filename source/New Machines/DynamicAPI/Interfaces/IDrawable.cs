namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IDrawable
    {
        /// <summary>
        /// Index to in-game resource. Must be unique inside group of same items.
        /// </summary>
        int TextureIndex { get; }

        /// <summary>
        /// Index to mod resource. <c>Null</c> if custom resource is not required.
        /// </summary>
        int? ResourceIndex { get; }

        /// <summary>
        /// Length of reserved sprite range from mod resource.
        /// </summary>
        int ResourceLength { get; }

        int ResourceHeight { get; }
    }
}
