using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Multiplayer.Synchronized;

namespace TehPers.Core.Multiplayer {
    internal class MultiplayerApi : IMultiplayerApi {
        private const string SYNC_CHANNEL = "TehPers.Core.Multiplayer:synchronization";
        private const string SYNC_INIT_CHANNEL = MultiplayerApi.SYNC_CHANNEL + "-init";
        private static readonly Dictionary<string, ISynchronized> _synchronized = new Dictionary<string, ISynchronized>();
        private static readonly HashSet<string> _initialized = new HashSet<string>();
        private static readonly Dictionary<string, ITehCoreApi> _synchronizedApis = new Dictionary<string, ITehCoreApi>();
        private static bool _synchronizationInitialized = false;

        private readonly ITehCoreApi _core;

        internal MultiplayerApi(ITehCoreApi core) {
            this._core = core;
            this.ApplyPatches();
        }

        private void ApplyPatches() {
            this._core.Log("Multiplayer", "Setting up...", LogLevel.Trace);
            MessageDelegator.PatchIfNeeded();
        }

        /// <inheritdoc />
        public MessageHandler RegisterMessageHandler(string channel, MessageReader handler) {
            string apiChannel = this.GetApiChannel(channel);
            this._core.Log("Multiplayer", $"Registering handler for channel {apiChannel}", LogLevel.Debug);
            MessageHandler messageHandler = new MessageHandler(apiChannel, handler);

            // Try to register the handler
            if (!MessageDelegator.RegisterMessageHandler(messageHandler)) {
                // Conflict
                this._core.Log("Multiplayer", $"Failed to register handler for channel {apiChannel} due to an ID conflict", LogLevel.Warn);
                return null;
            }

            return messageHandler;
        }

        /// <inheritdoc />
        public void SendMessage(string channel, MessageWriter messageWriter) {
            string apiChannel = this.GetApiChannel(channel);
            this._core.Log("Multiplayer", $"Sending message on channel {apiChannel}", LogLevel.Trace);
            MessageDelegator.SendMessage(apiChannel, messageWriter);
        }

        /// <inheritdoc />
        public bool Synchronize(string id, ISynchronized target) {
            string apiId = this.GetApiSynchronizedId(id);
            this._core.Log("Multiplayer", $"Synchronizing {apiId}", LogLevel.Trace);

            // Try to add the synchronized object
            if (MultiplayerApi._synchronized.GetOrAdd(apiId, () => target) != target) {
                // Conflict
                this._core.Log("Multiplayer", $"Failed to synchronize {apiId} due to an ID conflict", LogLevel.Warn);
                return false;
            }

            // Request the value of the synchronized object
            MultiplayerApi._synchronizedApis[apiId] = this._core;
            MultiplayerApi.SetupSynchronizationIfNeeded();
            MultiplayerApi.InitSynchronized(apiId);
            return true;

        }

        /// <inheritdoc />
        public bool Desynchronize(string id) {
            string apiId = this.GetApiSynchronizedId(id);
            this._core.Log("Multiplayer", $"Desynchronizing {apiId}", LogLevel.Trace);
            MultiplayerApi._synchronizedApis.Remove(apiId);
            MultiplayerApi._initialized.Remove(apiId);
            return MultiplayerApi._synchronized.Remove(apiId);
        }

        private string GetApiChannel(string channel) {
            return $"{this._core.Owner.ModManifest.UniqueID}:{channel}";
        }

        private string GetApiSynchronizedId(string id) {
            return $"{this._core.Owner.ModManifest.UniqueID}:{id}";
        }

        private static void SetupSynchronizationIfNeeded() {
            if (MultiplayerApi._synchronizationInitialized) {
                return;
            }
            MultiplayerApi._synchronizationInitialized = true;

            // Listen for any synchronized values being initialized
            MessageDelegator.RegisterMessageHandler(new MessageHandler(MultiplayerApi.SYNC_INIT_CHANNEL, (sender, reader) => {
                // Read whether this is a request
                bool request = reader.ReadBoolean();

                // Read the ID of the object
                string id = reader.ReadString();

                if (request) {
                    MultiplayerApi.HandleInitRequest(id);
                } else {
                    MultiplayerApi.HandleInitResponse(id, reader);
                }
            }));

            // Listen for any changes to synchronized values
            MultiplayerApi.GetDeltas();

            // Push any changes to synchronized values to other players
            GameEvents.EighthUpdateTick += MultiplayerApi.SendDeltas;

            // Force all synchronized objects to be reinitialized
            SaveEvents.AfterLoad += (sender, args) => {
                foreach (string synchronizedId in MultiplayerApi._synchronized.Keys) {
                    MultiplayerApi.InitSynchronized(synchronizedId);
                }

                // Debug
                IEnumerable<IGrouping<ITehCoreApi, string>> apiGroups = from kv in MultiplayerApi._synchronizedApis
                                                                        group kv.Key by kv.Value into g
                                                                        select g;
                foreach (IGrouping<ITehCoreApi, string> group in apiGroups) {
                    group.Key.Log("Multiplayer", $"{group.Count()} synchronized object(s) being reinitialized", LogLevel.Trace);
                }
            };
        }

        private static void InitSynchronized(string id) {
            // Only request the initial value of the object if this is not the server
            if (Game1.IsServer) {
                MultiplayerApi._initialized.Add(id);
                return;
            }

            // Mark the object as uninitialized if it has already been initialized
            MultiplayerApi._initialized.Remove(id);

            // Request the full value of the synchronized object
            MessageDelegator.SendMessage(MultiplayerApi.SYNC_INIT_CHANNEL, writer => {
                // Write that this is a request
                writer.Write(true);

                // Write the ID of the object
                writer.Write(id);
            });
        }

        private static void HandleInitRequest(string id) {
            // Only handle requests if this is the server
            if (!Game1.IsServer) {
                return;
            }

            // Send the full value of the requested object if found. If not found, then oh well...
            MessageDelegator.SendMessage(MultiplayerApi.SYNC_INIT_CHANNEL, writer => {
                // Write that this is not a request
                writer.Write(false);

                // Write the ID of the object
                writer.Write(id);

                // Check if the object is being synchronized
                if (MultiplayerApi._synchronized.TryGetValue(id, out ISynchronized synchronized)) {
                    // Write that the object was found
                    writer.Write(true);

                    // Write the full value of the object to a buffer
                    byte[] data;
                    using (MemoryStream dataStream = new MemoryStream())
                    using (BinaryWriter dataWriter = new BinaryWriter(dataStream)) {
                        synchronized.WriteFull(dataWriter);
                        synchronized.MarkClean();
                        data = dataStream.ToArray();
                    }

                    // Write the buffer
                    writer.Write(data.Length);
                    writer.Write(data);
                } else {
                    // Write that the object was not found
                    writer.Write(false);
                }
            });
        }

        private static void HandleInitResponse(string id, BinaryReader reader) {
            // Only handle responses if this is not the server
            if (Game1.IsServer) {
                return;
            }

            // Get the core API object if it exists
            ITehCoreApi coreApi = MultiplayerApi._synchronizedApis.ContainsKey(id) ? MultiplayerApi._synchronizedApis[id] : null;

            // Read whether the object was found
            bool success = reader.ReadBoolean();

            // Check if it was unsuccessful
            if (!success) {
                coreApi?.Log("Multiplayer", $"Failed to initialize {id} because server is not tracking it", LogLevel.Warn);
                return;
            }

            // Read the data
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);

            // Check if an object with this ID is being synchronized
            if (!MultiplayerApi._synchronized.TryGetValue(id, out ISynchronized synchronized)) {
                coreApi?.Log("Multiplayer", $"Failed to initialize {id} because client is not tracking it", LogLevel.Trace);
                return;
            }

            // Read the data into the synchronized object
            using (MemoryStream dataStream = new MemoryStream(data))
            using (BinaryReader dataReader = new BinaryReader(dataStream)) {
                if (synchronized.Dirty) {
                    coreApi?.Log("Multiplayer", $"Initializing dirty object {id}", LogLevel.Warn);
                }

                dataStream.Position = 0;
                synchronized.ReadFull(dataReader);
                synchronized.MarkClean();
            }

            // Mark the object as initialized
            MultiplayerApi._initialized.Add(id);
        }

        private static void SendDeltas(object sender, EventArgs args) {
            KeyValuePair<string, ISynchronized>[] deltas = MultiplayerApi._synchronized.Where(kv => kv.Value.Dirty).ToArray();

            // Don't bother sending an update if nothing has changed
            if (!deltas.Any()) {
                return;
            }

            // Send an update to other players
            MessageDelegator.SendMessage(MultiplayerApi.SYNC_CHANNEL, writer => {
                // Write how many deltas there are
                writer.Write(deltas.Length);

                // Write each delta
                foreach (KeyValuePair<string, ISynchronized> delta in deltas) {
                    // Write the object's ID
                    writer.Write(delta.Key);

                    // Write the data to a buffer
                    byte[] data;
                    using (MemoryStream dataStream = new MemoryStream())
                    using (BinaryWriter dataWriter = new BinaryWriter(dataStream)) {
                        delta.Value.WriteDelta(dataWriter);
                        delta.Value.MarkClean();
                        data = dataStream.ToArray();
                    }

                    // Write the buffer
                    writer.Write(data.Length);
                    writer.Write(data);
                }
            });

            // Debug output
            var deltaGroups = from delta in deltas
                              where MultiplayerApi._synchronizedApis.ContainsKey(delta.Key)
                              group delta.Key by MultiplayerApi._synchronizedApis[delta.Key] into g
                              select new { Api = g.Key, Count = g.Count() };
            foreach (var group in deltaGroups) {
                group.Api.Log("Multiplayer", $"{group.Count} delta(s) sent", LogLevel.Trace);
            }
        }

        private static void GetDeltas() {
            MessageDelegator.RegisterMessageHandler(new MessageHandler(MultiplayerApi.SYNC_CHANNEL, (farmer, reader) => {
                // Read how many deltas there are
                int remaining = reader.ReadInt32();

                // Read each delta
                while (remaining-- > 0) {
                    // Get delta information
                    string id = reader.ReadString();
                    int length = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(length);

                    // Get the core API object if it exists
                    ITehCoreApi coreApi = MultiplayerApi._synchronizedApis.ContainsKey(id) ? MultiplayerApi._synchronizedApis[id] : null;

                    // Check if an object with this ID is being synchronized
                    if (!MultiplayerApi._synchronized.TryGetValue(id, out ISynchronized synchronized)) {
                        coreApi?.Log("Multiplayer", $"Failed to update {id} because client is not tracking it", LogLevel.Info);
                        continue;
                    }

                    // Read the data into the synchronized object
                    if (id == "TehPers.SpiritsEve:keys") {
                        // TODO: Debug stuff
                        // data = new byte[] { 3, 0, 0, 0, 2, 4, 0, 0, 0, 12, 0, 0, 0, 3, 0, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 4, 2, 0, 0, 0, 4, 0, 0, 0, 7, 0, 0, 0 };
                    }
                    using (MemoryStream dataStream = new MemoryStream(data))
                    using (BinaryReader dataReader = new BinaryReader(dataStream)) {
                        // If the object is dirty, then it should be fully updated to make sure it is synchronized correctly
                        bool needsFullUpdate = synchronized.Dirty;

                        dataStream.Position = 0;
                        synchronized.ReadDelta(dataReader);
                        synchronized.MarkClean();

                        if (needsFullUpdate) {
                            coreApi?.Log("Multiplayer", $"Dirty object {id} received a delta, so it will be reinitialized", LogLevel.Debug);

                            if (Game1.IsServer) {
                                // Broadcast the current value to everyone
                                MultiplayerApi.HandleInitRequest(id);
                            } else {
                                // Request the full value from the server
                                MultiplayerApi.InitSynchronized(id);
                            }
                        }
                    }
                }
            }));
        }
    }
}