using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Comet.Extensions
{
    public static class XAMLExtensions
    {
        /// <summary>
        /// Render a UIElement into a bitmap IRandomAccessStream
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the IRandomAccessStream</returns>
        public static async Task<IRandomAccessStream> RenderToRandomAccessStreamAsync(this UIElement element)
        {
            if (element == null) throw new NullReferenceException();

            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(element);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();

            var displayInformation = DisplayInformation.GetForCurrentView();

            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                 BitmapAlphaMode.Premultiplied,
                                 (uint)rtb.PixelWidth,
                                 (uint)rtb.PixelHeight,
                                 displayInformation.RawDpiX,
                                 displayInformation.RawDpiY,
                                 pixels);

            await encoder.FlushAsync();
            stream.Seek(0);

            return stream;
        }

        /// <summary>
        /// Render a UIElement into a bitmap IRandomAccessStream
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the IRandomAccessStream</returns>
        [Obsolete("RenderToRandomAccessStream has been renamed to RenderToRandomAccessStreamAsync. This method will be removed in future releases")]
        public static Task<IRandomAccessStream> RenderToRandomAccessStream(this UIElement element)
        {
            return element.RenderToRandomAccessStreamAsync();
        }

        /// <summary>
        /// Render a UIElement into a bitmap
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the BitmapImage</returns>
        public static async Task<BitmapImage> RenderToBitmapImageAsync(this UIElement element)
        {
            using (var stream = await element.RenderToRandomAccessStreamAsync())
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(stream);
                return image;
            }
        }

        /// <summary>
        /// Render a UIElement into a bitmap
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the BitmapImage</returns>
        [Obsolete ("RenderToBitmapImage has been renamed to RenderToBitmapImageAsync. This method will be removed in future releases")]
        public static Task<BitmapImage> RenderToBitmapImage(this UIElement element)
        {
            return element.RenderToBitmapImageAsync();
        }

        /// <summary>
        /// Traverses the Visual Tree and returns a list of elements of type T
        /// </summary>
        /// <typeparam name="T">The type of elements to find</typeparam>
        /// <param name="parent">The root of the Visual Tree</param>
        /// <returns>A list of elements of type T, or null</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject parent)
               where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                yield break;
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If the child is not of the request child type child
                var childType = child as T;

                if (childType != null)
                {
                    yield return childType;
                }

                foreach (var grandChild in FindChildren<T>(child))
                {
                    yield return grandChild;
                }
            }
        }
    }
}
