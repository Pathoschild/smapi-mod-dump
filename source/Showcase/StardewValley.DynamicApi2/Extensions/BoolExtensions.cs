namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class BoolExtensions
    {
        public static string Serialize(this bool e)
        {
            return e.ToString().ToLower();
        }
    }
}