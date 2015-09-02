using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Comet.Controls
{
    public class RefreshableListView : ListView
    {
        private Border Root;
        private Border RefreshIndicator;
        private CompositeTransform RefreshIndicatorTransform;
        private ScrollViewer Scroller;
        private CompositeTransform ContentTransform;
        private Grid ScrollerContent;

        public event EventHandler RefreshCommand;

        private double lastOffset = 0.0;
        private double pullDistance = 0.0;

        private bool manipulating = false;

        public RefreshableListView()
        {
            DefaultStyleKey = typeof(RefreshableListView);
            SizeChanged += RefreshableListView_SizeChanged;
        }

        private void RefreshableListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        protected override void OnApplyTemplate()
        {
            Root = GetTemplateChild("Root") as Border;

            Scroller = this.GetTemplateChild("ScrollViewer") as ScrollViewer;
            Scroller.DirectManipulationCompleted += Scroller_DirectManipulationCompleted;
            Scroller.DirectManipulationStarted += Scroller_DirectManipulationStarted;

            ContentTransform = GetTemplateChild("ContentTransform") as CompositeTransform;

            ScrollerContent = GetTemplateChild("ScrollerContent") as Grid;

            RefreshIndicator = GetTemplateChild("RefreshIndicator") as Border;
            RefreshIndicatorTransform = GetTemplateChild("RefreshIndicatorTransform") as CompositeTransform;
            RefreshIndicator.SizeChanged += (s, e) =>
            {
                RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
            };
            
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            OverscrollMultiplier = (OverscrollCoefficient * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

            base.OnApplyTemplate();
        }
        
        #region dependencyProperties
        private double OverscrollMultiplier;
        public double OverscrollCoefficient
        {
            get { return (double)GetValue(OverscrollCoefficientProperty); }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    OverscrollMultiplier = (value * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    SetValue(OverscrollCoefficientProperty, value);
                }
                else
                    throw new IndexOutOfRangeException("OverscrollCoefficient has to be a double value between 0 and 1 inclusive.");
            }
        }

        // Using a DependencyProperty as the backing store for OverscrollCoefficient.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverscrollCoefficientProperty =
            DependencyProperty.Register("OverscrollCoefficient", typeof(double), typeof(RefreshableListView), new PropertyMetadata(0.5));

        #endregion


        private void Scroller_DirectManipulationStarted(object sender, object e)
        {
            manipulating = true;
        }

        private void Scroller_DirectManipulationCompleted(object sender, object e)
        {
            manipulating = false;
            RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
            ContentTransform.TranslateY = 0;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            if (!manipulating || Scroller.VerticalOffset > 0) return;
            Rect elementBounds = ScrollerContent.TransformToVisual(Root).TransformBounds(new Rect());

            var offset = elementBounds.Y;
            var delta = offset - lastOffset;
            lastOffset = offset;

            pullDistance += delta * OverscrollMultiplier;

            if (pullDistance > 0)
            {
                ContentTransform.TranslateY = pullDistance - offset;
                RefreshIndicatorTransform.TranslateY = pullDistance - offset - RefreshIndicator.ActualHeight;
            }
            else
            {
                ContentTransform.TranslateY = 0;
                RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;

            }
        }

    }


}
