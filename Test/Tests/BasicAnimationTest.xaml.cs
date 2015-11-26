using Comet.Animations;
using Comet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Test.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicAnimationTest : Page
    {
        List<string> Effects;
        List<string> Easings;

        public BasicAnimationTest()
        {
            this.InitializeComponent();

            Effects = new List<string>();
            Effects.Add("SlideIn");
            Effects.Add("SlideOut");
            Effects.Add("FadeIn");
            Effects.Add("FadeOut");
            
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (effectSelection.SelectedItem != null)
            {
                DoEffect(effectSelection.SelectedItem as string);
            }
        }

        private void DoEffect(string effect)
        {
            switch(effect)
            {
                case "SlideIn":
                    border.SlideIn();
                    break;
                case "SlideOut":
                    border.SlideOut();
                    break;
                case "FadeIn":
                    border.FadeIn().Go();
                    break;
                case "FadeOut":
                    border.FadeOut().Go();
                    break;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //border.RenderTransform = null;
            //border.Hide();
        }
    }
}
