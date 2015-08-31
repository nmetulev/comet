// code and inspiration from http://blogs.u2u.be/diederik/post/2015/06/01/A-pull-to-refresh-ListView-for-Windows-81-Universal-Apps.aspx

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Comet.Controls
{
    public class RefreshableListView : ListView
    {
        private Border RefreshIndicator;
        private ScrollViewer scroller;
        private CompositeTransform transform;
        private DispatcherTimer timer;


        private double RefreshHeaderHeight = 40;

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
            

            scroller = this.GetTemplateChild("ScrollViewer") as ScrollViewer;
            transform = new CompositeTransform();
            scroller.RenderTransform = transform;
            scroller.ViewChanging += Scroller_ViewChanging;
            //scroller.DirectManipulationCompleted += Scroller_DirectManipulationCompleted;
            //scroller.DirectManipulationStarted += Scroller_DirectManipulationStarted;
            scroller.ManipulationStarted += Scroller_ManipulationStarted;
            scroller.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateY;

            RefreshIndicator = GetTemplateChild("RefreshIndicator") as Border;
            RefreshIndicator.SizeChanged += (s, e) =>
            {
                transform.TranslateY = -RefreshIndicator.ActualHeight;
                scroller.Margin = new Thickness(0, 0, 0, -RefreshIndicator.ActualHeight);
                
            };

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();


            base.OnApplyTemplate();
        }

        private void Scroller_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {

        }

        //private void Scroller_DirectManipulationStarted(object sender, object e)
        //{
        //    Debug.WriteLine("scrollerManipulationStarte, offset:" + scroller.VerticalOffset);
        //    timer.Start();
        //}

        //private void Scroller_DirectManipulationCompleted(object sender, object e)
        //{
        //    Debug.WriteLine("scrollerManipulationEnded, offset:" + scroller.VerticalOffset);
        //    timer.Stop();
        //}

        private void Timer_Tick(object sender, object e)
        {
            //Debug.WriteLine("timer, offset:" + scroller.VerticalOffset);
            //var elem = GetTemplateChild("temp2") as Border;
            //var content = GetTemplateChild("Content") as Border;
            //Point elementBounds = elem.TransformToVisual(content).TransformPoint(new Point(0, 0));
            //var offset = elementBounds.Y;
            //(GetTemplateChild("temp") as Border).Height = offset > 0 ? offset * 2 : 0;
            ////ind.Height = offset > 0 ? offset : 0;
            //Debug.WriteLine(offset);

        }

        private void Scroller_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.NextView.VerticalOffset == 0)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }
    }


}
