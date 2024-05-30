/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class EventManager
    {
        public ManagedEvent<ItemReceivedEventArgs> ItemCollected { get; }

        public ManagedEvent<ItemReceivedEventArgs> ItemCrafted { get; }

        public ManagedEvent<GiftEventArgs> ItemGifted { get; }

        public ManagedEvent<SalableEventArgs> SalablePurchased { get; }

        public ManagedEvent<SalableEventArgs> SalableSold { get; }

        public ManagedEvent<FarmAnimalEventArgs> FarmAnimalPurchased { get; }

        public ManagedEvent<FarmAnimalEventArgs> FarmAnimalSold { get; }

        public BuildingConstructedEvent BuildingConstructed { get; }

        public IModEvents ModEvents { get; }

        public IMonitor Monitor { get; }

        public EventManager(IModEvents modEvents, IMultiplayerHelper multiplayer, IMonitor monitor)
        {
            ModEvents = modEvents;
            Monitor = monitor;

            ItemCollected = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCollected));
            ItemCrafted = new ManagedEvent<ItemReceivedEventArgs>(nameof(ItemCrafted));
            ItemGifted = new ManagedEvent<GiftEventArgs>(nameof(ItemGifted));
            SalablePurchased = new ManagedEvent<SalableEventArgs>(nameof(SalablePurchased));
            SalableSold = new ManagedEvent<SalableEventArgs>(nameof(SalableSold));
            FarmAnimalPurchased = new ManagedEvent<FarmAnimalEventArgs>(nameof(FarmAnimalPurchased));
            FarmAnimalSold = new ManagedEvent<FarmAnimalEventArgs>(nameof(FarmAnimalSold));
            BuildingConstructed = new BuildingConstructedEvent(nameof(BuildingConstructed), multiplayer);

            ModEvents.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == DeluxeJournalMod.Instance?.ModManifest.UniqueID)
            {
                if (GetType().GetProperty(e.Type, BindingFlags.Public | BindingFlags.Instance)?.GetValue(this) is IManagedNetEvent managed)
                {
                    try
                    {
                        managed.RaiseFromMessage(sender, e);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log(string.Format("Failed to raise broadcasted event '{0}'. See log file for details.", e.Type), LogLevel.Error);
                        Monitor.Log(ex.ToString(), LogLevel.Trace);
                    }
                }
            }
        }
    }
}
