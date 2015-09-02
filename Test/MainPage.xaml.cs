using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Test.Tests;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var testItems = new List<TestItem>();
            testItems.Add(new TestItem() { Name = "RefreshableListView", Page = typeof(RefreshableListViewTest) });
            testItems.Add(new TestItem() { Name = "SlidableListItem", Page = typeof(SlidableListItem) });
            list.ItemsSource = testItems;
            frame.Navigate(testItems[0].Page);

            //view.Navigate(new Uri("http://bing.com"));
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            splitview.IsPaneOpen = !splitview.IsPaneOpen;
            return;
            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(Root);
            IBuffer buffer = await rtb.GetPixelsAsync();
            byte[] outputArray = buffer.ToArray();

            var localFolder = ApplicationData.Current.LocalFolder;
            var rtbData = await localFolder.CreateFileAsync("test.png", CreationCollisionOption.ReplaceExisting);
            using (var rtbStrem = await rtbData.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, rtbStrem);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)(rtb.PixelWidth), (uint)(rtb.PixelHeight), 96, 96, outputArray);
                await encoder.FlushAsync();


                var device = new CanvasDevice();
                var bitmap = await CanvasBitmap.LoadAsync(device, rtbStrem);

                var renderer = new CanvasRenderTarget(device, bitmap.SizeInPixels.Width, bitmap.SizeInPixels.Height, bitmap.Dpi);

                using (var ds = renderer.CreateDrawingSession())
                {
                    var blur = new GaussianBlurEffect();
                    blur.BlurAmount = 8.0f;
                    blur.BorderMode = EffectBorderMode.Hard;
                    blur.Optimization = EffectOptimization.Quality;
                    blur.Source = bitmap;
                    ds.DrawImage(blur);
                }

                rtbStrem.Size = 0;
                rtbStrem.Seek(0);
                await renderer.SaveAsync(rtbStrem, CanvasBitmapFileFormat.Png);

                Blured.Source = new BitmapImage(new Uri("ms-appdata:///local/test.png"));

            }



        }

        private void list_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                frame.Navigate((e.AddedItems[0] as TestItem).Page);
                splitview.IsPaneOpen = false;
            }
        }
    }

    public class TestItem
    {
        public string Name { get; set; }
        public Type Page { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
