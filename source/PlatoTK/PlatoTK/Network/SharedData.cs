/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatoTK.Network
{
    internal enum SharedDataChangeType
    {
        Set = 0,
        Remove = 1,
    }
    internal class SharedDataChange
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public int ChangeType { get; set; }

        public long ChangedBy { get; set; }
    }

    internal class SharedData
    {
        public string Key { get; }

        public string Value { get; }

        public SharedData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    internal class SharedDataCollection : IDisposable
    {
        internal readonly HashSet<SharedData> Collection;

        internal readonly string Id;

        internal readonly IPlatoHelper Helper;

        internal readonly HashSet<Delegate> Observers;

        internal string AssignedId => Helper.ModHelper.ModRegistry.ModID + ":" + Id;

        public SharedDataCollection(IPlatoHelper helper, string id)
        {
            Id = id;
            Helper = helper;
            Collection = new HashSet<SharedData>();
            Observers = new HashSet<Delegate>();
            StartListener();
        }

        private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == Helper.ModHelper.ModRegistry.ModID && e.Type == $"SharedChange:{ AssignedId }")
                ParseChange(e.ReadAs<SharedDataChange>());
        }

        public void Remove(string key)
        {
            if (Collection.FirstOrDefault(entry => entry.Key == key) is SharedData data)
            {
                RegisterChange(data, SharedDataChangeType.Remove);
                Collection.Remove(data);
            }
        }

        public void Set<T>(string key, T value) where T : class, new()
        {
            var data = new SharedData(key, Utils.Serialization.SerializeValue(value));

            if(Collection.FirstOrDefault(e => e.Key == key) is SharedData existingData)
            {
                if (existingData.Value == data.Value)
                    return;

                Collection.Remove(existingData);
            }

            RegisterChange(data, SharedDataChangeType.Set);
            Collection.Add(data);
        }

        public T Get<T>(string key, bool allowNull) where T : class, new()
        {
            if (Collection.FirstOrDefault(e => e.Key == key) is SharedData data)
                return Utils.Serialization.DeserializeValue<T>(data.Value);

                return allowNull ? null : new T();
        }

        public void ShareChange(SharedDataChange data)
        {
            Helper.ModHelper.Multiplayer.SendMessage(
                message: data,
                messageType: $"SharedChange:{AssignedId}",
                modIDs: new string[] { Helper.ModHelper.ModRegistry.ModID });
        }

        public void ParseChange(SharedDataChange change)
        {
            if (change.ChangeType == (int)SharedDataChangeType.Remove)
            {
                if (Collection.FirstOrDefault(e => e.Key == change.Key) is SharedData data) {
                    Collection.Remove(data);
                    foreach (var observer in Observers)
                        observer.DynamicInvoke(new CollectionChangeArgs(data.Key, change.ChangedBy, true, data.Value, null));
                }
            }
            else
            {
                SharedData data = new SharedData(change.Key, change.Value);

                SharedData oldData = null;

                if (Collection.FirstOrDefault(e => e.Key == change.Key) is SharedData existingData)
                {
                    oldData = existingData;
                    Collection.Remove(existingData);
                }

                Collection.Add(data);

                foreach (var observer in Observers)
                    observer.DynamicInvoke(new CollectionChangeArgs(data.Key, change.ChangedBy, true, oldData.Value, data.Value));

            }
        }

        public void RegisterObserver(Delegate observer)
        {
            Observers.Add(observer);
        }

        public void RegisterChange(SharedData data, SharedDataChangeType type)
        {
            ShareChange(new SharedDataChange()
            {
                ChangeType = (int)type,
                Key = data.Key,
                Value = type != SharedDataChangeType.Remove ? data.Value : "",
                ChangedBy = Game1.player.UniqueMultiplayerID
            });
        }

        private void StopListener()
        {
            Helper.ModHelper.Events.Multiplayer.ModMessageReceived -= Multiplayer_ModMessageReceived;
        }

        private void StartListener()
        {
            Helper.ModHelper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
        }

        public void Dispose()
        {
            StopListener();
        }
    }
}
