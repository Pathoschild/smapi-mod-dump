using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CompactorChests
{
    public interface IJsonAssetsApi
    {
        void LoadAssets(string path);
        int GetBigCraftableId(string name);
    }

    public class CompactorChest : Chest
    {
        public static int PSI;

        public static Texture2D Sprites;

        private static FieldInfo frameInfo = typeof(Chest).GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        public CompactorChest(StardewValley.Object obj) : base(true)
        {
            ParentSheetIndex = PSI;
            Name = "Compactor Chest";
            var chest = obj as Chest;
            if (chest != null)
            {
                items.AddRange(chest.items);
                playerChoiceColor.Value = chest.playerChoiceColor.Value;
            }
        }

        

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var currentLidFrame = (int)frameInfo.GetValue(this);

            if (this.playerChoiceColor.Value.Equals(Color.Black))
            {
                spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)((y - 1) * 64))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, 130, 16, 32)), this.tint.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((float)y * 64 + 4.1) / 10000f);
                spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)((y - 1) * 64))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, currentLidFrame, 16, 32)), (this.tint.Value * alpha) * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 5) / 10000f);
                return;
            }
            spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, 168, 16, 32)), this.playerChoiceColor.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((float)y * 64 + 4.1) / 10000f);
            spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)(y * 64 + 20))), new Rectangle?(new Rectangle(0, 725, 16, 11)), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 6) / 10000f);
            spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, currentLidFrame + 46, 16, 32)), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 6) / 10000f);
            spriteBatch.Draw(Sprites, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, currentLidFrame + 38, 16, 32)), (this.playerChoiceColor.Value * alpha) * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 5) / 10000f);

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1, bool local = false)
        {
            base.draw(spriteBatch, x, y, alpha, local);
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }

        public override void drawAsProp(SpriteBatch b)
        {
            base.drawAsProp(b);
        }


        internal void Compact()
        {
            // unify color
            var colorables = items.Select(x => x as ColoredObject).Where(x => x != null).GroupBy(x => x.ParentSheetIndex);
            foreach (var group in colorables)
            {
                var minColor = group.First().color.Value;
                foreach (var o in group)
                {
                    o.color.Value = minColor;
                }
            }
            // reduce to minimal quality and restack
            var unique = items.Select(x => x as StardewValley.Object).Where(x => x != null).GroupBy(x => x.ParentSheetIndex);
            foreach(var group in unique)
            {
                var all = group.ToList();
                var minQual = group.Min(x => x.Quality);
                foreach(var o in group)
                {
                    o.Quality = minQual;
                }

                // combine stacks
                var total = group.Sum(x => x.Stack);
                var max = group.First().maximumStackSize();
                var needed = (int)Math.Ceiling((double)total / max);
                var keep = group.Take(needed);
                foreach(var i in group.Skip(needed))
                {
                    items.Remove(i);
                }
                var remaining = total;
                foreach(var k in keep)
                {
                    k.Stack = Math.Min(remaining, max);
                    remaining -= k.Stack;
                }
            }
        }
    }

    public class ModEntry : Mod
    {
        private IJsonAssetsApi JsonAssets;
        private int myChestID;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.World.ObjectListChanged += ObjectsChanged;
            helper.Events.GameLoop.TimeChanged += TimeChanged;
            helper.Events.World.DebrisListChanged += DebrisChanged;
            CompactorChest.Sprites = helper.Content.Load<Texture2D>("assets/Craftables.png", ContentSource.ModFolder);
        }

        private void DebrisChanged(object sender, DebrisListChangedEventArgs e)
        {
            
        }

        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Game1.IsMasterGame) return;
            foreach (var loc in Game1.locations)
            {
                foreach (var cc in loc.Objects.Values.Where(x => x is CompactorChest).Cast<CompactorChest>())
                {
                    cc.Compact();
                }
            }
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            foreach (var loc in Game1.locations)
            {
                foreach(var cc in loc.Objects.Pairs.Where(x => x.Value is CompactorChest).ToList())
                {
                    var chest = new Chest(true);
                    var old = cc.Value as CompactorChest;
                    chest.items.AddRange(old.items);
                    chest.playerChoiceColor.Value = old.playerChoiceColor.Value;
                    chest.ParentSheetIndex = myChestID;
                    loc.Objects[cc.Key] = chest;
                }
            }
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            myChestID = JsonAssets.GetBigCraftableId("Compactor Chest");
            CompactorChest.PSI = myChestID;

            foreach (var loc in Game1.locations)
            {
                foreach (var cc in loc.Objects.Pairs.Where(x => x.Value.ParentSheetIndex == myChestID && !(x.Value is CompactorChest)).ToList())
                {
                    loc.Objects[cc.Key] = new CompactorChest(cc.Value);
                }
            }
        }

        private void ObjectsChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var kvp in e.Added)
            {
                var o = kvp.Value;
                if (o.ParentSheetIndex == myChestID && !(o is CompactorChest))
                {
                    e.Location.Objects[kvp.Key] = new CompactorChest(o);
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // load Json Assets API
            this.JsonAssets = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (this.JsonAssets == null)
            {
                this.Monitor.Log("Can't access the Json Assets API. Is the mod installed correctly?", LogLevel.Error);
                return;
            }

            // inject Json Assets content pack
            this.JsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets"));


        }
    }

}
