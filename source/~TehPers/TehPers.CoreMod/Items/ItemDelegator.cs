using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Events;
using TehPers.CoreMod.Api.Items.ItemProviders;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items.Crafting;

namespace TehPers.CoreMod.Items {
    internal class ItemDelegator : IItemDelegator {
        private const int STARTING_INDEX = 100000;
        private static readonly Regex _legacyKeyRegex = new Regex("^(?<modId>[^:]+):(?<localKey>.*)$");

        private readonly ModCore _coreMod;
        private readonly List<IItemProvider> _providers = new List<IItemProvider>();
        private readonly Dictionary<ItemKey, int> _itemIndexes = new Dictionary<ItemKey, int>();
        private readonly Dictionary<ItemKey, TrackedIndex> _trackedIndices = new Dictionary<ItemKey, TrackedIndex>();
        private IDictionary<ItemKey, int> _saveIndexes;
        private readonly HashSet<ItemKey> _registeredKeys = new HashSet<ItemKey>();
        private readonly CraftingManager _craftingManager;

        public DynamicSpriteSheet CustomItemSpriteSheet { get; private set; }

        public ItemDelegator(ModCore coreMod) {
            this._coreMod = coreMod;
            this._craftingManager = CraftingManager.GetCraftingManager(coreMod, this);
        }

        public void AddProvider(Func<IItemDelegator, IItemProvider> providerFactory) {
            if (providerFactory?.Invoke(this) is IItemProvider provider) {
                this._providers.Add(provider);
            } else if (providerFactory == null) {
                throw new ArgumentNullException(nameof(providerFactory));
            } else {
                throw new ArgumentException("The provider factory did not return an item provider", nameof(providerFactory));
            }
        }

        public bool TryRegisterKey(in ItemKey key) {
            if (!this._registeredKeys.Add(key)) {
                return false;
            }

            this._trackedIndices.Add(key, new TrackedIndex());
            return true;

        }

        public bool TryGetIndex(in ItemKey key, out int index) {
            return this._itemIndexes.TryGetValue(key, out index);
        }

        public void OverrideSprite(in ItemKey key, ISpriteSheet parentSheet, Action<IDrawingInfo, Vector2, Vector2> @override) {
            if (!this._trackedIndices.TryGetValue(key, out TrackedIndex trackedIndex)) {
                return;
            }

            // Override when the item is drawn - use trackedIndex so that a dictionary lookup operation isn't required everytime this sprite sheet is drawn
            parentSheet.Drawing += (sender, info) => {
                int drawnIndex = parentSheet.GetIndex(info.SourceRectangle?.X ?? 0, info.SourceRectangle?.Y ?? 0);
                if (!parentSheet.TryGetSprite(drawnIndex, out ISprite sprite) || drawnIndex != trackedIndex.Index) {
                    // Don't override unless it's drawing the current item
                    return;
                }

                // Override drawing
                int xOffset = (info.SourceRectangle?.X ?? 0) - sprite.U;
                int yOffset = (info.SourceRectangle?.Y ?? 0) - sprite.V;
                int sourceWidth = info.SourceRectangle?.Width ?? sprite.Width;
                int sourceHeight = info.SourceRectangle?.Height ?? sprite.Height;
                @override(info, new Vector2((float) xOffset / sprite.Width, (float) yOffset / sprite.Height), new Vector2((float) sourceWidth / sprite.Width, (float) sourceHeight / sprite.Height));
            };
        }

        public IEnumerable<IItemProvider> GetItemProviders() {
            return this._providers.AsEnumerable();
        }

        public string RegisterCraftingRecipe(IRecipe recipe) {
            return this._craftingManager.AddRecipe(recipe);
        }

        public void Initialize() {
            this.RegisterMultiplayerEvents();
            this.RegisterSaveEvents();
            this.CustomItemSpriteSheet = new DynamicSpriteSheet(this._coreMod);

            this._craftingManager.Initialize();
        }

        public bool TryParseKey(string source, out ItemKey itemKey) {
            // Try to match the source
            Match match = ItemDelegator._legacyKeyRegex.Match(source);
            if (!match.Success) {
                return false;
            }

            // Try to get the owner of the item key
            if (!(this._coreMod.Helper.ModRegistry.Get(match.Groups["modId"].Value) is IModInfo owner)) {
                return false;
            }

            // Create the item key and add it to the save IDs
            itemKey = new ItemKey(owner.Manifest, match.Groups["localKey"].Value);
            return true;
        }

        public IEnumerable<ItemKey> GetRegisteredKeys() {
            return this._registeredKeys.AsEnumerable();
        }

        private void RegisterMultiplayerEvents() {
            // Whenever item information is received, handle the message
            this._coreMod.Helper.Events.Multiplayer.ModMessageReceived += (sender, args) => {
                // Check if the message is from this mod
                if (args.FromModID != this._coreMod.ModManifest.UniqueID) {
                    return;
                }

                // Check the type of message
                if (args.Type == "indexes") {
                    // Process new item information
                    this._coreMod.Monitor.Log("Received item information from host.", LogLevel.Info);

                    if (!(args.ReadAs<Dictionary<ItemKey, int>>() is Dictionary<ItemKey, int> hostIndexes)) {
                        this._coreMod.Monitor.Log("Failed to get indexes from the host. No indexes will be assigned.", LogLevel.Error);
                        hostIndexes = new Dictionary<ItemKey, int>();
                    }

                    this.LoadIndexes(hostIndexes, false, missingItems => {
                        this._coreMod.Monitor.Log("Some items were registered by the server, but not by your client:", LogLevel.Error);

                        foreach ((ItemKey key, int index) in missingItems) {
                            this._coreMod.Monitor.Log($" - {key} (#{index})", LogLevel.Error);
                        }

                        this._coreMod.Monitor.Log("Those items may be buggy and won't render correctly. (They will appear as \"Error Item\"s for you)", LogLevel.Error);
                    }, newItems => {
                        this._coreMod.Monitor.Log("Some items were registered by your client, but not the server:", LogLevel.Warn);

                        foreach (ItemKey key in newItems) {
                            this._coreMod.Monitor.Log($" - {key}", LogLevel.Warn);
                        }

                        this._coreMod.Monitor.Log("Those items will not be available on this server. This might cause issues with some installed mods.", LogLevel.Warn);
                    });
                }
            };

            // Whenever a player connects to a game hosted by the current player, send them the required item information
            this._coreMod.Helper.Events.Multiplayer.PeerContextReceived += (sender, args) => {
                // Check if host
                if (!Context.IsMainPlayer) {
                    return;
                }

                // Check if the peer has this mod installed
                if (!args.Peer.HasSmapi || args.Peer.Mods.All(m => m.ID != this._coreMod.ModManifest.UniqueID)) {
                    return;
                }

                // Send item information
                this._coreMod.Monitor.Log($"Sending item information to new peer ({args.Peer.PlayerID})", LogLevel.Info);
                this._coreMod.Helper.Multiplayer.SendMessage(this._itemIndexes, "indexes", new[] { this._coreMod.ModManifest.UniqueID }, new[] { args.Peer.PlayerID });
            };
        }

        private void RegisterSaveEvents() {
            // Whenever a save is loaded, load item information unless the player is a farmhand on a multiplayer server
            this._coreMod.Helper.Events.GameLoop.SaveLoaded += (sender, args) => {
                // Check if this player is a farmhand on a multiplayer server
                if (Context.IsMultiplayer && !Context.IsMainPlayer) {
                    return;
                }

                // Load all the key <-> index mapping for this save
                this.LoadIndexesFromSave();
            };

            // Whenever the game is save, store the item information in the save as well
            this._coreMod.Helper.Events.GameLoop.Saving += (sender, args) => {
                this._coreMod.Monitor.Log("Saving mod indexes...", LogLevel.Trace);

                // Write all assigned item indexes
                this._coreMod.Helper.Data.WriteSaveData("index2", this._saveIndexes);

                this._coreMod.Monitor.Log("Done!", LogLevel.Trace);
            };

            // Whenever the player exits a save, reset all the indexes
            this._coreMod.Helper.Events.GameLoop.ReturnedToTitle += (sender, args) => {
                this._coreMod.Monitor.Log("Removing all item indexes");

                // Clear all assigned item indexes
                this._itemIndexes.Clear();
            };
        }

        private void LoadIndexesFromSave() {
            // Load indexes for mod items for this save
            this._coreMod.Monitor.Log("Loading indexes from save...", LogLevel.Trace);

            // Try to load the custom indexes
            if (!(this._coreMod.Helper.Data.ReadSaveData<Dictionary<ItemKey, int>>("indexes2") is Dictionary<ItemKey, int> saveIds)) {
                saveIds = new Dictionary<ItemKey, int>();

                // Try to load legacy indexes
                if (this._coreMod.Helper.Data.ReadSaveData<Dictionary<string, int>>("indexes") is Dictionary<string, int> legacyIds) {
                    // Try to convert each legacy key into a new key
                    foreach ((string key, int index) in legacyIds) {
                        if (this.TryParseKey(key, out ItemKey itemKey)) {
                            saveIds.Add(itemKey, index);
                        } else {
                            this._coreMod.Monitor.Log($"Error parsing legacy key \"{key}\". A new index will be assigned to items with this key, which may cause issues with existing save files.", LogLevel.Warn);
                        }
                    }
                }
            }

            this.LoadIndexes(saveIds, true, missingItems => {
                this._coreMod.Monitor.Log("Some items were detected in the save, but not registered:", LogLevel.Error);

                foreach ((ItemKey key, int index) in missingItems) {
                    this._coreMod.Monitor.Log($" - {key} (#{index})", LogLevel.Error);
                }

                this._coreMod.Monitor.Log("Those items may be buggy and won't render correctly. (They will become \"Error Item\"s)", LogLevel.Error);
            });
        }

        private void LoadIndexes(IDictionary<ItemKey, int> knownIndexes, bool newItemsAllowed, Action<IEnumerable<(ItemKey key, int value)>> missingItemsCallback, Action<IEnumerable<ItemKey>> newItemsCallback = null) {
            // Load indexes for mod items for this save
            this._coreMod.Monitor.Log("Assigning indexes...", LogLevel.Info);

            // Just to be safe, clear the item indexes, even though this should be unnecessary
            this._itemIndexes.Clear();
            this._saveIndexes = knownIndexes;

            // Copy the set of registered keys
            HashSet<ItemKey> remainingKeys = new HashSet<ItemKey>(this._registeredKeys);

            // Assign known indexes
            foreach ((ItemKey key, int index) in knownIndexes) {
                // Check if the key is registered and not already processed
                if (!remainingKeys.Remove(key)) {
                    continue;
                }

                // Assign the index to that key
                this._coreMod.Monitor.Log($" - {key} assigned known index {index}", LogLevel.Debug);
                this._itemIndexes.Add(key, index);
                this._trackedIndices[key].Index = index;
            }

            // Assign missing indexes if new items are allowed
            if (remainingKeys.Any()) {
                if (newItemsAllowed) {
                    this._coreMod.Monitor.Log("New items detected, adding indexes for them:", LogLevel.Info);
                    int highestIndex = ItemDelegator.STARTING_INDEX.Yield().Concat(this._itemIndexes.Values).Max();
                    foreach (ItemKey key in remainingKeys) {
                        ++highestIndex;
                        this._coreMod.Monitor.Log($" - {key} assigned new index {highestIndex}");
                        this._itemIndexes.Add(key, highestIndex);
                        this._saveIndexes.Add(key, highestIndex);
                        this._trackedIndices[key].Index = highestIndex;
                    }
                } else {
                    newItemsCallback?.Invoke(remainingKeys);
                }
            }

            // Check for any known indexes for unregistered items
            (ItemKey key, int value)[] notRegistered = knownIndexes.Where(kv => !this._registeredKeys.Contains(kv.Key)).Select(kv => (kv.Key, kv.Value)).ToArray();
            if (notRegistered.Any()) {
                missingItemsCallback?.Invoke(notRegistered);
            }

            // Invalidate each provider's assets now that indexes have been assigned
            this.InvalidateAssets();
        }

        private void InvalidateAssets() {
            this._coreMod.Monitor.Log("Invalidating provider assets...", LogLevel.Trace);

            // Invalidate each provider's assets
            foreach (IItemProvider provider in this._providers) {
                provider.InvalidateAssets();
            }
        }

        public virtual void OnRecipeCrafted(RecipeCraftedEventArgs obj) {
            this.RecipeCrafting?.Invoke(obj);
        }

        public event Action<RecipeCraftedEventArgs> RecipeCrafting;

        private class TrackedIndex {
            public int? Index { get; set; }
        }
    }
}