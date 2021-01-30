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

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace NotFarFromTheTree {
    public class ChildDat {
        public static readonly ConcurrentDictionary<Child, Lazy<ChildDat>> CHILDREN_PARENT = new ConcurrentDictionary<Child, Lazy<ChildDat>>();
        
        private Child instance;
        private AnimatedSprite Sprite {
            get => this.instance.Sprite;
            set => this.instance.Sprite = value;
        }
        private string name;
        private NPC parent;
        private ModDataDictionary Data => this.instance.modData;
        
        /*
         * Age Checks
         */
        private int Age => this.instance.Age;
        private bool IsBaby => !this.IsToddler;
        private bool IsToddler => this.Age == Child.toddler;
        private string AgeString => this.IsToddler ? "Toddler" : "Baby";
        
        /*
         * Gender Checks
         */
        private int Gender => this.instance.Gender;
        private bool IsMale => this.Gender == NPC.male;
        
        private bool IsDarkSkin => this.instance.darkSkinned.Value;
        
        /*
         * Parent Checks
         */
        private bool IsEmpty => this.IsOrphan || string.IsNullOrWhiteSpace(this.ChildName);
        private bool IsOrphan => string.IsNullOrWhiteSpace(this.ParentName) || !this.HasSprite;
        private bool HasSprite;
        
        /*
         * Names
         */
        public string ChildName {
            get => this.name;
            set {
                this.name = value;
                if (this.instance == null || this.instance.Name != value)
                    this.instance = ChildDat.GetByName(value);
            }
        }
        public string ParentName {
            get => this.parent?.getName();
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    this.parent = null;
                } else {
                    NPC npc = Game1.getCharacterFromName<NPC>(value, true);
                    if (npc != null) {
                        // If the new value is different than the current value
                        bool updated = !npc.getName().Equals(this.ParentName);
                        
                        // Set the new parent value
                        this.parent = npc;
                        
                        if (this.parent != null && updated) {
                            // Load the new sprite to use
                            string spritePath = this.GetAssetPath(npc.getName());
                            this.HasSprite = Assets.LoadSprite(Assets.Wrap(spritePath), spritePath);
                            
                            // Refresh the sprite
                            this.CleanSprite();
                        }
                    }
                }
            }
        }
        
        public ChildDat() {}
        private ChildDat( Child child ) {
            this.instance = child;
            this.ChildName = child.Name;
            this.ParentName = "";
        }
        
        /*
         * Asset Handling Methods
         */
        
        public string GetAssetPath() => this.IsOrphan ? this.GetDefaultAssetPath() : this.GetAssetPath(this.ParentName);
        public string GetAssetPath(string parentName) => $"{parentName}{Path.DirectorySeparatorChar}{this.GetDefaultAssetPath()}";
        public string GetDefaultAssetPath() => $"{this.AgeString}{(this.IsMale || this.IsBaby ? "" : "_girl") + (this.IsDarkSkin ? "_dark" : "")}";
        
        public string GetAssetName() => this.IsOrphan ? this.GetDefaultAssetName() : Assets.Wrap(this.GetAssetPath());
        public string GetDefaultAssetName() => $"Characters{Path.DirectorySeparatorChar}{this.GetDefaultAssetPath()}";
        
        /*
         * Current Sprite Handling
         */
        
        public void CleanSprite() {
            if (this.instance == null)
                return;
            
            // Get the name of the asset to use
            string asset = this.GetAssetName();
            
            if (this.Sprite == null)
                this.Sprite = new AnimatedSprite(asset, 0, 22, 16);
            if (this.Age >= 3) {
                this.Sprite.textureName.Value = asset;
                this.Sprite.SpriteWidth = 16;
                this.Sprite.SpriteHeight = 32;
                this.Sprite.currentFrame = 0;
                this.instance.HideShadow = false;
            } else {
                this.Sprite.textureName.Value = asset;
                this.Sprite.SpriteWidth = 22;
                this.Sprite.SpriteHeight = this.instance.Age == 1 ? 32 : 16;
                this.Sprite.currentFrame = 0;
                if (this.Age == 1)
                    this.Sprite.currentFrame = 4;
                else if (this.Age == 2)
                    this.Sprite.currentFrame = 32;
                this.instance.HideShadow = true;
            }
            
            this.Sprite.UpdateSourceRect();
            this.instance.Breather = false;
        }
        
        /*
         * Information Handling
         */
        
        public bool Save( bool notify = true ) {
            bool saveSuccess = false;
            
            // Save the parent of the child
            ChildDat.CHILDREN_PARENT[this.instance] = new Lazy<ChildDat>(() => this);
            if (Context.IsMainPlayer) {
                this.Data[this.GetSaveKey()] = this.ParentName;
                saveSuccess = true;
            }
            
            // Update other players in multiplayer
            if (Context.IsMultiplayer && notify && this.SendToPeers())
                saveSuccess = true;
            
            return saveSuccess;
        }
        public string GetSaveKey() {
            return $"NFFTT/Child/{this.ChildName}";
        }
        
        public bool SendToPeers() {
            bool sent = false;
            
            foreach (IMultiplayerPeer peer in ModEntry.MOD_HELPER.Multiplayer.GetConnectedPlayers()) {
                // If peer is the host, have them update the save
                if ((!Context.IsMainPlayer) && peer.IsHost && (peer.GetMod(ModEntry.MOD_ID) != null))
                    sent = true;
                
                // Only send if host or to host
                if (Context.IsMainPlayer || (sent && peer.IsHost)) {
                    ModEntry.MOD_HELPER.Multiplayer.SendMessage(this, "ChildUpdate", modIDs: new[] {
                        ModEntry.MOD_ID
                    });
                }
            }
            
            return sent;
        }
        
        /*
         * Find a child in the game
         */
        public static Child GetByName( string name ) {
            // Check primary farmhouse
            if (Game1.getLocationFromName("FarmHouse") is FarmHouse farmHouse)
                foreach (NPC character in farmHouse.characters.Where(character => (character is Child) && character.getName().Equals(name)))
                    return (Child) character;
            
            Farm farm;
            if ((farm = Game1.getFarm()) == null)
                return null;
            
            // Check farmhand farmhouses
            Child child = null;
            foreach (NPC character in farm.buildings.Where(building => building.indoors.Value != null).SelectMany(building => building.indoors.Value.characters)) {
                if ((child = character as Child) != null && child.getName().Equals(name))
                    break;
            }
            
            return child;
        }
        
        public void LoadParent() {
            if (this.Data.TryGetValue(this.GetSaveKey(), out string parentName) && parentName != null) {
                this.ParentName = parentName;
            } else if (Context.IsMainPlayer) {
                // Attempt to read the value from the GAME DATA
                Farmer farmer = Game1.getFarmerMaybeOffline(this.instance.idOfParent.Value);
                NPC spouse = farmer?.getSpouse();
                if (spouse != null)
                    this.ParentName = spouse.getName();
            }
        }
        public static ChildDat LoadParent( Child child ) {
            ChildDat data = new ChildDat(child);
            
            if (Context.IsMainPlayer) {
                // Player Must be the HOST to Read from SAVE DATA
                // (Other players must be synced from the HOST to the CACHE ARRAY)
                if (data.Data.TryGetValue(data.GetSaveKey(), out string parentName) && parentName != null)
                    data.ParentName = parentName;
                else {
                    // Attempt to read the value from the GAME DATA
                    Farmer farmer = Game1.getFarmerMaybeOffline(child.idOfParent.Value);
                    NPC spouse = farmer?.getSpouse();
                    if (!string.IsNullOrWhiteSpace(spouse?.getName()))
                        data.ParentName = spouse.getName();
                }
            }
            
            return data;
        }
        
        public static ChildDat Of( Child child ) {
            return ChildDat.Of(child, null as NPC);
        }
        public static ChildDat Of( Child child, NPC parent ) {
            return ChildDat.Of(child, parent?.getName());
        }
        public static ChildDat Of( Child child, string parent ) {
            ChildDat data = ChildDat.CHILDREN_PARENT.GetOrAdd(child, ChildDat.Lazy(child)).Value;
            
            if (!string.IsNullOrWhiteSpace(parent))
                data.ParentName = parent;
            else if (string.IsNullOrWhiteSpace(data.ParentName))
                data.LoadParent();
            
            return data;
        }
        
        private static Lazy<ChildDat> Lazy( Child child ) {
            return new Lazy<ChildDat>(() => ChildDat.LoadParent(child), LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }
}