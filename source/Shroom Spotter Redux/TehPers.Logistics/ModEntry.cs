/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using SObject = StardewValley.Object;

namespace TehPers.Logistics {
    public class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            // Register an event for the first update tick to handle all core API calls
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => {
                if (helper.ModRegistry.GetApi("TehPers.CoreMod") is Func<IMod, ICoreApi> coreApiFactory) {
                    // Create core API
                    ICoreApi coreApi = coreApiFactory(this);

                    // Register custom machines
                    this.RegisterMachines(coreApi);
                }
            };
        }

        private void RegisterMachines(ICoreApi coreApi) {
            this.Monitor.Log("Registering machines...", LogLevel.Info);
            IItemApi itemApi = coreApi.Items;

            // Stone converter
            TextureInformation textureInfo = new TextureInformation(Game1.bigCraftableSpriteSheet, new Rectangle(16 * 3, 32 * 2, 16, 32));
            StoneConverterMachine stoneConverter = new StoneConverterMachine(this, "stoneConverter", textureInfo);
            itemApi.Register("stoneConverter", stoneConverter);
            
            // TODO: debug
            this.Helper.Events.Input.ButtonPressed += (sender, args) => {
                if (args.Button == SButton.NumPad3 && itemApi.TryGetInformation("stoneConverter", out IObjectInformation info) && info.Index is int index) {
                    SObject machine = new SObject(Vector2.Zero, index, false);
                    Game1.player.addItemToInventory(machine);
                }
            };

            this.Monitor.Log("Done");
        }
    }
}