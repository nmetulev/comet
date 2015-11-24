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
        public static Animation Move( this UIElement element)
        {
            Animation animation = new Animation();
            return animation;
        }

        public static Animation Move (this Animation animation)
        {
            return animation;
        }

        public static Animation Fade (this UIElement element)
        {
            Animation animation = new Animation();
            return animation;
        }

        public static Animation Fade (this Animation animation)
        {
            return animation;
        }
    }
}
