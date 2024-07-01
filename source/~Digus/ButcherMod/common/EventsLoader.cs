/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.events;
using AnimalHusbandryMod.farmer;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class EventsLoader
    {
        private static readonly List<CustomEvent> CustomEvents = new List<CustomEvent>();

        private const string SyncEventKey = "DIGUS.ANIMALHUSBANDRYMOD/AnimalContest.SyncEvent";

        private const char EventKeySeparator = ':';

        public void Edit(object sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.IsEquivalentTo("Data/Events/Town"))
            {
                args.Edit(asset =>
                {
                    if (DataLoader.ModConfig.DisableAnimalContest) return;
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (CustomEvent customEvent in CustomEvents)
                    {
                        data[customEvent.Key] = customEvent.Script;
                    }
                    CheckSyncEvent(data);
                });
            }
        }

        public static void CheckEventDay()
        {
            if (Context.IsMainPlayer)
            {
                CustomEvents.Clear();
                if (!DataLoader.ModConfig.DisableAnimalContest && AnimalContestController.IsContestDate())
                {
                    AnimalContestController.CleanTemporaryParticipant();
                    CustomEvent customEvent = AnimalContestEventBuilder.CreateEvent(SDate.Now());
                    SyncEvent(customEvent);
                    CustomEvents.Add(customEvent);
                    DataLoader.Helper.GameContent.InvalidateCache("Data/Events/Town");
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2640", DataLoader.i18n.Get("AnimalContest.Message.Name")) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2637"));
                }
                else
                {
                    Game1.getFarm().modData.Remove(SyncEventKey);
                }

                AnimalContestController.UpdateContestCount();
            }
        }

        public static void CheckUnseenEvents()
        {
            if (Context.IsMainPlayer)
            {
                CustomEvents.ForEach(e =>
                {
                    var id = e.Key.Split('/')[0];
                    if (!Game1.player.eventsSeen.Contains(id))
                    {
                        if (FarmerLoader.FarmerData.AnimalContestData.Find(i => i.EventId == id) is var
                                animalContestInfo && animalContestInfo != null)
                        {
                            AnimalContestController.EndEvent(animalContestInfo, false);
                        }
                    }
                });
            }
        }

        public static void EventListener()
        {
            Game1.getFarm().modData.OnValueAdded += (k, v) =>
            {
                if (k == "TempEventScript")
                {
                    DataLoader.Helper.GameContent.InvalidateCache("Data/Events/Town");
                }
            };
        }

        private static void SyncEvent(CustomEvent customEvent)
        {
            Game1.getFarm().modData[SyncEventKey] = customEvent.Key + EventKeySeparator + customEvent.Script;
        }

        private static void CheckSyncEvent(IDictionary<string, string> eventData)
        {
            if (!Context.IsMainPlayer && Game1.getFarm().modData.ContainsKey(SyncEventKey))
            {
                string eventInfo = Game1.getFarm().modData[SyncEventKey];
                string eventKey = eventInfo.Split(EventKeySeparator)[0];
                string eventScript = eventInfo.Substring(eventInfo.IndexOf(EventKeySeparator) + 1);
                eventData[eventKey] = eventScript;
            }
        }
    }
}
