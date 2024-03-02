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
    public class StackPanel : Container
    {
        private Orientation orientation = Orientation.LandScape;

        /// <summary>
        /// The orientation in which the <see cref="Container.Elements"/> of this <see cref="StackPanel"/> will be laid out
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

        public override bool ForcePurePosition => true;

        protected override void OnElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            base.OnElementsChanged(sender, e);
            int x = (int)GetPosition(true).X, y = (int)GetPosition(true).Y, maxAltSize = -1;
            List<Element> elements = new(FilteredElements);
            if (elements.Count <= 0)
                return;
            for (int i = 0; i < elements.Count; i++)
            {
                var el = elements[i];

                el.X = x;
                el.Y = y;
                
                if (Orientation == Orientation.Horizontal)
                {
                    x += (int)el.GetSizeForSpacing().X;
                    maxAltSize = Math.Max(maxAltSize, (int)el.GetSizeForSpacing().Y);
                }
                if (Orientation == Orientation.Vertical)
                {
                    y += (int)el.GetSizeForSpacing().Y;
                    maxAltSize = Math.Max(maxAltSize, (int)el.GetSizeForSpacing().X);
                }
            }

            if (AutoSize)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Width = x - X + (int)elements[^1].GetSizeForSpacing().X;
                    Height = maxAltSize;
                }
                if (Orientation == Orientation.Vertical)
                {
                    Height = y - Y + (int)elements[^1].GetSizeForSpacing().Y;
                    Width = maxAltSize;
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
                case nameof(FilteredElements):
                    OnElementsChanged(null!, new());
                    break;
            }
        }
    }
}
