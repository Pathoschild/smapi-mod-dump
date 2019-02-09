using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Machines;
using TehPers.CoreMod.Api.Structs;
using Object = StardewValley.Object;

namespace TehPers.Logistics {
    public class StoneConverterMachine : ModCraftable, IMachine {
        public StoneConverterMachine(IMod owner, string rawName, TextureInformation textureInfo) : base(owner, rawName, 500, textureInfo) { }

        public void AfterDraw(IMachineInformation machineInformation, SpriteBatch spriteBatch, int x, int y) { }

        public IEnumerable<IEnumerable<ObjectRequest>> Accept(IMachineInformation machineInfo, IEnumerable<ObjectRequest> items, Vector2 inputTile, out MachineAction doAccept) {
            State state = machineInfo.GetState<State>();

            // Check if already processing
            if (state.Processing) {
                doAccept = default;
                return null;
            }

            // Get all available stone
            IEnumerable<ObjectRequest> stone = items.Where(i => i.Item.ParentSheetIndex == Objects.Stone);

            // Create requests for up to 100 stone
            List<ObjectRequest> requests = new List<ObjectRequest>();
            int remaining = 100;
            foreach (ObjectRequest request in stone) {
                // Create a new request for stone
                if (request.Quantity <= remaining) {
                    // Requests are structs, so it's fine to just pass it around like this
                    requests.Add(request);
                    remaining -= request.Quantity;
                } else {
                    requests.Add(new ObjectRequest(request.Item, remaining));
                    remaining = 0;
                }

                // Check if reached 100 stone
                if (remaining == 0) {
                    // If the machine actually accepts something, make sure to update the state of the machine
                    doAccept = payload => {
                        state.Processing = true;
                        state.StartTime = SDateTime.Now;
                    };

                    // Return the requested stone as a single payload
                    return requests.AsEnumerable().Yield();
                }
            }

            // There isn't 100 stone available, so return null
            doAccept = default;
            return null;
        }

        public IEnumerable<IEnumerable<ObjectRequest>> Eject(IMachineInformation machineInfo, IEnumerable<ObjectRequest> items, Vector2 outputTile, out MachineAction doEject) {
            State state = machineInfo.GetState<State>();

            // Check if not done
            if (!(state.Processing && (SDateTime.Now - state.StartTime).TotalMinutes >= 60)) {
                doEject = null;
                return null;
            }

            // If the machine actually ejects something, make sure to update the state of the machine
            doEject = payload => state.Processing = false;

            // Return a new diamond payload
            return new ObjectRequest(new Object(Vector2.Zero, Objects.Diamond, 1)).Yield().Yield();
        }

        public void Placed(IMachineInformation machineInfo) {
            machineInfo.SetState(new State());
        }

        public void Removed(IMachineInformation machineInfo) { }

        public void UpdateTick(IMachineInformation machineInfo) { }

        public IEnumerable<ObjectRequest> InsertItem(IMachineInformation machineInfo, Object heldObject, IEnumerable<Item> inventory, Farmer source, out Action doInsert) {
            State state = machineInfo.GetState<State>();

            // Check if already processing
            if (state.Processing) {
                doInsert = default;
                return null;
            }

            // Check if enough stone is being held
            if (heldObject.ParentSheetIndex == Objects.Stone) {
                // If stone is actually inserted into the machine, make sure to update the state
                doInsert = () => {
                    state.Processing = true;
                    state.StartTime = SDateTime.Now;
                };

                // Request 100 stone
                return new ObjectRequest(heldObject, 100).Yield();
            }

            // Don't request anything
            doInsert = default;
            return null;
        }

        public IEnumerable<ObjectRequest> RemoveItem(IMachineInformation machineInfo, Farmer source, out Action doRemove) {
            State state = machineInfo.GetState<State>();

            // Check if done
            int elapsedMinutes = (SDateTime.Now - state.StartTime).TotalMinutes;
            if (state.Processing && elapsedMinutes >= 60) {
                // When the payload is removed from this machine, update the state
                doRemove = () => state.Processing = false;

                // Return a single diamond payload that this machine can provide
                return new ObjectRequest(new Object(Vector2.Zero, Objects.Diamond, 1)).Yield();
            }

            // If the machine actually ejects something, make sure to update the state of the machine
            doRemove = default;
            return null;
        }

        public class State {
            public bool Processing { get; set; } = false;
            public SDateTime StartTime { get; set; }
        }
    }
}