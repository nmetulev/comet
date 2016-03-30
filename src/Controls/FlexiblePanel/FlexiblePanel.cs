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
        public double ItemDesiredHeight
        {
            get { return (double)GetValue(ItemDesiredHeightProperty); }
            set { SetValue(ItemDesiredHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemMaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemDesiredHeightProperty =
            DependencyProperty.Register("ItemDesiredHeight", typeof(double), typeof(FlexiblePanel), new PropertyMetadata(150d));

        bool firstTime = true;

        public delegate void OrganizeCustomPanelLoadedEventHandler(object sender, EventArgs e);

        public event OrganizeCustomPanelLoadedEventHandler OrganizeCustomPanelLoaded;

        protected override Size MeasureOverride(Size availableSize)
        {
            var resultSize = new Size(0, 0);

            if (!this.Children.Any())
            {
                return resultSize;
            }


            double x = 0;
            double y = 0;

            List<List<UIElement>> items = new List<List<UIElement>>();

            int row = 0;

            List<UIElement> currentRow = new List<UIElement>();

            for (var i = 0; i < Children.Count; ++i)
            {
                var curChild = Children[i];
                curChild.Measure(new Size(availableSize.Width, ItemDesiredHeight));

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
                            item.Measure(new Size(availableSize.Width, ItemDesiredHeight * scaleFactor)); ;
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
                            item.Measure(new Size(availableSize.Width, ItemDesiredHeight * scaleFactor)); ;
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

                //child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                var visual = ElementCompositionPreview.GetElementVisual(child);

                if (firstTime)
                {
                    visual.Offset = new Vector3((float)x, (float)y, 0);
                    firstTime = false;
                    if (OrganizeCustomPanelLoaded != null) OrganizeCustomPanelLoaded(this, EventArgs.Empty);
                }
                else
                {
                    var offsetAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
                    offsetAnimation.Duration = TimeSpan.FromMilliseconds(200);
                    offsetAnimation.InsertKeyFrame(1, new Vector3((float)x, (float)y, 0));
                    visual.StartAnimation("Offset", offsetAnimation);
                }


                x += child.DesiredSize.Width;
                previousChildHeight = child.DesiredSize.Height;
            }

            return new Size(finalSize.Width, y + previousChildHeight);
        }
    }
}