using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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
        private WaitAroundConfig config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<WaitAroundConfig>();

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == config.menuKey && Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu == null && Context.IsPlayerFree)
                    Game1.activeClickableMenu = new WaitAroundMenu(this);
                else if (Game1.activeClickableMenu is WaitAroundMenu)
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
