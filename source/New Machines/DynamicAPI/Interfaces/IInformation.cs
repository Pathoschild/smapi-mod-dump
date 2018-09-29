namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IInformation
    {
        int ID { get; }

        /// <summary>
        /// Get serialized string.
        /// </summary>
        string ToString();
    }

    public interface ICropInformation : IInformation {}

    public interface IItemInformation : IInformation {}

    public interface ITreeInformation : IInformation {}
}