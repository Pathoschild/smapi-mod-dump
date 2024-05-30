/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Reflection;
using SObject = StardewValley.Object;

namespace GardenPotOptions {
    public class BetterIndoorPot : IndoorPot {
        private static readonly GameLocation EmptyLocation = new() { IsGreenhouse = true };

        private readonly NetRef<TerrainFeature?> _tree = new();

        public TerrainFeature? tree {
            get => this._tree.Value;
            set => this._tree.Value = value;
        }

        public string GetTreeSeedType {
            get => (this.tree is Tree t
                ? Tree.GetWildTreeSeedLookup().FirstOrDefault(d => d.Value?.Any(v => v?.Equals(t?.treeType?.Value) ?? false) ?? false).Key?.Replace("(O)", "")
                : (this.tree is FruitTree f
                    ? f.treeId.Value
                    : null)) ?? "-1";
        }

        protected override void initNetFields() {
            base.initNetFields();
            this.NetFields.AddField(this._tree);
        }

        public string GetPottedType() {
            var suffix = "";
            if (this.hoeDirt?.Value?.crop?.indexOfHarvest?.Value is not null) {
                suffix += new SObject(this.hoeDirt.Value.crop.indexOfHarvest.Value, 1).DisplayName;
            } else if (this.bush?.Value is not null) {
                suffix += this.bush.Value.size.Value == 3 ? new SObject("251", 1).DisplayName : "Bush";
            } else if (this.hoeDirt?.Value?.fertilizer?.Value is not null) {
                suffix += new SObject(this.hoeDirt.Value.fertilizer.Value.Replace("(O)", ""), 1).DisplayName;
            } else if (this.heldObject?.Value?.DisplayName is not null) {
                suffix += this.heldObject.Value.DisplayName;
            } else if (this.tree is not null) {
                suffix += new SObject(this.GetTreeSeedType, 1).DisplayName;
            }
            return suffix;
        }

        public static bool TryTransplant(IndoorPot? pot, out BetterIndoorPot bip) =>
            (bip = new BetterIndoorPot(pot)).IsFilled();

        public BetterIndoorPot() : base(Vector2.Zero) { }

        public BetterIndoorPot(IndoorPot? pot) : this() {
            this.TransplantInto(pot?.hoeDirt?.Value);
            this.TransplantInto(pot?.bush?.Value);
            this.TransplantInto(pot?.heldObject?.Value);
            this.showNextIndex.Value = pot?.showNextIndex?.Value ?? false;
            if (pot?.hoeDirt?.Value is not null) {
                pot.hoeDirt.Value = new HoeDirt();
            }
            if (pot?.bush?.Value is not null) {
                pot.bush.Value = null;
            }
            if (pot?.heldObject?.Value is not null) {
                pot.heldObject.Value = null;
            }
        }

        public BetterIndoorPot(HoeDirt? dirt) : this() => this.TransplantInto(dirt);

        public BetterIndoorPot(Bush? bush) : this() => this.TransplantInto(bush);

        public BetterIndoorPot(SObject? held) : this() => this.TransplantInto(held);

        public BetterIndoorPot(Tree? tree) : this() => this.TransplantInto(tree);

        public BetterIndoorPot(FruitTree? tree) : this() => this.TransplantInto(tree);

        public bool IsFilled() =>
            this.hoeDirt?.Value?.crop is not null
            || this.hoeDirt?.Value?.fertilizer?.Value is not null
            || this.bush?.Value is not null
            || this.heldObject?.Value is not null
            || this.tree is not null;

        public void TransplantInto(HoeDirt? dirt) {
            if (dirt is not null) {
                this.hoeDirt.Value = new(dirt.state.Value, dirt.crop) { Location = EmptyLocation };
                this.hoeDirt.Value.fertilizer.Value = dirt.fertilizer.Value;
                dirt.modData.Keys.Do(key => this.hoeDirt.Value.modData[key] = dirt.modData[key]);
                dirt.crop = null;
                dirt.fertilizer.Value = null;
                dirt.state.Value = 0;
                dirt.modData.Clear();
            }
        }

        public void TransplantOut(HoeDirt? dirt) {
            if (dirt is not null) {
                dirt.crop = this.hoeDirt?.Value?.crop;
                dirt.fertilizer.Value = this.hoeDirt?.Value?.fertilizer?.Value;
                dirt.state.Value = this.hoeDirt?.Value?.state?.Value ?? 0;
                this.hoeDirt?.Value?.modData?.Keys.Do(key => dirt.modData[key] = this.hoeDirt.Value.modData[key]);
                this.hoeDirt!.Value = null;
            }
        }

        private static void TryRemove<T>(GameLocation? location, Vector2 tile) {
            if (location is not null && location.terrainFeatures.TryGetValue(tile, out var t) && t is T) {
                location.terrainFeatures.Remove(tile);
            }
        }

        public void TransplantInto(Bush? bush) {
            if (bush is not null) {
                TryRemove<Bush>(bush.Location, bush.Tile);
                bush.inPot.Value = true;
                bush.Location = EmptyLocation;
                this.bush.Value = bush;
            }
        }

        private static readonly FieldInfo? BushYDrawOffset = typeof(Bush).GetField("yDrawOffset", BindingFlags.NonPublic | BindingFlags.Instance);

        public void TransplantOut(GameLocation? location, Vector2 tile) {
            if (location is not null) {
                if (this.bush?.Value is not null) {
                    var bush = this.bush.Value;
                    this.bush.Value = null;
                    bush.inPot.Value = false;
                    bush.Location = location;
                    bush.Tile = tile;
                    BushYDrawOffset?.SetValue(bush, 0);
                    location.terrainFeatures.Add(tile, bush);
                }
                if (this.tree is not null) {
                    var tree = this.tree;
                    this.tree = null;
                    tree.Location = location;
                    tree.Tile = tile;
                    location.terrainFeatures.Add(tile, tree);
                }
            }
        }

        public void TransplantInto(SObject? held) {
            if (held is not null) {
                held.Location = EmptyLocation;
                this.heldObject.Value = held;
            }
        }

        public void TransplantInto(Tree? tree) {
            if (tree is not null) {
                TryRemove<Tree>(tree.Location, tree.Tile);
                tree.Location = EmptyLocation;
                this.tree = tree;
            }
        }

        public void TransplantInto(FruitTree? tree) {
            if (tree is not null) {
                TryRemove<FruitTree>(tree.Location, tree.Tile);
                tree.Location = EmptyLocation;
                this.tree = tree;
            }
        }

        public bool TryTransplantOut(IndoorPot? pot) {
            if (this.IsFilled() && pot is not null) {
                if (this.bush?.Value is not null) {
                    pot.bush.Value = this.bush.Value;
                    this.bush.Value = null;
                    return true;
                }
                if (this.heldObject?.Value is not null) {
                    pot.heldObject.Value = this.heldObject.Value;
                    this.heldObject.Value = null;
                    return true;
                }
                if (this.hoeDirt?.Value?.crop is not null || this.hoeDirt?.Value?.fertilizer?.Value is not null) {
                    this.TransplantOut(pot.hoeDirt?.Value);
                    pot.showNextIndex.Value = this.showNextIndex.Value;
                    this.hoeDirt.Value = null;
                    return true;
                }
            }
            return false;
        }
    }
}
