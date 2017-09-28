using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Sockets;
using System.Collections.ObjectModel;
using Windows.Networking.Proximity;
using Windows.Storage.Streams;

namespace SmartBEAM
{
    public partial class controller : PhoneApplicationPage
    {
        
        public StreamSocket socket1;
        public string passchanneldata;

        public controller()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        private async void PaymentSendAsync(string command)
        {
            
            if (socket1 != null)
            {
                DataWriter writer = new DataWriter(socket1.OutputStream);
                writer.WriteString(command);
                await writer.StoreAsync();
            }
            else
            {
                MessageBox.Show("Please setup the Bluetooth Connection");
                this.NavigationService.Navigate(new Uri("/BT.xaml", UriKind.Relative));
            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            MainPage passsocket = e.Content as MainPage;
            if (passsocket != null)
            {
                passsocket.receviesocket = socket1;
            }
            PhoneApplicationService.Current.State["Text"] = passchanneldata;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void btnPower_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Power";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Mute";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Up";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Down";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Left";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Right";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "OK";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "VolumeUp";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void VolumeDown_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "VolumeDown";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "PlayPause";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Next";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Previous";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Stop";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnPlayForward_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "PlayForward";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnPlayBack_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "PlayBack";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Text";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Music";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Photo";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            passchanneldata = "Menu";
            if (this.NavigationService.CanGoBack)
            {

                this.NavigationService.GoBack();
            }
        }
    }
}