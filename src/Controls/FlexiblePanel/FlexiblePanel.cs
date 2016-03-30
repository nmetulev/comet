using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Comet.Controls
{
    public class FlexiblePanel : Panel
    {

        /// <summary>
        /// Gets or sets the desired size of each item. 
        /// If Orientation is horizontal, this value sets the desired height. Otherwise, this value sets the desired width.
        /// </summary>
        /// <value>
        /// The desired size of each item
        /// </value>
        public double ItemDesiredSize
        {
            get { return (double)GetValue(ItemDesiredSizeProperty); }
            set { SetValue(ItemDesiredSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemDesiredSize dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemDesiredSizeProperty =
            DependencyProperty.Register("ItemDesiredSize", typeof(double), typeof(FlexiblePanel), new PropertyMetadata(150d));


        /// <summary>
        /// Gets or sets the orientation in which child elements are aranged
        /// </summary>
        /// <value>
        /// The orientation
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the OrientationProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlexiblePanel), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or set if the child elements animate to the new location when control is resized
        /// </summary>
        /// <value>
        /// True/False
        /// </value>
        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        /// <summary>
        /// Identifies the AnimateProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register("Animate", typeof(bool), typeof(FlexiblePanel), new PropertyMetadata(true));

        private Size MeasureOverideHorizontal(Size availableSize)
        {
            var resultSize = new Size(0, 0);

            if (!this.Children.Any())
            {
                return resultSize;
            }

            double x = 0;
            double y = 0;

            List<List<UIElement>> items = new List<List<UIElement>>();

            List<UIElement> currentRow = new List<UIElement>();

            for (var i = 0; i < Children.Count; ++i)
            {
                var curChild = Children[i];
                curChild.Measure(new Size(availableSize.Width, ItemDesiredSize));

                if (x + curChild.DesiredSize.Width > availableSize.Width)
                {
                    var widthDifWithChild = (x + curChild.DesiredSize.Width) - availableSize.Width;
                    var widthDifWithoutChild = availableSize.Width - x;

                    double scaleFactor = 1;
                    if (widthDifWithChild < widthDifWithoutChild)
                    {
                        //increase size to all children including curCHild
                        currentRow.Add(curChild);
                        scaleFactor = availableSize.Width / (x + curChild.DesiredSize.Width);
                        foreach (var item in currentRow)
                        {
                            item.Measure(new Size(availableSize.Width, ItemDesiredSize * scaleFactor)); ;
                        }
                        currentRow.Clear();
                        y += curChild.DesiredSize.Height;
                        x = 0;
                    }
                    else
                    {
                        scaleFactor = availableSize.Width / x;
                        foreach (var item in currentRow)
                        {
                            item.Measure(new Size(availableSize.Width, ItemDesiredSize * scaleFactor)); ;
                        }

                        y += currentRow.First().DesiredSize.Height;
                        currentRow.Clear();
                        currentRow.Add(curChild);
                        x = curChild.DesiredSize.Width;
                    }
                }
                else
                {
                    x += curChild.DesiredSize.Width;
                    currentRow.Add(curChild);
                }
            }

            if (currentRow.Count > 0) y += currentRow.First().DesiredSize.Height;

            resultSize.Width = availableSize.Width;
            resultSize.Height = y;

            return resultSize;
        }

        private Size MeasureOverideVertical(Size availableSize)
        {
            return availableSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Horizontal)
                return MeasureOverideHorizontal(availableSize);
            else
                return MeasureOverideVertical(availableSize);
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
                if (Math.Floor(x + child.DesiredSize.Width) > finalSize.Width + 2)
                {
                    x = 0;
                    y += previousChildHeight;
                }

                if (Animate)
                {
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                    var visual = ElementCompositionPreview.GetElementVisual(child);

                    var offsetAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
                    offsetAnimation.Duration = TimeSpan.FromMilliseconds(200);
                    offsetAnimation.InsertKeyFrame(1, new Vector3((float)x, (float)y, 0));
                    visual.StartAnimation("Offset", offsetAnimation);

                    x += child.DesiredSize.Width;
                    previousChildHeight = child.DesiredSize.Height;
                }
                else
                {
                    child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }

            return new Size(finalSize.Width, y + previousChildHeight);
        }
    }
}