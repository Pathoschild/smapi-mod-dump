using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.events;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class EventsLoader : IAssetEditor
    {
        private static readonly List<CustomEvent> CustomEvents = new List<CustomEvent>();

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Events\\Town");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\Events\\Town"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                foreach (CustomEvent customEvent in CustomEvents)
                {
                    data[customEvent.Key] = customEvent.Script;
                }
            }
        }

        public static void CheckEventDay()
        {
            CustomEvents.Clear();
            if (Context.IsMainPlayer && !DataLoader.ModConfig.DisableAnimalContest && AnimalContestController.IsContestDate())
            {
                AnimalContestController.CleanTemporaryParticipant();
                CustomEvents.Add(AnimalContestEventBuilder.CreateEvent(SDate.Now()));
                DataLoader.Helper.Content.InvalidateCache("Data\\Events\\Town");
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2640", DataLoader.i18n.Get("AnimalContest.Message.Name")) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2637"));
            }
        }

        public static void CheckUnseenEvents()
        {
            CustomEvents.ForEach(e =>
            {
                var id = Convert.ToInt32(e.Key.Split('/')[0]);
                if (!Game1.player.eventsSeen.Contains(id))
                {
                    if (FarmerLoader.FarmerData.AnimalContestData.Find(i => i.EventId == id) is var animalContestInfo && animalContestInfo != null)
                    {
                        AnimalContestController.EndEvent(animalContestInfo, false);
                    }
                }
            });
        }

        public static void BroadcastEvent(int evtId)
        {
            CustomEvent customEvent = CustomEvents.FirstOrDefault(e => e.Key.StartsWith(evtId + "/"));
            if (customEvent != null)
            {
                DataLoader.Helper.Multiplayer.SendMessage(customEvent, "animalContestEvent");
            }
        }

        public static void AddEvent(CustomEvent customEvent)
        {
            CustomEvents.Add(customEvent);
            DataLoader.Helper.Content.InvalidateCache("Data\\Events\\Town");
        }
    }
}
