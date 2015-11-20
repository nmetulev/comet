using Sample.Data;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CometMailSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        ObservableCollection<Email> Emails;
        DateTime fakeNow;
        public MainPage()
        {
            this.InitializeComponent();
            fakeNow = DateTime.Now;
            Random gen = new Random();

            Emails = new ObservableCollection<Email>();
            for (int i = 0; i < 40; i++)
            {
                Email mail = new Email();
                mail.From = Faker.Name.FullName();
                mail.Subject = Faker.Lorem.Sentence();
                mail.Content = Faker.Lorem.Paragraph(3);
                mail.Time = DateTime.Now;
                mail.Read = gen.Next(100) < 80 ? true : false;
                Emails.Add(mail);
            }
        }

        private void SlidableListItem_RightCommandActivated(object sender, EventArgs e)
        {
            Emails.Remove((sender as Comet.Controls.SlidableListItem).DataContext as Email);
        }


        bool refreshing = false;

        private async void listView_RefreshActivated(object sender, EventArgs e)
        {
            if (!refreshing)
            {
                refreshing = true;
                progress.Visibility = Visibility.Visible;

                await Task.Delay(1000);

                for (int i = 0; i < 3; i++)
                {


                    Email mail = new Email();
                    mail.From = Faker.Name.FullName();
                    mail.Subject = Faker.Lorem.Sentence();
                    mail.Content = Faker.Lorem.Paragraph(3);
                    mail.Time = DateTime.Now;
                    mail.Read = false;

                    Emails.Insert(0, mail);

                    await Task.Delay(100 * (i + 1));
                }

                fakeNow = DateTime.Now;

                progress.Visibility = Visibility.Collapsed;

                refreshing = false;
            }

            

        }

        private void listView_PullProgressChanged(object sender, Comet.Controls.RefreshProgressEventArgs e)
        {
            transform.RotationX = 180 * e.PullProgress;

            refreshText.Visibility = e.PullProgress < 1 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
