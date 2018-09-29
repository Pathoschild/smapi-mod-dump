using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace WaitAroundSMAPI
{
    internal class WaitAroundMod : Mod
    {
        private static int LatestTime = 2550;
        private int _timeToWait;
        public int timeToWait
        {
            get { return _timeToWait; }
            set
            {
                if (value < 0)
                {
                    return;
                }
                int newTime = getTimeFromOffset(Game1.timeOfDay, value);
                if (newTime > LatestTime)
                {
                    _timeToWait = getOffsetFromTimes(Game1.timeOfDay, LatestTime);
                }
                else
                {
                    _timeToWait = value;
                }
            }
        }
        private WaitAroundMenu waitMenu { get; set; }
        private WaitAroundConfig config { get; set; }

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<WaitAroundConfig>();
            InputEvents.ButtonPressed += KeyPressed;
        }

        public void KeyPressed(object sender, EventArgsInput e)
        {
            if (e.Button == config.menuKey && Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu == null && Context.IsPlayerFree)
                    Game1.activeClickableMenu = new WaitAroundMenu(this);
                else if(Game1.activeClickableMenu is WaitAroundMenu)
                    ((WaitAroundMenu)Game1.activeClickableMenu).Close();
            }
        }

        public static int getTimeFromOffset(int startTime, int offset)
        {
            int time = startTime;
            for (; offset > 0; offset -= 10)
            {
                time += 10;
                if (time % 100 == 60)
                {
                    time += 40;
                }
            }
            return time;
        }

        public static int getOffsetFromTimes(int startTime, int endTime)
        {
            int offset = 0;
            for (int i = startTime + 10; i - 10 < endTime; i += 10)
            {
                offset += 10;
                if (i % 100 == 60)
                {
                    i += 40;
                }
            }
            return offset;
        }
    }
}
