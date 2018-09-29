namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IItem : IDrawable
    {
        int ID { get; }

        string Name { get; }
    }
}
