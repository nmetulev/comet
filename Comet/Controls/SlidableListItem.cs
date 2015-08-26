using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Comet.Controls
{
    public sealed class SlidableListItem : ContentControl
    {
        private Grid contentGrid;
        private CompositeTransform transform;
        private Grid slider;

        private StackPanel leftCommandPanel;
        private CompositeTransform leftCommandTransform;
        private StackPanel rightCommandPanel;
        private CompositeTransform rightCommandTransform;

        private DoubleAnimation contentAnimation;
        private Storyboard contentStoryboard;

        public event EventHandler RightCommandActivated;
        public event EventHandler LeftCommandActivated;

        private const double ACTIVATED_THRESHOLD = 200;

        public SlidableListItem()
        {
            this.DefaultStyleKey = typeof(SlidableListItem);
            Button butt = new Button();
        }

        protected override void OnApplyTemplate()
        {
            contentGrid = this.GetTemplateChild("ContentGrid") as Grid;
            transform = contentGrid.RenderTransform as CompositeTransform;
            slider = this.GetTemplateChild("Slider") as Grid;

            leftCommandPanel = this.GetTemplateChild("leftCommandPanel") as StackPanel;
            leftCommandTransform = leftCommandPanel.RenderTransform as CompositeTransform;
            rightCommandPanel = this.GetTemplateChild("rightCommandPanel") as StackPanel;
            rightCommandTransform = rightCommandPanel.RenderTransform as CompositeTransform;

            Setup();
            base.OnApplyTemplate();
        }




        public Symbol LeftIcon
        {
            get { return (Symbol)GetValue(LeftIconProperty); }
            set { SetValue(LeftIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIconProperty =
            DependencyProperty.Register("LeftIcon", typeof(Symbol), typeof(SlidableListItem), new PropertyMetadata(Symbol.Favorite, new PropertyChangedCallback(OnLeftIconChanged)));

        private static void OnLeftIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public Symbol RightIcon
        {
            get { return (Symbol)GetValue(RightIconProperty); }
            set { SetValue(RightIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightIconProperty =
            DependencyProperty.Register("RightIcon", typeof(Symbol), typeof(SlidableListItem), new PropertyMetadata(Symbol.Delete, new PropertyChangedCallback(OnRightIconChanged)));

        private static void OnRightIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }



        public string LeftLabel
        {
            get { return (string)GetValue(LeftLabelProperty); }
            set { SetValue(LeftLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftLabelProperty =
            DependencyProperty.Register("LeftLabel", typeof(string), typeof(SlidableListItem), new PropertyMetadata("", new PropertyChangedCallback(OnLeftLabelChanged)));

        private static void OnLeftLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public string RightLabel
        {
            get { return (string)GetValue(RightLabelProperty); }
            set { SetValue(RightLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightLabelProperty =
            DependencyProperty.Register("RightLabel", typeof(string), typeof(SlidableListItem), new PropertyMetadata("", new PropertyChangedCallback(OnRightLabelChanged)));

        private static void OnRightLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }



        public Brush CommandForeground
        {
            get { return (Brush)GetValue(CommandForegroundProperty); }
            set { SetValue(CommandForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandForegroundProperty =
            DependencyProperty.Register("CommandForeground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public Brush LeftCommandBackground
        {
            get { return (Brush)GetValue(LeftCommandBackgroundProperty); }
            set { SetValue(LeftCommandBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftCommandForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftCommandBackgroundProperty =
            DependencyProperty.Register("LeftCommandBackground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));




        public Brush RightCommandBackground
        {
            get { return (Brush)GetValue(RightCommandBackgroundProperty); }
            set { SetValue(RightCommandBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightCommandForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightCommandBackgroundProperty =
            DependencyProperty.Register("RightCommandBackground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));




        public bool MouseSlidingEnabled
        {
            get { return (bool)GetValue(MouseSlidingEnabledProperty); }
            set { SetValue(MouseSlidingEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseSlidingEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseSlidingEnabledProperty =
            DependencyProperty.Register("MouseSlidingEnabled", typeof(bool), typeof(SlidableListItem), new PropertyMetadata(false));



        public ICommand LeftCommand
        {
            get { return (ICommand)GetValue(LeftCommandProperty); }
            set
            {
                if (value != null)
                {
                    value.CanExecuteChanged += LeftCommand_CanExecuteChanged;
                }
                SetValue(LeftCommandProperty, value);
            }
        }

        private void LeftCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("LeftCommand");
        }

        // Using a DependencyProperty as the backing store for LeftCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftCommandProperty =
            DependencyProperty.Register("LeftCommand", typeof(ICommand), typeof(SlidableListItem), new PropertyMetadata(null));




        public ICommand RightCommand
        {
            get { return (ICommand)GetValue(RightCommandProperty); }
            set
            {
                if (value != null)
                {
                    value.CanExecuteChanged += RightCommand_CanExecuteChanged;
                }
                SetValue(RightCommandProperty, value);
            }
        }

        private void RightCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("RightCommand");
        }

        // Using a DependencyProperty as the backing store for RightCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightCommandProperty =
            DependencyProperty.Register("RightCommand", typeof(ICommand), typeof(SlidableListItem), new PropertyMetadata(null));





        private void Setup()
        {
            contentGrid.ManipulationStarted += ContentGrid_ManipulationStarted;
            contentGrid.ManipulationDelta += ContentGrid_ManipulationDelta;
            contentGrid.ManipulationCompleted += ContentGrid_ManipulationCompleted;

            contentAnimation = new DoubleAnimation();
            Storyboard.SetTarget(contentAnimation, transform);
            Storyboard.SetTargetProperty(contentAnimation, "TranslateX");
            contentAnimation.To = 0;
            contentAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));

            contentStoryboard = new Storyboard();
            contentStoryboard.Children.Add(contentAnimation);

        }

        private void ContentGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!MouseSlidingEnabled && e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            var x = transform.TranslateX;
            contentAnimation.From = x;
            contentStoryboard.Begin();

            if (x < -ACTIVATED_THRESHOLD)
            {
                if (RightCommandActivated != null)
                    RightCommandActivated(this, new EventArgs());
            }
            else if (x > ACTIVATED_THRESHOLD)
            {
                if (LeftCommandActivated != null)
                    LeftCommandActivated(this, new EventArgs());
                if (LeftCommand != null)
                    LeftCommand.Execute(null);
            }
        }

        private void ContentGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            if (!MouseSlidingEnabled && e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            transform.TranslateX += e.Delta.Translation.X;
            var abs = Math.Abs(transform.TranslateX);

            if (transform.TranslateX > 0)
            {
                slider.Background = RightCommandBackground as SolidColorBrush;

                leftCommandPanel.Opacity = 1;
                rightCommandPanel.Opacity = 0;

                if (abs < 200)
                    leftCommandTransform.TranslateX = transform.TranslateX / 2;
                else
                    leftCommandTransform.TranslateX = 20;
            }
            else
            {
                slider.Background = LeftCommandBackground as SolidColorBrush;

                rightCommandPanel.Opacity = 1;
                leftCommandPanel.Opacity = 0;

                if (abs < 200)
                    rightCommandTransform.TranslateX = transform.TranslateX / 2;
                else
                    rightCommandTransform.TranslateX = -20;
            }

        }

        private void ContentGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            
        }
    }
}
