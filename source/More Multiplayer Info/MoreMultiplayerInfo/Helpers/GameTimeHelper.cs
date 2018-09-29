namespace MoreMultiplayerInfo.Helpers
{
    public class GameTimeHelper
    {
        public static int GameTimeToMinutes(int input)
        {
            var hours = input / 100;
            var minutes = input % 100;

            return hours * 60 + minutes;
        }

    }
}
