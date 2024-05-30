/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells.Components;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;
using static System.Net.Mime.MediaTypeNames;

namespace ArsVenefici.Framework.Spells.Effects
{
    public class ZoneEffect : AbstractSpellEffect
    {
        ModEntry modEntry;

        private float radius;
        private ISpell spell;
        private int index;
        private readonly Texture2D Tex;

        public ZoneEffect(ModEntry modEntry, ISpell spell, Vector2 pos, float radius, int dur) : base(modEntry, pos, dur) 
        {
            this.modEntry = modEntry;

            this.spell = spell;
            this.radius = radius;

            this.Tex = new Texture2D(Game1.graphics.GraphicsDevice, Game1.tileSize * (int)radius, Game1.tileSize * (int)radius);

            int width = Tex.Width;
            int height = Tex.Height;
            int area = width * height;

            Color[] data = new Color[area];
            Color manaCol = new Color(0, 48, 255, 127);

            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = manaCol;
            }

            Tex.SetData(data);

            Vector2 tilePos = new Vector2(pos.X - radius, pos.Y - radius);
            Vector2 absolutePos = Utils.TilePosToAbsolutePos(tilePos);

            int boundingBoxRadius = Game1.tileSize * ((int)radius + 2);

            SetBoundingBox(new Rectangle((int)(absolutePos.X), (int)(absolutePos.Y), boundingBoxRadius, boundingBoxRadius));
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            isActive = --this.duration > 0 || GetOwner() == null;
        }

        public override void OneSecondUpdate(OneSecondUpdateTickingEventArgs e)
        {
            IEntity owner = GetOwner();

            int index = GetIndex();
            float radius = GetRadius();
            ISpell spell = GetSpell();

            var spellHelper = SpellHelper.Instance();

            ForAllInRange((int)radius, false, e => spellHelper.Invoke(modEntry, spell, owner, GetGameLocation(), new CharacterHitResult(e), 0, index, true));

            for (int x = (int)(pos.X - radius); x <= pos.X + radius; ++x)
            {
                for (int y = (int)(pos.Y - radius); y <= pos.Y + radius; ++y)
                {
                    TilePos newTilePos = new TilePos(x, y);
                    spellHelper.Invoke(modEntry, spell, owner, GetGameLocation(), new TerrainFeatureHitResult(pos, 0, newTilePos, false), 0, index, true);
                }
            }

            if (!isActive)
            {
                modEntry.ActiveEffects.Remove(this);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle((int)this.pos.X, (int)this.pos.Y, (int)radius, (int)radius);

            Vector2 tilePos = new Vector2((int)r.X, (int)r.Y);
            Vector2 absolutePos = Utils.TilePosToAbsolutePos(tilePos);
            Vector2 screenPos = Utils.TilePosToScreenPos(tilePos);

            //spriteBatch.Draw(Tex, screenPos, new Rectangle(0, 0, Tex.Width, Tex.Height), Color.White);

            float speed = -0.5f;

            for (int x = (int)-radius; x <= (int)radius; x++)
            {
                for (int y = (int)-radius; y <= (int)radius; y++)
                {
                    Vector2 vec = new Vector2(r.X + x, r.Y + y);
                    Vector2 absPos = Utils.TilePosToAbsolutePos(vec);

                    TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), absPos + new Vector2((float)Game1.random.Next(-12, 21), (float)Game1.random.Next(16)), false, 1f / 500f, new Color(0, 48, 255, 127))
                    {
                        alphaFade = (float)(1.0 / 1000.0 - (double)speed / 300.0),
                        alpha = 0.1f,
                        //motion = new Vector2(0.0f, speed),
                        //acceleration = new Vector2(0.0f, 0.0f),
                        interval = 99999f,
                        layerDepth = (float)(this.GetBoundingBox().Bottom - 3 - Game1.random.Next(5)) / 10000f,
                        scale = 8f,
                        scaleChange = 0.01f,
                        rotationChange = (float)((double)Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
                    };

                    this.GetGameLocation().temporarySprites.Add(sprite);
                }
            }
        }

        public override int GetDuration()
        {
            return this.duration;
        }

        public void SetDuration(int duration)
        {
            this.duration = duration;
        }

        public override void SetOwner(IEntity owner)
        {
            this.owner = owner;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }
        public float GetRadius()
        {
            return radius;
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public ISpell GetSpell()
        {
            return spell;
        }

        public void SetSpell(ISpell spell)
        {
            this.spell = spell;
        }
    }
}
