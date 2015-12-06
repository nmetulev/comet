using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Comet.Controls
{
    /// <summary>
    /// RangeSelector is a "double slider" control for range values.
    /// </summary>
    public sealed class RangeSelector : Control
    {
        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSelector), new PropertyMetadata(0.0, null));

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSelector), new PropertyMetadata(1.0, null));

        /// <summary>
        /// Identifies the RangeMin dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeMinProperty = DependencyProperty.Register("RangeMin", typeof(double), typeof(RangeSelector), new PropertyMetadata(0.0, null));

        /// <summary>
        /// Identifies the RangeMax dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeMaxProperty = DependencyProperty.Register("RangeMax", typeof(double), typeof(RangeSelector), new PropertyMetadata(1.0, null));

        Rectangle ActiveRectangle;
        Thumb MinThumb;
        Thumb MaxThumb;
        Canvas ContainerCanvas;
        double _oldValue;


        /// <summary>
        /// Event raised when lower or upper range values are changed.
        /// </summary>
        public event EventHandler<RangeChangedEventArgs> ValueChanged;

        /// <summary>
        /// Create a default range selector control.
        /// </summary>
        public RangeSelector()
        {
            DefaultStyleKey = typeof(RangeSelector);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            ActiveRectangle = GetTemplateChild("ActiveRectangle") as Rectangle;
            MinThumb = GetTemplateChild("MinThumb") as Thumb;
            MaxThumb = GetTemplateChild("MaxThumb") as Thumb;
            ContainerCanvas = GetTemplateChild("ContainerCanvas") as Canvas;

            MinThumb.DragCompleted += Thumb_DragCompleted;
            MinThumb.DragDelta += MinThumb_DragDelta;
            MinThumb.DragStarted += MinThumb_DragStarted;

            MaxThumb.DragCompleted += Thumb_DragCompleted;
            MaxThumb.DragDelta += MaxThumb_DragDelta;
            MaxThumb.DragStarted += MaxThumb_DragStarted;

            ContainerCanvas.SizeChanged += ContainerCanvas_SizeChanged;

            base.OnApplyTemplate();
        }

        private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SyncThumbs();
        }

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                if (value > Maximum)
                {
                    value = Maximum;
                }
                SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                if (value < Minimum)
                {
                    value = Minimum;
                }
                SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current lower limit value of the range.
        /// </summary>
        /// <value>
        /// The current lower limit.
        /// </value>
        public double RangeMin
        {
            get
            {
                return (double)GetValue(RangeMinProperty);
            }
            set
            {
                if (value < Minimum)
                {
                    value = Minimum;
                }

                if (value > Maximum)
                {
                    value = Maximum;
                }

                if (value > RangeMax)
                {
                    RangeMax = value;
                }

                SetValue(RangeMinProperty, value);
                SyncThumbs();
            }
        }

        /// <summary>
        /// Gets or sets the current upper limit value of the range.
        /// </summary>
        /// <value>
        /// The current upper limit.
        /// </value>
        public double RangeMax
        {
            get
            {
                return (double)GetValue(RangeMaxProperty);
            }
            set
            {
                if (value < Minimum)
                {
                    value = Minimum;
                }

                if (value > Maximum)
                {
                    value = Maximum;
                }

                if (value < RangeMin)
                {
                    RangeMin = value;
                }
                SetValue(RangeMaxProperty, value);
                SyncThumbs();
            }
        }

        private void SyncThumbs()
        {
            if (ContainerCanvas == null)
            {
                return;
            }

            var relativeLeft = ((RangeMin - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;
            var relativeRight = ((RangeMax - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;

            Canvas.SetLeft(MinThumb, relativeLeft);
            Canvas.SetLeft(ActiveRectangle, relativeLeft);

            Canvas.SetLeft(MaxThumb, relativeRight);

            ActiveRectangle.Width = Canvas.GetLeft(MaxThumb) - Canvas.GetLeft(MinThumb);
        }

        private void MinThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            RangeMin = DragThumb(MinThumb, 0, Canvas.GetLeft(MaxThumb), e);
        }

        private void MaxThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            RangeMax = DragThumb(MaxThumb, Canvas.GetLeft(MinThumb), ContainerCanvas.ActualWidth, e);
        }

        private double DragThumb(Thumb thumb, double min, double max, DragDeltaEventArgs e)
        {
            var currentPos = Canvas.GetLeft(thumb);
            var nextPos = currentPos + e.HorizontalChange;

            nextPos = Math.Max(min, nextPos);
            nextPos = Math.Min(max, nextPos);

            Canvas.SetLeft(thumb, nextPos);

            return (Minimum + (nextPos / ContainerCanvas.ActualWidth) * (Maximum - Minimum)); ;
        }

        private void MinThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            Canvas.SetZIndex(MinThumb, 10);
            Canvas.SetZIndex(MaxThumb, 0);
            _oldValue = RangeMin;
        }

        private void MaxThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            Canvas.SetZIndex(MinThumb, 0);
            Canvas.SetZIndex(MaxThumb, 10);
            _oldValue = RangeMax;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ValueChanged != null)
            {
                if ((sender.Equals(MinThumb)))
                {
                    ValueChanged(this, new RangeChangedEventArgs(_oldValue, RangeMin, RangeSelectorProperty.MinimumValue));
                }
                else
                {
                    ValueChanged(this, new RangeChangedEventArgs(_oldValue, RangeMax, RangeSelectorProperty.MaximumValue));
                }
            }
        }
    }
}

