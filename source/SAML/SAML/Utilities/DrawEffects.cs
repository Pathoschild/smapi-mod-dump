/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SAML.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    /// <summary>
    /// A wrapper for non-basic draw method arguments
    /// </summary>
    public class DrawEffects : INotifyPropertyChanged
    {
        public readonly static DrawEffects Default = new();

        private float rotation = 0.0f;
        private Vector2 origin = Vector2.Zero;
        private float scale = 1f;
        private SpriteEffects spriteEffects = SpriteEffects.None;
        private float zIndex = 1f;
        private float hoverScaleIncrease = 0.25f;

        /// <summary>
        /// Controls the rotation of the drawn texture / text
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The central point to use for rotation
        /// </summary>
        public Vector2 Origin
        {
            get => origin;
            set
            {
                origin = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The scale at which the texture / text will be drawn
        /// </summary>
        public float Scale
        {
            get => scale;
            set
            {
                scale = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Built-in effects to apply to the texture / text (see <see cref="Microsoft.Xna.Framework.Graphics.SpriteEffects"/>)
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get => spriteEffects;
            set
            {
                spriteEffects = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The layer depth controls how two overlapping textures / texts will be rendered (Which will be on top)
        /// </summary>
        public float ZIndex
        {
            get => zIndex;
            set
            {
                zIndex = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The maximum scale increase to apply when the target is under the mouse
        /// </summary>
        public float HoverScaleIncrease
        {
            get => hoverScaleIncrease;
            set
            {
                hoverScaleIncrease = value;
                invokePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void invokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
