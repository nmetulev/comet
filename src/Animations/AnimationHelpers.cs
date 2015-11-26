using Comet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Comet.Animations
{
    public static class AnimationHelpers
    {
        public static Animation Move(this UIElement element, double x, double y)
        {
            Animation animation = new Animation(element);
            return animation.Move(x, y);
        }

        public static Animation Move(this Animation animation, double x, double y)
        {
            animation.AddTranslationX(x);
            animation.AddTranslationY(y);
            return animation;
        }

        public static Task SlideIn(this UIElement element)
        {
            return element.SlideIn(Direction.Down);
        }

        
        public static Task SlideIn(this UIElement element, Direction direction)
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
            return element.Move(finalX, finalY).GoAsync();
        }

        public static void SlideOut(this UIElement element)
        {
        }



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


        

        // todo

        // zoom in, zoom out, skew

        // hide,show, toggle

        // easing functions

        // fadeto

        // slidin, slideout (direction)
    }
}
