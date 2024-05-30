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
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Effects
{
    public abstract class AbstractSpellEffect : IActiveEffect, IEntity
    {
        ModEntry modEntry;

        protected IEntity owner;
        protected readonly Vector2 pos;
        private Rectangle boundingBox;
        protected int duration;

        public object entity { get { return this; } }

        public bool isActive = true;

        public AbstractSpellEffect(ModEntry modEntry, Vector2 pos, int dur)
        {
            this.modEntry = modEntry;

            this.pos = pos;
            this.duration = dur;
        }

        public abstract void Update(UpdateTickedEventArgs e);

        public abstract void OneSecondUpdate(OneSecondUpdateTickingEventArgs e);

        public abstract void Draw(SpriteBatch spriteBatch);

        public IEntity GetOwner()
        { 
            return owner; 
        }

        public abstract void SetOwner(IEntity owner);

        public abstract int GetDuration();

        protected void ForAllInRange(int radius, bool skipOwner, Action<Character> consumer)
        {
            float x = pos.X, y = pos.Y;

            List<Character> list = GameLocationUtils.GetCharacters(this, boundingBox);

            foreach (var character in list)
            {
                //if (e == this) 
                //    continue;
                
                if (skipOwner && character == GetOwner()) 
                    continue;

                //if (e is AbstractSpellEffect) 
                //    continue;

                consumer.Invoke(character);
            }
        }

        public GameLocation GetGameLocation()
        {
            return owner.GetGameLocation();
        }

        public Rectangle GetBoundingBox()
        {
            return boundingBox;
        }

        public void SetBoundingBox(Rectangle boundingBox)
        {
            this.boundingBox = boundingBox;
        }

        public Vector2 GetPosition()
        {
            return pos;
        }

        public int GetHorizontalMovement()
        {
            return 1;
        }

        public int GetVerticalMovement()
        {
            return 1;
        }
    }
}
