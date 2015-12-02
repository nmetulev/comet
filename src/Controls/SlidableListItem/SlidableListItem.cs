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
    /// <summary>
    /// ContentControl prividing functinality for sliding left or right to expose functions
    /// </summary>
    [TemplatePart(Name = PART_CONTENT_GRID, Type = typeof(Grid))]
    [TemplatePart(Name = PART_COMMAND_CONTAINER, Type = typeof(Grid))]
    [TemplatePart(Name = PART_LEFT_COMMAND_PANEL, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_RIGHT_COMMAND_PANEL, Type = typeof(StackPanel))]
    public class SlidableListItem : ContentControl
    {
        #region Private Variables
        const string PART_CONTENT_GRID = "ContentGrid";
        const string PART_COMMAND_CONTAINER = "CommandContainer";
        const string PART_LEFT_COMMAND_PANEL = "LeftCommandPanel";
        const string PART_RIGHT_COMMAND_PANEL = "RightCommandPanel";
        // Content Container
        private Grid contentGrid;
        // transform for sliding content
        private CompositeTransform transform;
        // container for command content
        private Grid commandContainer;
        // container for left command content
        private StackPanel leftCommandPanel;
        // transform for left command content
        private CompositeTransform leftCommandTransform;
        // container for right command content
        private StackPanel rightCommandPanel;
        // transform for right command content
        private CompositeTransform rightCommandTransform;
        // doubleanimation for snaping back to default position
        private DoubleAnimation contentAnimation;
        // storyboard for snaping back to default position
        private Storyboard contentStoryboard;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the user swipes to the left to activate the right action
        /// </summary>
        public event EventHandler RightCommandRequested;

        /// <summary>
        /// Occurs when the user swipes to the right to activate the left action
        /// </summary>
        public event EventHandler LeftCommandRequested;
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="SlidableListItem"/>
        /// </summary>
        public SlidableListItem()
        {
            this.DefaultStyleKey = typeof(SlidableListItem);
        }

        #region Methods and Events
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding
        /// layout pass) call <see cref="OnApplyTemplate"/>. In simplest terms, this means the method
        /// is called just before a UI element displays in an application. Override this
        /// method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            contentGrid = this.GetTemplateChild(PART_CONTENT_GRID) as Grid;
            commandContainer = this.GetTemplateChild(PART_COMMAND_CONTAINER) as Grid;
            leftCommandPanel = this.GetTemplateChild(PART_LEFT_COMMAND_PANEL) as StackPanel;
            rightCommandPanel = this.GetTemplateChild(PART_RIGHT_COMMAND_PANEL) as StackPanel;

            transform = contentGrid.RenderTransform as CompositeTransform;

            leftCommandTransform = leftCommandPanel.RenderTransform as CompositeTransform;
            rightCommandTransform = rightCommandPanel.RenderTransform as CompositeTransform;

            contentGrid.ManipulationDelta += ContentGrid_ManipulationDelta;
            contentGrid.ManipulationCompleted += ContentGrid_ManipulationCompleted;

            contentAnimation = new DoubleAnimation();
            Storyboard.SetTarget(contentAnimation, transform);
            Storyboard.SetTargetProperty(contentAnimation, "TranslateX");
            contentAnimation.To = 0;
            contentAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));

            contentStoryboard = new Storyboard();
            contentStoryboard.Children.Add(contentAnimation);

            commandContainer.Background = LeftBackground as SolidColorBrush;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handler for when slide manipulation is complete
        /// </summary>
        private void ContentGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!MouseSlidingEnabled && e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            var x = transform.TranslateX;
            contentAnimation.From = x;
            contentStoryboard.Begin();

            leftCommandTransform.TranslateX = 0;
            rightCommandTransform.TranslateX = 0;
            leftCommandPanel.Opacity = 1;
            rightCommandPanel.Opacity = 1;

            if (x < -ActivationWidth)
            {
                if (RightCommandRequested != null)
                    RightCommandRequested(this, new EventArgs());
                if (RightCommand != null)
                    RightCommand.Execute(null);
            }
            else if (x > ActivationWidth)
            {
                if (LeftCommandRequested != null)
                    LeftCommandRequested(this, new EventArgs());
                if (LeftCommand != null)
                    LeftCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handler for when slide manipulation is underway
        /// </summary>
        private void ContentGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            if (!MouseSlidingEnabled && e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            transform.TranslateX += e.Delta.Translation.X;
            var abs = Math.Abs(transform.TranslateX);

            if (transform.TranslateX > 0)
            {
                commandContainer.Background = LeftBackground as SolidColorBrush;

                leftCommandPanel.Opacity = 1;
                rightCommandPanel.Opacity = 0;

                if (abs < ActivationWidth)
                    leftCommandTransform.TranslateX = transform.TranslateX / 2;
                else
                    leftCommandTransform.TranslateX = 20;
            }
            else
            {
                commandContainer.Background = RightBackground as SolidColorBrush;

                rightCommandPanel.Opacity = 1;
                leftCommandPanel.Opacity = 0;

                if (abs < ActivationWidth)
                    rightCommandTransform.TranslateX = transform.TranslateX / 2;
                else
                    rightCommandTransform.TranslateX = -20;
            }

        }
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the amount of pixels the content needs to be swiped for an 
        /// action to be requested
        /// </summary>
        public double ActivationWidth
        {
            get { return (double)GetValue(ActivationWidthProperty); }
            set { SetValue(ActivationWidthProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="ActivationWidth"/> property
        /// </summary>
        public static readonly DependencyProperty ActivationWidthProperty =
            DependencyProperty.Register("ActivationWidth", typeof(double), typeof(SlidableListItem), new PropertyMetadata(80));

        /// <summary>
        /// Gets or sets the left icon symbol
        /// </summary>
        public Symbol LeftIcon
        {
            get { return (Symbol)GetValue(LeftIconProperty); }
            set { SetValue(LeftIconProperty, value); }
        }

        /// <summary>
        /// Indeifies the <see cref="LeftIcon"/> property
        /// </summary>
        public static readonly DependencyProperty LeftIconProperty =
            DependencyProperty.Register("LeftIcon", typeof(Symbol), typeof(SlidableListItem), new PropertyMetadata(Symbol.Favorite));
        
        /// <summary>
        /// Gets or sets the right icon symbol
        /// </summary>
        public Symbol RightIcon
        {
            get { return (Symbol)GetValue(RightIconProperty); }
            set { SetValue(RightIconProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="RightIcon"/> property
        /// </summary>
        public static readonly DependencyProperty RightIconProperty =
            DependencyProperty.Register("RightIcon", typeof(Symbol), typeof(SlidableListItem), new PropertyMetadata(Symbol.Delete));

        /// <summary>
        /// Gets or sets the left label
        /// </summary>
        public string LeftLabel
        {
            get { return (string)GetValue(LeftLabelProperty); }
            set { SetValue(LeftLabelProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="LeftLabel"/> property
        /// </summary>
        public static readonly DependencyProperty LeftLabelProperty =
            DependencyProperty.Register("LeftLabel", typeof(string), typeof(SlidableListItem), new PropertyMetadata(""));
        
        /// <summary>
        /// Gets or sets the right label
        /// </summary>
        public string RightLabel
        {
            get { return (string)GetValue(RightLabelProperty); }
            set { SetValue(RightLabelProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="RightLabel"/> property
        /// </summary>
        public static readonly DependencyProperty RightLabelProperty =
            DependencyProperty.Register("RightLabel", typeof(string), typeof(SlidableListItem), new PropertyMetadata(""));
        
        /// <summary>
        /// Gets or sets the left foreground color
        /// </summary>
        public Brush LeftForeground
        {
            get { return (Brush)GetValue(LeftForegroundProperty); }
            set { SetValue(LeftForegroundProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="LeftForeground"/> property
        /// </summary>
        public static readonly DependencyProperty LeftForegroundProperty =
            DependencyProperty.Register("LeftForeground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the right foreground color
        /// </summary>
        public Brush RightForeground
        {
            get { return (Brush)GetValue(RightForegroundProperty); }
            set { SetValue(RightForegroundProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="RightForeground"/> property
        /// </summary>
        public static readonly DependencyProperty RightForegroundProperty =
            DependencyProperty.Register("RightForeground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the left background color
        /// </summary>
        public Brush LeftBackground
        {
            get { return (Brush)GetValue(LeftBackgroundProperty); }
            set { SetValue(LeftBackgroundProperty, value); }
        }

        /// <summary>
        /// Indetifies the <see cref="LeftBackground"/> property
        /// </summary>
        public static readonly DependencyProperty LeftBackgroundProperty =
            DependencyProperty.Register("LeftBackground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Gets or sets the right background color
        /// </summary>
        public Brush RightBackground
        {
            get { return (Brush)GetValue(RightBackgroundProperty); }
            set { SetValue(RightBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RightBackground"/> property
        /// </summary>
        public static readonly DependencyProperty RightBackgroundProperty =
            DependencyProperty.Register("RightBackground", typeof(Brush), typeof(SlidableListItem), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        /// <summary>
        /// Gets or sets the ability to slide the control with the mouse. False by default
        /// </summary>
        public bool MouseSlidingEnabled
        {
            get { return (bool)GetValue(MouseSlidingEnabledProperty); }
            set { SetValue(MouseSlidingEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MouseSlidingEnabled"/> property
        /// </summary>
        public static readonly DependencyProperty MouseSlidingEnabledProperty =
            DependencyProperty.Register("MouseSlidingEnabled", typeof(bool), typeof(SlidableListItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the ICommand for left command request
        /// </summary>
        public ICommand LeftCommand
        {
            get { return (ICommand)GetValue(LeftCommandProperty); }
            set
            {
                //if (value != null)
                //{
                //    value.CanExecuteChanged += LeftCommand_CanExecuteChanged;
                //}
                SetValue(LeftCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="LeftCommand"/> property
        /// </summary>
        public static readonly DependencyProperty LeftCommandProperty =
            DependencyProperty.Register("LeftCommand", typeof(ICommand), typeof(SlidableListItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ICommand for right command request
        /// </summary>
        public ICommand RightCommand
        {
            get { return (ICommand)GetValue(RightCommandProperty); }
            set
            {
                //if (value != null)
                //{
                //    value.CanExecuteChanged += RightCommand_CanExecuteChanged;
                //}
                SetValue(RightCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="RightCommand"/> property
        /// </summary>
        public static readonly DependencyProperty RightCommandProperty =
            DependencyProperty.Register("RightCommand", typeof(ICommand), typeof(SlidableListItem), new PropertyMetadata(null));

        #endregion
    }
}
