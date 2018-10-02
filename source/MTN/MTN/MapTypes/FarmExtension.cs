using Harmony;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace MTN.MapTypes
{
    public class FarmExtension : Farm {
        private Farm inheritedMap;
        public new NetCollection<Item> shippingBin;
        public new NetInt piecesOfHay;
        public new Item lastItemShipped;

        private static Action<GameLocation, SpriteBatch> BuildableDraw = (Action<GameLocation, SpriteBatch>)Delegate.CreateDelegate(typeof(Action<GameLocation, SpriteBatch>), typeof(BuildableGameLocation).GetMethod("draw"));
        private static Func<GameLocation, Location, Rectangle, Farmer, bool> BuildableCheckAction = (Func<GameLocation, Location, Rectangle, Farmer, bool>)Delegate.CreateDelegate(typeof(Func<GameLocation, Location, Rectangle, Farmer, bool>), typeof(BuildableGameLocation).GetMethod("checkAction"));

        public FarmExtension() { }

        public FarmExtension(string mapPath, string name, NetCollection<Item> bin, NetInt haystack) : base(mapPath, name) {
            shippingBin = bin;
            piecesOfHay = haystack;
        }

        public FarmExtension(string mapPath, string name, Farm f) : base(mapPath, name) {
            shippingBin = f.shippingBin;
            import(f);
        }

        public override void draw(SpriteBatch b) {
            BuildableDraw(this, b);
            foreach (ResourceClump stump in resourceClumps) {
                stump.draw(b, stump.tile);
            }
            foreach (KeyValuePair<long, FarmAnimal> kvp in animals.Pairs) {
                kvp.Value.draw(b);
            }
        }

        public override bool checkAction(Location tileLocation, Rectangle viewport, Farmer who) {
            return BuildableCheckAction(this, tileLocation, viewport, who);
        }

        public void import(Farm f) {
            characters.Set(f.characters);
            Traverse.Create(this).Field("objects").SetValue(f.objects);
            temporarySprites = f.temporarySprites;
            Traverse.Create(this).Field("farmers").SetValue(f.farmers);
            projectiles.Set(f.projectiles);
            //terrainFeatures.Set(f.terrainFeatures);
            Traverse.Create(this).Field("terrainFeatures").SetValue(f.terrainFeatures);
            debris.Set(f.debris);
            buildings.Set(f.buildings);
            //animals.Set(f.animals);
            Traverse.Create(this).Field("animals").SetValue(f.animals);
            resourceClumps.Set(f.resourceClumps);
            piecesOfHay = f.piecesOfHay;
            grandpaScore = f.grandpaScore;
            hasSeenGrandpaNote = f.hasSeenGrandpaNote;

            foreach (var c in characters) {
                c.currentLocation = this;
            }
            foreach (var p in farmers) {
                p.currentLocation = this;
            }
            foreach (var c in f.characters) {
                c.currentLocation = f;
            }
            foreach (var p in f.farmers) {
                p.currentLocation = f;
            }
        }

        public void export(ref Farm f) {
            f.characters.Set(characters);
            Traverse.Create(f).Field("objects").SetValue(objects);
            f.temporarySprites = temporarySprites;
            Traverse.Create(f).Field("farmers").SetValue(farmers);
            f.projectiles.Set(projectiles);
            Traverse.Create(f).Field("terrainFeatures").SetValue(terrainFeatures);
            f.debris.Set(debris);
            f.buildings.Set(buildings);
            //f.animals.Set(animals);
            Traverse.Create(f).Field("animals").SetValue(animals);
            f.resourceClumps.Set(resourceClumps);
            f.piecesOfHay.Set(piecesOfHay);
            f.grandpaScore = grandpaScore;
            f.hasSeenGrandpaNote = hasSeenGrandpaNote;

            foreach (var c in characters) {
                c.currentLocation = this;
            }
            foreach (var p in farmers) {
                p.currentLocation = this;
            }
            foreach (var c in f.characters) {
                c.currentLocation = f;
            }
            foreach (var p in f.farmers) {
                p.currentLocation = f;
            }
        }
    }
}
