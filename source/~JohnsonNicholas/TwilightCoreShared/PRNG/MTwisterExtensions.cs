namespace TwilightShards.Common
{
    public static class MTwisterExtensions
    {
        public static string GetRandomItem(this string[] array, MersenneTwister mt)
        {
            int l = array.Length;

            return array[mt.Next(l - 1)];
        }

        public static int GetRandomItem(this int[] array, MersenneTwister mt)
        {
            int l = array.Length;

            return array[mt.Next(l - 1)];
        }

    }
}
