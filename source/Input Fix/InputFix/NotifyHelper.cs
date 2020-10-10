/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace InputFix
{
    public enum NotifyPlace
    {
        Monitor,
        GameHUD,
        MonitorAndGameHUD
    }

    public enum NotifyMoment
    {
        Immediate,
        GameLaunched,
        SaveLoaded
    }

    public class NotifyHelper
    {
        private IModHelper helper;
        private IMonitor monitor;

        public NotifyHelper(IMonitor monitor, IModHelper helper)
        {
            this.monitor = monitor;
            this.helper = helper;
        }

        public void Notify(string text, NotifyPlace place, NotifyMoment moment, LogLevel level = LogLevel.Info)
        {
            switch (moment)
            {
                case NotifyMoment.Immediate:
                    doNotify(place, text, level);
                    break;

                case NotifyMoment.GameLaunched:
                    helper.Events.GameLoop.GameLaunched += new System.EventHandler<StardewModdingAPI.Events.GameLaunchedEventArgs>((sender, e) =>
                    {
                        doNotify(place, text, level);
                    });
                    break;

                case NotifyMoment.SaveLoaded:
                    helper.Events.GameLoop.SaveLoaded += new System.EventHandler<StardewModdingAPI.Events.SaveLoadedEventArgs>((sender, e) =>
                    {
                        doNotify(place, text, level);
                    });
                    break;
            }
        }

        private void doNotify(NotifyPlace place, string text, LogLevel level)
        {
            switch (place)
            {
                case NotifyPlace.Monitor:
                    NotifyMonitor(text, level);
                    break;

                case NotifyPlace.GameHUD:
                    NotifyHUD(text);
                    break;

                case NotifyPlace.MonitorAndGameHUD:
                    NotifyHUD(text);
                    NotifyMonitor(text, level);
                    break;
            }
        }

        public void NotifyHUD(string text)
        {
            var msg = new HUDMessage(text);
            msg.noIcon = true;
            Game1.addHUDMessage(msg);
            msg.timeLeft = 500 * msg.message.Length;
        }

        public void NotifyMonitor(string text, LogLevel level = LogLevel.Info)
        {
            monitor.Log(text, level);
        }

        public void NotifyMonitorOnce(string text, LogLevel level = LogLevel.Info)
        {
            monitor.LogOnce(text, level);
        }
    }
}