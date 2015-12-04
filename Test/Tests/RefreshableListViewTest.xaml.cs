using Sample.Data;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Test.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RefreshableListViewTest : Page
    {
        ObservableCollection<Item> Items;
        public RefreshableListViewTest()
        {
            LeakTracker.Add(this);
            this.InitializeComponent();
            Items = new ObservableCollection<Item>();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            populateData();
        }

        private void populateData()
        {
            Items.Clear();
            for (int i = 0; i < 40; i++)
            {
                Items.Add(new Item() { Title = "Item " + new Random().Next(10000) });
            }
        }

        private void listView_RefreshCommand(object sender, EventArgs e)
        {
            populateData();
        }

        private void listView_PullProgressChanged(object sender, Comet.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;

            refreshindicator.Background = e.PullProgress < 1.0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Blue);
        }
    }
}
