/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Elements
{
    public class WrapPanel : Container
    {
        private Orientation orientation = Orientation.LandScape;
        private int maxWidth = 400;
        private int maxHeight = 400;

        /// <summary>
        /// The orientation in which the <see cref="Container.Elements"/> of this <see cref="WrapPanel"/> will be laid out
        /// </summary>
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The maximum width used for layout of this <see cref="WrapPanel"/>
        /// </summary>
        /// <remarks>
        /// Only applicable for <see cref="Orientation.Horizontal"/> and <see cref="Orientation.LandScape"/> mode
        /// </remarks>
        public int MaxWidth
        {
            get => maxWidth;
            set
            {
                maxWidth = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The maximum height used for layout of this <see cref="WrapPanel"/>
        /// </summary>
        /// <remarks>
        /// Only applicable for <see cref="Orientation.Vertical"/> and <see cref="Orientation.Portrait"/> mode
        /// </remarks>
        public int MaxHeight
        {
            get => maxHeight;
            set
            {
                maxHeight = value;
                invokePropertyChanged();
            }
        }

        public override bool ForcePurePosition => true;

        protected override void OnElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            base.OnElementsChanged(sender, e);
            int x = (int)GetPosition(true).X, y = (int)GetPosition(true).Y, stacks = 0, maxMainSize = -1, maxAltSize = -1, totalAltSize = 0;

            var elements = new List<Element>(FilteredElements);
            if (elements.Count <= 0)
                return;

            for (int i = 0; i < elements.Count; i++)
            {
                var el = elements[i];

                if (Orientation == Orientation.Horizontal)
                {
                    if (x - (int)GetPosition(true).X + el.GetSize(true).X > MaxWidth && i != 0)
                    {
                        stacks++;
                        totalAltSize += maxAltSize;
                        maxMainSize = Math.Max(maxMainSize, x - (int)GetPosition(true).X);
                        y += maxAltSize;
                        x = (int)GetPosition(true).X;
                        maxAltSize = -1;
                    }
                    el.X = x;
                    el.Y = y;
                    x += (int)el.GetSizeForSpacing().X;
                    maxAltSize = Math.Max(maxAltSize, (int)el.GetSizeForSpacing().Y);
                }
                if (Orientation == Orientation.Vertical)
                {
                    if (y - (int)GetPosition(true).Y + el.GetSize().Y > MaxHeight && i != 0)
                    {
                        stacks++;
                        totalAltSize += maxAltSize;
                        maxMainSize = Math.Max(maxMainSize, y - (int)GetPosition(true).Y);
                        x += maxAltSize;
                        y = (int)GetPosition(true).Y;
                        maxAltSize = -1;
                    }
                    el.X = x;
                    el.Y = y;
                    y += (int)el.GetSizeForSpacing().Y;
                    maxAltSize = Math.Max(maxAltSize, (int)el.GetSizeForSpacing().X);
                }
            }

            if (maxAltSize < 0)
                maxAltSize = 0;
            if (maxMainSize < 0)
                maxMainSize = 0;

            if (AutoSize)
            {
                totalAltSize += maxAltSize;

                if (Orientation == Orientation.Horizontal)
                {
                    maxMainSize = Math.Max(maxMainSize, x - (int)GetPosition(true).X);
                    Width = maxMainSize;
                    Height = totalAltSize;
                }
                if (Orientation == Orientation.Vertical)
                {
                    maxMainSize = Math.Max(maxMainSize, y - (int)GetPosition(true).Y);
                    Height = maxMainSize;
                    Width = totalAltSize;
                }
            }
        }

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(X):
                case nameof(Y):
                case nameof(Orientation):
                case nameof(Filter):
                case nameof(MaxWidth):
                case nameof(MaxHeight):
                case nameof(FilteredElements):
                    OnElementsChanged(null!, new());
                    break;
            }
        }
    }
}
