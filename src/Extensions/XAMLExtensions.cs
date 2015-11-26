using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
        public static async Task<IRandomAccessStream> RenderToRandomAccessStream(this UIElement element)
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
        /// Render a UIElement into a bitmap
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the BitmapImage</returns>
        public static async Task<BitmapImage> RenderToBitmapImage(this UIElement element)
        {
            using (var stream = await element.RenderToRandomAccessStream())
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(stream);
                return image;
            }
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
            return parent._FindChildren<T>();
        }

        /// <summary>
        /// A helper function for FindChildren
        /// Traverses the Visual Tree and returns a list of elements of type T
        /// </summary>
        /// <typeparam name="T">The type of elements to find</typeparam>
        /// <param name="parent">The root of the Visual Tree</param>
        /// <param name="list">a list to be used in the recusive calls</param>
        /// <returns>A list of elements of type T, or null</returns>
        private static IEnumerable<T> _FindChildren<T>(this DependencyObject parent, List<T> list = null)
               where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            if (list == null) list = new List<T>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child

                T childType = child as T;
                if (childType == null)
                {
                    _FindChildren<T>(child, list);
                }
                else
                {
                    list.Add(childType);
                }
            }

            return list;
        }

        /// <summary>
        /// Begins a Storyboard animation and returns a task that completes when the 
        /// animaton is complete
        /// </summary>
        /// <param name="storyboard">The storyoard to be started</param>
        /// <returns>Task that completes when the animation is complete</returns>
        public static Task BeginAsync(this Storyboard storyboard)
        {
            var taskSource = new TaskCompletionSource<object>();
            EventHandler<object> completed = null;
            completed += (s, e) =>
            {
                storyboard.Completed -= completed;
                taskSource.SetResult(null);
            };

            storyboard.Completed += completed;
            storyboard.Begin();

            return taskSource.Task;
        }

        /// <summary>
        /// Returns a Point that contains absolute X and Y coordinates relative
        /// to current window. Note: if element is not part of the visual tree,
        /// the result will be 0, 0
        /// </summary>
        /// <param name="element">Element to find absolute coordinates</param>
        /// <returns>Point with absolute X and Y</returns>
        public static Point GetAbsoluteCoordinates(this UIElement element)
        {
            return element.TransformToVisual(Window.Current.Content)
                             .TransformPoint(new Point(0, 0));
        }

        /// <summary>
        /// Changes Visibility of element to Collapsed
        /// </summary>
        /// <param name="element">Element to hide</param>
        public static void Hide(this UIElement element)
        {
            element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes Visibility of element to Visible
        /// </summary>
        /// <param name="element">Element to show</param>
        public static void Show(this UIElement element)
        {
            element.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Changes Visibility of element to Collapsed or Visible depending on the
        /// current state
        /// </summary>
        /// <param name="element">Element to toggle visibility</param>
        public static void ToggleVisibility(this UIElement element)
        {
            element.Visibility = element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
