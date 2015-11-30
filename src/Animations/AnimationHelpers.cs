using Comet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Comet.Animations
{
    public static class AnimationHelpers
    {
        private const Easing _defaultEasing = Easing.SineEaseInOut;
        private const double _defaultDuration = 400;

        public static Animation Move(this UIElement element, double x, double y, double duration = _defaultDuration, Easing easing = _defaultEasing)
        {
            Animation animation = new Animation(element);
            return animation.Move(x, y, duration, easing);
        }

        public static Animation Move(this Animation animation, double x, double y, double duration = _defaultDuration, Easing easing = _defaultEasing)
        {
            var func = EasingEnumToEasingFunction(easing);
            animation.AddTranslationX(x, duration: duration, easingFunction: func);
            animation.AddTranslationY(y, duration: duration, easingFunction: func);
            return animation;
        }

        #region SlideIn
        public static Task SlideIn(this UIElement element)
        {
            return element.SlideIn(Direction.Down);
        }

        public static Task SlideIn(this UIElement element, Direction direction)
        {
            return element.SlideIn(direction, _defaultDuration, _defaultEasing);
        }

        public static Task SlideIn(this UIElement element, Easing easing)
        {
            return element.SlideIn(Direction.Down, _defaultDuration, easing);
        }

        public static Task SlideIn(this UIElement element, Direction direction = Direction.Down, double duration = _defaultDuration, Easing easing = _defaultEasing)
        {
            //if (!(element.RenderTransform is CompositeTransform))
                element.RenderTransform = new CompositeTransform();

            CompositeTransform tr = element.RenderTransform as CompositeTransform;

            var p = element.GetAbsoluteCoordinates();

            double startX = tr.TranslateX;
            double startY = tr.TranslateY;
            double finalX = startX;
            double finalY = startY;
            switch (direction)
            {
                case Direction.Down:
                    startY = -p.Y - element.DesiredSize.Height;
                    break;
                case Direction.Up:
                    startY = Window.Current.Bounds.Height - p.Y;
                    break;
                case Direction.Left:
                    startX = -p.X - element.DesiredSize.Width;
                    break;
                case Direction.Right:
                    startX = Window.Current.Bounds.Width - p.X;
                    break;
            }
            
            tr.TranslateX = startX;
            tr.TranslateY = startY;
            return element.Move(finalX, finalY, duration, easing).GoAsync();
        }
        #endregion

        #region SlideOut
        public static Task SlideOut(this UIElement element)
        {
            return element.SlideOut(Direction.Up, _defaultDuration, _defaultEasing);
        }

        public static Task SlideOut(this UIElement element, Direction direction)
        {
            return element.SlideOut(direction, _defaultDuration, _defaultEasing);
        }

        public static Task SlideOut(this UIElement element, Easing easing)
        {
            return element.SlideOut(Direction.Up, _defaultDuration, easing);
        }

        public static Task SlideOut(this UIElement element, Direction direction = Direction.Up, double duration = _defaultDuration, Easing easing = _defaultEasing)
        {
            //if (!(element.RenderTransform is CompositeTransform))
                element.RenderTransform = new CompositeTransform();

            CompositeTransform tr = element.RenderTransform as CompositeTransform;

            var p = element.GetAbsoluteCoordinates();
            
            double finalX = 0;
            double finalY = 0;
            switch (direction)
            {
                case Direction.Up:
                    finalY = -p.Y - element.DesiredSize.Height;
                    break;
                case Direction.Down:
                    finalY = Window.Current.Bounds.Height - p.Y;
                    break;
                case Direction.Left:
                    finalX = -p.X - element.DesiredSize.Width;
                    break;
                case Direction.Right:
                    finalX = Window.Current.Bounds.Width - p.X;
                    break;
            }
            
            return element.Move(finalX, finalY, duration, easing).GoAsync();
        }
        #endregion


        public static Animation FadeOut (this UIElement element)
        {
            Animation animation = new Animation(element);
            animation.AddOpacity(0);
            return animation;
        }

        public static Animation FadeOut (this Animation animation)
        {
            animation.AddOpacity(0);
            return animation;
        }

        public static Animation FadeIn(this UIElement element)
        {
            Animation animation = new Animation(element);
            animation.AddOpacity(1);
            return animation;
        }

        public static Animation FadeIn(this Animation animation)
        {
            animation.AddOpacity(1);
            return animation;
        }

        private static EasingFunctionBase EasingEnumToEasingFunction(Easing easing)
        {
            EasingFunctionBase func;
            switch (easing)
            {
                case Easing.Linear:
                    return null;

                case Easing.SineEaseIn:
                    func = new SineEase();
                    func.EasingMode = EasingMode.EaseIn;
                    return func;
                case Easing.SineEaseOut:
                    func = new SineEase();
                    func.EasingMode = EasingMode.EaseOut;
                    return func;
                case Easing.SineEaseInOut:
                    func = new SineEase();
                    func.EasingMode = EasingMode.EaseInOut;
                    return func;

                case Easing.CubicEaseIn:
                    func = new CubicEase();
                    func.EasingMode = EasingMode.EaseIn;
                    return func;
                case Easing.CubicEaseOut:
                    func = new CubicEase();
                    func.EasingMode = EasingMode.EaseOut;
                    return func;
                case Easing.CubicEaseInOut:
                    func = new CubicEase();
                    func.EasingMode = EasingMode.EaseInOut;
                    return func;

                case Easing.BounceEaseIn:
                    func = new BounceEase();
                    func.EasingMode = EasingMode.EaseIn;
                    (func as BounceEase).Bounces = 2;
                    (func as BounceEase).Bounciness = 6;
                    return func;
                case Easing.BounceEaseOut:
                    func = new BounceEase();
                    func.EasingMode = EasingMode.EaseOut;
                    (func as BounceEase).Bounces = 2;
                    (func as BounceEase).Bounciness = 6;
                    return func;
                case Easing.BounceEaseInOut:
                    func = new BounceEase();
                    func.EasingMode = EasingMode.EaseInOut;
                    (func as BounceEase).Bounces = 2;
                    (func as BounceEase).Bounciness = 6;
                    return func;

                default:
                    return null;
            }
        }
        

        // todo

        // zoom in, zoom out, skew

        // hide,show, toggle

        // easing functions

        // fadeto
    }

    public enum Easing
    {
        Linear,

        BackEaseIn,
        BackEaseOut,
        BackEaseInOut,

        CircleEaseIn,
        CircleEaseOut,
        CircleEaseInOut,

        CubicEaseIn,
        CubicEaseOut,
        CubicEaseInOut,

        ElasticEaseIn,
        ElasticEaseOut,
        ElasticEaseInOut,

        PowerEaseIn,
        PowerEaseOut,
        PowerEaseInOut,

        QuadEaseIn,
        QuadEaseOut,
        QuadEaseInOut,

        QuinticEaseIn,
        QuinticEaseOut,
        QuinticEaseInOut,

        QuerticEaseIn,
        QuerticEaseOut,
        QuerticEaseInOut,

        SineEaseIn,
        SineEaseOut,
        SineEaseInOut,

        BounceEaseIn,
        BounceEaseOut,
        BounceEaseInOut,
    }
}
