/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-NFFTT
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-NFFTT
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using StardewValley.Characters;
using System.Collections.Generic;
using System.IO;

namespace NotFarFromTheTree {
    public class ModEntry : Mod, IAssetLoader {

        Dictionary<string, Texture2D> assets = new Dictionary<string, Texture2D>();
        Dictionary<string, ChildDat> childrenParent = new Dictionary<string, ChildDat>();

        /*
         * Mod Initializer
         */
        public override void Entry(IModHelper helper) {
            // Load event
            helper.Events.Specialized.LoadStageChanged += this.OnInitialLoad;

            // Move location event
            helper.Events.Player.Warped += this.OnSceneChanged;
            helper.Events.World.NpcListChanged += this.OnCharacterLoad;

            // When receiving messages from other mods
            helper.Events.Multiplayer.ModMessageReceived += this.OnMessageNotification;

            // Register command
            helper.ConsoleCommands.Add("child_parent", "Change the resemblance of one of your children to an NPC.\n\nUsage: child_parent <gender> <NPC>\n- gender: The gender of your child (\"boy\" or \"girl\").\n- NPC: An NPC from the town (IE; \"Harvey\").\n", this.CommandUpdateChild);
        }

        /*
         * When the player changes houses (Update overrides)
         */
        private void OnSceneChanged(object sender, WarpedEventArgs e) {
            Farmer player = e.Player;
            GameLocation location = e.NewLocation;
            
            // If we didn't enter a farmhouse, ignore
            if (!(location is FarmHouse))
                return;

            FarmHouse farmHouse = (FarmHouse)location;

            // Check that the farmhouse is owned by a player (Any player)
            Farmer farmHand = farmHouse.owner;
            if (farmHand == null)
                return;

            this.UpdateSprites( farmHouse, farmHand );
        }
        private void OnCharacterLoad(object sender, NpcListChangedEventArgs e) {
            if (Context.IsMainPlayer && Context.IsMultiplayer) {
                IEnumerable<NPC> added = e.Added;
                using (IEnumerator<NPC> characters = added.GetEnumerator()) {
                    while (characters.MoveNext()) {
                        NPC character = characters.Current;
                        if (character is Child && this.loadChildsParent( (Child)character, out ChildDat dat)) {
                            if ( dat != null )
                                this.sendToPeers( dat );
                        }
                    }
                }
            }
        }

        /*
         * When the mod first loads (Load initial overrides)
         */
        private void OnInitialLoad(object sender, LoadStageChangedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            GameLocation playerLocation = Game1.player.currentLocation;

            if (playerLocation is FarmHouse)
                this.UpdateSprites( (FarmHouse)playerLocation, Game1.player );
        }

        /*
         * Event Handlers
         */
        private void OnMessageNotification(object sender, ModMessageReceivedEventArgs e) {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "ChildUpdate") {
                ChildDat childDat = e.ReadAs<ChildDat>();
                Child child = getChildByName(childDat.child);
                NPC parent = Game1.getCharacterFromName<NPC>(childDat.parent, true);

                if (child != null) {
                    this.saveChildParent(
                        child,
                        parent,
                        Context.IsMainPlayer
                    );

                    if ((child.currentLocation != null) && child.currentLocation.Equals(Game1.currentLocation))
                        this.UpdateSprite( child, parent );
                }
            }
        }

        private void CommandUpdateChild(string command, string[] args) {
            // Get input length
            if (args.Length < 2) {
                if (args.Length == 0)
                    this.Monitor.Log("Must specify a <gender> and an <NPC>.", LogLevel.Error);
                else if (args.Length == 1)
                    this.Monitor.Log("Must specify an <NPC>.", LogLevel.Error);
                return;
            }

            // Get the inputted gender
            if (!((args[0] == "boy") || (args[0] == "girl"))) {
                this.Monitor.Log("Specified <gender> must be \"boy\" or \"girl\".", LogLevel.Error);
                return;
            }
            int gender = (args[0] == "boy" ? NPC.male : NPC.female);

            // Get the inputted spouse
            NPC parent = Game1.getCharacterFromName(args[1], true);
            if (parent == null) {
                this.Monitor.Log($"Specified <NPC> \"{args[1]}\" could not be located.", LogLevel.Error);
                return;
            }

            // Get players current location
            GameLocation location = Game1.player.currentLocation;
            if (!(location is FarmHouse)) {
                this.Monitor.Log("Could not location children (Not in a farmhouse?).", LogLevel.Error);
                return;
            }

            // Get the farmhouse
            FarmHouse farmHouse = (FarmHouse)location;
            if ((farmHouse.owner == null) || (!farmHouse.owner.Equals(Game1.player))) {
                this.Monitor.Log("Could not location children (This is not your house?).", LogLevel.Error);
                return;
            }

            // Get the child by gender
            Child child = null;
            foreach (Child test in farmHouse.getChildren())
                if (test.gender == gender)
                    child = test;
            if (child == null) {
                this.Monitor.Log("Specified <gender> could not be found (Don't have that child?).", LogLevel.Error);
                return;
            }

            this.UpdateSprite(child, parent);
            if (!this.saveChildParent(child, parent))
                Game1.addHUDMessage(new HUDMessage("Host is not using mod \"NotFarFromTheTree\". Updated Children will not save.", HUDMessage.error_type));
        }

        /*
         * Sprite Changer
         */
        private void UpdateSprites(FarmHouse farmHouse, Farmer houseOwner) {
            // Check that the farmhand has a spouse
            NPC spouse = houseOwner.getSpouse();
            
            // For each child in the house
            List<Child> children = farmHouse.getChildren();
            for (int i = 0; i < children.Count; i++) {
                Child child = children[i];

                // Get the childs NPC-parent
                NPC parent = this.getChildParentOrDefault(child, spouse);

                if (parent != null)
                    this.UpdateSprite(child, parent);
            }
        }
        private void UpdateSprite(Child child, NPC parent) {
            string childAssetPath = this.getAssetPath(child);

            // Check if the texture exists
            string assetPath = (parent == null ? null :
                Path.Combine("assets", parent.getName(), childAssetPath + ".png")
            );
            string absPath = (parent == null ? null :
                Path.Combine(Helper.DirectoryPath, assetPath)
            );

            if ((parent == null) || (!File.Exists(absPath))) {
                this.resetAsset(child);
                this.Monitor.Log($"Could not find file asset {assetPath}", LogLevel.Debug);
            } else {
                // Load the texture
                Texture2D assets = this.Helper.Content.Load<Texture2D>(assetPath, ContentSource.ModFolder);

                string childAssetName = this.getAssetName(child);

                // Store the texture for future loading
                this.assets[childAssetName] = assets;

                // Invalidate the currently loaded texture
                this.Helper.Content.InvalidateCache(childAssetName);
            }
        }

        /*
         * Asset Handling Methods
         */
        private string getAssetPath(Child child) {
            string age = this.getChildAge(child);
            return age + (child.gender == NPC.male || age.Equals("Baby") ? "" : "_girl") + (child.darkSkinned ? "_dark" : "");
        }

        private string getAssetName(Child child) {
            return "Characters\\" + this.getAssetPath( child );
        }

        private string getChildAge(Child child) {
            switch ( child.Age ) {
                case Child.newborn:
                case Child.baby:
                case Child.crawler:
                    return "Baby";
                case Child.toddler:
                    return "Toddler";
            }
            return null;
        }
        private string getChildGender(Child child) {
            return child.gender == NPC.male ? "Male" : "Female";
        }
        private string getChildSaveKey(Child child) {
            return $"nfftt_Child_{this.getChildGender(child)}_{child.idOfParent}";
        }

        private void resetAsset(Child child) {
            string assetName = this.getAssetName( child );
            
            // Remove the associated file from our cache
            this.assets.Remove(assetName);

            // Invalidate the games cache of the file
            this.Helper.Content.InvalidateCache(assetName);
        }

        /*
         * Saving / Updating Methods
         */
        private NPC getChildParentOrDefault(Child child, NPC spouse) {
            // Check for a saved parent
            if (this.loadChildsParent( child, out ChildDat data)) {
                // Get the NPC from the saved String
                NPC npc = Game1.getCharacterFromName(data.parent, true);
                if (npc != null)
                    return npc;
            } else if (spouse != null) // If no save data exists
                this.saveChildParent(child, spouse, Context.IsMainPlayer);
            
            return spouse;
        }
        private bool loadChildsParent(Child child, out ChildDat data) {
            string key = this.getChildSaveKey(child);
            if (this.childrenParent.TryGetValue(key, out ChildDat cache)) {
                // Attempt to read the value from the CACHE ARRAY
                return (
                    data = cache
                ) != null;
            } else if (Context.IsMainPlayer) {
                // Player Must be the HOST to Read from Save Data
                // (Other players must be synced from the HOST to the CACHE ARRAY)
                return (
                    data = Helper.Data.ReadSaveData<ChildDat>(key)
                ) != null;
            } else {
                data = null;
                return false;
            }
        }

        private bool saveChildParent(Child child, NPC parent) {
            return this.saveChildParent(child, parent, true);
        }
        private bool saveChildParent(Child child, NPC parent, bool notify) {
            bool saveSuccess = false;

            ChildDat childDat = new ChildDat( child.getName(), parent.getName() );
            string saveKey = this.getChildSaveKey( child );

            // Save the parent of the child
            this.childrenParent[saveKey] = childDat;
            if (Context.IsMainPlayer) {
                this.Helper.Data.WriteSaveData(saveKey, childDat);
                saveSuccess = true;
            }

            // Update other players in multiplayer
            if (Context.IsMultiplayer && notify) {
                if (this.sendToPeers(childDat))
                    saveSuccess = true;
            }

            return saveSuccess;
        }
        private bool sendToPeers(ChildDat childDat) {
            bool sent = false;

            foreach (IMultiplayerPeer peer in this.Helper.Multiplayer.GetConnectedPlayers()) {
                // If peer is the host, have them update the save
                if ((!Context.IsMainPlayer) && peer.IsHost && (peer.GetMod(this.ModManifest.UniqueID) != null))
                    sent = true;
                // Only send if host or to host
                if (Context.IsMainPlayer || (sent && peer.IsHost)) {
                    this.Helper.Multiplayer.SendMessage(childDat, "ChildUpdate", modIDs: new[] {
                        this.ModManifest.UniqueID
                    });
                }
            }

            return sent;
        }

        /*
         * Asset Handling Overrides
         */
        public bool CanLoad<T>(IAssetInfo asset) {
            return this.assets.ContainsKey(asset.AssetName);
        }

        public T Load<T>(IAssetInfo asset) {
            return (T)(object)this.assets[asset.AssetName];
        }

        public static Child getChildByName(string name) {
            Child child = null;
            // Check primary farmhouse
            FarmHouse farmHouse = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            foreach (NPC character in farmHouse.characters) {
                if ((character is Child) && character.getName().Equals(name))
                    return (Child)character;
            }

            // Check farmhand farmhouses
            Farm farm = Game1.getFarm();
            if (farm != null) {
                foreach(Building building in farm.buildings) {
                    if (building.indoors.Value != null) {
                        foreach (NPC character in building.indoors.Value.characters) {
                            if ((character is Child) && character.getName().Equals(name))
                                return (Child)character;
                        }
                    }
                }
            }
            return child;
        }

        public class ChildDat {
            public string child { get; set; }
            public string parent { get; set; }

            public ChildDat(string childName, string parentNPC) {
                this.child = childName;
                this.parent = parentNPC;
            }

        }
    }
}
