using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Test.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlexiblePanelTest : Page
    {
        ObservableCollection<string> Images = new ObservableCollection<string>();
        private DispatcherTimer timer;

        public FlexiblePanelTest()
        {
            this.InitializeComponent();
            GetImages();

            Window.Current.SizeChanged += Current_SizeChanged;
            MyList.Width = Window.Current.Bounds.Width;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            MyList.Width = Window.Current.Bounds.Width;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            timer.Start();
        }

        private async Task GetImages()
        {
            Images.Clear();

            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(new Uri("https://www.reddit.com/r/earthporn.json"));
                Data.RedditListing listings = await JsonConvert.DeserializeObjectAsync<Data.RedditListing>(result);

                foreach (var item in listings.data.children)
                {
                    if (item.data.preview != null &&
                        item.data.preview.images != null &&
                        item.data.preview.images.Count > 0)
                        Images.Add(item.data.preview.images.First().source.url);
                }
            }
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
        }
    }
}
