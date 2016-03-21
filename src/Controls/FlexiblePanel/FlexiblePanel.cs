using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Comet.Controls
{
    public class FlexiblePanel : Panel
    {
        public double ItemDesiredHeight
        {
            get { return (double)GetValue(ItemDesiredHeightProperty); }
            set { SetValue(ItemDesiredHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemMaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemDesiredHeightProperty =
            DependencyProperty.Register("ItemDesiredHeight", typeof(double), typeof(FlexiblePanel), new PropertyMetadata(150d));



        protected override Size MeasureOverride(Size availableSize)
        {
            var resultSize = new Size(0, 0);

            if (!this.Children.Any())
            {
                return resultSize;
            }

            double y = 0;
            double x = 0;

            double maxX = 0;
            List<List<UIElement>> items = new List<List<UIElement>>();

            int row = 0;

            foreach (var child in Children)
            {

                child.Measure(new Size(availableSize.Width, ItemDesiredHeight));


                if (x + child.DesiredSize.Width > availableSize.Width)
                {
                    x = 0;
                    y += child.DesiredSize.Height;
                    row++;
                }

                x += child.DesiredSize.Width;

                maxX = Math.Max(x, maxX);

                if (items.Count <= row) items.Add(new List<UIElement>());
                items[row].Add(child);
            }
            y = 0;
            foreach (var rowItems in items)
            {
                double itemsWidth = 0;
                foreach (var item in rowItems)
                {
                    itemsWidth += item.DesiredSize.Width;
                }
                var scaleFactor = availableSize.Width / itemsWidth;
                foreach (var item in rowItems)
                {
                    item.Measure(new Size(availableSize.Width, ItemDesiredHeight * scaleFactor)); ;
                }
                y += rowItems[0].DesiredSize.Height;
            }

            resultSize.Width = maxX;
            resultSize.Height = y;

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!this.Children.Any())
            {
                return finalSize;
            }

            double y = 0;
            double x = 0;

            var previousChildHeight = 0d;

            foreach (var child in this.Children)
            {
                if (x + child.DesiredSize.Width > finalSize.Width)
                {
                    x = 0;
                    y += previousChildHeight;
                }

                child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));

                x += child.DesiredSize.Width;
                previousChildHeight = child.DesiredSize.Height;
            }

            return new Size(finalSize.Width, y + previousChildHeight);
        }
    }
}