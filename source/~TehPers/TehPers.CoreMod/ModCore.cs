using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.ContentPacks;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Integration;
using TehPers.CoreMod.Items;

namespace TehPers.CoreMod {
    public class ModCore : Mod {
        private CoreApiFactory _coreApiFactory;
        private readonly ItemDelegator _itemDelegator;
        
        public ModCore() {
            // Patch needs to happen in constructor otherwise it doesn't work with Better Artisan Good Icons for some reason.
            // If that mod patches SObject.drawWhenHeld before this mod patches SpriteBatch.Draw, then the items don't appear
            // in the farmer's hands when held.
            DrawingDelegator.PatchIfNeeded();

            // Also do other patches here because why not
            // TODO: MachineDelegator.PatchIfNeeded();

            // Create the item delegator
            this._itemDelegator = new ItemDelegator(this);
        }

        public override void Entry(IModHelper helper) {
            // Create the custom item sprite sheet
            this._coreApiFactory = new CoreApiFactory(this, this._itemDelegator);

            // Get this mod's core API
            ICoreApi coreApi = this._coreApiFactory.GetApi(this);

            // Register events for the item delegator
            this._itemDelegator.Initialize();

            // Register events for the machine delegator
            // TODO: MachineDelegator.RegisterEvents(this);

            // Load content packs after the game is launched
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => this.Helper.Events.GameLoop.UpdateTicking += this.UpdateTicking_LoadContentPacks;
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => this.LoadIntegrations(coreApi);

            // Add console commands
            this.RegisterConsoleCommands(coreApi);

            this.Monitor.Log("Core mod loaded!", LogLevel.Info);
        }

        private void LoadIntegrations(ICoreApi coreApi) {
            if (this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets") is IJsonAssetsApi jsonAssetsApi) {
                coreApi.Items.RegisterProvider(_ => new JsonAssetsItemProvider(coreApi, jsonAssetsApi));
            }
        }

        private void UpdateTicking_LoadContentPacks(object sender, UpdateTickingEventArgs e) {
            // Remove this event handler so it only fires once
            this.Helper.Events.GameLoop.UpdateTicking -= this.UpdateTicking_LoadContentPacks;

            // Load content packs
            ContentPackLoader contentPackLoader = new ContentPackLoader(this._coreApiFactory.GetApi(this));
            contentPackLoader.LoadContentPacks();
        }

        private void OnRenderingHud_DisplaySpriteSheet(object sender, RenderingHudEventArgs args) {
            args.SpriteBatch.Draw(this._itemDelegator.CustomItemSpriteSheet.TrackedTexture.CurrentTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);
        }

        public override object GetApi() {
            return this._coreApiFactory;
        }

        private void RegisterConsoleCommands(ICoreApi coreApi) {
            // Toggle sprite sheet command
            bool spriteSheetVisible = false;
            this.Helper.ConsoleCommands.Add("tcm_togglespritesheet", "Toggles drawing the custom item sprite sheet in the top left corner of the screen.", (s, strings) => {
                spriteSheetVisible = !spriteSheetVisible;
                if (spriteSheetVisible) {
                    this.Helper.Events.Display.RenderingHud += this.OnRenderingHud_DisplaySpriteSheet;
                } else {
                    this.Helper.Events.Display.RenderingHud -= this.OnRenderingHud_DisplaySpriteSheet;
                }
            });

            // Add item spawning command
            this.Helper.ConsoleCommands.Add("tcm_additem", "Adds an item to the player's inventory.\nUsage: core_additem <mod_id>:<item_name> [quantity]\n- mod_id: The unique ID of the mod that created the item\n- item_name: The key the item was registered as for that mod.\n- quantity: The quantity of the item to add to your inventory. Only applicable to stackable objects.", (s, args) => {
                // Check for arguments
                if (!args.Any()) {
                    this.Monitor.Log("Invalid usage: missing item key.", LogLevel.Error);
                    return;
                }

                // Item key
                if (!coreApi.Items.TryParseKey(args[0], out ItemKey itemKey)) {
                    this.Monitor.Log("Invalid usage: Item key is invalid.", LogLevel.Error);
                    return;
                }

                // Quantity
                if (args.Length <= 1 || !int.TryParse(args[1], out int quantity)) {
                    quantity = 1;
                }

                // Add items
                while (quantity-- > 0) {
                    if (!coreApi.Items.TryCreate(itemKey, out Item item)) {
                        this.Monitor.Log($"Invalid usage: Item key doesn't correspond to an item in this save: {itemKey}");
                        return;
                    }

                    Game1.player.addItemToInventory(item);
                }
            });

            // Add item list command
            this.Helper.ConsoleCommands.Add("tcm_listitems", "Lists all items registered through the item API.", (s, args) => {
                // Sort the registered item keys
                IOrderedEnumerable<ItemKey> registeredKeys = this._itemDelegator.GetRegisteredKeys().OrderBy(k => k.ToString(), StringComparer.OrdinalIgnoreCase);

                // Output each item
                this.Monitor.Log("Registered items:", LogLevel.Info);
                foreach (ItemKey key in registeredKeys) {
                    string index = coreApi.Items.TryCreate(key, out Item item) ? $"#{item.ParentSheetIndex}" : "No index assigned";
                    this.Monitor.Log($" - {key} ({index})", LogLevel.Info);
                }

                // Disclaimer
                this.Monitor.Log("Note: Only items registered through the API are listed. Some items may be registered through other mods and still have keys assigned to them.");
            });
        }
    }
}
