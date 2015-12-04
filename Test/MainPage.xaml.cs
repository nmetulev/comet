using System;
using System.Collections.Generic;
using Test.Tests;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            testItems.Add(new TestItem() { Name = "PullToRefreshListView", Page = typeof(RefreshableListViewTest) });
            testItems.Add(new TestItem() { Name = "SlidableListItem", Page = typeof(SlidableListItem) });
            testItems.Add(new TestItem() { Name = "Extension:UIElementToImage", Page = typeof(UIElementToImage) });
            testItems.Add(new TestItem() { Name = "RangeSelector", Page = typeof(RangeSelectorTest) });
            list.ItemsSource = testItems;
            frame.Navigate(testItems[0].Page);

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            splitview.IsPaneOpen = !splitview.IsPaneOpen;
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
