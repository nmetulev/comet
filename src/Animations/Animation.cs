using Comet.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;


namespace Comet.Animations
{
    public class Animation
    {
        private Storyboard _story;

        private UIElement _element;

        private Dictionary<string, Timeline> _timelines;

        // default duration in milliseconds
        private const double _defaultDuration = 400.0;

        public Animation(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element must be a UIElement and must not be null");
            _element = element;
            _story = new Storyboard();
            _timelines = new Dictionary<string, Timeline>();
        }

        public void Go()
        {
            _story.Stop();
            _story.Begin();
        }

        public Task GoAsync()
        {
            _story.Stop();
            return _story.BeginAsync();
        }


        private DoubleAnimation AddOrUpdateDoubleAnimation(string key, double To, double duration)
        {
            DoubleAnimation anim;
            if (_timelines.ContainsKey(key))
            {
                anim = _timelines[key] as DoubleAnimation;
            }
            else
            {
                if (!(_element.RenderTransform is CompositeTransform))
                    _element.RenderTransform = new CompositeTransform();

                anim = new DoubleAnimation();
                _story.Children.Add(anim);
                _timelines[key] = anim;
            }

            anim.To = To;
            anim.Duration = TimeSpan.FromMilliseconds(duration);
            Storyboard.SetTarget(anim, _element);

            return anim;
        }

        private DoubleAnimation AddOrUpdateDoubleAnimation(string key, double From, double To, double duration)
        {
            DoubleAnimation anim = AddOrUpdateDoubleAnimation(key, To, duration);
            anim.From = From;
            return anim;
        }

        public void AddTranslationX(double To)
        {
            AddTranslationX(To, _defaultDuration);
        }

        public void AddTranslationX(double To, double Duration)
        {
            AddTranslationX((_element.RenderTransform as CompositeTransform).TranslateX, To, Duration);
        }

        private void AddTranslationX(double From, double To, double Duration)
        {
            var anim = AddOrUpdateDoubleAnimation("TranslateX",From, To, Duration);
            Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
        }

        public void AddTranslationY(double value)
        {
            AddTranslationY(value, _defaultDuration);
        }

        public void AddTranslationY(double To, double Duration)
        {
            AddTranslationY((_element.RenderTransform as CompositeTransform).TranslateY, To, Duration);
        }

        public void AddTranslationY(double From, double To, double Duration)
        {
            var anim = AddOrUpdateDoubleAnimation("TranslateY", From, To, Duration);
            Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
        }

        public void AddOpacity(double To)
        {
            AddOpacity(To, _defaultDuration);
        }

        public void AddOpacity(double To, double Duration)
        {
            AddOpacity(_element.Opacity, To, Duration);
        }

        public void AddOpacity(double From, double To, double Duration)
        {
            var anim = AddOrUpdateDoubleAnimation("Opacity", From, To, Duration);
            Storyboard.SetTargetProperty(anim, "(UIElement.Opacity)");
        }
    }
}
