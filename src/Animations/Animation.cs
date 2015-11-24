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
        private const float _defaultDuration = 100f;

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

        public void AddTranslationX(double value)
        {
            AddTranslationX(value, _defaultDuration);
        }

        public void AddTranslationX(double value, double duration)
        {
            var anim = AddOrUpdateDoubleAnimation("TranslateX", value, duration);
            anim.From = (_element.RenderTransform as CompositeTransform).TranslateX;
            Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
        }

        public void AddTranslationY(double value)
        {
            AddTranslationY(value, _defaultDuration);
        }

        public void AddTranslationY(double value, double duration)
        {
            var anim = AddOrUpdateDoubleAnimation("TranslateY", value, duration);
            anim.From = (_element.RenderTransform as CompositeTransform).TranslateY;
            Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
        }

        public void AddOpacity(double value)
        {
            AddOpacity(value, _defaultDuration);
        }

        public void AddOpacity(double value, double duration)
        {
            var anim = AddOrUpdateDoubleAnimation("Opacity", value, duration);
            Storyboard.SetTargetProperty(anim, "(UIElement.Opacity)");
        }
    }
}
