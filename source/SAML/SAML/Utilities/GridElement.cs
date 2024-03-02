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
using SAML.Events;
using System.Runtime.CompilerServices;

namespace SAML.Utilities
{
    public class GridElement : INotifyPropertyChanged
    {
        private bool decorate = true;
        private DecorationStyle edgeDecoration = DecorationStyle.Bauble;
        private DecorationStyle intersectionDecoration = DecorationStyle.Bauble;
        private int minValue = 0;
        private string value = "*";
        private int maxValue = int.MaxValue;

        /// <summary>
        /// Whether or not to apply decoration on this <see cref="GridElement"/>
        /// </summary>
        public bool Decorate
        {
            get => decorate;
            set
            {
                decorate = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The decoration to apply to the edges of this <see cref="GridElement"/>
        /// </summary>
        public DecorationStyle EdgeDecoration
        {
            get => edgeDecoration;
            set
            {
                edgeDecoration = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The decoration to apply when two <see cref="GridElement"/>'s interesect with one another
        /// </summary>
        public DecorationStyle IntersectionDecoration
        {
            get => intersectionDecoration;
            set
            {
                intersectionDecoration = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The minimum value this <see cref="GridElement"/> can shrink to
        /// </summary>
        public int MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The parsable value to use for sizing this <see cref="GridElement"/>
        /// </summary>
        /// <remarks>
        /// The available values are
        /// <list type="bullet">
        /// <item>A static number value (e.g. 400)</item>
        /// <item>A calculated star value (e.g. *, 4*)</item>
        /// </list>
        /// The default is * for automatic
        /// </remarks>
        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The maximum value this <see cref="GridElement"/> can grow to
        /// </summary>
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                invokePropertyChanged();
            }
        }

        internal Rectangle CachedBounds;

        internal int MeasuredSize = -1;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invoke the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">(Optional) The name of the property that has changed</param>
        /// <remarks>
        /// The name of the property is infered and does not need to be manually added if this call is made in the setter (see examples above)
        /// </remarks>
        protected void invokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
