using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Comet.Controls
{
    public class PullToRefreshListView : ListView
    {
        private Border Root;
        private Border RefreshIndicatorBorder;
        private CompositeTransform RefreshIndicatorTransform;
        private ScrollViewer Scroller;
        private CompositeTransform ContentTransform;
        private Grid ScrollerContent;

        private TextBlock DefaultIndicatorContent;

        public event EventHandler RefreshActivated;
        public event EventHandler<RefreshProgressEventArgs> PullProgressChanged;

        private double lastOffset = 0.0;
        private double pullDistance = 0.0;

        private bool manipulating = false;
        DateTime lastRefreshActivation = default(DateTime);
        bool refreshActivated = false;


        public PullToRefreshListView()
        {
            DefaultStyleKey = typeof(PullToRefreshListView);
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

            RefreshIndicatorBorder = GetTemplateChild("RefreshIndicator") as Border;
            RefreshIndicatorTransform = GetTemplateChild("RefreshIndicatorTransform") as CompositeTransform;

            DefaultIndicatorContent = GetTemplateChild("DefaultIndicatorContent") as TextBlock;
            DefaultIndicatorContent.Visibility = RefreshIndicatorContent == null ? Visibility.Visible : Visibility.Collapsed;

            RefreshIndicatorBorder.SizeChanged += (s, e) =>
            {
                RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;
            };
            
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            OverscrollMultiplier = (OverscrollLimit * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

            base.OnApplyTemplate();
        }
        
        #region dependencyProperties

        private double OverscrollMultiplier;

        public double OverscrollLimit
        {
            get { return (double)GetValue(OverscrollLimitProperty); }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    OverscrollMultiplier = (value * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    SetValue(OverscrollLimitProperty, value);
                }
                else
                    throw new IndexOutOfRangeException("OverscrollCoefficient has to be a double value between 0 and 1 inclusive.");
            }
        }

        public static readonly DependencyProperty OverscrollLimitProperty =
            DependencyProperty.Register("OverscrollLimit", typeof(double), typeof(PullToRefreshListView), new PropertyMetadata(0.5));



        public double PullThreshold
        {
            get { return (double)GetValue(PullThresholdProperty); }
            set { SetValue(PullThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PullThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullThresholdProperty =
            DependencyProperty.Register("PullThreshold", typeof(double), typeof(PullToRefreshListView), new PropertyMetadata(100.0));




        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(PullToRefreshListView), new PropertyMetadata(null));




        public object RefreshIndicatorContent
        {
            get { return (object)GetValue(RefreshIndicatorContentProperty); }
            set
            {
                if (DefaultIndicatorContent != null)
                    DefaultIndicatorContent.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                SetValue(RefreshIndicatorContentProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RefreshIndicator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshIndicatorContentProperty =
            DependencyProperty.Register("RefreshIndicatorContent", typeof(object), typeof(PullToRefreshListView), new PropertyMetadata(null));



        #endregion


        private void Scroller_DirectManipulationStarted(object sender, object e)
        {
            if (Scroller.VerticalOffset == 0)
                manipulating = true;
        }

        private void Scroller_DirectManipulationCompleted(object sender, object e)
        {
            manipulating = false;
            RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;
            ContentTransform.TranslateY = 0;

            if (refreshActivated)
            {
                if (RefreshActivated != null)
                    RefreshActivated(this, new EventArgs());
                if (RefreshCommand != null && RefreshCommand.CanExecute(null))
                    RefreshCommand.Execute(null);
            }

            refreshActivated = false;
            lastRefreshActivation = default(DateTime);

            if (RefreshIndicatorContent == null)
                DefaultIndicatorContent.Text = "Pull to Refresh";
        }



        

        private void CompositionTarget_Rendering(object sender, object e)
        {
            if (!manipulating) return;
            //else if (Scroller.VerticalOffset > 0)
            //{
            //    manipulating = false;
            //    return;
            //}
            Rect elementBounds = ScrollerContent.TransformToVisual(Root).TransformBounds(new Rect());

            var offset = elementBounds.Y;
            var delta = offset - lastOffset;
            lastOffset = offset;

            pullDistance += delta * OverscrollMultiplier;

            if (pullDistance > 0)
            {
                ContentTransform.TranslateY = pullDistance - offset;
                RefreshIndicatorTransform.TranslateY = pullDistance - offset - RefreshIndicatorBorder.ActualHeight;
            }
            else
            {
                ContentTransform.TranslateY = 0;
                RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;

            }

            var pullProgress = 0.0;

            if (pullDistance >= PullThreshold)
            {
                lastRefreshActivation = DateTime.Now;
                refreshActivated = true;
                pullProgress = 1.0;
                if (RefreshIndicatorContent == null)
                    DefaultIndicatorContent.Text = "Relese to Refresh";
            }
            else if (lastRefreshActivation != DateTime.MinValue)
            {
                TimeSpan timeSinceActivated = DateTime.Now - lastRefreshActivation;
                // if more then a second since activation, deactivate
                if (timeSinceActivated.TotalMilliseconds > 1000)
                {
                    refreshActivated = false;
                    lastRefreshActivation = default(DateTime);
                    pullProgress = pullDistance / PullThreshold;
                    if (RefreshIndicatorContent == null)
                        DefaultIndicatorContent.Text = "Pull to Refresh";
                }
                else
                {
                    pullProgress = 1.0;
                }
            }
            else
            {
                pullProgress = pullDistance / PullThreshold;
            }

            if (PullProgressChanged != null)
            {
                PullProgressChanged(this, new RefreshProgressEventArgs() { PullProgress = pullProgress });
            }
            
        }

    }

    public class RefreshProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Value from 0.0 to 1.0 where 1.0 is active
        /// </summary>
        public double PullProgress { get; set; }
    }


}
