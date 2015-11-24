using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Comet.Animations
{
    public class Animation
    {
        private Storyboard _story { get; set; }

        public Animation()
        {
            _story = new Storyboard();
        }
    }
}
