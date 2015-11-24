using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Comet.Animations
{
    public static class AnimationHelpers
    {
        public static Animation Move(this UIElement element, double x, double y)
        {
            Animation animation = new Animation(element);
            animation.AddTranslationX(x);
            animation.AddTranslationY(y);
            return animation;
        }

        public static Animation Move (this Animation animation, double x, double y)
        {
            animation.AddTranslationX(x);
            animation.AddTranslationY(y);
            return animation;
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
