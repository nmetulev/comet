using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sample.Data;
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
    public sealed partial class SlidableListItem : Page
    {
        ObservableCollection<Item> Items;

        public SlidableListItem()
        {
            this.InitializeComponent();
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            for (var i = 0; i<1000; i++)
            {
                items.Add(new Item() { Title = "Item " + i });

            }
            

            Items = items;
        }

        private void SlidableListItem_RightCommandActivated(object sender, EventArgs e)
        {
            Items.Remove((sender as Comet.Controls.SlidableListItem).DataContext as Item);
        }
    }
}
