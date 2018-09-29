using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace LogDebugStuffEtc
{
    public class Class1 : Mod
    {
        private IModHelper _modHelper;
        private bool _eventApplied;

        public override void Entry(IModHelper helper)
        {
            _modHelper = helper;
            InputEvents.ButtonPressed += LogStuff;


        }

        private void LogStuff(object sender, EventArgsInput e)
        {
            if (e.Button == SButton.NumPad0)
            {
                foreach (var player in Game1.getOnlineFarmers())
                {
                    var eventsSeen = string.Join(", ", (IList<int>)player.eventsSeen);

                    Monitor.Log($"{player.Name} Events Seen: ({eventsSeen})");

                    var dialogEvents = string.Join(", ", player.activeDialogueEvents.Values);

                    Monitor.Log($"{player.Name} Active Dialog Events: ({dialogEvents})");

                    Monitor.Log($"{player.Name} Hidden? {player.hidden.ToString()}");
                }
            }
        }
    }
}
