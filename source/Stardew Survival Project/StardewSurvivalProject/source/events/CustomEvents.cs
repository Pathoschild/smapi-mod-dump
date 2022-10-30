/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Events;

namespace StardewSurvivalProject.source.events
{
    public class GiftEventArgs
    {
        internal GiftEventArgs(NPC npc, StardewValley.Object o)
        {
            this.Npc = npc;
            this.Gift = o;
        }

        public NPC Npc { get; }
        public StardewValley.Object Gift { get; }
    }

    class CustomEvents
    {
        public static event EventHandler OnItemEaten;

        public static event EventHandler OnToolUsed;

        public static event EventHandler<GiftEventArgs> OnGiftGiven;

        internal static void InvokeOnItemEaten(Farmer farmer)
        {
            if (CustomEvents.OnItemEaten == null || !farmer.IsLocalPlayer)
                return;

            var args = new EventArgs();
            var name = "CustomEvents.onItemEaten";

            foreach (EventHandler handler in CustomEvents.OnItemEaten.GetInvocationList())
            {
                try
                {
                    handler.Invoke(farmer, args);
                }
                catch (Exception e)
                {
                    LogHelper.Error($"Exception while handling event {name}:\n{e}");
                }
            }
        }

        internal static void InvokeOnToolUsed(Farmer farmer)
        {
            if (CustomEvents.OnToolUsed == null || !farmer.IsLocalPlayer)
                return;

            var args = new EventArgs();
            var name = "CustomEvents.onToolUsed";

            foreach (EventHandler handler in CustomEvents.OnToolUsed.GetInvocationList())
            {
                try
                {
                    handler.Invoke(farmer, args);
                }
                catch (Exception e)
                {
                    LogHelper.Error($"Exception while handling event {name}:\n{e}");
                }
            }
        }

        internal static void InvokeOnGiftGiven(NPC npc, StardewValley.Object gift, Farmer giver)
        {
            if (CustomEvents.OnGiftGiven == null)
                return;

            var args = new GiftEventArgs(npc, gift);
            var name = "CustomEvents.onGiftGiven";

            foreach (EventHandler<GiftEventArgs> handler in CustomEvents.OnGiftGiven.GetInvocationList())
            {
                try
                {
                    handler.Invoke(giver, args);
                }
                catch (Exception e)
                {
                    LogHelper.Error($"Exception while handling event {name}:\n{e}");
                }
            }
        }
    }

}
